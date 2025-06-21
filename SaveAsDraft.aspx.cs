using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
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
            int accountId = GetLoggedInAccountID();
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT ClaimID, PlanName, ModifiedDate, 'Claims' AS Source FROM Claims
                    WHERE AccountID = @AccountID AND Status = 'DRAFT'
                    UNION
                    SELECT ClaimID, PlanName, ModifiedDate, 'EverCare' AS Source FROM EverCareClaims
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

        protected void rptDrafts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            int claimId = Convert.ToInt32(args[0]);
            string sourceTable = args[1];

            if (e.CommandName == "Delete")
            {
                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string deleteQuery = sourceTable == "Claims"
                        ? "DELETE FROM Claims WHERE ClaimID = @ClaimID AND Status = 'DRAFT';"
                        : "DELETE FROM EverCareClaims WHERE ClaimID = @ClaimID AND Status = 'DRAFT';";

                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadDrafts();
            }

            if (e.CommandName == "Edit")
            {
                if (sourceTable == "Claims")
                {
                    Response.Redirect($"ClaimForm.aspx?claimId={claimId}");
                }
                else if (sourceTable == "EverCare")
                {
                    Response.Redirect($"EverClaimForm.aspx?claimId={claimId}");
                }
            }
        }

        protected void rptDrafts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lnkEdit = (LinkButton)e.Item.FindControl("lnkEdit");
                var lnkDelete = (LinkButton)e.Item.FindControl("lnkDelete");

                DataRowView row = (DataRowView)e.Item.DataItem;
                int claimId = Convert.ToInt32(row["ClaimID"]);
                string source = row["Source"].ToString();

                // Store ClaimID and Source together
                lnkEdit.CommandArgument = $"{claimId}|{source}";
                lnkDelete.CommandArgument = $"{claimId}|{source}";
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
