using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Singlife
{
    public partial class ClaimHistory : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClaims();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadClaims();
        }

        private void LoadClaims()
        {
            int accountId = Convert.ToInt32(Session["AccountID"]);
            string planFilter = txtSearchPlan.Text.Trim();
            string statusFilter = ddlStatus.SelectedValue;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string baseSql = @"
                    SELECT 
                        c.ClaimID, c.PlanName, c.CreatedDate, 
                        ISNULL(sc.Status, 'Received') AS ReviewStatus, sc.Comment,
                        'Normal' AS ClaimType
                    FROM Claims c
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
                    WHERE c.AccountID = @AccountID

                    UNION ALL

                    SELECT 
                        ec.ClaimID, ec.PlanName, ec.CreatedDate, 
                        ISNULL(sec.Status, 'Received') AS ReviewStatus, sec.Comment,
                        'EverCare' AS ClaimType
                    FROM EverCareClaims ec
                    LEFT JOIN StaffEverClaims sec ON ec.ClaimID = sec.ClaimID
                    WHERE ec.AccountID = @AccountID
                ";

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT * FROM (");
                sqlBuilder.Append(baseSql);
                sqlBuilder.Append(") AS AllClaims WHERE 1=1 ");

                if (!string.IsNullOrEmpty(planFilter))
                {
                    sqlBuilder.Append(" AND PlanName LIKE @PlanFilter ");
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    sqlBuilder.Append(" AND ReviewStatus = @StatusFilter ");
                }

                sqlBuilder.Append(" ORDER BY CreatedDate DESC ");

                SqlCommand cmd = new SqlCommand(sqlBuilder.ToString(), conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                if (!string.IsNullOrEmpty(planFilter))
                {
                    cmd.Parameters.AddWithValue("@PlanFilter", "%" + planFilter + "%");
                }
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    cmd.Parameters.AddWithValue("@StatusFilter", statusFilter);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptClaimHistory.DataSource = dt;
                rptClaimHistory.DataBind();
            }
        }

        public string GetStatusClass(object statusObj)
        {
            string status = statusObj?.ToString()?.ToLower() ?? "received";

            if (status == "approved")
                return "status-approved";
            else if (status == "action needed")
                return "status-action";
            else if (status == "successfully reuploaded")
                return "status-success-reupload";
            else
                return "status-received";
        }

        public string ShowCommentIfNeeded(object statusObj, object commentObj)
        {
            string status = statusObj?.ToString()?.ToLower();
            if (status == "action needed" && commentObj != null && !string.IsNullOrWhiteSpace(commentObj.ToString()))
            {
                return $"<div class='claim-comment'>Staff Comment: {commentObj}</div>";
            }

            return "";
        }

        public string ShowEditButton(object claimIdObj, object createdDateObj, object statusObj, object claimTypeObj)
        {
            string status = statusObj?.ToString()?.ToLower() ?? "received";
            string claimType = claimTypeObj?.ToString()?.ToLower() ?? "normal";
            int claimId = Convert.ToInt32(claimIdObj);

            string editUrl = "";
            string viewUrl = "";

            if (claimType == "evercare")
            {
                editUrl = $"EverEditClaims.aspx?claimId={claimId}";
                viewUrl = $"EverApproveClaim.aspx?claimId={claimId}";
            }
            else
            {
                editUrl = $"EditSubmitClaims.aspx?claimId={claimId}";
                viewUrl = $"ApprovedClaim.aspx?claimId={claimId}";
            }

            if (status == "approved")
            {
                return $"<a class='edit-link' href='{viewUrl}'>→ View Details</a>";
            }
            else if (status == "successfully reuploaded")
            {
                string successUrl = GetSuccessUrl(claimId);
                return $"<a class='edit-link' href='{successUrl}'>→ View Details</a>";
            }
            else if (status == "action needed")
            {
                string actionNeededUrl = (claimType == "evercare")
                    ? $"EverActionNeeded.aspx?claimId={claimId}"
                    : $"ActionNeededClaim.aspx?claimId={claimId}";

                return $"<a class='edit-link' href='{actionNeededUrl}'>→ Action Needed: Upload</a>";
            }
            else if (status == "received")
            {
                DateTime createdDate = Convert.ToDateTime(createdDateObj);
                if ((DateTime.Now - createdDate).TotalDays <= 2)
                {
                    return $"<a class='edit-link' href='{editUrl}'>Edit Claim</a>";
                }
            }

            return "";
        }

        private string GetSuccessUrl(int claimId)
        {
            string result = $"SuccessfulUploaded.aspx?claimId={claimId}"; // default to normal
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string checkSql = "SELECT COUNT(*) FROM EverCareClaims WHERE ClaimID = @ClaimID";
                using (SqlCommand cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        result = $"SuccessfulEverCareUpload.aspx?claimId={claimId}";
                    }
                }
            }
            return result;
        }

        // Example method to submit new claim and insert default status
        public void SubmitNewClaim(string planName /* add more parameters as needed */)
        {
            int accountId = Convert.ToInt32(Session["AccountID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string insertClaimSql = @"
                    INSERT INTO Claims (AccountID, PlanName, CreatedDate)
                    VALUES (@AccountID, @PlanName, GETDATE());
                    SELECT SCOPE_IDENTITY();
                ";

                SqlCommand cmdClaim = new SqlCommand(insertClaimSql, conn);
                cmdClaim.Parameters.AddWithValue("@AccountID", accountId);
                cmdClaim.Parameters.AddWithValue("@PlanName", planName);

                int newClaimId = Convert.ToInt32(cmdClaim.ExecuteScalar());

                string insertStaffStatusSql = @"
                    INSERT INTO StaffClaims (ClaimID, Status, Comment)
                    VALUES (@ClaimID, 'Received', NULL);
                ";

                SqlCommand cmdStaff = new SqlCommand(insertStaffStatusSql, conn);
                cmdStaff.Parameters.AddWithValue("@ClaimID", newClaimId);
                cmdStaff.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
