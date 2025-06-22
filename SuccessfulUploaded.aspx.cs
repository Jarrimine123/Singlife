using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Singlife
{
    public partial class SuccessfulUploaded : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && int.TryParse(Request.QueryString["claimId"], out int claimId))
            {
                LoadClaimInfo(claimId);
            }
        }

        private void LoadClaimInfo(int claimId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        c.PlanName, c.DiagnosisDate, c.TreatmentCountry, c.CancerType,
                        c.FirstDiagnosis, c.ReceivedTreatment, c.ConfirmedBySpecialist,
                        c.TreatmentStartDate, c.Hospital, c.TherapyType, c.UsedFreeScreening,
                        c.DeclarationConfirmed, c.CreatedDate,
                        c.TreatmentFilePath, c.ScreeningFilePath, c.OtherFilesPath, c.ReloadFilesPath,
                        sc.Status, sc.Comment,
                        u.Name
                    FROM Claims c
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
                    LEFT JOIN Users u ON c.AccountID = u.AccountID
                    WHERE c.ClaimID = @ClaimID";

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
                            return $"<div class='value'><span class='label'>{label}:</span> <a href='{url}' target='_blank'>View File</a></div>";
                        }

                        string html = "<div class='info-panel'>";
                        html += $"<div class='value'><span class='label'>Patient Name:</span> {reader["Name"]}</div>";
                        html += $"<div class='value'><span class='label'>Plan:</span> {reader["PlanName"]}</div>";
                        html += $"<div class='value'><span class='label'>Diagnosis Date:</span> {FormatDate(reader["DiagnosisDate"])}</div>";
                        html += $"<div class='value'><span class='label'>Treatment Country:</span> {reader["TreatmentCountry"]}</div>";
                        html += $"<div class='value'><span class='label'>Cancer Type:</span> {reader["CancerType"]}</div>";
                        html += $"<div class='value'><span class='label'>First Diagnosis:</span> {FormatBool(reader["FirstDiagnosis"])}</div>";
                        html += $"<div class='value'><span class='label'>Received Treatment:</span> {FormatBool(reader["ReceivedTreatment"])}</div>";
                        html += $"<div class='value'><span class='label'>Confirmed by Specialist:</span> {FormatBool(reader["ConfirmedBySpecialist"])}</div>";
                        html += $"<div class='value'><span class='label'>Treatment Start Date:</span> {FormatDate(reader["TreatmentStartDate"])}</div>";
                        html += $"<div class='value'><span class='label'>Hospital:</span> {reader["Hospital"]}</div>";
                        html += $"<div class='value'><span class='label'>Therapy Type:</span> {reader["TherapyType"]}</div>";
                        html += $"<div class='value'><span class='label'>Used Free Screening:</span> {FormatBool(reader["UsedFreeScreening"])}</div>";
                        html += $"<div class='value'><span class='label'>Declaration Confirmed:</span> {FormatBool(reader["DeclarationConfirmed"])}</div>";
                        html += $"<div class='value'><span class='label'>Submitted On:</span> {FormatDate(reader["CreatedDate"])}</div>";

                        html += $"<div class='value'><span class='label'>Staff Review Status:</span> {reader["Status"]}</div>";
                        html += $"<div class='value'><span class='label'>Staff Comment:</span> {reader["Comment"]}</div>";

                        html += BuildFileLink("Treatment File", reader["TreatmentFilePath"]);
                        html += BuildFileLink("Screening File", reader["ScreeningFilePath"]);
                        html += BuildFileLink("Other File", reader["OtherFilesPath"]);
                        html += BuildFileLink("Reuploaded File", reader["ReloadFilesPath"]);

                        html += "</div>";

                        litClaimDetails.Text = html;
                    }
                }
            }
        }
    }
}
