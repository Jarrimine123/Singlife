using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class OncoShieldQuote : Page
    {
        private const decimal BaseRate = 0.0050M;
        private const decimal SmokerExtra = 0.0010M;
        private const decimal MinCoverageAmount = 50000M;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetForm();
            }
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;

            if (Session["AccountID"] == null)
            {
                ShowValidationError("⚠️ Please log in to get a quote.");
                return;
            }

            if (!decimal.TryParse(txtCoverage.Text, out decimal coverage) || coverage <= 0)
            {
                ShowValidationError("❌ Please enter a valid coverage amount greater than zero.");
                return;
            }

            if (coverage < MinCoverageAmount)
            {
                ShowValidationError($"❌ Minimum coverage amount is SGD {MinCoverageAmount:N0}.");
                return;
            }

            if (!int.TryParse(txtAge.Text, out int age) || age < 18 || age > 80)
            {
                ShowValidationError("❌ Please enter a valid age between 18 and 80.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            string smoker = ddlSmoker.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

            decimal finalRate = BaseRate + (smoker == "Yes" ? SmokerExtra : 0);
            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            string premiumDisplay = frequency == "Annual"
                ? $"Annual: <strong>SGD {annualPremium:F2}</strong>"
                : $"Monthly: <strong>SGD {monthlyPremium:F2}</strong>";

            lblResult.Text = $"<strong>Quote Summary:</strong><br/>" +
                             $"Coverage Amount: SGD {coverage:N0}<br/>" +
                             $"Age: {age} &nbsp;&nbsp;|&nbsp;&nbsp; Smoker: {smoker}<br/>" +
                             $"Payment Frequency: {frequency}<br/><br/>" +
                             $"<strong>Estimated Premium:</strong><br/>" + premiumDisplay;

            pnlResult.Visible = true;
            pnlActions.Visible = true;

            SaveQuoteToDatabase(accountId, coverage, age, smoker, annualPremium, monthlyPremium, frequency);
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                ShowValidationError("⚠️ Please log in to add to cart.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            if (!decimal.TryParse(txtCoverage.Text, out decimal coverage)) return;

            string frequency = ddlFrequency.SelectedValue; // <--- capture the selected frequency
            decimal finalRate = BaseRate + (ddlSmoker.SelectedValue == "Yes" ? SmokerExtra : 0);
            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO CartItems 
                        (AccountID, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium, PaymentFrequency)
                         VALUES 
                        (@AccountID, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium, @PaymentFrequency)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@ProductName", "Medical Insurance");
                    cmd.Parameters.AddWithValue("@PlanName", "OncoShield");
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annualPremium);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthlyPremium);
                    cmd.Parameters.AddWithValue("@PaymentFrequency", frequency); // <-- now saved

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ResetForm();
            Response.Redirect("Cart.aspx");
        }


        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtCoverage.Text, out decimal coverage)) return;
            if (!int.TryParse(txtAge.Text, out int age)) return;
            string smoker = ddlSmoker.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

            decimal finalRate = BaseRate + (smoker == "Yes" ? SmokerExtra : 0);
            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            string url = $"Checkout.aspx?product=OncoShield&coverage={coverage}&age={age}&smoker={smoker}&annual={annualPremium}&monthly={monthlyPremium}&frequency={frequency}";

            ResetForm();
            Response.Redirect(url);
        }

        private void ShowValidationError(string message)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.Visible = true;
        }

        private void SaveQuoteToDatabase(int accountId, decimal coverage, int age, string smoker, decimal annual, decimal monthly, string frequency)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO OncoShieldQuotes 
                                 (AccountID, CoverageAmount, Age, Smoker, AnnualPremium, MonthlyPremium, Frequency, QuoteDate)
                                 VALUES (@AccountID, @CoverageAmount, @Age, @Smoker, @AnnualPremium, @MonthlyPremium, @Frequency, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@Smoker", smoker == "Yes" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annual);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthly);
                    cmd.Parameters.AddWithValue("@Frequency", frequency);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ResetForm()
        {
            txtCoverage.Text = "";
            txtAge.Text = "";
            ddlSmoker.SelectedIndex = 0;
            ddlFrequency.SelectedIndex = 0;

            lblResult.Text = "";
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;
        }
    }
}
