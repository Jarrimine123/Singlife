using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Singlife
{
    public partial class EverApproveClaim : System.Web.UI.Page
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
                        e.PlanName, e.AdmissionDate, e.DischargeDate, e.HospitalName, e.WardType,
                        e.DidTestsBefore, e.DidFollowUpAfter, e.CpfUsed, e.DeclarationConfirmed,
                        e.HospitalDocPath, e.FollowupDocPath, e.OtherFilesPath, e.CreatedDate,
                        s.Comment, s.OutcomeFilePath
                    FROM EverCareClaims e
                    LEFT JOIN StaffEverClaims s ON e.ClaimID = s.ClaimID
                    WHERE e.ClaimID = @ClaimID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string html = $@"
                            <div class='value'><span class='label'>Plan:</span> {reader["PlanName"]}</div>
                            <div class='value'><span class='label'>Admission Date:</span> {FormatDate(reader["AdmissionDate"])}</div>
                            <div class='value'><span class='label'>Discharge Date:</span> {FormatDate(reader["DischargeDate"])}</div>
                            <div class='value'><span class='label'>Hospital Name:</span> {reader["HospitalName"]}</div>
                            <div class='value'><span class='label'>Ward Type:</span> {reader["WardType"]}</div>
                            <div class='value'><span class='label'>Did Tests Before:</span> {FormatBool(reader["DidTestsBefore"])}</div>
                            <div class='value'><span class='label'>Did Follow-Up After:</span> {FormatBool(reader["DidFollowUpAfter"])}</div>
                            <div class='value'><span class='label'>Used CPF:</span> {FormatBool(reader["CpfUsed"])}</div>
                            <div class='value'><span class='label'>Declaration Confirmed:</span> {FormatBool(reader["DeclarationConfirmed"])}</div>
                            <div class='value'><span class='label'>Submitted On:</span> {FormatDate(reader["CreatedDate"])}</div>";

                        // Uploaded file links
                        if (!reader.IsDBNull(reader.GetOrdinal("HospitalDocPath")) && !string.IsNullOrWhiteSpace(reader["HospitalDocPath"].ToString()))
                        {
                            string hospitalDoc = reader["HospitalDocPath"].ToString();
                            html += $"<div class='value'><span class='label'>Hospital Document:</span> <a href='/{hospitalDoc}' target='_blank'>📎 View File</a></div>";
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("FollowupDocPath")) && !string.IsNullOrWhiteSpace(reader["FollowupDocPath"].ToString()))
                        {
                            string followupDoc = reader["FollowupDocPath"].ToString();
                            html += $"<div class='value'><span class='label'>Follow-Up Document:</span> <a href='/{followupDoc}' target='_blank'>📎 View File</a></div>";
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("OtherFilesPath")) && !string.IsNullOrWhiteSpace(reader["OtherFilesPath"].ToString()))
                        {
                            string otherFiles = reader["OtherFilesPath"].ToString();
                            html += $"<div class='value'><span class='label'>Other Supporting Documents:</span> <a href='/{otherFiles}' target='_blank'>📎 View File</a></div>";
                        }

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
                                lnkOutcomeFile.NavigateUrl = ResolveUrl(filePath);
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
