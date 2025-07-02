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
                SELECT 
                    P.PurchaseID, 
                    P.PlanName, 
                    P.MonthlyPremium,
                    P.AnnualPremium,
                    P.PaymentFrequency,
                    ISNULL(RP.NextBillingDate, P.NextBillingDate) AS NextBillingDate,
                    ISNULL(RP.PaymentMethod, 'None') AS PaymentMethod, 
                    RP.Status,

                    CASE 
                        WHEN P.PaymentFrequency = 'Annual' THEN P.AnnualPremium
                        ELSE P.MonthlyPremium
                    END AS AmountDue,

                    (SELECT COUNT(*) 
                     FROM PaymentHistory PH 
                     WHERE PH.PurchaseID = P.PurchaseID AND PH.Status = 'Success') + 1 AS TotalPaymentCount,

                    CASE 
                        WHEN RP.PaymentMethod = 'GIRO' AND RP.GIROFormPath IS NOT NULL AND RP.Status IN ('Active', 'Pending') THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END AS IsGiroActive

                FROM Purchases P
                LEFT JOIN RecurringPayment RP ON P.PurchaseID = RP.PurchaseID AND RP.Status IN ('Active', 'Pending')
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

        // Corrected to get amount based on payment frequency
        private decimal GetAmountDueForPurchase(int purchaseId)
        {
            decimal amount = 0;
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT PaymentFrequency, MonthlyPremium, AnnualPremium FROM Purchases WHERE PurchaseID = @PurchaseID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string freq = reader["PaymentFrequency"]?.ToString() ?? "Monthly";
                            decimal monthly = reader["MonthlyPremium"] != DBNull.Value ? Convert.ToDecimal(reader["MonthlyPremium"]) : 0;
                            decimal annual = reader["AnnualPremium"] != DBNull.Value ? Convert.ToDecimal(reader["AnnualPremium"]) : 0;
                            amount = freq.Equals("Annual", StringComparison.OrdinalIgnoreCase) ? annual : monthly;
                        }
                    }
                }
            }
            return amount;
        }

        private DateTime CalculateNextBillingDate(int purchaseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            DateTime baseDate = DateTime.Today;
            string paymentFrequency = "Monthly"; // default fallback

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT PaymentFrequency, NextBillingDate FROM Purchases WHERE PurchaseID = @PurchaseID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            paymentFrequency = reader["PaymentFrequency"]?.ToString() ?? "Monthly";
                            if (reader["NextBillingDate"] != DBNull.Value)
                            {
                                baseDate = Convert.ToDateTime(reader["NextBillingDate"]);
                            }
                        }
                    }
                }
            }

            if (paymentFrequency.Equals("Annual", StringComparison.OrdinalIgnoreCase))
            {
                return baseDate.AddYears(1);
            }
            else // monthly or default
            {
                return baseDate.AddMonths(1);
            }
        }

        protected void btnSubmitCard_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int purchaseId = Convert.ToInt32(btn.CommandArgument);

            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            TextBox txtCardholderName = (TextBox)item.FindControl("txtCardholderName");
            TextBox txtCardNumber = (TextBox)item.FindControl("txtCardNumber");
            TextBox txtExpiry = (TextBox)item.FindControl("txtExpiry");
            TextBox txtCVV = (TextBox)item.FindControl("txtCVV");

            Label lblMessage = GetOrCreateLabel(item, "lblCardMessage");

            lblMessage.Text = "";
            lblMessage.CssClass = "text-danger";

            if (string.IsNullOrWhiteSpace(txtCardholderName.Text) ||
                string.IsNullOrWhiteSpace(txtCardNumber.Text) ||
                string.IsNullOrWhiteSpace(txtExpiry.Text) ||
                string.IsNullOrWhiteSpace(txtCVV.Text))
            {
                lblMessage.Text = "Please fill in all card details.";
                return;
            }

            if (txtCardNumber.Text.Length != 16 || !IsValidExpiry(txtExpiry.Text) ||
                txtCVV.Text.Length < 3 || txtCVV.Text.Length > 4)
            {
                lblMessage.Text = "Please enter valid card information.";
                return;
            }

            decimal amountDue = GetAmountDueForPurchase(purchaseId);
            string last4 = txtCardNumber.Text.Substring(txtCardNumber.Text.Length - 4);

            int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : 0;
            DateTime nextBillingDate = CalculateNextBillingDate(purchaseId);

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
                            cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string insertPaymentSql = @"
                        INSERT INTO PaymentHistory (PurchaseID, Amount, PaymentDate, Method, Status)
                        VALUES (@PurchaseID, @Amount, GETDATE(), 'Card', 'Success')";
                    using (SqlCommand cmdPayment = new SqlCommand(insertPaymentSql, conn))
                    {
                        cmdPayment.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmdPayment.Parameters.AddWithValue("@Amount", amountDue);
                        cmdPayment.ExecuteNonQuery();
                    }
                }

                lblMessage.CssClass = "text-success";
                lblMessage.Text = "Payment successful! Thank you.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "Payment Confirmation",
                    $"Dear user,\n\nYour payment of ${amountDue:F2} for Purchase ID {purchaseId} has been successfully processed.\n\nThank you,\nSinglife Team"
                );

                // Clear fields
                txtCardholderName.Text = "";
                txtCardNumber.Text = "";
                txtExpiry.Text = "";
                txtCVV.Text = "";

                LoadUserPlans();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Payment failed: " + ex.Message;
            }
        }

        protected void btnSubmitPayNow_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int purchaseId = Convert.ToInt32(btn.CommandArgument);
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            TextBox txtPayNowRef = (TextBox)item.FindControl("txtPayNowRef");
            FileUpload fuPayNowReceipt = (FileUpload)item.FindControl("fuPayNowReceipt");
            Label lblMessage = (Label)item.FindControl("lblPayNowMessage");

            lblMessage.Text = "";
            lblMessage.CssClass = "text-danger";

            if (string.IsNullOrWhiteSpace(txtPayNowRef.Text))
            {
                lblMessage.Text = "Please enter the transaction reference number.";
                return;
            }

            string receiptFileName = null;
            if (fuPayNowReceipt.HasFile)
            {
                string ext = Path.GetExtension(fuPayNowReceipt.FileName).ToLower();
                if (ext != ".jpg" && ext != ".png" && ext != ".pdf")
                {
                    lblMessage.Text = "Only JPG, PNG or PDF files are accepted for receipt.";
                    return;
                }

                string folderPath = Server.MapPath("~/PayNowReceipts/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                receiptFileName = $"PayNow_{purchaseId}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                string savePath = Path.Combine(folderPath, receiptFileName);
                fuPayNowReceipt.SaveAs(savePath);
            }

            decimal amountDue = GetAmountDueForPurchase(purchaseId);
            int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : 0;
            DateTime nextBillingDate = CalculateNextBillingDate(purchaseId);

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM RecurringPayment WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'PayNow'";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        int count = (int)cmdCheck.ExecuteScalar();

                        string sql;
                        if (count > 0)
                        {
                            sql = @"UPDATE RecurringPayment
                                    SET NextBillingDate = @NextDate, Amount = @Amount, Status = 'Active'
                                    WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'PayNow'";
                        }
                        else
                        {
                            sql = @"INSERT INTO RecurringPayment 
                                    (AccountID, PurchaseID, Amount, PaymentMethod, NextBillingDate, PaymentFrequency, Status)
                                    VALUES (@AccountID, @PurchaseID, @Amount, 'PayNow', @NextDate, 'Monthly', 'Active')";
                        }

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                            cmd.Parameters.AddWithValue("@Amount", amountDue);
                            cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string sqlPaymentHistory = @"
                        INSERT INTO PaymentHistory
                        (AccountID, PurchaseID, Amount, PaymentDate, Method, Status, Remarks)
                        VALUES (@AccountID, @PurchaseID, @Amount, GETDATE(), 'PayNow', 'Success', @Remarks)";

                    using (SqlCommand cmd = new SqlCommand(sqlPaymentHistory, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@Amount", amountDue);
                        cmd.Parameters.AddWithValue("@Remarks", txtPayNowRef.Text + (receiptFileName != null ? " | Receipt: " + receiptFileName : ""));
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.CssClass = "text-success";
                lblMessage.Text = "PayNow payment confirmed successfully! Thank you.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "PayNow Payment Confirmation",
                    $"Your PayNow payment of ${amountDue:F2} for Purchase ID {purchaseId} has been recorded successfully."
                );

                txtPayNowRef.Text = "";
                // Note: Cannot programmatically clear FileUpload control easily without page reload.

                LoadUserPlans();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to confirm PayNow payment: " + ex.Message;
            }
        }

        protected void btnUploadGiro_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int purchaseId = Convert.ToInt32(btn.CommandArgument);

            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            FileUpload fuGiroForm = (FileUpload)item.FindControl("fuGiroForm");
            Label lblMessage = GetOrCreateLabel(item, "lblGiroUploadMessage");

            lblMessage.Text = "";
            lblMessage.CssClass = "text-danger";

            if (!fuGiroForm.HasFile || !fuGiroForm.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                lblMessage.Text = "Please upload a valid PDF GIRO form.";
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                string folderPath = Server.MapPath("~/GiroForms/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filename = $"GiroForm_{purchaseId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string savePath = Path.Combine(folderPath, filename);
                fuGiroForm.SaveAs(savePath);

                int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : 0;
                decimal amountDue = GetAmountDueForPurchase(purchaseId);
                DateTime nextBillingDate = CalculateNextBillingDate(purchaseId);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string checkSql = @"SELECT TOP 1 RecurringPaymentID FROM RecurringPayment 
                                        WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'GIRO' AND Status IN ('Active', 'Pending')";

                    object existingId;
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        existingId = cmdCheck.ExecuteScalar();
                    }

                    string sql;
                    if (existingId != null)
                    {
                        sql = @"UPDATE RecurringPayment 
                                SET NextBillingDate = @NextDate, GiroFormPath = @GiroFormPath, Amount = @Amount, Status = 'Pending'
                                WHERE RecurringPaymentID = @RecurringPaymentID";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                            cmd.Parameters.AddWithValue("@GiroFormPath", filename);
                            cmd.Parameters.AddWithValue("@Amount", amountDue);
                            cmd.Parameters.AddWithValue("@RecurringPaymentID", Convert.ToInt32(existingId));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        sql = @"INSERT INTO RecurringPayment 
                                (AccountID, PurchaseID, Amount, PaymentMethod, GiroFormPath, NextBillingDate, PaymentFrequency, Status)
                                VALUES (@AccountID, @PurchaseID, @Amount, 'GIRO', @GiroFormPath, @NextDate, 'Monthly', 'Pending')";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                            cmd.Parameters.AddWithValue("@Amount", amountDue);
                            cmd.Parameters.AddWithValue("@GiroFormPath", filename);
                            cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                lblMessage.CssClass = "text-success";
                lblMessage.Text = "GIRO form uploaded. Awaiting activation.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "GIRO Form Submitted",
                    $"Dear user,\n\nYour GIRO authorization form has been received for Purchase ID {purchaseId}.\nWe will notify you once the GIRO is active.\n\nThank you,\nSinglife Team"
                );

                LoadUserPlans();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to upload GIRO form: " + ex.Message;
            }
        }

        protected void btnCancelGiro_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int purchaseId = Convert.ToInt32(btn.CommandArgument);
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            Label lblMessage = GetOrCreateLabel(item, "lblGiroCancelMessage");

            lblMessage.Text = "";
            lblMessage.CssClass = "text-danger";

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"UPDATE RecurringPayment 
                                   SET Status = 'Cancelled', NextBillingDate = NULL 
                                   WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'GIRO' AND Status IN ('Active', 'Pending')";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            lblMessage.CssClass = "text-success";
                            lblMessage.Text = "GIRO has been successfully cancelled.";
                        }
                        else
                        {
                            lblMessage.Text = "No active GIRO found to cancel.";
                        }
                    }
                }

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "GIRO Payment Cancellation Confirmed",
                    $@"Dear Customer,

Your GIRO arrangement for Purchase ID {purchaseId} has been successfully cancelled.

You may now choose to pay using other available methods such as Card or PayNow.

Thank you,
Singlife Support Team"
                );

                LoadUserPlans();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to cancel GIRO: " + ex.Message;
            }
        }

        private bool IsValidExpiry(string expiry)
        {
            if (string.IsNullOrWhiteSpace(expiry)) return false;
            string[] parts = expiry.Split('/');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out int month)) return false;
            if (!int.TryParse(parts[1], out int year)) return false;
            if (month < 1 || month > 12) return false;

            year += 2000;
            DateTime expDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
            return expDate >= DateTime.Today;
        }

        private void SendEmailNotification(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("singlifeeeeeeke@gmail.com", "Singlife Team");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "pnfupbxiznvokifd"; // Gmail App Password

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
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
                // Swallow email sending exceptions silently
            }
        }

        private Label GetOrCreateLabel(RepeaterItem item, string labelId)
        {
            Label lbl = (Label)item.FindControl(labelId);
            if (lbl == null)
            {
                lbl = new Label() { ID = labelId };
                item.Controls.Add(lbl);
            }
            return lbl;
        }

        protected void rptPlans_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;

                PlaceHolder phGiroActive = (PlaceHolder)e.Item.FindControl("phGiroActive");
                Button btnPayNow = (Button)e.Item.FindControl("btnPayNow");
                Button btnCard = (Button)e.Item.FindControl("btnCard");
                Button btnConfirmPayNow = (Button)e.Item.FindControl("btnConfirmPayNow");

                bool isGiroActive = row["IsGiroActive"] != DBNull.Value && Convert.ToBoolean(row["IsGiroActive"]);

                if (phGiroActive != null)
                {
                    phGiroActive.Visible = isGiroActive;
                }

                if (isGiroActive)
                {
                    if (btnPayNow != null)
                    {
                        btnPayNow.Enabled = false;
                        if (!btnPayNow.CssClass.Contains("disabled"))
                            btnPayNow.CssClass += " disabled";
                    }

                    if (btnCard != null)
                    {
                        btnCard.Enabled = false;
                        if (!btnCard.CssClass.Contains("disabled"))
                            btnCard.CssClass += " disabled";
                    }

                    if (btnConfirmPayNow != null)
                    {
                        btnConfirmPayNow.Enabled = false;
                        if (!btnConfirmPayNow.CssClass.Contains("disabled"))
                            btnConfirmPayNow.CssClass += " disabled";
                    }
                }
                else
                {
                    // Optional: Re-enable buttons if needed
                    if (btnPayNow != null)
                    {
                        btnPayNow.Enabled = true;
                        btnPayNow.CssClass = btnPayNow.CssClass.Replace(" disabled", "");
                    }
                    if (btnCard != null)
                    {
                        btnCard.Enabled = true;
                        btnCard.CssClass = btnCard.CssClass.Replace(" disabled", "");
                    }
                    if (btnConfirmPayNow != null)
                    {
                        btnConfirmPayNow.Enabled = true;
                        btnConfirmPayNow.CssClass = btnConfirmPayNow.CssClass.Replace(" disabled", "");
                    }
                }
            }
        }

        protected void rptPlans_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string purchaseId = e.CommandArgument?.ToString();

            if (string.IsNullOrEmpty(purchaseId))
                return;

            switch (e.CommandName)
            {
                case "ShowPayNowModal":
                    ShowModal("payNowModal_" + purchaseId);
                    break;

                case "ShowCardModal":
                    ShowModal("cardModal_" + purchaseId);
                    break;

                case "ShowPayNowConfirmModal":
                    ShowModal("payNowConfirmModal_" + purchaseId);
                    break;

                    // You can add more commands if needed
            }
        }

        // Helper method to trigger Bootstrap modal display via JavaScript
        private void ShowModal(string modalId)
        {
            string script = $"var myModal = new bootstrap.Modal(document.getElementById('{modalId}')); myModal.show();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), modalId + "_show", script, true);
        }

    }
}
