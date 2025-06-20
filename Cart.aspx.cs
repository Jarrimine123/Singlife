using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["AccountID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }
                LoadCart();
            }
        }

        private void LoadCart()
        {
            int accountId = Convert.ToInt32(Session["AccountID"]);
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
            SELECT CartID, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium, PaymentFrequency, DateAdded 
            FROM CartItems 
            WHERE AccountID = @AccountID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        pnlCart.Visible = true;
                        pnlEmpty.Visible = false;

                        gvCart.DataSource = dt;
                        gvCart.DataBind();

                        btnCheckout.Enabled = true;
                    }
                    else
                    {
                        pnlCart.Visible = false;
                        pnlEmpty.Visible = true;
                        btnCheckout.Enabled = false;
                    }
                }
            }
        }

        protected void gvCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int cartId = Convert.ToInt32(gvCart.DataKeys[rowIndex].Value);

                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "DELETE FROM CartItems WHERE CartID = @CartID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CartID", cartId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadCart();
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            Response.Redirect("Checkout.aspx");
        }
    }
}
