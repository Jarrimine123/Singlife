using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class Payment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserPlans();
            }
        }

        private void LoadUserPlans()
        {
            if (Session["AccountID"] == null) return;

            int accountId = Convert.ToInt32(Session["AccountID"]);
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT P.PurchaseID, P.PlanName, P.MonthlyPremium, 
                           ISNULL(RP.NextBillingDate, P.NextBillingDate) AS NextBillingDate,
                           ISNULL(RP.PaymentMethod, 'None') AS PaymentMethod, RP.Status
                    FROM Purchases P
                    LEFT JOIN RecurringPayment RP ON P.PurchaseID = RP.PurchaseID AND RP.Status = 'Active'
                    WHERE P.AccountID = @AccountID
                    ORDER BY P.PurchaseDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        rptPlans.DataSource = dt;
                        rptPlans.DataBind();
                    }
                }
            }
        }

        protected void rptPlans_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string purchaseIdStr = e.CommandArgument?.ToString();
            if (!int.TryParse(purchaseIdStr, out int purchaseId)) return;

            ViewState["SelectedPurchaseID"] = purchaseId;

            switch (e.CommandName)
            {
                case "PayNow":
                    ScriptManager.RegisterStartupScript(this, GetType(), "showPayNowQR", $"showQR({purchaseId});", true);
                    break;

                case "Card":
                    ScriptManager.RegisterStartupScript(this, GetType(), "showCardModal", $"showCardModal({purchaseId});", true);
                    break;

                case "Giro":
                    ScriptManager.RegisterStartupScript(this, GetType(), "showGiroModal", $"showGiroModal({purchaseId});", true);
                    break;
            }
        }

        protected void btnSubmitCard_Click(object sender, EventArgs e)
        {
            lblCardMessage.Text = "";
            lblCardMessage.CssClass = "text-danger";

            if (string.IsNullOrWhiteSpace(txtCardNumber.Text) ||
                string.IsNullOrWhiteSpace(txtExpiry.Text) ||
                string.IsNullOrWhiteSpace(txtCVV.Text))
            {
                lblCardMessage.Text = "Please fill in all card details.";
                return;
            }

            if (txtCardNumber.Text.Length != 16 || !IsValidExpiry(txtExpiry.Text) ||
                txtCVV.Text.Length < 3 || txtCVV.Text.Length > 4)
            {
                lblCardMessage.Text = "Please enter valid card information.";
                return;
            }

            if (ViewState["SelectedPurchaseID"] == null)
            {
                lblCardMessage.Text = "No plan selected.";
                return;
            }

            int purchaseId = (int)ViewState["SelectedPurchaseID"];
            int accountId = Convert.ToInt32(Session["AccountID"]);
            decimal amountDue = GetAmountDueForPurchase(purchaseId);

            string last4 = txtCardNumber.Text.Substring(txtCardNumber.Text.Length - 4);

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM RecurringPayment WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'Card'";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        int count = (int)cmdCheck.ExecuteScalar();

                        string sql;
                        if (count > 0)
                        {
                            sql = @"UPDATE RecurringPayment 
                                    SET NextBillingDate = @NextDate, CardLast4 = @CardLast4, Amount = @Amount, Status = 'Active'
                                    WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'Card'";
                        }
                        else
                        {
                            sql = @"INSERT INTO RecurringPayment 
                                    (AccountID, PurchaseID, Amount, PaymentMethod, CardLast4, NextBillingDate, PaymentFrequency, Status)
                                    VALUES (@AccountID, @PurchaseID, @Amount, 'Card', @CardLast4, @NextDate, 'Monthly', 'Active')";
                        }

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                            cmd.Parameters.AddWithValue("@Amount", amountDue);
                            cmd.Parameters.AddWithValue("@CardLast4", last4);
                            cmd.Parameters.AddWithValue("@NextDate", DateTime.Today.AddMonths(1));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                lblCardMessage.CssClass = "text-success";
                lblCardMessage.Text = "Payment successful! Thank you.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "Payment Confirmation",
                    $"Dear user,\n\nYour payment of ${amountDue:F2} for Purchase ID {purchaseId} has been successfully processed.\n\nThank you,\nSinglife Team"
                );

                txtCardNumber.Text = txtExpiry.Text = txtCVV.Text = "";
            }
            catch (Exception ex)
            {
                lblCardMessage.Text = "Payment failed: " + ex.Message;
            }
        }

        protected void btnUploadGiro_Click(object sender, EventArgs e)
        {
            lblGiroUploadMessage.Text = "";
            if (ViewState["SelectedPurchaseID"] == null)
            {
                lblGiroUploadMessage.Text = "No plan selected.";
                return;
            }

            int purchaseId = (int)ViewState["SelectedPurchaseID"];

            if (!fuGiroForm.HasFile)
            {
                lblGiroUploadMessage.Text = "Please upload your GIRO authorization form (PDF).";
                return;
            }

            if (!fuGiroForm.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                lblGiroUploadMessage.Text = "Only PDF files are accepted.";
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                string folderPath = Server.MapPath("~/GiroForms/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filename = $"GiroForm_{purchaseId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string savePath = Path.Combine(folderPath, filename);
                fuGiroForm.SaveAs(savePath);

                int accountId = Convert.ToInt32(Session["AccountID"]);
                decimal amountDue = GetAmountDueForPurchase(purchaseId);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM RecurringPayment WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'GIRO'";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        int count = (int)cmdCheck.ExecuteScalar();

                        string sql;
                        if (count > 0)
                        {
                            sql = @"UPDATE RecurringPayment 
                                    SET NextBillingDate = @NextDate, GiroFormPath = @GiroFormPath, Amount = @Amount, Status = 'Pending'
                                    WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'GIRO'";
                        }
                        else
                        {
                            sql = @"INSERT INTO RecurringPayment 
                                    (AccountID, PurchaseID, Amount, PaymentMethod, GiroFormPath, NextBillingDate, PaymentFrequency, Status)
                                    VALUES (@AccountID, @PurchaseID, @Amount, 'GIRO', @GiroFormPath, @NextDate, 'Monthly', 'Pending')";
                        }

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                            cmd.Parameters.AddWithValue("@Amount", amountDue);
                            cmd.Parameters.AddWithValue("@GiroFormPath", filename);
                            cmd.Parameters.AddWithValue("@NextDate", DateTime.Today.AddMonths(1));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                lblGiroUploadMessage.CssClass = "text-success";
                lblGiroUploadMessage.Text = "GIRO form uploaded successfully. Your GIRO setup will be processed soon.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "GIRO Form Submitted",
                    $"Dear user,\n\nYour GIRO authorization form has been received for Purchase ID {purchaseId}.\nWe will notify you once the GIRO is active.\n\nThank you,\nSinglife Team"
                );
            }
            catch (Exception ex)
            {
                lblGiroUploadMessage.Text = "Failed to upload GIRO form: " + ex.Message;
            }
        }

        private decimal GetAmountDueForPurchase(int purchaseId)
        {
            decimal amount = 0;
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT MonthlyPremium FROM Purchases WHERE PurchaseID = @PurchaseID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        amount = Convert.ToDecimal(result);
                    }
                }
            }
            return amount;
        }

        private bool IsValidExpiry(string expiry)
        {
            if (string.IsNullOrWhiteSpace(expiry)) return false;
            string[] parts = expiry.Split('/');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out int month)) return false;
            if (!int.TryParse(parts[1], out int year)) return false;
            if (month < 1 || month > 12) return false;

            // Assuming YY format, convert to 20YY
            year += 2000;
            DateTime expDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
            return expDate >= DateTime.Today;
        }

        private void SendEmailNotification(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("your-email@example.com", "Singlife Team");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "your-email-password";

                var smtp = new SmtpClient
                {
                    Host = "smtp.example.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch
            {
                // Logging email sending failure is recommended
            }
        }
    }
}
