using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class Staffclaim : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        private string uploadFolder = "~/Uploads/Outcomes/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClaims();
            }
        }

        private void LoadClaims()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        c.ClaimID, c.AccountID, c.PlanName, c.Status, c.DiagnosisDate, c.TreatmentCountry, c.CancerType,
                        c.FirstDiagnosis, c.ReceivedTreatment, c.ConfirmedBySpecialist, c.TreatmentStartDate, c.Hospital,
                        c.TherapyType, c.TreatmentFilePath, c.UsedFreeScreening, c.ScreeningFilePath, c.OtherFilesPath,
                        c.ReloadFilesPath, c.DeclarationConfirmed, NULL AS AdmissionDate, NULL AS DischargeDate,
                        NULL AS HospitalName, NULL AS WardType, NULL AS DidTestsBefore, NULL AS DidFollowUpAfter,
                        NULL AS CpfUsed, NULL AS HospitalDocPath, NULL AS FollowupDocPath, c.ClaimType, c.CreatedDate,
                        u.Name, sc.Status AS ReviewStatus, sc.Comment AS ReviewComment, sc.OutcomeFilePath
                    FROM Claims c
                    INNER JOIN Users u ON c.AccountID = u.AccountID
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
                    WHERE c.Status = 'SUBMITTED'

                    UNION ALL

                    SELECT 
                        ec.ClaimID, ec.AccountID, ec.PlanName, ec.Status, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
                        NULL, NULL, NULL, NULL, NULL, ec.OtherFilesPath, NULL, ec.DeclarationConfirmed, ec.AdmissionDate,
                        ec.DischargeDate, ec.HospitalName, ec.WardType, ec.DidTestsBefore, ec.DidFollowUpAfter,
                        ec.CpfUsed, ec.HospitalDocPath, ec.FollowupDocPath, ec.ClaimType, ec.CreatedDate, u.Name,
                        sec.Status AS ReviewStatus, sec.Comment AS ReviewComment, sec.OutcomeFilePath
                    FROM EverCareClaims ec
                    INNER JOIN Users u ON ec.AccountID = u.AccountID
                    LEFT JOIN StaffEverClaims sec ON ec.ClaimID = sec.ClaimID
                    WHERE ec.Status = 'SUBMITTED'

                    ORDER BY CreatedDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptClaims.DataSource = dt;
                rptClaims.DataBind();
            }
        }

        public string GetClaimDetails(object dataItemObj)
        {
            var dataItem = (DataRowView)dataItemObj;
            var html = new StringBuilder();

            foreach (DataColumn col in dataItem.DataView.Table.Columns)
            {
                string colName = col.ColumnName;
                if (colName == "ClaimID" || colName == "Name" || colName == "ReviewStatus" ||
                    colName == "ReviewComment" || colName == "OutcomeFilePath" || colName == "AccountID" ||
                    colName == "CreatedDate" || colName == "ClaimType")
                    continue;

                object value = dataItem[colName];
                if (value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                    continue;

                string label = Regex.Replace(colName, "([a-z])([A-Z])", "$1 $2");
                string raw = value.ToString();
                string display;

                if (value is DateTime dt)
                {
                    display = dt.ToString("yyyy-MM-dd");
                }
                else if (value is bool b)
                {
                    display = b ? "Yes" : "No";
                }
                else if (IsFilePath(raw))
                {
                    string url = ResolveUrl(raw.StartsWith("~") ? raw : "~/" + raw.TrimStart('/'));
                    string fileName = Path.GetFileName(raw);
                    display = $"<a href='{url}' target='_blank'>{fileName}</a>";
                }
                else
                {
                    display = HttpUtility.HtmlEncode(raw);
                }

                html.Append($"<div class='row-label'>{label}:</div><div class='row-value'>{display}</div>");
            }

            return html.ToString();
        }

        private bool IsFilePath(string path)
        {
            string[] extensions = { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx" };
            return !string.IsNullOrEmpty(path) && extensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        protected void rptClaims_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "UpdateStatus")
            {
                int claimId = Convert.ToInt32(e.CommandArgument);
                string claimType = ((HiddenField)e.Item.FindControl("hfClaimType")).Value;
                var ddlStatus = (DropDownList)e.Item.FindControl("ddlStatus");
                var txtComment = (TextBox)e.Item.FindControl("txtComment");
                var fuOutcomeFile = (FileUpload)e.Item.FindControl("fuOutcomeFile");

                string newStatus = ddlStatus.SelectedValue;
                string comment = txtComment.Text.Trim();
                string outcomePath = null;

                if (fuOutcomeFile.HasFile)
                {
                    string fileExt = Path.GetExtension(fuOutcomeFile.FileName);
                    string fileName = $"Outcome_{claimId}_{Guid.NewGuid()}{fileExt}";
                    string savePath = Server.MapPath(uploadFolder + fileName);
                    Directory.CreateDirectory(Server.MapPath(uploadFolder));
                    fuOutcomeFile.SaveAs(savePath);
                    outcomePath = uploadFolder + fileName;
                }

                SaveStaffReview(claimId, newStatus, comment, outcomePath, claimType);
                lblMessage.Text = "Claim review updated.";
                lblMessage.Visible = true;
                LoadClaims();
            }
        }

        private void SaveStaffReview(int claimId, string status, string comment, string outcomePath, string claimType)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string table = claimType == "EverCare" ? "StaffEverClaims" : "StaffClaims";

                string checkSql = $"SELECT COUNT(*) FROM {table} WHERE ClaimID = @ClaimID";
                SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@ClaimID", claimId);
                int exists = (int)checkCmd.ExecuteScalar();

                string sql = exists > 0 ?
                    $"UPDATE {table} SET Status = @Status, Comment = @Comment, ReviewedDate = GETDATE(), OutcomeFilePath = ISNULL(@OutcomeFilePath, OutcomeFilePath) WHERE ClaimID = @ClaimID" :
                    $"INSERT INTO {table} (ClaimID, StaffID, Status, Comment, ReviewedDate, OutcomeFilePath) VALUES (@ClaimID, @StaffID, @Status, @Comment, GETDATE(), @OutcomeFilePath)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Comment", comment);
                cmd.Parameters.AddWithValue("@OutcomeFilePath", (object)outcomePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StaffID", 1); // Placeholder staff ID

                cmd.ExecuteNonQuery();
            }
        }
    }
}
