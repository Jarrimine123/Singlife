using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class EverCareQuote : Page
    {
        private const decimal BaseRate = 0.0070M; // 0.70%
        private const decimal PreExistingExtra = 0.0020M; // +0.20% if pre-existing conditions
        private const decimal CriticalIllnessExtra = 0.0015M; // +0.15% if CI add-on
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

            decimal coverage;
            string preExisting = ddlPreExisting.SelectedValue;
            string criticalIllness = ddlCriticalIllness.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

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

            if (string.IsNullOrEmpty(preExisting) || string.IsNullOrEmpty(criticalIllness) || string.IsNullOrEmpty(frequency))
            {
                ShowValidationError("❌ Please select all options including Premium Payment Frequency.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);

            decimal finalRate = BaseRate;
            if (preExisting == "Yes") finalRate += PreExistingExtra;
            if (criticalIllness == "Yes") finalRate += CriticalIllnessExtra;

            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            decimal displayedPremium = frequency == "Monthly" ? monthlyPremium : annualPremium;

            lblResult.Text = $"<strong>Quote Summary:</strong><br/>" +
                             $"Coverage Amount: SGD {coverage:N0}<br/>" +
                             $"Pre-existing Conditions: {preExisting} <br/>" +
                             $"Critical Illness Add-on: {criticalIllness}<br/>" +
                             $"Payment Frequency: {frequency}<br/><br/>" +
                             $"<strong>Estimated Premium:</strong><br/>" +
                             $"{frequency}: <strong>SGD {displayedPremium:F2}</strong>";

            pnlResult.Visible = true;
            pnlActions.Visible = true;

            SaveQuoteToDatabase(accountId, coverage, preExisting, criticalIllness, annualPremium, monthlyPremium, frequency);
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                ShowValidationError("⚠️ Please log in to add to cart.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);

            decimal coverage = decimal.TryParse(txtCoverage.Text, out var cov) ? cov : 0;
            string preExisting = ddlPreExisting.SelectedValue;
            string criticalIllness = ddlCriticalIllness.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

            decimal finalRate = BaseRate;
            if (preExisting == "Yes") finalRate += PreExistingExtra;
            if (criticalIllness == "Yes") finalRate += CriticalIllnessExtra;

            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO CartItems 
                    (AccountID, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium, PaymentFrequency)
                    VALUES (@AccountID, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium, @Frequency)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@ProductName", "Medical Insurance");
                    cmd.Parameters.AddWithValue("@PlanName", "EverCare Plan");
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annualPremium);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthlyPremium);
                    cmd.Parameters.AddWithValue("@Frequency", frequency);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ResetForm();
            Response.Redirect("Cart.aspx");
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            decimal coverage = decimal.TryParse(txtCoverage.Text, out var cov) ? cov : 0;
            string preExisting = ddlPreExisting.SelectedValue;
            string criticalIllness = ddlCriticalIllness.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

            decimal finalRate = BaseRate;
            if (preExisting == "Yes") finalRate += PreExistingExtra;
            if (criticalIllness == "Yes") finalRate += CriticalIllnessExtra;

            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            // ✅ Match PlanName format with Add to Cart (add the space)
            string url = $"Checkout.aspx?product=EverCare Plan&coverage={coverage}&preExisting={preExisting}&criticalIllness={criticalIllness}&frequency={frequency}&annual={annualPremium}&monthly={monthlyPremium}";

            ResetForm();
            Response.Redirect(url);
        }


        private void ShowValidationError(string message)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.Visible = true;
        }

        private void SaveQuoteToDatabase(int accountId, decimal coverage, string preExisting, string criticalIllness, decimal annual, decimal monthly, string frequency)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO EverCareQuotes 
                                (AccountID, CoverageAmount, PreExistingConditions, CriticalIllnessAddon, AnnualPremium, MonthlyPremium, Frequency, QuoteDate)
                                VALUES (@AccountID, @CoverageAmount, @PreExisting, @CriticalIllness, @AnnualPremium, @MonthlyPremium, @Frequency, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@PreExisting", preExisting);
                    cmd.Parameters.AddWithValue("@CriticalIllness", criticalIllness);
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
            ddlPreExisting.SelectedIndex = 0;
            ddlCriticalIllness.SelectedIndex = 0;
            ddlFrequency.SelectedIndex = 0;

            lblResult.Text = "";
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;
        }
    }
}
