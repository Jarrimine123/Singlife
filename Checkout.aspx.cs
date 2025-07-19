using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using static System.Collections.Specialized.BitVector32;

namespace Singlife
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set your Stripe secret key from config
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"];

            if (!IsPostBack)
            {
                if (Session["AccountID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (!string.IsNullOrEmpty(Request.QueryString["product"]))
                {
                    LoadSingleQuoteFromQuery();
                }
                else
                {
                    LoadCartItems();
                }
            }
        }

        private void LoadSingleQuoteFromQuery()
        {
            string productName = Request.QueryString["product"];
            decimal.TryParse(Request.QueryString["coverage"], out decimal coverage);
            decimal.TryParse(Request.QueryString["annual"], out decimal annualPremium);
            decimal.TryParse(Request.QueryString["monthly"], out decimal monthlyPremium);
            string frequency = Request.QueryString["frequency"] ?? "Annual";

            DataTable dt = new DataTable();
            dt.Columns.Add("ProductName");
            dt.Columns.Add("PlanName");
            dt.Columns.Add("CoverageAmount", typeof(decimal));
            dt.Columns.Add("AnnualPremium", typeof(decimal));
            dt.Columns.Add("MonthlyPremium", typeof(decimal));
            dt.Columns.Add("PaymentFrequency");

            DataRow row = dt.NewRow();
            row["ProductName"] = "Medical Insurance";
            row["PlanName"] = productName;
            row["CoverageAmount"] = coverage;
            row["AnnualPremium"] = annualPremium;
            row["MonthlyPremium"] = monthlyPremium;
            row["PaymentFrequency"] = frequency;

            dt.Rows.Add(row);
            ViewState["CartItems"] = dt;

            gvOrderSummary.DataSource = dt;
            gvOrderSummary.DataBind();

            decimal totalPremium = (frequency == "Monthly") ? monthlyPremium : annualPremium;
            lblTotalMonthly.Text = totalPremium.ToString("C");
        }

        private void LoadCartItems()
        {
            int accountId = Convert.ToInt32(Session["AccountID"]);
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT ProductName, PlanName, CoverageAmount, AnnualPremium, PaymentFrequency 
                                 FROM CartItems WHERE AccountID = @AccountID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ViewState["CartItems"] = dt;
                gvOrderSummary.DataSource = dt;
                gvOrderSummary.DataBind();

                decimal totalPremium = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string freq = row["PaymentFrequency"].ToString();
                    decimal annual = Convert.ToDecimal(row["AnnualPremium"]);
                    totalPremium += (freq == "Monthly") ? (annual / 12) : annual;
                }

                lblTotalMonthly.Text = totalPremium.ToString("C");
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address))
            {
                ShowAlert("⚠️ Please fill in all fields.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowAlert("❌ Please enter a valid email address.");
                return;
            }

            DataTable dt = ViewState["CartItems"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                ShowAlert("🛒 Your cart is empty.");
                return;
            }

            try
            {
                var domain = Request.Url.GetLeftPart(UriPartial.Authority);

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = domain + "/ThankYou.aspx?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "/Checkout.aspx",
                    CustomerEmail = email,
                    Metadata = new Dictionary<string, string>
                    {
                        { "AccountID", Session["AccountID"].ToString() },
                        { "FullName", name },
                        { "Email", email },
                        { "Phone", phone },
                        { "Address", address }
                    },
                    LineItems = new List<SessionLineItemOptions>()
                };

                foreach (DataRow row in dt.Rows)
                {
                    decimal amountDecimal = (row["PaymentFrequency"].ToString() == "Monthly") ?
                        Convert.ToDecimal(row["AnnualPremium"]) / 12 : Convert.ToDecimal(row["AnnualPremium"]);

                    long amountCents = (long)(amountDecimal * 100);

                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "sgd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{row["ProductName"]} - {row["PlanName"]}"
                            },
                            UnitAmount = amountCents,
                        },
                        Quantity = 1,
                    });
                }

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Redirect(session.Url);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error creating payment session: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        private void ShowAlert(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message}');", true);
        }

        public string GetCoverageDisplay(object dataItem)
        {
            var row = dataItem as System.Data.DataRowView;
            if (row == null) return "";

            string product = row["ProductName"]?.ToString() ?? "";
            decimal amount = row["CoverageAmount"] != DBNull.Value ? Convert.ToDecimal(row["CoverageAmount"]) : 0;

            return product == "Travel Insurance" ? $"{amount:N0} days" : $"SGD {amount:N0}";
        }

        public string GetPremiumDisplay(object dataItem)
        {
            var row = dataItem as System.Data.DataRowView;
            if (row == null) return "";

            string product = row["ProductName"]?.ToString() ?? "";
            decimal annual = row["AnnualPremium"] != DBNull.Value ? Convert.ToDecimal(row["AnnualPremium"]) : 0;

            decimal monthly = 0;
            if (row.Row.Table.Columns.Contains("MonthlyPremium") && row["MonthlyPremium"] != DBNull.Value)
                monthly = Convert.ToDecimal(row["MonthlyPremium"]);
            else
                monthly = annual / 12;

            string frequency = row["PaymentFrequency"]?.ToString() ?? "Annual";

            if (product == "Travel Insurance")
                return $"SGD {annual:F2} (One-Time)";
            else
                return frequency == "Monthly"
                    ? $"SGD {monthly:F2} / mo"
                    : $"SGD {annual:F2} / yr";
        }
    }
}
