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
    public partial class EverCareQuote : System.Web.UI.Page
    {
        private const decimal BaseRate = 0.0070M; // 0.70%
        private const decimal PreExistingExtra = 0.0020M; // +0.20% if pre-existing conditions
        private const decimal CriticalIllnessExtra = 0.0015M; // +0.15% if CI add-on
        private const decimal MinCoverageAmount = 50000M;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;

            decimal coverage;
            string preExisting = ddlPreExisting.SelectedValue;
            string criticalIllness = ddlCriticalIllness.SelectedValue;

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

            if (string.IsNullOrEmpty(preExisting) || string.IsNullOrEmpty(criticalIllness))
            {
                ShowValidationError("❌ Please select an option for both Pre-existing Conditions and Critical Illness Add-on.");
                return;
            }

            decimal finalRate = BaseRate;

            if (preExisting == "Yes")
                finalRate += PreExistingExtra;

            if (criticalIllness == "Yes")
                finalRate += CriticalIllnessExtra;

            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            lblResult.Text = $"<strong>Quote Summary:</strong><br/>" +
                             $"Coverage Amount: SGD {coverage:N0}<br/>" +
                             $"Pre-existing Conditions: {preExisting} <br/>" +
                             $"Critical Illness Add-on: {criticalIllness}<br/><br/>" +
                             $"<strong>Estimated Premiums:</strong><br/>" +
                             $"Annual: <strong>SGD {annualPremium:F2}</strong><br/>" +
                             $"Monthly: <strong>SGD {monthlyPremium:F2}</strong>";

            pnlResult.Visible = true;
            pnlActions.Visible = true;

            int accountId;
            if (Session["AccountID"] != null && int.TryParse(Session["AccountID"].ToString(), out accountId))
            {
                // Session ID present
            }
            else
            {
                accountId = new Random().Next(1000, 9999);
                Session["AccountID"] = accountId;
            }

            SaveQuoteToDatabase(accountId, coverage, preExisting, criticalIllness, annualPremium, monthlyPremium);
        }

        private void ShowValidationError(string message)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.Visible = true;
        }

        private void SaveQuoteToDatabase(int accountId, decimal coverage, string preExisting, string criticalIllness, decimal annual, decimal monthly)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO EverCareQuotes 
                                (AccountID, CoverageAmount, PreExistingConditions, CriticalIllnessAddon, AnnualPremium, MonthlyPremium, QuoteDate)
                                VALUES (@AccountID, @CoverageAmount, @PreExisting, @CriticalIllness, @AnnualPremium, @MonthlyPremium, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@PreExisting", preExisting);
                    cmd.Parameters.AddWithValue("@CriticalIllness", criticalIllness);
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
            int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : new Random().Next(1000, 9999);
            Session["AccountID"] = accountId;

            decimal coverage = decimal.TryParse(txtCoverage.Text, out var cov) ? cov : 0;
            string preExisting = ddlPreExisting.SelectedValue;
            string criticalIllness = ddlCriticalIllness.SelectedValue;

            decimal finalRate = BaseRate;
            if (preExisting == "Yes") finalRate += PreExistingExtra;
            if (criticalIllness == "Yes") finalRate += CriticalIllnessExtra;

            decimal annualPremium = coverage * finalRate;
            decimal monthlyPremium = annualPremium / 12;

            string connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO CartItems 
                    (AccountID, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium)
                    VALUES (@AccountID, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@ProductName", "Medical Insurance");
                    cmd.Parameters.AddWithValue("@PlanName", "EverCare Plan");
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annualPremium);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthlyPremium);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            Response.Redirect("Cart.aspx");
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            Response.Redirect("Purchase.aspx?plan=EverCare");
        }
    }
}

