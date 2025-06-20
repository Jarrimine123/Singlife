using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
                        c.*, u.Name,
                        sc.Status AS ReviewStatus,
                        sc.Comment AS ReviewComment,
                        sc.OutcomeFilePath
                    FROM Claims c
                    INNER JOIN Users u ON c.AccountID = u.AccountID
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
                    WHERE c.Status = 'SUBMITTED'
                    ORDER BY c.CreatedDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptClaims.DataSource = dt;
                rptClaims.DataBind();
            }
        }

        protected void rptClaims_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "UpdateStatus")
            {
                int claimId = Convert.ToInt32(e.CommandArgument);
                DropDownList ddlStatus = (DropDownList)e.Item.FindControl("ddlStatus");
                TextBox txtComment = (TextBox)e.Item.FindControl("txtComment");
                FileUpload fuOutcomeFile = (FileUpload)e.Item.FindControl("fuOutcomeFile");

                string newStatus = ddlStatus.SelectedValue;
                string comment = txtComment.Text.Trim();
                string outcomePath = null;

                // Save outcome file if uploaded
                if (fuOutcomeFile.HasFile)
                {
                    string fileExt = Path.GetExtension(fuOutcomeFile.FileName);
                    string fileName = "Outcome_" + claimId + "_" + Guid.NewGuid() + fileExt;
                    string savePath = Server.MapPath(uploadFolder + fileName);
                    Directory.CreateDirectory(Server.MapPath(uploadFolder)); // ensure folder exists
                    fuOutcomeFile.SaveAs(savePath);
                    outcomePath = uploadFolder + fileName;
                }

                SaveStaffReview(claimId, newStatus, comment, outcomePath);

                lblMessage.Text = "Claim review updated.";
                lblMessage.Visible = true;

                // No reload to preserve input
                LoadClaims();
            }
        }

        private void SaveStaffReview(int claimId, string status, string comment, string outcomePath)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string checkSql = "SELECT COUNT(*) FROM StaffClaims WHERE ClaimID = @ClaimID";
                SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@ClaimID", claimId);
                int exists = (int)checkCmd.ExecuteScalar();

                string sql;
                if (exists > 0)
                {
                    sql = @"UPDATE StaffClaims 
                            SET Status = @Status, Comment = @Comment, ReviewedDate = GETDATE(), 
                                OutcomeFilePath = ISNULL(@OutcomeFilePath, OutcomeFilePath)
                            WHERE ClaimID = @ClaimID";
                }
                else
                {
                    sql = @"INSERT INTO StaffClaims (ClaimID, StaffID, Status, Comment, ReviewedDate, OutcomeFilePath)
                            VALUES (@ClaimID, @StaffID, @Status, @Comment, GETDATE(), @OutcomeFilePath)";
                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Comment", comment);
                cmd.Parameters.AddWithValue("@OutcomeFilePath", (object)outcomePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StaffID", 1); // Replace with actual StaffID if available

                cmd.ExecuteNonQuery();
            }
        }

        protected string GetFileLink(object path)
        {
            string url = path?.ToString();
            if (!string.IsNullOrEmpty(url))
            {
                string fileName = Path.GetFileName(url);
                return $"<a href='{ResolveUrl(url)}' target='_blank'>{fileName}</a>";
            }
            return "";
        }
    }
}
