using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class GiroApproval : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGiroRequests();
            }
        }

        protected void LoadGiroRequests(string accountFilter = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT RP.RecurringPaymentID, P.PurchaseID, P.AccountID, A.CustomerName,
                           RP.Amount, RP.NextBillingDate, RP.Status, RP.GiroFormPath
                    FROM RecurringPayment RP
                    INNER JOIN Purchases P ON RP.PurchaseID = P.PurchaseID
                    INNER JOIN Accounts A ON P.AccountID = A.AccountID
                    WHERE RP.PaymentMethod = 'GIRO'";

                if (!string.IsNullOrEmpty(accountFilter))
                {
                    sql += " AND P.AccountID = @AccountID";
                }
                if (dateFrom.HasValue)
                {
                    sql += " AND RP.NextBillingDate >= @DateFrom";
                }
                if (dateTo.HasValue)
                {
                    sql += " AND RP.NextBillingDate <= @DateTo";
                }

                sql += " ORDER BY RP.NextBillingDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(accountFilter))
                        cmd.Parameters.AddWithValue("@AccountID", accountFilter);

                    if (dateFrom.HasValue)
                        cmd.Parameters.AddWithValue("@DateFrom", dateFrom.Value);

                    if (dateTo.HasValue)
                        cmd.Parameters.AddWithValue("@DateTo", dateTo.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        rptGiroPending.DataSource = dt;
                        rptGiroPending.DataBind();
                    }
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string accountFilter = txtAccountFilter.Text.Trim();
            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            if (DateTime.TryParse(txtDateFrom.Text, out DateTime fromDate))
                dateFrom = fromDate;

            if (DateTime.TryParse(txtDateTo.Text, out DateTime toDate))
                dateTo = toDate;

            LoadGiroRequests(accountFilter, dateFrom, dateTo);
        }

        protected void rptGiroPending_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Approve" || e.CommandName == "Reject")
            {
                int recurringPaymentId = Convert.ToInt32(e.CommandArgument);
                string newStatus = e.CommandName == "Approve" ? "Active" : "Rejected";

                string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "UPDATE RecurringPayment SET Status = @Status WHERE RecurringPaymentID = @ID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@ID", recurringPaymentId);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = $"GIRO request {(newStatus == "Active" ? "approved" : "rejected")}.";

                LoadGiroRequests();
            }
        }
    }
}
