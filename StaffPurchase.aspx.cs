using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class StaffPurchase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPurchases();
            }
        }

        private void LoadPurchases(string name = "", string plan = "")
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT FullName, Email, Phone, ProductName, PlanName, CoverageAmount, 
                                        MonthlyPremium, CardLast4, PurchaseDate
                                 FROM Purchases
                                 WHERE 1 = 1";

                if (!string.IsNullOrWhiteSpace(name))
                    query += " AND FullName LIKE @Name";

                if (!string.IsNullOrWhiteSpace(plan))
                    query += " AND PlanName LIKE @Plan";

                query += " ORDER BY PurchaseDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrWhiteSpace(name))
                    cmd.Parameters.AddWithValue("@Name", "%" + name + "%");

                if (!string.IsNullOrWhiteSpace(plan))
                    cmd.Parameters.AddWithValue("@Plan", "%" + plan + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvPurchases.DataSource = dt;
                gvPurchases.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string plan = txtPlan.Text.Trim();
            LoadPurchases(name, plan);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtPlan.Text = "";
            LoadPurchases(); // reload all
        }
    }
}


