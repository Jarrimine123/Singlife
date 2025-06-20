using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class OncoShieldQuote : System.Web.UI.Page
    {
        private const decimal BaseRate = 0.0050M; // 0.50%
        private const decimal SmokerExtra = 0.0010M; // +0.10% if smoker
        private const decimal MinCoverageAmount = 50000M; // Minimum coverage SGD 50,000

        protected void Page_Load(object sender, EventArgs e)
        {

        }

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

            decimal finalRate = BaseRate;

            if (smoker == "Yes")
            {
                finalRate += SmokerExtra;
            }

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

            // Generate random AccountID for testing if user not logged in
            int accountId;
            if (Session["AccountID"] != null && int.TryParse(Session["AccountID"].ToString(), out accountId))
            {
                // Use real session ID
            }
            else
            {
                // Generate random AccountID for testing
                Random rnd = new Random();
                accountId = rnd.Next(1000, 10000); // 1000–9999
                Session["AccountID"] = accountId; // <-- persist to session
            }

            // Save the quote
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
                string query = @"INSERT INTO OncoShieldQuotes (AccountID, CoverageAmount, Age, Smoker, AnnualPremium, MonthlyPremium, QuoteDate)
                                 VALUES (@AccountID, @CoverageAmount, @Age, @Smoker, @AnnualPremium, @MonthlyPremium, GETDATE())";

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
                    conn.Close();
                }
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            int accountId;
            if (Session["AccountID"] != null && int.TryParse(Session["AccountID"].ToString(), out accountId))
            {
                // Use real account ID
            }
            else
            {
                // For testing, generate a random account ID and store it in session
                Random rnd = new Random();
                accountId = rnd.Next(1000, 10000);
                Session["AccountID"] = accountId;  // <-- persist AccountID in session
            }

            decimal coverage;
            decimal annualPremium;
            decimal monthlyPremium;

            // Parse values from previous quote (must match btnCalculate_Click values)
            if (!decimal.TryParse(txtCoverage.Text, out coverage)) coverage = 0;
            annualPremium = coverage * (ddlSmoker.SelectedValue == "Yes" ? (BaseRate + SmokerExtra) : BaseRate);
            monthlyPremium = annualPremium / 12;

            // Save cart item to DB
            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO CartItems (AccountID, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium)
                 VALUES (@AccountID, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium)";

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
                    conn.Close();
                }
            }

            // Redirect to cart page
            Response.Redirect("Cart.aspx");
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            // Redirect to the purchase page with the selected plan
            Response.Redirect("Purchase.aspx?plan=OncoShield");
        }
    }
}
