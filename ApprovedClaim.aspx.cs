using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class ApprovedClaim : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["claimId"], out int claimId))
                {
                    LoadClaimDetails(claimId);
                }
                else
                {
                    lblMessage.Text = "Invalid Claim ID.";
                    lblMessage.Visible = true;
                }
            }
        }

        private void LoadClaimDetails(int claimId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        c.PlanName, c.DiagnosisDate, c.TreatmentCountry, c.CancerType,
                        c.FirstDiagnosis, c.ReceivedTreatment, c.ConfirmedBySpecialist,
                        c.TreatmentStartDate, c.Hospital, c.TherapyType, c.UsedFreeScreening,
                        c.DeclarationConfirmed, c.CreatedDate,
                        sc.Comment, sc.OutcomeFilePath
                    FROM Claims c
                    LEFT JOIN StaffClaims sc ON c.ClaimID = sc.ClaimID
                    WHERE c.ClaimID = @ClaimID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string html = $@"
                            <div class='value'><span class='label'>Plan:</span> {reader["PlanName"]}</div>
                            <div class='value'><span class='label'>Diagnosis Date:</span> {FormatDate(reader["DiagnosisDate"])}</div>
                            <div class='value'><span class='label'>Treatment Country:</span> {reader["TreatmentCountry"]}</div>
                            <div class='value'><span class='label'>Cancer Type:</span> {reader["CancerType"]}</div>
                            <div class='value'><span class='label'>First Diagnosis:</span> {FormatBool(reader["FirstDiagnosis"])}</div>
                            <div class='value'><span class='label'>Received Treatment:</span> {FormatBool(reader["ReceivedTreatment"])}</div>
                            <div class='value'><span class='label'>Confirmed by Specialist:</span> {FormatBool(reader["ConfirmedBySpecialist"])}</div>
                            <div class='value'><span class='label'>Treatment Start Date:</span> {FormatDate(reader["TreatmentStartDate"])}</div>
                            <div class='value'><span class='label'>Hospital:</span> {reader["Hospital"]}</div>
                            <div class='value'><span class='label'>Therapy Type:</span> {reader["TherapyType"]}</div>
                            <div class='value'><span class='label'>Used Free Screening:</span> {FormatBool(reader["UsedFreeScreening"])}</div>
                            <div class='value'><span class='label'>Declaration Confirmed:</span> {FormatBool(reader["DeclarationConfirmed"])}</div>
                            <div class='value'><span class='label'>Submitted On:</span> {FormatDate(reader["CreatedDate"])}</div>";

                        litClaimInfo.Text = html;

                        bool hasComment = !reader.IsDBNull(reader.GetOrdinal("Comment")) && !string.IsNullOrWhiteSpace(reader["Comment"].ToString());
                        bool hasOutcomeFile = !reader.IsDBNull(reader.GetOrdinal("OutcomeFilePath")) && !string.IsNullOrWhiteSpace(reader["OutcomeFilePath"].ToString());

                        if (hasComment || hasOutcomeFile)
                        {
                            pnlOutcome.Visible = true;
                            litComment.Text = hasComment ? reader["Comment"].ToString() : "-";

                            if (hasOutcomeFile)
                            {
                                string filePath = reader["OutcomeFilePath"].ToString();
                                lnkOutcomeFile.Text = "Download Outcome File";
                                lnkOutcomeFile.NavigateUrl = ResolveUrl(filePath); // ✅ Fix: use full path from DB
                            }
                            else
                            {
                                lnkOutcomeFile.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = "Claim not found.";
                        lblMessage.Visible = true;
                    }
                }
            }
        }

        private string FormatDate(object dateObj)
        {
            if (dateObj == DBNull.Value) return "-";
            return Convert.ToDateTime(dateObj).ToString("dd MMM yyyy");
        }

        private string FormatBool(object boolObj)
        {
            if (boolObj == DBNull.Value) return "-";
            return Convert.ToBoolean(boolObj) ? "Yes" : "No";
        }
    }
}
