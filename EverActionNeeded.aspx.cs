using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Singlife
{
    public partial class EverActionNeeded : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && int.TryParse(Request.QueryString["claimId"], out int claimId))
            {
                LoadClaimDetails(claimId);
            }
        }

        private void LoadClaimDetails(int claimId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        c.PlanName, c.AdmissionDate, c.DischargeDate, c.HospitalName, c.WardType,
                        c.DidTestsBefore, c.DidFollowUpAfter, c.CpfUsed, c.DeclarationConfirmed, c.CreatedDate,
                        c.ReuploadFilePath,
                        sc.Status, sc.Comment,
                        u.Email
                    FROM EverCareClaims c
                    LEFT JOIN StaffEverClaims sc ON c.ClaimID = sc.ClaimID
                    LEFT JOIN Users u ON c.AccountID = u.AccountID
                    WHERE c.ClaimID = @ClaimID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        litClaimInfo.Text = $@"
                            <div class='value'><span class='label'>Plan:</span> {reader["PlanName"]}</div>
                            <div class='value'><span class='label'>Admission Date:</span> {FormatDate(reader["AdmissionDate"])} </div>
                            <div class='value'><span class='label'>Discharge Date:</span> {FormatDate(reader["DischargeDate"])} </div>
                            <div class='value'><span class='label'>Hospital Name:</span> {reader["HospitalName"]}</div>
                            <div class='value'><span class='label'>Ward Type:</span> {reader["WardType"]}</div>
                            <div class='value'><span class='label'>Did Tests Before:</span> {FormatBool(reader["DidTestsBefore"])} </div>
                            <div class='value'><span class='label'>Follow-Up After:</span> {FormatBool(reader["DidFollowUpAfter"])} </div>
                            <div class='value'><span class='label'>CPF Used:</span> {FormatBool(reader["CpfUsed"])} </div>
                            <div class='value'><span class='label'>Declaration Confirmed:</span> {FormatBool(reader["DeclarationConfirmed"])} </div>
                            <div class='value'><span class='label'>Submitted On:</span> {FormatDate(reader["CreatedDate"])}</div>";

                        // Show reuploaded file link if exists
                        if (reader["ReuploadFilePath"] != DBNull.Value && !string.IsNullOrEmpty(reader["ReuploadFilePath"].ToString()))
                        {
                            string reuploadPath = reader["ReuploadFilePath"].ToString();
                            litClaimInfo.Text += $"<div class='value'><span class='label'>Reuploaded File:</span> <a href='{ResolveUrl("~/" + reuploadPath)}' target='_blank'>Download</a></div>";
                        }

                        string status = reader["Status"]?.ToString()?.ToLower() ?? "received";
                        if (status == "action needed")
                        {
                            pnlOutcome.Visible = true;
                            litComment.Text = reader["Comment"].ToString();
                            pnlUpload.Visible = true;
                        }

                        ViewState["UserEmail"] = reader["Email"].ToString();
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["claimId"], out int claimId))
            {
                ShowError("Invalid claim ID.");
                return;
            }

            if (!fuDocument.HasFile)
            {
                ShowError("Please select a file to upload.");
                return;
            }

            string folderPath = Server.MapPath("~/Uploads/Reloads/");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = $"EverCareReload_{claimId}_{Guid.NewGuid()}{Path.GetExtension(fuDocument.FileName)}";
            string filePath = Path.Combine(folderPath, fileName);
            fuDocument.SaveAs(filePath);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    UPDATE EverCareClaims 
                    SET ReuploadFilePath = @FilePath, ModifiedDate = GETDATE()
                    WHERE ClaimID = @ClaimID;

                    UPDATE StaffEverClaims
                    SET Status = 'Successfully Reuploaded', ReviewedDate = GETDATE()
                    WHERE ClaimID = @ClaimID;";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FilePath", "Uploads/Reloads/" + fileName);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            SendEmailAlert(ViewState["UserEmail"]?.ToString());

            pnlUpload.Visible = false;
            pnlOutcome.Visible = false;
            btnSubmit.Enabled = false;

            lblMessage.Text = "✅ File uploaded successfully. Your EverCare claim is now marked as 'Successfully Reuploaded'.<br /><a href='ClaimHistory.aspx' class='btn btn-sm btn-outline-primary mt-2'>← Back to Claim History</a>";
            lblMessage.CssClass = "text-success";
            lblMessage.Visible = true;
        }

        private void SendEmailAlert(string email)
        {
            if (string.IsNullOrEmpty(email)) return;

            try
            {
                MailMessage message = new MailMessage();
                message.To.Add(email);
                message.From = new MailAddress("singlifeteam@gmail.com");
                message.Subject = "EverCare Claim Document Re-uploaded";
                message.Body = "We’ve received your updated document for EverCare. Your claim status is now 'Successfully Reuploaded'. Our team will review it shortly.";
                message.IsBodyHtml = false;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new NetworkCredential("singlifeteam@gmail.com", "lfpafqorspawhzag"); // App password
                client.EnableSsl = true;

                client.Send(message);
            }
            catch (Exception ex)
            {
                ShowError("Upload succeeded, but failed to send email: " + ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = "text-danger";
            lblMessage.Visible = true;
        }

        private string FormatDate(object dateObj)
        {
            return (dateObj == DBNull.Value) ? "-" : Convert.ToDateTime(dateObj).ToString("dd MMM yyyy");
        }

        private string FormatBool(object boolObj)
        {
            return (boolObj == DBNull.Value) ? "-" : (Convert.ToBoolean(boolObj) ? "Yes" : "No");
        }
    }
}
