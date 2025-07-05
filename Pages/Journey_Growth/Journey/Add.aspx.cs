using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Singlife.Data;
using Singlife.Model;

namespace Singlife.Pages.Journey_Growth.Journey
{
    public partial class Add : System.Web.UI.Page
    {

        protected int accountID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] != null)
            {
                accountID = Convert.ToInt32(Session["AccountID"]);

                if (!IsPostBack)
                {
                    BindJourneyMappings();
                }
            }
            else
            {
                // Handle missing session, e.g., redirect to login
                Response.Redirect("~/Login.aspx");
            }
        }

        private void BindJourneyMappings()
        {
            JourneyMappingData dataLayer = new JourneyMappingData();
            gvJourneyMappings.DataSource = dataLayer.ReadJourneyMapping(accountID);
            gvJourneyMappings.DataBind();
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int year = int.Parse(txtYear.Text.Trim());
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();

            JourneyMappingData dataLayer = new JourneyMappingData();
            bool success = dataLayer.AddJourneyMapping(accountID, year, title, description);

            if (success)
            {
                lblMessage.Text = "Journey mapping added successfully!";
                // Optionally clear form fields:
                txtTitle.Text = "";
                txtYear.Text = "";
                txtDescription.Text = "";
                BindJourneyMappings();
            }
            else
            {
                lblMessage.Text = "Error: Could not add journey mapping.";
                lblMessage.CssClass = "text-danger";
            }
        }
    }
}