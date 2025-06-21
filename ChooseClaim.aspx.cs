using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class ChooseClaim : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPurchasedPlans();
            }
        }

        private void LoadPurchasedPlans()
        {
            if (Session["AccountID"] == null || !int.TryParse(Session["AccountID"].ToString(), out int accountId))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT ProductName, PlanName FROM Purchases WHERE AccountID = @AccountID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                rptPlans.DataSource = reader;
                rptPlans.DataBind();
            }
        }

        protected void rptPlans_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string planName = DataBinder.Eval(e.Item.DataItem, "PlanName")?.ToString();
                var lblDesc = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("lblShortDesc");

                if (lblDesc != null && !string.IsNullOrEmpty(planName))
                {
                    switch (planName.Trim())
                    {
                        case "EverCare Plan":
                            lblDesc.InnerText = "Covers hospital stays, surgery & specialist access — even ambulance rides.";
                            break;
                        case "OncoShield Plan":
                            lblDesc.InnerText = "Boosted cancer coverage, modern therapies & 1 free annual screening.";
                            break;
                        default:
                            lblDesc.InnerText = "Comprehensive insurance plan with tailored protection for your needs.";
                            break;
                    }
                }
            }
        }

        // 🔁 Dynamic claim page redirection logic
        protected string GetClaimPageUrl(string planName)
        {
            switch (planName.Trim())
            {
                case "EverCare Plan":
                    return "EverCareTimeClaim.aspx?plan=" + Server.UrlEncode(planName);
                case "OncoShield":
                    return "OncoShieldTimeClaim.aspx?plan=" + Server.UrlEncode(planName);
                default:
                    return "GenericClaim.aspx?plan=" + Server.UrlEncode(planName);
            }
        }
    }
}
