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

                // Check if coming from Buy Now with query string parameters
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

            DataTable dt = new DataTable();
            dt.Columns.Add("ProductName");
            dt.Columns.Add("PlanName");
            dt.Columns.Add("CoverageAmount", typeof(decimal));
            dt.Columns.Add("AnnualPremium", typeof(decimal));
            dt.Columns.Add("MonthlyPremium", typeof(decimal));

            DataRow row = dt.NewRow();

            if (productName == "EverCare")
            {
                string preExisting = Request.QueryString["preExisting"];
                string criticalIllness = Request.QueryString["criticalIllness"];

                dt.Columns.Add("PreExisting");
                dt.Columns.Add("CriticalIllness");

                row["ProductName"] = "Medical Insurance";
                row["PlanName"] = "EverCare Plan";
                row["CoverageAmount"] = coverage;
                row["AnnualPremium"] = annualPremium;
                row["MonthlyPremium"] = monthlyPremium;
                row["PreExisting"] = preExisting;
                row["CriticalIllness"] = criticalIllness;
            }
            else if (productName == "OncoShield")
            {
                int age = 0;
                int.TryParse(Request.QueryString["age"], out age);
                string smoker = Request.QueryString["smoker"];

                dt.Columns.Add("Age", typeof(int));
                dt.Columns.Add("Smoker");

                row["ProductName"] = "Medical Insurance";
                row["PlanName"] = "OncoShield Plan";
                row["CoverageAmount"] = coverage;
                row["AnnualPremium"] = annualPremium;
                row["MonthlyPremium"] = monthlyPremium;
                row["Age"] = age;
                row["Smoker"] = smoker;
            }
            else
            {
                // Default fallback, just basic columns
                row["ProductName"] = productName;
                row["PlanName"] = productName + " Plan";
                row["CoverageAmount"] = coverage;
                row["AnnualPremium"] = annualPremium;
                row["MonthlyPremium"] = monthlyPremium;
            }

            dt.Rows.Add(row);
            ViewState["CartItems"] = dt;

            gvOrderSummary.DataSource = dt;
            gvOrderSummary.DataBind();

            lblTotalMonthly.Text = monthlyPremium.ToString("C");
        }

        private void LoadCartItems()
        {
            int accountId = Convert.ToInt32(Session["AccountID"]);

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

            // Add your existing validation here (omitted for brevity)
            // Please add validation like in your original code to check fields

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

                // If the order was from the cart, clear cart items
                if (string.IsNullOrEmpty(Request.QueryString["product"]))
                {
                    SqlCommand deleteCmd = new SqlCommand("DELETE FROM CartItems WHERE AccountID = @AccountID", conn);
                    deleteCmd.Parameters.AddWithValue("@AccountID", accountId);
                    deleteCmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("ThankYou.aspx");
        }
    }
}
