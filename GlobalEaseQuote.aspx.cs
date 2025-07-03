using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;

namespace Singlife
{
    public partial class GlobalEaseQuote : Page
    {
        private const int MaxDuration = 180;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetForm();
            }
        }

        protected void ddlPlanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isFamily = ddlPlanType.SelectedValue == "Family";
            phIndividualAge.Visible = !isFamily;
            phFamilyAges.Visible = isFamily;
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

            if (!int.TryParse(txtDuration.Text, out int days) || days <= 0 || days > MaxDuration)
            {
                ShowValidationError("❌ Please enter a valid trip duration between 1 and 180 days.");
                return;
            }

            string destination = ddlDestination.SelectedValue;
            string purpose = ddlPurpose.SelectedValue;
            string medical = ddlMedical.SelectedValue;
            string planType = ddlPlanType.SelectedValue;

            decimal premium = 0;
            string ageSummary = "";

            List<int> ageList = new List<int>();

            if (planType == "Individual")
            {
                if (!int.TryParse(txtAge.Text, out int age) || age < 1 || age > 100)
                {
                    ShowValidationError("❌ Please enter a valid age between 1 and 100.");
                    return;
                }

                if (age < 22)
                {
                    ShowValidationError("⚠️ You must be at least 22 to buy travel insurance independently.");
                    return;
                }

                ageList.Add(age);
                ageSummary = $"Traveller's Age: {age}<br/>";
            }
            else // Family
            {
                string[] parts = txtFamilyAges.Text.Split(',');
                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int age) && age >= 1 && age <= 100)
                    {
                        ageList.Add(age);
                    }
                    else
                    {
                        ShowValidationError("❌ Please enter valid ages (1–100) separated by commas.");
                        return;
                    }
                }

                if (ageList.Count == 0)
                {
                    ShowValidationError("❌ Please enter at least one valid age.");
                    return;
                }

                ageSummary = $"Family Members: {ageList.Count}<br/>Ages: {txtFamilyAges.Text}<br/>";
            }

            bool isFamilyPlan = planType == "Family";
            premium = CalculatePremium(ageList, days, destination, isFamilyPlan);

            lblResult.Text = $"<strong>Quote Summary:</strong><br/>" +
                             $"Plan Type: {planType}<br/>" +
                             $"Destination: {destination}<br/>" +
                             $"Duration: {days} days<br/>" +
                             $"{ageSummary}" +
                             $"Purpose: {purpose}<br/>" +
                             $"Medical Condition: {medical}<br/><br/>" +
                             $"<strong>Estimated Premium:</strong><br/>SGD {premium:F2}";

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
            if (!int.TryParse(txtDuration.Text, out int days)) return;

            string destination = ddlDestination.SelectedValue;
            string planType = ddlPlanType.SelectedValue;
            bool isFamilyPlan = planType == "Family";
            List<int> ageList = GetAgeList(planType);

            if (ageList == null || ageList.Count == 0)
            {
                ShowValidationError("❌ Invalid ages entered.");
                return;
            }

            decimal premium = CalculatePremium(ageList, days, destination, isFamilyPlan);

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO CartItems 
                        (AccountID, ProductName, PlanName, CoverageAmount, DurationDays, 
                         AnnualPremium, MonthlyPremium, EstimatedPremium, PaymentFrequency)
                         VALUES 
                        (@AccountID, @ProductName, @PlanName, NULL, @DurationDays, 
                         NULL, NULL, @EstimatedPremium, @PaymentFrequency)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@ProductName", "Travel Insurance");
                    cmd.Parameters.AddWithValue("@PlanName", "GlobeEase Travel Plan");
                    cmd.Parameters.AddWithValue("@DurationDays", days);
                    cmd.Parameters.AddWithValue("@EstimatedPremium", premium);
                    cmd.Parameters.AddWithValue("@PaymentFrequency", "One-Time");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ResetForm();
            Response.Redirect("Cart.aspx");
        }


        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtDuration.Text, out int days))
            {
                ShowValidationError("❌ Invalid trip duration.");
                return;
            }

            string destination = ddlDestination.SelectedValue;
            string purpose = ddlPurpose.SelectedValue;
            string medical = ddlMedical.SelectedValue;
            string planType = ddlPlanType.SelectedValue;

            bool isFamilyPlan = planType == "Family";
            List<int> ageList = GetAgeList(planType);

            if (ageList == null || ageList.Count == 0)
            {
                ShowValidationError("❌ Invalid ages entered.");
                return;
            }

            decimal premium = CalculatePremium(ageList, days, destination, isFamilyPlan);

            string url = $"Checkout.aspx?product=GlobalEase" +
                         $"&type={planType}" +
                         $"&duration={days}" +
                         $"&destination={destination}" +
                         $"&purpose={purpose}" +
                         $"&medical={medical}" +
                         $"&premium={premium:F2}";

            ResetForm();
            Response.Redirect(url);
        }


        private decimal CalculatePremium(List<int> ages, int days, string destination, bool isFamily)
        {
            decimal total = 0;
            decimal destinationMultiplier = 1.0M;

            if (destination == "Europe")
                destinationMultiplier = 1.2M;
            else if (destination == "Worldwide")
                destinationMultiplier = 1.5M;

            foreach (int age in ages)
            {
                decimal ageRate;
                if (age <= 18)
                    ageRate = 1.5M;
                else if (age <= 60)
                    ageRate = 2.5M;
                else
                    ageRate = 3.0M;

                total += days * ageRate * destinationMultiplier;
            }

            if (isFamily)
                total *= 0.9M; // 10% discount

            return total;
        }

        private List<int> GetAgeList(string planType)
        {
            List<int> ageList = new List<int>();

            if (planType == "Individual")
            {
                if (int.TryParse(txtAge.Text, out int age) && age >= 1 && age <= 100)
                    ageList.Add(age);
            }
            else
            {
                string[] parts = txtFamilyAges.Text.Split(',');
                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int age) && age >= 1 && age <= 100)
                        ageList.Add(age);
                }
            }

            return ageList;
        }

        private void ShowValidationError(string message)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.Visible = true;
        }

        private void ResetForm()
        {
            txtDuration.Text = "";
            txtAge.Text = "";
            txtFamilyAges.Text = "";
            ddlPlanType.SelectedIndex = 0;
            ddlDestination.SelectedIndex = 0;
            ddlPurpose.SelectedIndex = 0;
            ddlMedical.SelectedIndex = 0;
            phIndividualAge.Visible = true;
            phFamilyAges.Visible = false;

            lblResult.Text = "";
            lblValidationMessage.Visible = false;
            pnlResult.Visible = false;
            pnlActions.Visible = false;
        }
    }
}
