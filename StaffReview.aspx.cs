using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Singlife
{
    public partial class StaffReview : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAllReviews();
                lblMessage.Text = "";
            }
        }

        private void LoadAllReviews()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT ReviewID, AccountID, PlanName, Rating, ReviewText, ReviewDate, IsApproved 
                    FROM ProductReviews 
                    ORDER BY ReviewDate DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    rptReviews.DataSource = reader;
                    rptReviews.DataBind();
                }
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            int reviewID = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);
            UpdateReviewApprovalStatus(reviewID, true);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            int reviewID = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);
            DeleteReview(reviewID);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int reviewID = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);
            DeleteReview(reviewID);
        }

        private void UpdateReviewApprovalStatus(int reviewID, bool approve)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "UPDATE ProductReviews SET IsApproved = @IsApproved WHERE ReviewID = @ReviewID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ReviewID", reviewID);
                    cmd.Parameters.AddWithValue("@IsApproved", approve ? 1 : 0);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        lblMessage.CssClass = "text-success";
                        lblMessage.Text = approve ? "Review approved successfully." : "Review marked as pending.";
                        LoadAllReviews();
                    }
                    else
                    {
                        lblMessage.CssClass = "text-danger";
                        lblMessage.Text = "Operation failed. Please try again.";
                    }
                }
            }
        }

        private void DeleteReview(int reviewID)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "DELETE FROM ProductReviews WHERE ReviewID = @ReviewID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ReviewID", reviewID);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        lblMessage.CssClass = "text-success";
                        lblMessage.Text = "Review deleted successfully.";
                        LoadAllReviews();
                    }
                    else
                    {
                        lblMessage.CssClass = "text-danger";
                        lblMessage.Text = "Delete operation failed. Please try again.";
                    }
                }
            }
        }
    }
}
