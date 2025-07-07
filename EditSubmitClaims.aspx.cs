using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml.Linq;

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
                        diagnosisDate.Text = reader["DiagnosisDate"] is DBNull ? "" : Convert.ToDateTime(reader["DiagnosisDate"]).ToString("yyyy-MM-dd");
                        treatmentCountry.Text = reader["TreatmentCountry"] is DBNull ? "" : reader["TreatmentCountry"].ToString();
                        cancerType.Text = reader["CancerType"] is DBNull ? "" : reader["CancerType"].ToString();

                        bool firstDiagnosis = reader["FirstDiagnosis"] is DBNull ? false : Convert.ToBoolean(reader["FirstDiagnosis"]);
                        firstYes.Checked = firstDiagnosis;
                        firstNo.Checked = !firstDiagnosis;

                        bool receivedTreatment = reader["ReceivedTreatment"] is DBNull ? false : Convert.ToBoolean(reader["ReceivedTreatment"]);
                        treatmentYes.Checked = receivedTreatment;
                        treatmentNo.Checked = !receivedTreatment;

                        bool confirmed = reader["ConfirmedBySpecialist"] is DBNull ? false : Convert.ToBoolean(reader["ConfirmedBySpecialist"]);
                        confirmedYes.Checked = confirmed;
                        confirmedNo.Checked = !confirmed;

                        treatmentDate.Text = reader["TreatmentStartDate"] is DBNull ? "" : Convert.ToDateTime(reader["TreatmentStartDate"]).ToString("yyyy-MM-dd");
                        hospital.Text = reader["Hospital"] is DBNull ? "" : reader["Hospital"].ToString();
                        therapyType.SelectedValue = reader["TherapyType"] is DBNull ? "" : reader["TherapyType"].ToString();

                        bool usedScreening = reader["UsedFreeScreening"] is DBNull ? false : Convert.ToBoolean(reader["UsedFreeScreening"]);
                        screeningYes.Checked = usedScreening;
                        screeningNo.Checked = !usedScreening;

                        bool declarationConfirmed = reader["DeclarationConfirmed"] is DBNull ? false : Convert.ToBoolean(reader["DeclarationConfirmed"]);
                        declaration.Checked = declarationConfirmed;

                        litTreatmentFile.Text = GenerateFileBlock(reader["TreatmentFilePath"] is DBNull ? null : reader["TreatmentFilePath"].ToString());
                        litScreeningFile.Text = GenerateFileBlock(reader["ScreeningFilePath"] is DBNull ? null : reader["ScreeningFilePath"].ToString());
                        litOtherFiles.Text = GenerateFileBlock(reader["OtherFilesPath"] is DBNull ? null : reader["OtherFilesPath"].ToString());
                    }
                    else
                    {
                        lblError.Text = "Claim not found or not editable.";
                        lblError.Visible = true;
                    }
                }
            }
        }

        private string GenerateFileBlock(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return "<div class='text-muted'>No file uploaded.</div>";
            }
            string fileName = Path.GetFileName(filePath);
            return $@"
                <div class='mb-2'>
                    <a class='file-link text-success' href='/{filePath}' target='_blank'>&#128206; {fileName}</a><br />
                </div>";
        }

        protected void btnUpdateClaim_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(diagnosisDate.Text, out DateTime diagDate))
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
                cmd.Parameters.AddWithValue("@TreatmentCountry", treatmentCountry.Text);
                cmd.Parameters.AddWithValue("@CancerType", cancerType.Text);
                cmd.Parameters.AddWithValue("@FirstDiagnosis", firstYes.Checked);
                cmd.Parameters.AddWithValue("@ReceivedTreatment", treatmentYes.Checked);
                cmd.Parameters.AddWithValue("@ConfirmedBySpecialist", confirmedYes.Checked);

                if (string.IsNullOrEmpty(treatmentDate.Text))
                    cmd.Parameters.AddWithValue("@TreatmentStartDate", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@TreatmentStartDate", DateTime.Parse(treatmentDate.Text));

                cmd.Parameters.AddWithValue("@Hospital", hospital.Text);
                cmd.Parameters.AddWithValue("@TherapyType", therapyType.SelectedValue);
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

        private void HandleFileUpload(FileUpload fu, string columnName)
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
