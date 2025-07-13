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
    ISNULL(RP.Status, 'None') AS GiroStatus,
    P.PurchaseDate,

    CASE 
        WHEN P.PaymentFrequency = 'Annual' THEN P.AnnualPremium
        ELSE P.MonthlyPremium
    END AS AmountDue,

    (SELECT COUNT(*) 
     FROM PaymentHistory PH 
     WHERE PH.PurchaseID = P.PurchaseID AND PH.Status = 'Success') + 1 AS TotalPaymentCount,

    CASE 
        WHEN RP.PaymentMethod = 'GIRO' AND RP.GIROFormPath IS NOT NULL AND LOWER(RP.Status) = 'active' THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS IsGiroActive

FROM Purchases P
LEFT JOIN RecurringPayment RP 
    ON RP.PurchaseID = P.PurchaseID 
    AND LOWER(RP.Status) IN ('active', 'pending')
WHERE P.AccountID = @AccountID
  AND P.PaymentFrequency <> 'One-Time'
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

                    // ✅ Cancel any existing GIRO entries
                    string cancelGiroSql = @"
                UPDATE RecurringPayment
                SET Status = 'Cancelled', NextBillingDate = NULL
                WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'GIRO' AND LOWER(Status) IN ('active', 'pending')";
                    using (SqlCommand cancelGiroCmd = new SqlCommand(cancelGiroSql, conn))
                    {
                        cancelGiroCmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cancelGiroCmd.ExecuteNonQuery();
                    }

                    // ✅ Insert or Update Card RecurringPayment WITHOUT Status
                    string checkSql = "SELECT COUNT(*) FROM RecurringPayment WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'Card'";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        int count = (int)cmdCheck.ExecuteScalar();

                        string sql;
                        if (count > 0)
                        {
                            sql = @"UPDATE RecurringPayment 
                            SET NextBillingDate = @NextDate, CardLast4 = @CardLast4, Amount = @Amount, Status = NULL
                            WHERE PurchaseID = @PurchaseID AND PaymentMethod = 'Card'";
                        }
                        else
                        {
                            sql = @"INSERT INTO RecurringPayment 
                            (AccountID, PurchaseID, Amount, PaymentMethod, CardLast4, NextBillingDate, PaymentFrequency)
                            VALUES (@AccountID, @PurchaseID, @Amount, 'Card', @CardLast4, @NextDate, 'Monthly')";
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

                    // ✅ Insert into PaymentHistory
                    string insertPaymentSql = @"
                INSERT INTO PaymentHistory (PurchaseID, Amount, PaymentDate, Method, Status)
                VALUES (@PurchaseID, @Amount, GETDATE(), 'Card', 'Success')";
                    using (SqlCommand cmdPayment = new SqlCommand(insertPaymentSql, conn))
                    {
                        cmdPayment.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmdPayment.Parameters.AddWithValue("@Amount", amountDue);
                        cmdPayment.ExecuteNonQuery();
                    }

                    // ✅ Update Purchases table's NextBillingDate
                    string updatePurchaseSql = @"UPDATE Purchases SET NextBillingDate = @NextDate WHERE PurchaseID = @PurchaseID";
                    using (SqlCommand cmd = new SqlCommand(updatePurchaseSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // ✅ UI Feedback
                lblMessage.CssClass = "text-success";
                lblMessage.Text = "Payment successful! Thank you.";

                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "Payment Confirmation",
                    $"Dear user,\n\nYour payment of ${amountDue:F2} for Purchase ID {purchaseId} has been successfully processed.\n\nThank you,\nSinglife Team"
                );

                // ✅ Clear input fields
                txtCardholderName.Text = "";
                txtCardNumber.Text = "";
                txtExpiry.Text = "";
                txtCVV.Text = "";

                LoadUserPlans(); // ✅ Refresh UI
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

            try
            {
                // 📎 Save receipt file if uploaded
                if (fuPayNowReceipt.HasFile)
                {
                    string ext = Path.GetExtension(fuPayNowReceipt.FileName).ToLower();
                    if (ext != ".jpg" && ext != ".png" && ext != ".pdf")
                    {
                        lblMessage.Text = "Only JPG, PNG, or PDF files are accepted for the receipt.";
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
                DateTime nextBillingDate = CalculateNextBillingDate(purchaseId);

                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // ✅ Insert into PaymentHistory (without AccountID/Remarks)
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO PaymentHistory
                (PurchaseID, Amount, PaymentDate, Method, Status)
                VALUES (@PurchaseID, @Amount, GETDATE(), 'PayNow', 'Success')", conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@Amount", amountDue);
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ Update Purchases with NextBillingDate
                    using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Purchases 
                SET NextBillingDate = @NextDate 
                WHERE PurchaseID = @PurchaseID", conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ Insert into PayNowPayments
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO PayNowPayments 
                (PurchaseID, ReferenceNumber, ReceiptPath)
                VALUES (@PurchaseID, @ReferenceNumber, @ReceiptPath)", conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@ReferenceNumber", txtPayNowRef.Text.Trim());
                        cmd.Parameters.AddWithValue("@ReceiptPath", receiptFileName ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                // ✅ Success feedback
                lblMessage.CssClass = "text-success";
                lblMessage.Text = "PayNow payment confirmed successfully! Thank you.";

                // ✅ Send email notification
                SendEmailNotification(
                    Session["UserEmail"]?.ToString() ?? "user@example.com",
                    "PayNow Payment Confirmation",
                    $"Your PayNow payment of ${amountDue:F2} for Purchase ID {purchaseId} has been recorded successfully."
                );

                txtPayNowRef.Text = "";
                // ⚠️ FileUpload control cannot be cleared without a page reload

                LoadUserPlans(); // Refresh UI
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Failed to confirm PayNow payment: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("❌ PayNow Exception: " + ex.ToString());
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

                    if (existingId != null)
                    {
                        // 🛠️ UPDATE only GIROFormPath and set status to Pending
                        string updateSql = @"UPDATE RecurringPayment 
                    SET GiroFormPath = @GiroFormPath, Status = 'Pending'
                    WHERE RecurringPaymentID = @RecurringPaymentID";

                        using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@GiroFormPath", filename);
                            cmd.Parameters.AddWithValue("@RecurringPaymentID", Convert.ToInt32(existingId));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // 🛠️ INSERT only basic data without NextBillingDate or Amount
                        string insertSql = @"INSERT INTO RecurringPayment 
                    (AccountID, PurchaseID, PaymentMethod, GiroFormPath, PaymentFrequency, Status)
                    VALUES (@AccountID, @PurchaseID, 'GIRO', @GiroFormPath, 'Monthly', 'Pending')";

                        using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                            cmd.Parameters.AddWithValue("@GiroFormPath", filename);
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
                var fromAddress = new MailAddress("singlifeteam@gmail.com", "Singlife Team");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "lfpafqorspawhzag"; // Gmail App Password

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
            catch (Exception ex)
            {
                Response.Write("<p style='color:red;'>Email Send Error: " + ex.Message + "</p>");
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

                // Get controls
                var phGiroStatus = (PlaceHolder)e.Item.FindControl("phGiroStatus");
                var lblGiroStatusText = (Label)e.Item.FindControl("lblGiroStatusText");
                var btnCancelGiro = (Button)e.Item.FindControl("btnCancelGiro");
                var lblGiroCancelMessage = (Label)e.Item.FindControl("lblGiroCancelMessage");

                var btnPayNow = (Button)e.Item.FindControl("btnPayNow");
                var btnCard = (Button)e.Item.FindControl("btnCard");
                var btnConfirmPayNow = (Button)e.Item.FindControl("btnConfirmPayNow");
                var btnGiroUpload = (Button)e.Item.FindControl("btnGiroUpload");

                // Read Giro Status
                string giroStatus = row["GiroStatus"] != DBNull.Value ? row["GiroStatus"].ToString().ToLower() : "";
                string paymentMethod = row["PaymentMethod"] != DBNull.Value ? row["PaymentMethod"].ToString().ToLower() : "";

                bool isGiroActive = giroStatus == "active";
                bool isGiroPending = giroStatus == "pending";
                bool isGiroOn = isGiroActive || isGiroPending;

                // Show GIRO status and message
                if (phGiroStatus != null)
                {
                    phGiroStatus.Visible = (paymentMethod == "giro") && isGiroOn;

                    if (phGiroStatus.Visible)
                    {
                        if (lblGiroStatusText != null)
                        {
                            lblGiroStatusText.Text = isGiroActive
                                ? "GIRO Payment Active - Auto deduction enabled"
                                : "GIRO Pending - Awaiting approval";
                        }

                        if (btnCancelGiro != null)
                            btnCancelGiro.Visible = true;

                        if (lblGiroCancelMessage != null)
                            lblGiroCancelMessage.Text = "";
                    }
                }

                // Disable PayNow & Card buttons if GIRO is active or pending
                bool disablePayments = isGiroOn;

                ToggleButtonState(btnPayNow, !disablePayments);
                ToggleButtonState(btnCard, !disablePayments);
                ToggleButtonState(btnConfirmPayNow, !disablePayments);

                // GIRO upload only disabled if GIRO is active
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

            int accountId = 0;
            string userEmail = "user@example.com";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // 🔍 Step 1: Fetch AccountID and Email
                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT U.AccountID, U.Email 
                FROM Purchases P
                INNER JOIN Users U ON P.AccountID = U.AccountID
                WHERE P.PurchaseID = @PurchaseID", conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                accountId = Convert.ToInt32(reader["AccountID"]);
                                userEmail = reader["Email"].ToString();
                            }
                        }
                    }

                    if (accountId == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ No AccountID found for PurchaseID {purchaseId}");
                        return;
                    }

                    // 💳 Step 2: Insert into PaymentHistory
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO PaymentHistory (PurchaseID, Amount, PaymentDate, Method, Status)
                VALUES (@PurchaseID, @Amount, GETDATE(), 'GIRO', 'Success')", conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@Amount", amountDue);
                        cmd.ExecuteNonQuery();
                    }

                    // 🔁 Step 3: Update RecurringPayment only if GIRO is Active
                    using (SqlCommand cmd = new SqlCommand(@"
                UPDATE RecurringPayment 
                SET NextBillingDate = @NextDate
                WHERE PurchaseID = @PurchaseID AND LOWER(PaymentMethod) = 'giro' AND LOWER(Status) = 'active'", conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] RecurringPayment updated: {rowsAffected}");
                    }

                    // 📅 Step 4: Update Purchases
                    using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Purchases SET NextBillingDate = @NextDate WHERE PurchaseID = @PurchaseID", conn))
                    {
                        cmd.Parameters.AddWithValue("@NextDate", nextBillingDate);
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 📧 Step 5: Notify user
                SendEmailNotification(
                    userEmail,
                    "GIRO Payment Successful",
                    $"Dear user,\n\nWe have successfully deducted ${amountDue:F2} via GIRO for Purchase ID {purchaseId}.\n\nThank you,\nSinglife Team"
                );

                LoadUserPlans(); // Refresh UI if applicable
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ProcessGiroPayment failed: {ex.Message}");
            }
        }

    }
}
