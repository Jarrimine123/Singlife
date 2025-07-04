using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace Singlife
{
    public partial class EditSubmitClaims : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        int claimId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["claimId"], out claimId))
            {
                lblError.Text = "Invalid claim ID.";
                lblError.Visible = true;
                return;
            }

            if (!IsPostBack)
            {
                LoadClaimData(claimId);
            }
        }

        private void LoadClaimData(int claimId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM Claims WHERE ClaimID = @ClaimID AND Status = 'Submitted'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        diagnosisDate.Value = reader["DiagnosisDate"] is DBNull ? "" : Convert.ToDateTime(reader["DiagnosisDate"]).ToString("yyyy-MM-dd");
                        treatmentCountry.Value = reader["TreatmentCountry"] is DBNull ? "" : reader["TreatmentCountry"].ToString();
                        cancerType.Value = reader["CancerType"] is DBNull ? "" : reader["CancerType"].ToString();

                        bool firstDiagnosis = reader["FirstDiagnosis"] is DBNull ? false : Convert.ToBoolean(reader["FirstDiagnosis"]);
                        firstYes.Checked = firstDiagnosis;
                        firstNo.Checked = !firstDiagnosis;

                        bool receivedTreatment = reader["ReceivedTreatment"] is DBNull ? false : Convert.ToBoolean(reader["ReceivedTreatment"]);
                        treatmentYes.Checked = receivedTreatment;
                        treatmentNo.Checked = !receivedTreatment;

                        bool confirmed = reader["ConfirmedBySpecialist"] is DBNull ? false : Convert.ToBoolean(reader["ConfirmedBySpecialist"]);
                        confirmedYes.Checked = confirmed;
                        confirmedNo.Checked = !confirmed;

                        treatmentDate.Value = reader["TreatmentStartDate"] is DBNull ? "" : Convert.ToDateTime(reader["TreatmentStartDate"]).ToString("yyyy-MM-dd");
                        hospital.Value = reader["Hospital"] is DBNull ? "" : reader["Hospital"].ToString();
                        therapyType.Value = reader["TherapyType"] is DBNull ? "" : reader["TherapyType"].ToString();

                        bool usedScreening = reader["UsedFreeScreening"] is DBNull ? false : Convert.ToBoolean(reader["UsedFreeScreening"]);
                        screeningYes.Checked = usedScreening;
                        screeningNo.Checked = !usedScreening;

                        bool declarationConfirmed = reader["DeclarationConfirmed"] is DBNull ? false : Convert.ToBoolean(reader["DeclarationConfirmed"]);
                        declaration.Checked = declarationConfirmed;

                        litTreatmentFile.Text = GenerateFileBlock(reader["TreatmentFilePath"] is DBNull ? null : reader["TreatmentFilePath"].ToString(), "Treatment");
                        litScreeningFile.Text = GenerateFileBlock(reader["ScreeningFilePath"] is DBNull ? null : reader["ScreeningFilePath"].ToString(), "Screening");
                        litOtherFiles.Text = GenerateFileBlock(reader["OtherFilesPath"] is DBNull ? null : reader["OtherFilesPath"].ToString(), "Other");
                    }
                    else
                    {
                        lblError.Text = "Claim not found or not editable.";
                        lblError.Visible = true;
                    }
                }
            }
        }

        private string GenerateFileBlock(string filePath, string type)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return $"<div class='text-muted'>No file uploaded.</div>";
            }
            string fileName = Path.GetFileName(filePath);
            return $@"
                <div class='mb-2'>
                    <a class='file-link text-success' href='/{filePath}' target='_blank'>&#128206; {fileName}</a><br />
                </div>";
        }

        protected void btnDeleteTreatment_Click(object sender, EventArgs e) => DeleteFile("TreatmentFilePath");
        protected void btnDeleteScreening_Click(object sender, EventArgs e) => DeleteFile("ScreeningFilePath");
        protected void btnDeleteOthers_Click(object sender, EventArgs e) => DeleteFile("OtherFilesPath");

        private void DeleteFile(string columnName)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand getCmd = new SqlCommand($"SELECT {columnName} FROM Claims WHERE ClaimID = @ClaimID", conn);
                getCmd.Parameters.AddWithValue("@ClaimID", claimId);
                var pathObj = getCmd.ExecuteScalar();
                string existingPath = pathObj is DBNull ? null : pathObj.ToString();

                if (!string.IsNullOrEmpty(existingPath))
                {
                    string fullPath = Server.MapPath("~/" + existingPath);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);

                    SqlCommand clearCmd = new SqlCommand($"UPDATE Claims SET {columnName} = NULL WHERE ClaimID = @ClaimID", conn);
                    clearCmd.Parameters.AddWithValue("@ClaimID", claimId);
                    clearCmd.ExecuteNonQuery();
                }
            }
            Response.Redirect(Request.RawUrl);
        }

        protected void btnUpdateClaim_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(diagnosisDate.Value, out DateTime diagDate))
            {
                lblError.Text = "Invalid diagnosis date.";
                lblError.Visible = true;
                return;
            }

            if (!declaration.Checked)
            {
                lblError.Text = "Please confirm the declaration.";
                lblError.Visible = true;
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT CreatedDate FROM Claims WHERE ClaimID = @ClaimID", conn);
                checkCmd.Parameters.AddWithValue("@ClaimID", claimId);
                var result = checkCmd.ExecuteScalar();
                if (result == null || (DateTime.Now - Convert.ToDateTime(result)).TotalDays > 2)
                {
                    lblError.Text = "Edit window has expired.";
                    lblError.Visible = true;
                    return;
                }
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    UPDATE Claims SET
                        DiagnosisDate = @DiagnosisDate,
                        TreatmentCountry = @TreatmentCountry,
                        CancerType = @CancerType,
                        FirstDiagnosis = @FirstDiagnosis,
                        ReceivedTreatment = @ReceivedTreatment,
                        ConfirmedBySpecialist = @ConfirmedBySpecialist,
                        TreatmentStartDate = @TreatmentStartDate,
                        Hospital = @Hospital,
                        TherapyType = @TherapyType,
                        UsedFreeScreening = @UsedFreeScreening,
                        DeclarationConfirmed = @DeclarationConfirmed,
                        ModifiedDate = GETDATE()
                    WHERE ClaimID = @ClaimID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DiagnosisDate", diagDate);
                cmd.Parameters.AddWithValue("@TreatmentCountry", treatmentCountry.Value);
                cmd.Parameters.AddWithValue("@CancerType", cancerType.Value);
                cmd.Parameters.AddWithValue("@FirstDiagnosis", firstYes.Checked);
                cmd.Parameters.AddWithValue("@ReceivedTreatment", treatmentYes.Checked);
                cmd.Parameters.AddWithValue("@ConfirmedBySpecialist", confirmedYes.Checked);

                if (string.IsNullOrEmpty(treatmentDate.Value))
                    cmd.Parameters.AddWithValue("@TreatmentStartDate", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@TreatmentStartDate", DateTime.Parse(treatmentDate.Value));

                cmd.Parameters.AddWithValue("@Hospital", hospital.Value);
                cmd.Parameters.AddWithValue("@TherapyType", therapyType.Value);
                cmd.Parameters.AddWithValue("@UsedFreeScreening", screeningYes.Checked);
                cmd.Parameters.AddWithValue("@DeclarationConfirmed", declaration.Checked);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            HandleFileUpload(fuTreatment, "TreatmentFilePath");
            HandleFileUpload(fuScreening, "ScreeningFilePath");
            HandleFileUpload(fuOthers, "OtherFilesPath");

            lblError.CssClass = "text-success";
            lblError.Text = "Claim updated successfully.";
            lblError.Visible = true;

            LoadClaimData(claimId);
        }

        private void HandleFileUpload(System.Web.UI.WebControls.FileUpload fu, string columnName)
        {
            if (fu.HasFile)
            {
                string ext = Path.GetExtension(fu.FileName);
                string filename = Guid.NewGuid().ToString() + ext;
                string folder = "~/Uploads/";
                string savePath = Path.Combine(Server.MapPath(folder), filename);
                string dbPath = "Uploads/" + filename;

                fu.SaveAs(savePath);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"UPDATE Claims SET {columnName} = @Path WHERE ClaimID = @ClaimID", conn);
                    cmd.Parameters.AddWithValue("@Path", dbPath);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
