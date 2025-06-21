

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Singlife
{
    public partial class ActionNeededClaim : System.Web.UI.Page
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
                        c.PlanName, c.DiagnosisDate, c.TreatmentCountry, c.CancerType,
                        c.FirstDiagnosis, c.ReceivedTreatment, c.ConfirmedBySpecialist,
                        c.TreatmentStartDate, c.Hospital, c.TherapyType, c.UsedFreeScreening,
                        c.DeclarationConfirmed, c.CreatedDate,
                        sc.Status, sc.Comment,
                        u.Email
                    FROM Claims c
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
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
                            <div class='value'><span class='label'>Diagnosis Date:</span> {FormatDate(reader["DiagnosisDate"])} </div>
                            <div class='value'><span class='label'>Treatment Country:</span> {reader["TreatmentCountry"]}</div>
                            <div class='value'><span class='label'>Cancer Type:</span> {reader["CancerType"]}</div>
                            <div class='value'><span class='label'>First Diagnosis:</span> {FormatBool(reader["FirstDiagnosis"])} </div>
                            <div class='value'><span class='label'>Received Treatment:</span> {FormatBool(reader["ReceivedTreatment"])} </div>
                            <div class='value'><span class='label'>Confirmed by Specialist:</span> {FormatBool(reader["ConfirmedBySpecialist"])} </div>
                            <div class='value'><span class='label'>Treatment Start Date:</span> {FormatDate(reader["TreatmentStartDate"])} </div>
                            <div class='value'><span class='label'>Hospital:</span> {reader["Hospital"]}</div>
                            <div class='value'><span class='label'>Therapy Type:</span> {reader["TherapyType"]}</div>
                            <div class='value'><span class='label'>Used Free Screening:</span> {FormatBool(reader["UsedFreeScreening"])} </div>
                            <div class='value'><span class='label'>Declaration Confirmed:</span> {FormatBool(reader["DeclarationConfirmed"])} </div>
                            <div class='value'><span class='label'>Submitted On:</span> {FormatDate(reader["CreatedDate"])}";

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
                lblMessage.Text = "Invalid claim ID.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }

            if (!fuDocument.HasFile)
            {
                lblMessage.Text = "Please select a file to upload.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }

            string folderPath = Server.MapPath("~/Uploads/Reloads/");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = $"Reload_{claimId}_{Guid.NewGuid()}{Path.GetExtension(fuDocument.FileName)}";
            string filePath = Path.Combine(folderPath, fileName);
            fuDocument.SaveAs(filePath);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    UPDATE Claims 
                    SET ReloadFilesPath = @FilePath, ModifiedDate = GETDATE()
                    WHERE ClaimID = @ClaimID;

                    UPDATE StaffClaims
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

            lblMessage.Text = "✅ File uploaded successfully. Your claim is now marked as 'Successfully Reuploaded'.<br /><a href='ClaimHistory.aspx' class='btn btn-sm btn-outline-primary mt-2'>← Back to Claim History</a>";
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
                message.From = new MailAddress("singlifeeeeeeke@gmail.com");
                message.Subject = "Claim Document Re-uploaded";
                message.Body = "We’ve received your updated document. Your claim status is now 'Successfully Reuploaded'. Our team will review it shortly.";
                message.IsBodyHtml = false;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new NetworkCredential("singlifeeeeeeke@gmail.com", "pnfupbxiznvokifd"); // App password
                client.EnableSsl = true;

                client.Send(message);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Upload succeeded, but failed to send email: " + ex.Message;
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
            }
        }

        private string FormatDate(object dateObj)
        {
            if (dateObj == DBNull.Value) return "-";
            return Convert.ToDateTime(dateObj).ToString("dd MMM yyyy");
        }

        private string FormatBool(object boolObj)
        {
            if (boolObj == DBNull.Value) return "-";
            return Convert.ToBoolean(boolObj) ? "Yes" : "No";
        }
    }
}
