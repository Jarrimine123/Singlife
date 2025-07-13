using System;
using System.Web.UI;

namespace Singlife
{
    public partial class PremiumCalculator : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                double payout = double.Parse(txtPayout.Text);
                int payoutYears = int.Parse(txtPayoutYears.Text);
                int premiumYears = int.Parse(txtPremiumYears.Text);
                double rate = 0.04;

                double numerator = 1 - Math.Pow(1 + rate, -payoutYears);
                double denominator = Math.Pow(1 + rate, premiumYears) - 1;
                double annualPremium = payout * (numerator / denominator);

                lblResult.Text = $"You need to contribute <b>${annualPremium:F2}</b> per year for {premiumYears} years.";
            }
            catch (Exception)
            {
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Text = "Error: Please ensure all fields are filled in correctly.";
            }
        }
    }
}
