using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class RetirementInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLearnMore_Click(object sender, EventArgs e)
        {
            Response.Redirect("RetirementDetails.aspx");
        }

        protected void btnCalculator_Click(object sender, EventArgs e)
        {
            Response.Redirect("RetirementCalculators.aspx");
        }

        protected void btnContact_Click(object sender, EventArgs e)
        {
            Response.Redirect("GetInTouch.aspx");
        }

    }
}