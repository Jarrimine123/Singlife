using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

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
            int accountId = GetLoggedInAccountID(); // Replace with actual logic

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ClaimID, PlanName, ModifiedDate FROM Claims WHERE AccountID = @AccountID AND Status = 'DRAFT' ORDER BY ModifiedDate DESC", conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                rptDrafts.DataSource = reader;
                rptDrafts.DataBind();

                lblNoDrafts.Visible = !reader.HasRows;
            }
        }

        protected void rptDrafts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int claimId = Convert.ToInt32(e.CommandArgument);
                string connStr = ConfigurationManager.ConnectionStrings["YourDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Claims WHERE ClaimID = @ClaimID AND Status = 'DRAFT'", conn);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadDrafts(); // Refresh list
            }
        }

        private int GetLoggedInAccountID()
        {
            // TODO: Replace this with actual session/user logic
            return 1;
        }
    }
}
