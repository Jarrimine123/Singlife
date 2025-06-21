using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Singlife
{
    public partial class ClaimHistory : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadClaimHistory();
            }
        }

        private void LoadClaimHistory(string planFilter = "", string statusFilter = "")
        {
            int userId = Convert.ToInt32(Session["AccountID"] ?? 1);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        c.ClaimID, c.PlanName, c.CreatedDate,
                        ISNULL(sc.Status, 'Received') AS ReviewStatus,
                        sc.Comment
                    FROM Claims c
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
                    WHERE c.AccountID = @UserID";

                if (!string.IsNullOrEmpty(planFilter))
                    sql += " AND c.PlanName LIKE @PlanName";

                if (!string.IsNullOrEmpty(statusFilter))
                    sql += " AND ISNULL(sc.Status, 'Received') = @Status";

                sql += " ORDER BY c.CreatedDate DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                if (!string.IsNullOrEmpty(planFilter))
                    cmd.Parameters.AddWithValue("@PlanName", "%" + planFilter + "%");
                if (!string.IsNullOrEmpty(statusFilter))
                    cmd.Parameters.AddWithValue("@Status", statusFilter);

                conn.Open();
                rptClaimHistory.DataSource = cmd.ExecuteReader();
                rptClaimHistory.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string planName = txtSearchPlan.Text.Trim();
            string status = ddlStatus.SelectedValue;
            LoadClaimHistory(planName, status);
        }

        public string GetStatusClass(object statusObj)
        {
            string status = (statusObj ?? "Received").ToString().ToLower();
            if (status == "approved") return "status-approved";
            if (status == "action needed") return "status-action";
            return "status-received";
        }

        public string ShowCommentIfNeeded(object statusObj, object commentObj)
        {
            string status = (statusObj ?? "").ToString().ToLower();
            string comment = commentObj?.ToString();
            if (status == "action needed" && !string.IsNullOrWhiteSpace(comment))
            {
                return $"<div class='claim-comment'><strong>Comment:</strong> {comment}</div>";
            }
            return string.Empty;
        }

        public string ShowEditButton(object claimIdObj, object createdDateObj, object statusObj)
        {
            string status = (statusObj ?? "").ToString().ToLower();
            DateTime createdDate = Convert.ToDateTime(createdDateObj);
            int claimId = Convert.ToInt32(claimIdObj);

            if (status == "received" && (DateTime.Now - createdDate).TotalDays <= 2)
            {
                return $"<a class='edit-link' href='EditClaim.aspx?claimId={claimId}'>Edit Claim</a>";
            }
            else if (status == "approved")
            {
                return $"<a class='edit-link' href='ApprovedClaim.aspx?claimId={claimId}'>&#8594; View Details</a>";
            }
            else if (status == "action needed")
            {
                return $"<a class='edit-link' href='ActionNeededClaim.aspx?claimId={claimId}'>&#8594; View Details</a>";
            }

            return string.Empty;
        }

    }
}
