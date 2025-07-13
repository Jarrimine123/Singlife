using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

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
                            Session["UserEmail"] = email;
                            Session["AccountID"] = reader["AccountID"];
                            Response.Redirect("HomePage.aspx");
                        }
                        else
                        {
                            ShowCustomModal("Incorrect password.");
                        }
                    }
                    else
                    {
                        ShowCustomModal("Email not found.");
                    }
                }
            }
        }

        private void ShowCustomModal(string message)
        {
            string safeMessage = message.Replace("'", "\\'");
            string script = $"showModal('{safeMessage}');";
            ClientScript.RegisterStartupScript(this.GetType(), "CustomModal", script, true);
        }
    }
}
