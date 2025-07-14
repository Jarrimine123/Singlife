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
            if (!IsPostBack && Session["Email"] != null)
            {
                txtEmail.Text = Session["Email"].ToString();
                txtEmail.ReadOnly = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string preferred = rdoEmail.Checked ? "Email" : rdoPhone.Checked ? "Phone" : "";

            string connStr = ConfigurationManager.ConnectionStrings["SinglifeDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO InterestForm (Name, Email, PhoneNumber, PreferredContactMethod)
                                 VALUES (@Name, @Email, @PhoneNumber, @PreferredContactMethod)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                    cmd.Parameters.AddWithValue("@PreferredContactMethod", preferred);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Thanks! We’ll be in touch soon.');", true);

            txtName.Text = "";
            if (Session["Email"] == null)
                txtEmail.Text = "";
            txtPhone.Text = "";
            rdoEmail.Checked = false;
            rdoPhone.Checked = false;
        }
    }
}
