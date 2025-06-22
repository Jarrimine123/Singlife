using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Singlife
{
    public partial class SuccessfulEverCareUpload : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && int.TryParse(Request.QueryString["claimId"], out int claimId))
            {
                LoadEverCareClaimDetails(claimId);
            }
        }

        private void LoadEverCareClaimDetails(int claimId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        ec.PlanName, ec.AdmissionDate, ec.DischargeDate, ec.HospitalName,
                        ec.WardType, ec.DidTestsBefore, ec.DidFollowUpAfter, ec.CpfUsed,
                        ec.DeclarationConfirmed, ec.CreatedDate,
                        ec.HospitalDocPath, ec.FollowupDocPath, ec.OtherFilesPath,
                        se.Status, se.Comment, se.OutcomeFilePath,
                        u.Name
                    FROM EverCareClaims ec
                    LEFT JOIN StaffEverClaims se ON ec.ClaimID = se.ClaimID
                    LEFT JOIN Users u ON ec.AccountID = u.AccountID
                    WHERE ec.ClaimID = @ClaimID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string FormatBool(object val) => val == DBNull.Value ? "-" : (Convert.ToBoolean(val) ? "Yes" : "No");
                        string FormatDate(object val) => val == DBNull.Value ? "-" : Convert.ToDateTime(val).ToString("dd MMM yyyy");

                        string BuildFileLink(string label, object val)
                        {
                            if (val == DBNull.Value || string.IsNullOrEmpty(val.ToString()))
                                return $"<div class='value'><span class='label'>{label}:</span> None</div>";

                            string url = ResolveUrl("~/" + val.ToString());
                            return $"<div class='value'><span class='label'>{label}:</span> <a href='{url}' target='_blank' rel='noopener noreferrer'>View File</a></div>";
                        }

                        string html = "<div class='info-panel'>";
                        html += $"<div class='value'><span class='label'>Patient Name:</span> {reader["Name"]}</div>";
                        html += $"<div class='value'><span class='label'>Plan Name:</span> {reader["PlanName"]}</div>";
                        html += $"<div class='value'><span class='label'>Admission Date:</span> {FormatDate(reader["AdmissionDate"])}</div>";
                        html += $"<div class='value'><span class='label'>Discharge Date:</span> {FormatDate(reader["DischargeDate"])}</div>";
                        html += $"<div class='value'><span class='label'>Hospital Name:</span> {reader["HospitalName"]}</div>";
                        html += $"<div class='value'><span class='label'>Ward Type:</span> {reader["WardType"]}</div>";
                        html += $"<div class='value'><span class='label'>Did Tests Before Admission:</span> {FormatBool(reader["DidTestsBefore"])}</div>";
                        html += $"<div class='value'><span class='label'>Did Follow-up After Discharge:</span> {FormatBool(reader["DidFollowUpAfter"])}</div>";
                        html += $"<div class='value'><span class='label'>Used CPF:</span> {FormatBool(reader["CpfUsed"])}</div>";
                        html += $"<div class='value'><span class='label'>Declaration Confirmed:</span> {FormatBool(reader["DeclarationConfirmed"])}</div>";
                        html += $"<div class='value'><span class='label'>Created Date:</span> {FormatDate(reader["CreatedDate"])}</div>";

                        html += $"<div class='value'><span class='label'>Staff Review Status:</span> {reader["Status"]}</div>";
                        html += $"<div class='value'><span class='label'>Staff Comment:</span> {reader["Comment"]}</div>";

                        html += BuildFileLink("Hospital Document", reader["HospitalDocPath"]);
                        html += BuildFileLink("Follow-up Document", reader["FollowupDocPath"]);
                        html += BuildFileLink("Other Files", reader["OtherFilesPath"]);
                        html += BuildFileLink("Outcome File (Staff)", reader["OutcomeFilePath"]);

                        html += "</div>";
                        litEverCareDetails.Text = html;
                    }
                }
            }
        }
    }
}
