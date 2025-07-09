using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace Singlife
{
    public partial class Checkout : System.Web.UI.Page
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

            // ✅ Changed: Use productName directly without appending " Plan"
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
                string query = @"SELECT CartID, ProductName, PlanName, CoverageAmount, AnnualPremium, PaymentFrequency 
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
            if (Session["AccountID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string cardNumber = txtCardNumber.Text.Trim();
            string expiry = txtExpiry.Text.Trim();
            string cvv = txtCVV.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(cardNumber) ||
                string.IsNullOrEmpty(expiry) || string.IsNullOrEmpty(cvv))
            {
                ShowAlert("⚠️ Please fill in all fields.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowAlert("❌ Please enter a valid email address.");
                return;
            }

            if (!long.TryParse(cardNumber, out _) || cardNumber.Length != 16)
            {
                ShowAlert("❌ Card number must be 16 digits.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(expiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                ShowAlert("❌ Expiry date must be in MM/YY format.");
                return;
            }

            if (!int.TryParse(cvv, out _) || (cvv.Length != 3 && cvv.Length != 4))
            {
                ShowAlert("❌ CVV must be 3 or 4 digits.");
                return;
            }

            DataTable dt = ViewState["CartItems"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                ShowAlert("🛒 Your cart is empty.");
                return;
            }

            int accountId = Convert.ToInt32(Session["AccountID"]);
            Guid purchaseGroupId = Guid.NewGuid();
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                foreach (DataRow row in dt.Rows)
                {
                    decimal annual = Convert.ToDecimal(row["AnnualPremium"]);
                    decimal monthly = annual / 12;
                    string frequency = row["PaymentFrequency"].ToString();
                    string paymentMethod = frequency == "Monthly" ? "Card Monthly" : "Card Annual";
                    DateTime purchaseDate = DateTime.Now;
                    DateTime nextBillingDate = frequency == "Monthly" ? purchaseDate.AddMonths(1) : purchaseDate.AddYears(1);

                    string insertQuery = @"
                        INSERT INTO Purchases 
                        (PurchaseGroupID, AccountID, FullName, Email, Phone, Address, ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium, PaymentMethod, CardLast4, PaymentFrequency, NextBillingDate)
                        VALUES 
                        (@PurchaseGroupID, @AccountID, @FullName, @Email, @Phone, @Address, @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium, @PaymentMethod, @CardLast4, @PaymentFrequency, @NextBillingDate)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseGroupID", purchaseGroupId);
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@ProductName", row["ProductName"]);
                        cmd.Parameters.AddWithValue("@PlanName", row["PlanName"]);
                        cmd.Parameters.AddWithValue("@CoverageAmount", row["CoverageAmount"]);
                        cmd.Parameters.AddWithValue("@AnnualPremium", annual);
                        cmd.Parameters.AddWithValue("@MonthlyPremium", monthly);
                        cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@CardLast4", cardNumber.Substring(cardNumber.Length - 4));
                        cmd.Parameters.AddWithValue("@PaymentFrequency", frequency);
                        cmd.Parameters.AddWithValue("@NextBillingDate", nextBillingDate);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (string.IsNullOrEmpty(Request.QueryString["product"]))
                {
                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM CartItems WHERE AccountID = @AccountID", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@AccountID", accountId);
                        deleteCmd.ExecuteNonQuery();
                    }
                }
            }

            SendEmailAlert(email, dt);
            Response.Redirect("ThankYou.aspx");
        }

        private void ShowAlert(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message}');", true);
        }

        private void SendEmailAlert(string email, DataTable cartItems)
        {
            if (string.IsNullOrEmpty(email) || cartItems == null || cartItems.Rows.Count == 0)
                return;

            try
            {
                string planSummary = "";
                foreach (DataRow row in cartItems.Rows)
                {
                    string planName = row["PlanName"].ToString();
                    string coverage = Convert.ToDecimal(row["CoverageAmount"]).ToString("C");
                    string frequency = row["PaymentFrequency"].ToString();
                    string premium = frequency == "Monthly"
                        ? (Convert.ToDecimal(row["AnnualPremium"]) / 12).ToString("C")
                        : Convert.ToDecimal(row["AnnualPremium"]).ToString("C");

                    planSummary += $"- {planName}: {coverage} ({frequency}, Premium: {premium})\n";
                }

                MailMessage message = new MailMessage();
                message.To.Add(email);
                message.From = new MailAddress("singlifeteam@gmail.com", "Singlife Team");
                message.Subject = "🛒 Singlife Insurance Purchase Confirmation";
                message.Body = $"Thank you for your purchase!\n\nYou’ve successfully purchased the following plan(s):\n\n{planSummary}" +
                               "\nYour policy will be processed and activated shortly.\n\nRegards,\nSinglife Team";
                message.IsBodyHtml = false;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new NetworkCredential("singlifeteam@gmail.com", "lfpafqorspawhzag");
                client.EnableSsl = true;

                client.Send(message);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Purchase succeeded, but email failed: " + ex.Message;
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
            }
        }

        protected string GetCoverageDisplay(object dataItem)
        {
            var row = dataItem as System.Data.DataRowView;
            if (row == null) return "";

            string product = row["ProductName"]?.ToString() ?? "";
            decimal amount = row["CoverageAmount"] != DBNull.Value ? Convert.ToDecimal(row["CoverageAmount"]) : 0;

            return product == "Travel Insurance" ? $"{amount:N0} days" : $"SGD {amount:N0}";
        }

        protected string GetPremiumDisplay(object dataItem)
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
