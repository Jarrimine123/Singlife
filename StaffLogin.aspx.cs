using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class StaffLogin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }
    

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Hardcoded staff credentials
            string staffUsername = "admin";
            string staffPassword = "admin123"; // Change this to whatever you want

            if (username == staffUsername && password == staffPassword)
            {
                // You can redirect to a staff dashboard or admin page
                Response.Redirect("StaffHomePage.aspx");
            }
            else
            {
                lblError.Text = "Invalid staff credentials.";
            }
        }
    }
}