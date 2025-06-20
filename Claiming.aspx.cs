using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Singlife
{
    public partial class Claiming : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!IsValidSubmission())
            {
                lblError.Text = "Please fill in all required fields before submitting.";
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false;
            SaveClaim("SUBMITTED");
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveClaim("DRAFT");
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChooseClaim.aspx");
        }

        private void SaveClaim(string status)
        {
            if (Session["AccountID"] == null || !int.TryParse(Session["AccountID"].ToString(), out int accountId))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO Claims (
                                    AccountID, PlanName, DiagnosisDate, TreatmentCountry, CancerType,
                                    FirstDiagnosis, ReceivedTreatment, ConfirmedBySpecialist,
                                    TreatmentStartDate, Hospital, TherapyType, UsedFreeScreening,
                                    DeclarationConfirmed, TreatmentFilePath, ScreeningFilePath, OtherFilesPath, Status
                                ) VALUES (
                                    @AccountID, @PlanName, @DiagnosisDate, @TreatmentCountry, @CancerType,
                                    @IsFirstDiagnosis, @HasReceivedTreatment, @IsConfirmedBySpecialist,
                                    @TreatmentStartDate, @HospitalName, @TherapyType, @UsedFreeScreening,
                                    @Declaration, @FilePathTreatment, @FilePathScreening, @FilePathOthers, @Status
                                )";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);
                cmd.Parameters.AddWithValue("@PlanName", "OncoShield Plan");
                cmd.Parameters.AddWithValue("@DiagnosisDate", string.IsNullOrEmpty(diagnosisDate.Value) ? (object)DBNull.Value : diagnosisDate.Value);
                cmd.Parameters.AddWithValue("@TreatmentCountry", treatmentCountry.Value);
                cmd.Parameters.AddWithValue("@CancerType", cancerType.Value);
                cmd.Parameters.AddWithValue("@IsFirstDiagnosis", firstYes.Checked);
                cmd.Parameters.AddWithValue("@HasReceivedTreatment", treatmentYes.Checked);
                cmd.Parameters.AddWithValue("@IsConfirmedBySpecialist", confirmedYes.Checked);
                cmd.Parameters.AddWithValue("@TreatmentStartDate", string.IsNullOrEmpty(treatmentDate.Value) ? (object)DBNull.Value : treatmentDate.Value);
                cmd.Parameters.AddWithValue("@HospitalName", hospital.Value);
                cmd.Parameters.AddWithValue("@TherapyType", therapyType.Value);
                cmd.Parameters.AddWithValue("@UsedFreeScreening", screeningYes.Checked);
                cmd.Parameters.AddWithValue("@Declaration", declaration.Checked);
                cmd.Parameters.AddWithValue("@FilePathTreatment", SaveFile(fileUploadTreatment));
                cmd.Parameters.AddWithValue("@FilePathScreening", SaveFile(fileUploadScreening));
                cmd.Parameters.AddWithValue("@FilePathOthers", SaveFile(fileUploadOthers));
                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (status == "DRAFT")
                Response.Redirect("SaveAsDraft.aspx");
            else
                Response.Redirect("ChooseClaim.aspx");
        }

        private bool IsValidSubmission()
        {
            if (string.IsNullOrWhiteSpace(diagnosisDate.Value) ||
                string.IsNullOrWhiteSpace(treatmentCountry.Value) ||
                string.IsNullOrWhiteSpace(cancerType.Value))
                return false;

            if (!(firstYes.Checked || firstNo.Checked))
                return false;
            if (!(treatmentYes.Checked || treatmentNo.Checked))
                return false;
            if (!(confirmedYes.Checked || confirmedNo.Checked))
                return false;

            if (!declaration.Checked)
                return false;

            return true;
        }

        private string SaveFile(System.Web.UI.WebControls.FileUpload fileUpload)
        {
            if (fileUpload.HasFile)
            {
                string folderPath = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Path.GetFileName(fileUpload.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                fileUpload.SaveAs(filePath);

                return "Uploads/" + fileName;
            }
            return null;
        }
    }
}
