using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace Singlife
{
    public partial class OncoshieldPlan : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadReviews();
                CheckIfUserCanReview();
            }
        }

        private void LoadReviews(string ratingFilter = "all")
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = @"
                    SELECT R.ReviewID, R.AccountID, U.Name, R.Rating, R.ReviewText, R.ReviewDate
                    FROM ProductReviews R
                    INNER JOIN Users U ON R.AccountID = U.AccountID
                    WHERE R.PlanName = @PlanName AND R.IsApproved = 1";

                if (ratingFilter != "all")
                {
                    if (ratingFilter == "5")
                        query += " AND R.Rating = 5";
                    else
                        query += " AND R.Rating >= @RatingFilter";
                }

                query += " ORDER BY R.ReviewDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PlanName", "OncoShield");

                    if (ratingFilter != "all" && ratingFilter != "5")
                        cmd.Parameters.AddWithValue("@RatingFilter", Convert.ToInt32(ratingFilter));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ReviewsRepeater.DataSource = dt;
                    ReviewsRepeater.DataBind();
                }
            }
        }

        protected void ddlFilterStars_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = ddlFilterStars.SelectedValue;
            LoadReviews(selectedValue);
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                lblMessage.Text = "You must be logged in to submit a review.";
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            int rating = Convert.ToInt32(ddlRating.SelectedValue);
            string reviewText = txtReview.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    INSERT INTO ProductReviews (AccountID, PlanName, Rating, ReviewText)
                    VALUES (@AccountID, @PlanName, @Rating, @ReviewText)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@PlanName", "OncoShield");
                    cmd.Parameters.AddWithValue("@Rating", rating);
                    cmd.Parameters.AddWithValue("@ReviewText", reviewText);
                    cmd.ExecuteNonQuery();
                }
            }

            lblMessage.Text = "✅ Your review has been submitted and is pending approval.";
            txtReview.Text = "";
            reviewForm.Visible = false;

            LoadReviews(); // Refresh display after submit
        }

        private void CheckIfUserCanReview()
        {
            if (Session["AccountID"] == null)
            {
                reviewForm.Visible = false;
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Check if user purchased the plan
                string purchaseQuery = @"
                    SELECT COUNT(*) FROM Purchases
                    WHERE AccountID = @AccountID AND PlanName = @PlanName";

                using (SqlCommand cmdPurchase = new SqlCommand(purchaseQuery, conn))
                {
                    cmdPurchase.Parameters.AddWithValue("@AccountID", accountId);
                    cmdPurchase.Parameters.AddWithValue("@PlanName", "OncoShield");
                    int purchaseCount = (int)cmdPurchase.ExecuteScalar();

                    if (purchaseCount == 0)
                    {
                        reviewForm.Visible = false; // No purchase, no review
                        return;
                    }
                }

                // Check if user already submitted a review and it's pending approval
                string reviewPendingQuery = @"
                    SELECT COUNT(*) FROM ProductReviews
                    WHERE AccountID = @AccountID AND PlanName = @PlanName AND IsApproved = 0";

                using (SqlCommand cmdReview = new SqlCommand(reviewPendingQuery, conn))
                {
                    cmdReview.Parameters.AddWithValue("@AccountID", accountId);
                    cmdReview.Parameters.AddWithValue("@PlanName", "OncoShield");
                    int pendingCount = (int)cmdReview.ExecuteScalar();

                    // If pending review exists, hide form
                    reviewForm.Visible = (pendingCount == 0);
                }
            }
        }

        protected string GetStarHtml(int rating)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rating; i++)
            {
                sb.Append("<i class='fas fa-star'></i>");
            }
            for (int i = rating; i < 5; i++)
            {
                sb.Append("<i class='far fa-star'></i>");
            }
            return sb.ToString();
        }
    }
}
