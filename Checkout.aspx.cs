using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Singlife
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCartItems();
            }
        }

        private void LoadCartItems()
        {
            int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : 0;
            if (accountId == 0)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT CartID, ProductName, PlanName, CoverageAmount, AnnualPremium
                                 FROM CartItems WHERE AccountID = @AccountID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ViewState["CartItems"] = dt;

                gvOrderSummary.DataSource = dt;
                gvOrderSummary.DataBind();

                decimal totalMonthly = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalMonthly += Convert.ToDecimal(row["AnnualPremium"]) / 12;
                }

                lblTotalMonthly.Text = totalMonthly.ToString("C");
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : 0;
            if (accountId == 0)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string cardNumber = txtCardNumber.Text.Trim();
            string expiry = txtExpiry.Text.Trim();
            string cvv = txtCVV.Text.Trim();

            // Basic empty field validation
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(cardNumber) ||
                string.IsNullOrEmpty(expiry) || string.IsNullOrEmpty(cvv))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please fill in all details.');", true);
                return;
            }

            // Email format validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter a valid email address.');", true);
                return;
            }

            // Phone number validation (digits only)
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter a valid phone number (digits only).');", true);
                return;
            }

            // Card number validation (digits only, length 13-16)
            if (!System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^\d{13,16}$"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter a valid card number (13 to 16 digits).');", true);
                return;
            }

            // Expiry date validation (MM/YY)
            if (!System.Text.RegularExpressions.Regex.IsMatch(expiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter expiry date in MM/YY format.');", true);
                return;
            }
            else
            {
                // Check if expiry date is in the future
                try
                {
                    string[] parts = expiry.Split('/');
                    int month = int.Parse(parts[0]);
                    int year = int.Parse(parts[1]) + 2000; // Assuming 21 -> 2021 etc.

                    var lastDayOfMonth = DateTime.DaysInMonth(year, month);
                    DateTime expiryDate = new DateTime(year, month, lastDayOfMonth, 23, 59, 59);

                    if (expiryDate < DateTime.Now)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Card expiry date must be in the future.');", true);
                        return;
                    }
                }
                catch
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid expiry date format.');", true);
                    return;
                }
            }

            // CVV validation (3 or 4 digits)
            if (!System.Text.RegularExpressions.Regex.IsMatch(cvv, @"^\d{3,4}$"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter a valid CVV (3 or 4 digits).');", true);
                return;
            }

            DataTable dt = ViewState["CartItems"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Your cart is empty.');", true);
                return;
            }

            Guid purchaseGroupId = Guid.NewGuid(); // Shared ID for this checkout

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                foreach (DataRow row in dt.Rows)
                {
                    decimal annual = Convert.ToDecimal(row["AnnualPremium"]);
                    decimal monthly = annual / 12;

                    string insertQuery = @"INSERT INTO Purchases 
                        (PurchaseGroupID, AccountID, FullName, Email, Phone, Address, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium, PaymentMethod, CardLast4, PaymentFrequency)
                        VALUES 
                        (@PurchaseGroupID, @AccountID, @FullName, @Email, @Phone, @Address, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium, @PaymentMethod, @CardLast4, @PaymentFrequency)";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@PurchaseGroupID", purchaseGroupId);
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@FullName", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@ProductName", row["ProductName"]);
                    cmd.Parameters.AddWithValue("@PlanName", row["PlanName"]);
                    cmd.Parameters.AddWithValue("@CoverageAmount", row["CoverageAmount"]);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annual);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthly);
                    cmd.Parameters.AddWithValue("@PaymentMethod", "Card Monthly");
                    cmd.Parameters.AddWithValue("@CardLast4", cardNumber.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : "");
                    cmd.Parameters.AddWithValue("@PaymentFrequency", "Monthly");

                    cmd.ExecuteNonQuery();
                }

                // Clear cart
                SqlCommand deleteCmd = new SqlCommand("DELETE FROM CartItems WHERE AccountID = @AccountID", conn);
                deleteCmd.Parameters.AddWithValue("@AccountID", accountId);
                deleteCmd.ExecuteNonQuery();
            }

            Response.Redirect("ThankYou.aspx");
        }
    }
}
