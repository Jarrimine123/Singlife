using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Singlife
{
    public partial class SaveAsDraft : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDrafts();
            }
        }

        private void LoadDrafts()
        {
            int accountId = GetLoggedInAccountID();
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT ClaimID, PlanName, ModifiedDate, 'Claims' AS Source FROM Claims
                    WHERE AccountID = @AccountID AND Status = 'DRAFT'
                    UNION
                    SELECT ClaimID, PlanName, SubmissionDate AS ModifiedDate, 'EverCare' AS Source FROM EverCareClaims
                    WHERE AccountID = @AccountID AND Status = 'DRAFT'
                    ORDER BY ModifiedDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                rptDrafts.DataSource = reader;
                rptDrafts.DataBind();

                lblNoDrafts.Visible = !reader.HasRows;
            }
        }

        protected void rptDrafts_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int claimId = Convert.ToInt32(e.CommandArgument);
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string deleteQuery = @"
                        DELETE FROM Claims WHERE ClaimID = @ClaimID AND Status = 'DRAFT';
                        DELETE FROM EverCareClaims WHERE ClaimID = @ClaimID AND Status = 'DRAFT';";

                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadDrafts();
            }
        }

        private int GetLoggedInAccountID()
        {
            if (Session["AccountID"] != null)
            {
                return Convert.ToInt32(Session["AccountID"]);
            }
            else
            {
                Response.Redirect("Login.aspx");
                return 0;
            }
        }
    }
}
