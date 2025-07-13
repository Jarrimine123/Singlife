using System;
using System.Web.UI;

namespace Singlife
{
    public partial class PayoutCalculator : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                double contribution = double.Parse(txtContribution.Text);
                int payoutYears = int.Parse(txtPayoutYears.Text);
                int premiumYears = int.Parse(txtPremiumYears.Text);
                double rate = 0.04;

                // Step 1: Calculate Future Value after contribution period
                double fv = contribution * (Math.Pow(1 + rate, premiumYears) - 1) / rate;

                // Step 2: Calculate annual payout
                double payout = fv * rate / (1 - Math.Pow(1 + rate, -payoutYears));

                lblResult.Text = $"Your expected annual payout is <b>${payout:F2}</b> for {payoutYears} years.";
            }
            catch (Exception)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Text = "Error: Please ensure all fields are filled in correctly.";
            }
        }
    }
}
