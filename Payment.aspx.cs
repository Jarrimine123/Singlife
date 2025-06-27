using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.UI;

namespace Singlife
{
    public partial class Payment : System.Web.UI.Page
    {
        protected decimal amountDue = 0;
        protected string planName = "";
        protected bool isGiroActive = false;
        protected DateTime? nextBillingDate = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserPlanInfo();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            phPaymentMethods.Visible = !isGiroActive;
            phGiroActive.Visible = isGiroActive;

            DataBind();

            // Show card modal again if error present
            if (!string.IsNullOrEmpty(lblCardMessage.Text) && lblCardMessage.CssClass.Contains("text-danger"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowCardModal", "showModal('cardModal');", true);
            }

            // Show GIRO upload modal if error message present
            if (!string.IsNullOrEmpty(lblGiroUploadMessage.Text) && lblGiroUploadMessage.CssClass.Contains("text-danger"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowGiroModal", "showModal('giroUploadModal');", true);
            }
        }

        private void LoadUserPlanInfo()
        {
            if (Session["AccountID"] == null)
            {
                lblPlanName.Text = "No active plan.";
                lblAmountDue.Text = "";
                lblNextBillingDate.Text = "";
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT TOP 1 P.PlanName, P.MonthlyPremium, RP.NextBillingDate, RP.Status
                    FROM Purchases P
                    LEFT JOIN RecurringPayment RP ON P.AccountID = RP.AccountID AND RP.Status = 'Active' AND RP.PaymentMethod = 'GIRO'
                    WHERE P.AccountID = @AccountID
                    ORDER BY P.PurchaseDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            planName = reader["PlanName"].ToString();
                            amountDue = reader.GetDecimal(reader.GetOrdinal("MonthlyPremium"));
                            lblPlanName.Text = planName;
                            lblAmountDue.Text = $"Amount Due: ${amountDue:F2}";

                            if (!reader.IsDBNull(reader.GetOrdinal("NextBillingDate")))
                            {
                                nextBillingDate = reader.GetDateTime(reader.GetOrdinal("NextBillingDate"));
                                lblNextBillingDate.Text = $"Next Billing Date: {nextBillingDate:dd MMM yyyy}";
                                isGiroActive = true;
                            }
                            else
                            {
                                lblNextBillingDate.Text = "";
                                isGiroActive = false;
                            }
                        }
                        else
                        {
                            lblPlanName.Text = "No active plan.";
                            lblAmountDue.Text = "";
                            lblNextBillingDate.Text = "";
                            isGiroActive = false;
                        }
                    }
                }
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

            try
            {
                int accountId = Convert.ToInt32(Session["AccountID"]);
                string last4 = txtCardNumber.Text.Substring(txtCardNumber.Text.Length - 4);

                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM RecurringPayment WHERE AccountID = @AccountID AND PaymentMethod = 'Card'";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@AccountID", accountId);
                        int count = (int)cmdCheck.ExecuteScalar();

                        string sql;
                        if (count > 0)
                        {
                            sql = @"UPDATE RecurringPayment 
                                    SET NextBillingDate = @NextDate, CardLast4 = @CardLast4, Amount = @Amount, Status = 'Active'
                                    WHERE AccountID = @AccountID AND PaymentMethod = 'Card'";
                        }
                        else
                        {
                            sql = @"INSERT INTO RecurringPayment 
                                    (AccountID, Amount, PaymentMethod, CardLast4, NextBillingDate, PaymentFrequency, Status)
                                    VALUES (@AccountID, @Amount, 'Card', @CardLast4, @NextDate, 'Monthly', 'Active')";
                        }

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
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
                    $"Dear user,\n\nYour payment of ${amountDue:F2} for the '{planName}' plan has been successfully processed.\n\nThank you,\nSinglife Team"
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
            lblGiroUploadMessage.CssClass = "text-danger";

            if (!fuGiroForm.HasFile)
            {
                lblGiroUploadMessage.Text = "Please select a PDF file to upload.";
                return;
            }

            if (fuGiroForm.PostedFile.ContentType != "application/pdf")
            {
                lblGiroUploadMessage.Text = "Only PDF files are allowed.";
                return;
            }

            if (fuGiroForm.PostedFile.ContentLength > 5 * 1024 * 1024) // 5 MB limit
            {
                lblGiroUploadMessage.Text = "File size must be less than 5MB.";
                return;
            }

            try
            {
                int accountId = Convert.ToInt32(Session["AccountID"]);

                // Save file to /Uploads/Giro folder
                string uploadsFolder = Server.MapPath("~/Uploads/Giro/");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"Giro_{accountId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(uploadsFolder, fileName);

                fuGiroForm.SaveAs(filePath);

                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Insert or update RecurringPayment with GIRO info
                    string checkSql = "SELECT COUNT(*) FROM RecurringPayment WHERE AccountID = @AccountID AND PaymentMethod = 'GIRO'";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@AccountID", accountId);
                        int count = (int)cmdCheck.ExecuteScalar();

                        string sql;
                        if (count > 0)
                        {
                            sql = @"UPDATE RecurringPayment
                                    SET Status = 'Active', NextBillingDate = @NextDate, GIROFormPath = @GiroFilePath
                                    WHERE AccountID = @AccountID AND PaymentMethod = 'GIRO'";
                        }
                        else
                        {
                            sql = @"INSERT INTO RecurringPayment 
                                    (AccountID, Amount, PaymentMethod, NextBillingDate, PaymentFrequency, Status, GIROFormPath)
                                    VALUES (@AccountID, @Amount, 'GIRO', @NextDate, 'Monthly', 'Active', @GiroFilePath)";
                        }

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@Amount", amountDue);
                            cmd.Parameters.AddWithValue("@NextDate", DateTime.Today.AddMonths(1));
                            cmd.Parameters.AddWithValue("@GiroFilePath", "/Uploads/Giro/" + fileName);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                isGiroActive = true;
                LoadUserPlanInfo();

                lblGiroUploadMessage.CssClass = "text-success";
                lblGiroUploadMessage.Text = "GIRO form uploaded successfully! Your GIRO payment will be activated soon.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "GIRO Setup Confirmation",
                    $"Dear user,\n\nYour GIRO authorization form has been successfully uploaded and your GIRO payment setup is being processed.\n\nThank you,\nSinglife Team"
                );
            }
            catch (Exception ex)
            {
                lblGiroUploadMessage.Text = "Upload failed: " + ex.Message;
            }
        }

        private bool IsValidExpiry(string expiry)
        {
            if (expiry.Length != 5 || expiry[2] != '/') return false;
            if (!int.TryParse(expiry.Substring(0, 2), out int month) || month < 1 || month > 12) return false;
            if (!int.TryParse(expiry.Substring(3, 2), out int year)) return false;

            year += 2000;
            return new DateTime(year, month, DateTime.DaysInMonth(year, month)) >= DateTime.Today;
        }

        private void SendEmailNotification(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = "singlifeeeeeeke@gmail.com"; // Replace with your email
                var fromPassword = "pnfupbxiznvokifd";        // Replace with your app password

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(fromEmail, fromPassword);

                    var mail = new MailMessage
                    {
                        From = new MailAddress(fromEmail, "Singlife Notification"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    mail.To.Add(toEmail);
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email failed: " + ex.Message);
                // Fail silently, do not throw exception to user
            }
        }
    }
}
