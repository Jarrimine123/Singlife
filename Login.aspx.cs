using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT AccountID, PasswordHash FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedPassword = reader["PasswordHash"].ToString();
                        if (password == storedPassword)
                        {
                            // ✅ Save both AccountID and UserEmail to session
                            Session["UserEmail"] = email;
                            Session["AccountID"] = reader["AccountID"];

                            Response.Redirect("HomePage.aspx"); // Redirect to quote page or homepage
                        }
                        else
                        {
                            lblError.Text = "Incorrect password.";
                        }
                    }
                    else
                    {
                        lblError.Text = "Email not found.";
                    }
                }
            }
        }

    }
}

