using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class OncoShieldQuote : Page
    {
        private const decimal BaseRate = 0.0050M; // 0.50%
        private const decimal SmokerExtra = 0.0010M; // +0.10% if smoker
        private const decimal MinCoverageAmount = 50000M;

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;

            decimal coverage;
            int age;
            string smoker = ddlSmoker.SelectedValue;

            if (!decimal.TryParse(txtCoverage.Text, out coverage) || coverage <= 0)
            {
                ShowValidationError("❌ Please enter a valid coverage amount greater than zero.");
                return;
            }

            if (coverage < MinCoverageAmount)
            {
                ShowValidationError($"❌ Minimum coverage amount is SGD {MinCoverageAmount:N0}.");
                return;
            }

            if (!int.TryParse(txtAge.Text, out age) || age < 18 || age > 80)
            {
                ShowValidationError("❌ Please enter a valid age between 18 and 80.");
                return;
            }

            if (Session["AccountID"] == null)
            {
                ShowValidationError("⚠️ Please log in to get a quote.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            decimal finalRate = BaseRate;
            if (smoker == "Yes") finalRate += SmokerExtra;

            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            lblResult.Text = $"<strong>Quote Summary:</strong><br/>" +
                             $"Coverage Amount: SGD {coverage:N0}<br/>" +
                             $"Age: {age} &nbsp;&nbsp;|&nbsp;&nbsp; Smoker: {smoker}<br/><br/>" +
                             $"<strong>Estimated Premiums:</strong><br/>" +
                             $"Annual: <strong>SGD {annualPremium:F2}</strong><br/>" +
                             $"Monthly: <strong>SGD {monthlyPremium:F2}</strong>";

            pnlResult.Visible = true;
            pnlActions.Visible = true;

            SaveQuoteToDatabase(accountId, coverage, age, smoker, annualPremium, monthlyPremium);
        }

        private void ShowValidationError(string message)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.Visible = true;
        }

        private void SaveQuoteToDatabase(int accountId, decimal coverage, int age, string smoker, decimal annual, decimal monthly)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO OncoShieldQuotes 
                                (AccountID, CoverageAmount, Age, Smoker, AnnualPremium, MonthlyPremium, QuoteDate)
                                 VALUES 
                                (@AccountID, @CoverageAmount, @Age, @Smoker, @AnnualPremium, @MonthlyPremium, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@Smoker", smoker == "Yes" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annual);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthly);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                ShowValidationError("⚠️ Please log in to add to cart.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            decimal coverage;
            if (!decimal.TryParse(txtCoverage.Text, out coverage))
            {
                ShowValidationError("⚠️ Invalid coverage amount.");
                return;
            }

            decimal annualPremium = coverage * (ddlSmoker.SelectedValue == "Yes" ? (BaseRate + SmokerExtra) : BaseRate);
            decimal monthlyPremium = annualPremium / 12;

            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO CartItems 
                                (AccountID, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium)
                                 VALUES 
                                (@AccountID, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@ProductName", "Medical Insurance");
                    cmd.Parameters.AddWithValue("@PlanName", "OncoShield");
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annualPremium);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthlyPremium);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("Cart.aspx");
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            Response.Redirect("Purchase.aspx?plan=OncoShield");
        }
    }
}
