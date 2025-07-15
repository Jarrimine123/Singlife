using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class GetInTouch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["email"] != null)
            {
                txtEmail.Text = Session["email"].ToString();
                txtEmail.ReadOnly = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string method = rdoEmail.Checked ? "Email" : "Phone";

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO InterestForm (FullName, Email, Phone, PreferredContact) VALUES (@Name, @Email, @Phone, @Method)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Method", method);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showModal();", true);
        }
    }
}
