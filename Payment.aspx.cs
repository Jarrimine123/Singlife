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
    ISNULL(RP.Status, 'None') AS GiroStatus, -- ✅ Ensure available for Eval
    P.PurchaseDate,

    CASE 
        WHEN P.PaymentFrequency = 'Annual' THEN P.AnnualPremium
        ELSE P.MonthlyPremium
    END AS AmountDue,

    -- ✅ Total successful payments +1 for display
    (SELECT COUNT(*) 
     FROM PaymentHistory PH 
     WHERE PH.PurchaseID = P.PurchaseID AND PH.Status = 'Success') + 1 AS TotalPaymentCount,

    -- ✅ Flag for whether GIRO is active
    CASE 
        WHEN RP.PaymentMethod = 'GIRO' AND RP.GIROFormPath IS NOT NULL AND LOWER(RP.Status) = 'active' THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS IsGiroActive

FROM Purchases P
LEFT JOIN RecurringPayment RP 
    ON RP.PurchaseID = P.PurchaseID 
    AND LOWER(RP.Status) IN ('active', 'pending')
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

                    string updatePurchaseSql = @"UPDATE Purchases SET NextBillingDate = @NextDate WHERE PurchaseID = @PurchaseID";
                    using (SqlCommand cmd = new SqlCommand(updatePurchaseSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.ExecuteNonQuery();
                    }
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

                    string updatePurchaseSql = @"UPDATE Purchases SET NextBillingDate = @NextDate WHERE PurchaseID = @PurchaseID";
                    using (SqlCommand cmd = new SqlCommand(updatePurchaseSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.ExecuteNonQuery();
                    }

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
            Label lblMessage = (Label)item.FindControl("lblGiroUploadMessage");

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
                                WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'GIRO'";

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
                lblMessage.Text = "GIRO form uploaded. Awaiting approval.";

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

                // 🔍 Get controls
                PlaceHolder phGiroStatus = (PlaceHolder)e.Item.FindControl("phGiroStatus");
                Label lblGiroStatusText = (Label)e.Item.FindControl("lblGiroStatusText");
                Button btnCancelGiro = (Button)e.Item.FindControl("btnCancelGiro");
                Button btnPayNow = (Button)e.Item.FindControl("btnPayNow");
                Button btnCard = (Button)e.Item.FindControl("btnCard");
                Button btnConfirmPayNow = (Button)e.Item.FindControl("btnConfirmPayNow");
                Button btnGiroUpload = (Button)e.Item.FindControl("btnGiroUpload");

                // 🧠 Check GIRO status
                string giroStatus = row["GiroStatus"] != DBNull.Value ? row["GiroStatus"].ToString().ToLower() : "";
                bool isGiroActive = giroStatus == "active";
                bool isGiroPending = giroStatus == "pending";

                // ✅ Show GIRO status and Cancel button if Active or Pending
                if (phGiroStatus != null && lblGiroStatusText != null && btnCancelGiro != null)
                {
                    if (isGiroActive || isGiroPending)
                    {
                        phGiroStatus.Visible = true;

                        if (isGiroActive)
                            lblGiroStatusText.Text = "GIRO is active for this plan.";
                        else
                            lblGiroStatusText.Text = "GIRO form uploaded. Awaiting approval.";

                        btnCancelGiro.Visible = true; // ✅ Show cancel button for both active and pending
                    }
                    else
                    {
                        phGiroStatus.Visible = false;
                        btnCancelGiro.Visible = false;
                    }
                }

                // 🚫 Disable PayNow & Card if GIRO is active or pending
                bool disablePayments = isGiroActive || isGiroPending;

                ToggleButtonState(btnPayNow, !disablePayments);
                ToggleButtonState(btnCard, !disablePayments);
                ToggleButtonState(btnConfirmPayNow, !disablePayments);

                // 📝 GIRO Upload only disabled when active
                ToggleButtonState(btnGiroUpload, !isGiroActive);
            }
        }


        // ✅ Utility: Add or remove 'disabled' CSS class
        private string AddDisabledClass(string cssClass)
        {
            return cssClass.Contains("disabled") ? cssClass : cssClass.Trim() + " disabled";
        }

        private string RemoveDisabledClass(string cssClass)
        {
            return cssClass.Replace("disabled", "").Trim();
        }

        private void ToggleButtonState(Button btn, bool enabled)
        {
            if (btn == null) return;

            btn.Enabled = enabled;
            btn.CssClass = enabled ? RemoveDisabledClass(btn.CssClass) : AddDisabledClass(btn.CssClass);
        }

        protected void rptPlans_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument?.ToString(), out int purchaseId))
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

                case "ShowGiroModal":
                    ShowModal("giroModal_" + purchaseId);
                    break;

                case "ProcessGiroPayment":
                    ProcessGiroPayment(purchaseId);
                    break;

                case "CancelGiro":
                    {
                        // 🛠️ Ensure lblGiroCancelMessage exists
                        Label lblMessage = (Label)e.Item.FindControl("lblGiroCancelMessage");
                        if (lblMessage == null)
                        {
                            lblMessage = new Label();
                            e.Item.Controls.Add(lblMessage);
                        }

                        CancelGiro(purchaseId, lblMessage);
                        break;
                    }
            }
        }


        private void CancelGiro(int purchaseId, Label lblMessage)
        {
            if (lblMessage == null) return;

            lblMessage.Text = "";
            lblMessage.CssClass = "text-danger";

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                int rows = 0;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"UPDATE RecurringPayment 
                           SET Status = 'Cancelled', NextBillingDate = NULL 
                           WHERE PurchaseID = @PurchaseID 
                             AND LOWER(PaymentMethod) = 'giro' 
                             AND LOWER(Status) IN ('active', 'pending')";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        rows = cmd.ExecuteNonQuery();
                    }
                }

                if (rows > 0)
                {
                    lblMessage.CssClass = "text-success";
                    lblMessage.Text = "GIRO has been successfully cancelled.";

                    SendEmailNotification(
                        Session["UserEmail"]?.ToString() ?? "user@example.com",
                        "GIRO Payment Cancellation Confirmed",
                        $@"Dear Customer,

Your GIRO arrangement for Purchase ID {purchaseId} has been successfully cancelled.

You may now choose to pay using other available methods such as Card or PayNow.

Thank you,
Singlife Support Team"
                    );
                }
                else
                {
                    lblMessage.Text = $"No active or pending GIRO found to cancel for Purchase ID {purchaseId}.";
                }

                LoadUserPlans();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to cancel GIRO: " + ex.Message;
            }
        }



        // Helper method to trigger Bootstrap modal display via JavaScript
        private void ShowModal(string modalId)
        {
            string script = $"var myModal = new bootstrap.Modal(document.getElementById('{modalId}')); myModal.show();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), modalId + "_show", script, true);
        }


        private void ProcessGiroPayment(int purchaseId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
            decimal amountDue = GetAmountDueForPurchase(purchaseId);
            DateTime nextBillingDate = CalculateNextBillingDate(purchaseId);
            int accountId = Session["AccountID"] != null ? Convert.ToInt32(Session["AccountID"]) : 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // 1. Insert into PaymentHistory
                    string insertPaymentSql = @"
                INSERT INTO PaymentHistory (AccountID, PurchaseID, Amount, PaymentDate, Method, Status, Remarks)
                VALUES (@AccountID, @PurchaseID, @Amount, GETDATE(), 'GIRO', 'Success', 'Auto deduction')";
                    using (SqlCommand cmd = new SqlCommand(insertPaymentSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@Amount", amountDue);
                        int inserted = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] PaymentHistory inserted: {inserted}");
                    }

                    // 2. Update RecurringPayment with case-insensitive match
                    string updateRecurringSql = @"
                UPDATE RecurringPayment 
                SET NextBillingDate = @NextDate
                WHERE PurchaseID = @PurchaseID 
                  AND LOWER(PaymentMethod) = 'giro' 
                  AND LOWER(Status) = 'active'";
                    int updatedRecurring = 0;
                    using (SqlCommand cmd = new SqlCommand(updateRecurringSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        updatedRecurring = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] RecurringPayment updated: {updatedRecurring}");
                    }

                    // 3. Update Purchases (optional but good to track)
                    string updatePurchaseSql = @"
                UPDATE Purchases 
                SET NextBillingDate = @NextDate 
                WHERE PurchaseID = @PurchaseID";
                    int updatedPurchase = 0;
                    using (SqlCommand cmd = new SqlCommand(updatePurchaseSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        updatedPurchase = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Purchases updated: {updatedPurchase}");
                    }

                    if (updatedRecurring == 0)
                    {
                        // If GIRO deduction logic didn’t match any records
                        System.Diagnostics.Debug.WriteLine($"[WARNING] No active GIRO found for PurchaseID {purchaseId}");
                        // Optional: show user message
                        // lblErrorMessage.Text = "GIRO deduction failed. Please ensure GIRO is active.";
                    }
                }

                // Send confirmation email
                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "GIRO Payment Successful",
                    $"Dear user,\n\nWe have successfully deducted ${amountDue:F2} via GIRO for Purchase ID {purchaseId}.\n\nThank you,\nSinglife Team"
                );

                LoadUserPlans(); // Refresh UI
            }
            catch (Exception ex)
            {
                // Optional: log or show error
                System.Diagnostics.Debug.WriteLine($"[ERROR] GIRO Payment failed: {ex.Message}");
            }
        }



    }
}
