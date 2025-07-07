using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class LifeGrowQuote : Page
    {
        private const decimal BaseRate = 0.0042M;
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

            if (!int.TryParse(txtAge.Text, out int age) || age < 18 || age > 70)
            {
                ShowValidationError("❌ Please enter a valid age between 18 and 70.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            string goal = ddlGoal.SelectedValue;
            string retireAge = ddlRetireAge.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

            decimal annualPremium = coverage * BaseRate;
            decimal monthlyPremium = annualPremium / 12;

            string premiumDisplay = frequency == "Annual"
                ? $"Annual: <strong>SGD {annualPremium:F2}</strong>"
                : $"Monthly: <strong>SGD {monthlyPremium:F2}</strong>";

            lblResult.Text = $"<strong>Quote Summary:</strong><br/>" +
                             $"Coverage Amount: SGD {coverage:N0}<br/>" +
                             $"Age: {age} &nbsp;&nbsp;|&nbsp;&nbsp; Retirement Goal: {goal}<br/>" +
                             $"Planned Retirement Age: {retireAge}<br/>" +
                             $"Payment Frequency: {frequency}<br/><br/>" +
                             $"<strong>Estimated Premium:</strong><br/>" + premiumDisplay;

            pnlResult.Visible = true;
            pnlActions.Visible = true;
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
            decimal annualPremium = coverage * BaseRate;
            decimal monthlyPremium = annualPremium / 12;
            string frequency = ddlFrequency.SelectedValue;

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
                    cmd.Parameters.AddWithValue("@ProductName", "Life Insurance");
                    cmd.Parameters.AddWithValue("@PlanName", "LifeGrow Retirement Plus");
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverage);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annualPremium);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthlyPremium);
                    cmd.Parameters.AddWithValue("@PaymentFrequency", frequency);

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

            string goal = ddlGoal.SelectedValue;
            string retireAge = ddlRetireAge.SelectedValue;
            string frequency = ddlFrequency.SelectedValue;

            decimal annualPremium = coverage * BaseRate;
            decimal monthlyPremium = annualPremium / 12;

            string url = $"Checkout.aspx?product=LifeGrow&coverage={coverage}&age={age}&goal={goal}&retireAge={retireAge}&annual={annualPremium}&monthly={monthlyPremium}&frequency={frequency}";

            ResetForm();
            Response.Redirect(url);
        }

        private void ShowValidationError(string message)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.Visible = true;
        }

        private void ResetForm()
        {
            txtCoverage.Text = "";
            txtAge.Text = "";
            ddlGoal.SelectedIndex = 0;
            ddlRetireAge.SelectedIndex = 0;
            ddlFrequency.SelectedIndex = 0;

            lblResult.Text = "";
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;
        }
    }
}
