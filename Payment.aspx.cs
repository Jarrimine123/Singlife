using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Singlife
{
    public partial class Payment : System.Web.UI.Page
    {
        protected decimal amountDue = 0;
        protected string planName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserPlanInfo();
                CheckIfGiroIsActive(); // Call this to check if GIRO already set up
            }
        }

        private void LoadUserPlanInfo()
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            var accountId = Session["AccountID"];
            if (accountId == null)
            {
                lblPlanName.Text = "Please login to view your plan.";
                lblAmountDue.Text = "";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 1 PurchaseID, PlanName, PaymentFrequency, MonthlyPremium, AnnualPremium
                        FROM Purchases
                        WHERE AccountID = @AccountID
                        ORDER BY PurchaseDate ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ViewState["PurchaseID"] = reader.GetInt32(0);
                                planName = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                                string paymentFreq = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                decimal monthlyPremium = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3);
                                decimal annualPremium = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);

                                amountDue = paymentFreq.Equals("Monthly", StringComparison.OrdinalIgnoreCase) ? monthlyPremium :
                                            paymentFreq.Equals("Annual", StringComparison.OrdinalIgnoreCase) ? annualPremium : 0;

                                lblPlanName.Text = planName;
                                lblAmountDue.Text = amountDue > 0 ? "Amount Due: $" + amountDue.ToString("F2") : "No amount due at this time.";
                            }
                            else
                            {
                                lblPlanName.Text = "No active plans found.";
                                lblAmountDue.Text = "";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblPlanName.Text = "Error loading plan info.";
                lblAmountDue.Text = "";
            }
        }

        private void CheckIfGiroIsActive()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = @"
                        SELECT COUNT(*) 
                        FROM RecurringPayment 
                        WHERE AccountID = @AccountID AND PaymentMethod = 'GIRO' AND Status = 'Active'";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountID", Session["AccountID"]);
                        int giroCount = (int)cmd.ExecuteScalar();
                        if (giroCount > 0)
                        {
                            // Hide PayNow and GIRO options on frontend
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideOptions", @"
                                document.querySelectorAll('.card-method').forEach(function(card){
                                    if (card.textContent.includes('PayNow') || card.textContent.includes('GIRO')) {
                                        card.style.display = 'none';
                                    }
                                });
                            ", true);
                        }
                    }
                }
            }
            catch { }
        }

        protected void btnSubmitGiro_Click(object sender, EventArgs e)
        {
            lblGiroMessage.Text = "";
            string savePath = null;

            if (fuGiroForm.HasFile)
            {
                string ext = Path.GetExtension(fuGiroForm.FileName).ToLower();
                if (ext != ".pdf")
                {
                    lblGiroMessage.Text = "Only PDF files are allowed.";
                    return;
                }
                savePath = Server.MapPath("~/Uploads/GIRO/" + Guid.NewGuid() + ext);
                try
                {
                    fuGiroForm.SaveAs(savePath);
                }
                catch (Exception ex)
                {
                    lblGiroMessage.Text = "Upload failed: " + ex.Message;
                    return;
                }
            }

            string bankAccount = txtBankAccount.Text.Trim();
            if (string.IsNullOrEmpty(bankAccount) && savePath == null)
            {
                lblGiroMessage.Text = "Please enter your bank account number or upload GIRO form.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd1 = new SqlCommand("INSERT INTO GiroRequests (AccountID, BankAccount, GiroFormPath, RequestDate) VALUES (@AccountID, @BankAccount, @GiroFormPath, GETDATE())", conn);
                    cmd1.Parameters.AddWithValue("@AccountID", Session["AccountID"] ?? DBNull.Value);
                    cmd1.Parameters.AddWithValue("@BankAccount", bankAccount);
                    cmd1.Parameters.AddWithValue("@GiroFormPath", savePath ?? (object)DBNull.Value);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("INSERT INTO RecurringPayment (AccountID, PurchaseID, Amount, PaymentMethod, GiroFormPath, PaymentFrequency, Status, StartDate) VALUES (@AccountID, @PurchaseID, @Amount, 'GIRO', @GiroFormPath, @Freq, 'Active', GETDATE())", conn);
                    cmd2.Parameters.AddWithValue("@AccountID", Session["AccountID"] ?? DBNull.Value);
                    cmd2.Parameters.AddWithValue("@PurchaseID", ViewState["PurchaseID"] ?? DBNull.Value);
                    cmd2.Parameters.AddWithValue("@Amount", amountDue);
                    cmd2.Parameters.AddWithValue("@GiroFormPath", savePath ?? (object)DBNull.Value);
                    cmd2.Parameters.AddWithValue("@Freq", amountDue == 0 ? "Unknown" : (amountDue < 1000 ? "Monthly" : "Annual"));
                    cmd2.ExecuteNonQuery();
                }

                lblGiroMessage.CssClass = "text-success";
                lblGiroMessage.Text = "GIRO setup submitted successfully!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideAfterGiro", "setTimeout(() => location.reload(), 1500);", true);
            }
            catch (Exception ex)
            {
                lblGiroMessage.Text = "Error saving GIRO setup: " + ex.Message;
            }
        }

        protected void btnSubmitCard_Click(object sender, EventArgs e)
        {
            lblCardMessage.Text = "";

            string cardNumber = txtCardNumber.Text.Trim();
            string expiry = txtExpiry.Text.Trim();
            string cvv = txtCVV.Text.Trim();

            if (cardNumber.Length != 16 || !long.TryParse(cardNumber, out _))
            {
                lblCardMessage.Text = "Invalid card number.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(expiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                lblCardMessage.Text = "Expiry date must be MM/YY format.";
                return;
            }
            if ((cvv.Length != 3 && cvv.Length != 4) || !int.TryParse(cvv, out _))
            {
                lblCardMessage.Text = "Invalid CVV.";
                return;
            }
            if (amountDue <= 0)
            {
                lblCardMessage.Text = "No payment is due at this time.";
                return;
            }

            string last4 = cardNumber.Substring(cardNumber.Length - 4);
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd1 = new SqlCommand("INSERT INTO Payments (AccountID, PaymentDate, PaymentMethod, CardLast4, Amount) VALUES (@AccountID, GETDATE(), 'Card', @CardLast4, @Amount)", conn);
                    cmd1.Parameters.AddWithValue("@AccountID", Session["AccountID"] ?? DBNull.Value);
                    cmd1.Parameters.AddWithValue("@CardLast4", last4);
                    cmd1.Parameters.AddWithValue("@Amount", amountDue);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("INSERT INTO RecurringPayment (AccountID, PurchaseID, Amount, PaymentMethod, CardLast4, PaymentFrequency, Status, StartDate) VALUES (@AccountID, @PurchaseID, @Amount, 'Card', @CardLast4, @Freq, 'Active', GETDATE())", conn);
                    cmd2.Parameters.AddWithValue("@AccountID", Session["AccountID"] ?? DBNull.Value);
                    cmd2.Parameters.AddWithValue("@PurchaseID", ViewState["PurchaseID"] ?? DBNull.Value);
                    cmd2.Parameters.AddWithValue("@Amount", amountDue);
                    cmd2.Parameters.AddWithValue("@CardLast4", last4);
                    cmd2.Parameters.AddWithValue("@Freq", amountDue < 1000 ? "Monthly" : "Annual");
                    cmd2.ExecuteNonQuery();
                }

                lblCardMessage.CssClass = "text-success";
                lblCardMessage.Text = "Payment successful! Thank you.";
                btnSubmitCard.Enabled = false;
            }
            catch (Exception ex)
            {
                lblCardMessage.Text = "Payment failed: " + ex.Message;
            }
        }
    }
}
