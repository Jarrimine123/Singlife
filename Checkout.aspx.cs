using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace Singlife
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["AccountID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // Load either single quote or cart
                if (!string.IsNullOrEmpty(Request.QueryString["product"]))
                {
                    LoadSingleQuoteFromQuery();
                }
                else
                {
                    LoadCartItems();
                }
            }
        }

        private void LoadSingleQuoteFromQuery()
        {
            string productName = Request.QueryString["product"];
            decimal coverage = 0;
            decimal annualPremium = 0;
            decimal monthlyPremium = 0;

            decimal.TryParse(Request.QueryString["coverage"], out coverage);
            decimal.TryParse(Request.QueryString["annual"], out annualPremium);
            decimal.TryParse(Request.QueryString["monthly"], out monthlyPremium);

            string frequency = Request.QueryString["frequency"] ?? "Annual";

            DataTable dt = new DataTable();
            dt.Columns.Add("ProductName");
            dt.Columns.Add("PlanName");
            dt.Columns.Add("CoverageAmount", typeof(decimal));
            dt.Columns.Add("AnnualPremium", typeof(decimal));
            dt.Columns.Add("MonthlyPremium", typeof(decimal));
            dt.Columns.Add("PaymentFrequency");

            DataRow row = dt.NewRow();

            // You can expand this for more plans if needed
            row["ProductName"] = "Medical Insurance";
            row["PlanName"] = productName == "EverCare" ? "EverCare Plan" : productName + " Plan";
            row["CoverageAmount"] = coverage;
            row["AnnualPremium"] = annualPremium;
            row["MonthlyPremium"] = monthlyPremium;
            row["PaymentFrequency"] = frequency;

            dt.Rows.Add(row);
            ViewState["CartItems"] = dt;

            gvOrderSummary.DataSource = dt;
            gvOrderSummary.DataBind();

            decimal totalPremium = (frequency == "Monthly") ? monthlyPremium : annualPremium;
            lblTotalMonthly.Text = totalPremium.ToString("C");
        }

        private void LoadCartItems()
        {
            int accountId = Convert.ToInt32(Session["AccountID"]);

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT CartID, ProductName, PlanName, CoverageAmount, AnnualPremium, PaymentFrequency
                                 FROM CartItems WHERE AccountID = @AccountID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ViewState["CartItems"] = dt;

                gvOrderSummary.DataSource = dt;
                gvOrderSummary.DataBind();

                decimal totalPremium = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string freq = row["PaymentFrequency"].ToString();
                    decimal annual = Convert.ToDecimal(row["AnnualPremium"]);

                    if (freq == "Monthly")
                        totalPremium += annual / 12;
                    else
                        totalPremium += annual;
                }

                lblTotalMonthly.Text = totalPremium.ToString("C");
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string cardNumber = txtCardNumber.Text.Trim();
            string expiry = txtExpiry.Text.Trim();
            string cvv = txtCVV.Text.Trim();

            // TODO: Add proper validation for these fields here!

            DataTable dt = ViewState["CartItems"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Your cart is empty.');", true);
                return;
            }

            Guid purchaseGroupId = Guid.NewGuid();

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                foreach (DataRow row in dt.Rows)
                {
                    decimal annual = Convert.ToDecimal(row["AnnualPremium"]);
                    decimal monthly = annual / 12;
                    string frequency = row["PaymentFrequency"].ToString();

                    string paymentMethod = frequency == "Monthly" ? "Card Monthly" : "Card Annual";

                    string insertQuery = @"
                        INSERT INTO Purchases 
                        (PurchaseGroupID, AccountID, FullName, Email, Phone, Address, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium, PaymentMethod, CardLast4, PaymentFrequency)
                        VALUES 
                        (@PurchaseGroupID, @AccountID, @FullName, @Email, @Phone, @Address, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium, @PaymentMethod, @CardLast4, @PaymentFrequency)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
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
                        cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@CardLast4", cardNumber.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : "");
                        cmd.Parameters.AddWithValue("@PaymentFrequency", frequency);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear cart if purchase came from cart
                if (string.IsNullOrEmpty(Request.QueryString["product"]))
                {
                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM CartItems WHERE AccountID = @AccountID", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@AccountID", accountId);
                        deleteCmd.ExecuteNonQuery();
                    }
                }
            }

            Response.Redirect("ThankYou.aspx");
        }
    }
}
