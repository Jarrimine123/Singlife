using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class Claiming : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Optional: Load data for editing drafts, etc.
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveClaim("SUBMITTED");
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveClaim("DRAFT");
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            // Do not save to DB, just redirect
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
                                    IsFirstDiagnosis, HasReceivedTreatment, IsConfirmedBySpecialist,
                                    TreatmentStartDate, HospitalName, TherapyType, UsedFreeScreening,
                                    Declaration, FilePathTreatment, FilePathScreening, FilePathOthers, Status
                                ) VALUES (
                                    @AccountID, @PlanName, @DiagnosisDate, @TreatmentCountry, @CancerType,
                                    @IsFirstDiagnosis, @HasReceivedTreatment, @IsConfirmedBySpecialist,
                                    @TreatmentStartDate, @HospitalName, @TherapyType, @UsedFreeScreening,
                                    @Declaration, @FilePathTreatment, @FilePathScreening, @FilePathOthers, @Status
                                )";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);
                cmd.Parameters.AddWithValue("@PlanName", "OncoShield Plan");
                cmd.Parameters.AddWithValue("@DiagnosisDate", diagnosisDate.Value);
                cmd.Parameters.AddWithValue("@TreatmentCountry", treatmentCountry.Value);
                cmd.Parameters.AddWithValue("@CancerType", cancerType.Value);
                cmd.Parameters.AddWithValue("@IsFirstDiagnosis", firstYes.Checked);
                cmd.Parameters.AddWithValue("@HasReceivedTreatment", treatmentYes.Checked);
                cmd.Parameters.AddWithValue("@IsConfirmedBySpecialist", confirmedYes.Checked);
                cmd.Parameters.AddWithValue("@TreatmentStartDate", treatmentDate.Value);
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

            Response.Redirect("ChooseClaim.aspx");
        }

        private string SaveFile(FileUpload fileUpload)
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