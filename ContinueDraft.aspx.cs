using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Singlife
{
    public partial class ContinueDraft : Page
    {
        protected int claimId = 0;
        protected int accountId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            accountId = Convert.ToInt32(Session["AccountID"]);

            if (!IsPostBack)
            {
                if (!int.TryParse(Request.QueryString["claimId"], out claimId))
                {
                    Response.Redirect("SaveAsDraft.aspx");
                    return;
                }

                ViewState["claimId"] = claimId;

                LoadDraft(claimId, accountId);
            }
            else
            {
                if (ViewState["claimId"] != null)
                    claimId = (int)ViewState["claimId"];
                else
                    Response.Redirect("SaveAsDraft.aspx");
            }
        }

        private void LoadDraft(int claimId, int accountId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM Claims WHERE ClaimID = @ClaimID AND AccountID = @AccountID AND Status = 'DRAFT'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    diagnosisDate.Value = reader["DiagnosisDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DiagnosisDate"]).ToString("yyyy-MM-dd");
                    treatmentCountry.Value = reader["TreatmentCountry"]?.ToString() ?? "";
                    cancerType.Value = reader["CancerType"]?.ToString() ?? "";

                    bool isFirstDiagnosis = reader["FirstDiagnosis"] != DBNull.Value && Convert.ToBoolean(reader["FirstDiagnosis"]);
                    firstYes.Checked = isFirstDiagnosis;
                    firstNo.Checked = !isFirstDiagnosis;

                    bool hasReceivedTreatment = reader["ReceivedTreatment"] != DBNull.Value && Convert.ToBoolean(reader["ReceivedTreatment"]);
                    treatmentYes.Checked = hasReceivedTreatment;
                    treatmentNo.Checked = !hasReceivedTreatment;

                    bool isConfirmed = reader["ConfirmedBySpecialist"] != DBNull.Value && Convert.ToBoolean(reader["ConfirmedBySpecialist"]);
                    confirmedYes.Checked = isConfirmed;
                    confirmedNo.Checked = !isConfirmed;

                    treatmentDate.Value = reader["TreatmentStartDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["TreatmentStartDate"]).ToString("yyyy-MM-dd");
                    hospital.Value = reader["Hospital"]?.ToString() ?? "";
                    therapyType.Value = reader["TherapyType"]?.ToString() ?? "";

                    bool usedScreening = reader["UsedFreeScreening"] != DBNull.Value && Convert.ToBoolean(reader["UsedFreeScreening"]);
                    screeningYes.Checked = usedScreening;
                    screeningNo.Checked = !usedScreening;

                    declaration.Checked = reader["DeclarationConfirmed"] != DBNull.Value && Convert.ToBoolean(reader["DeclarationConfirmed"]);
                }
                else
                {
                    Response.Redirect("SaveAsDraft.aspx");
                }
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            string uploadedFilePath = SaveUploadedFile();
            SaveClaim("DRAFT", uploadedFilePath);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!IsValidSubmission())
            {
                lblError.Text = "Please fill in all required fields before submitting.";
                lblError.Visible = true;
                return;
            }

            string uploadedFilePath = SaveUploadedFile();
            SaveClaim("SUBMITTED", uploadedFilePath);
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            Response.Redirect("SaveAsDraft.aspx");
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

        private string SaveUploadedFile()
        {
            if (fileUploadTreatment.HasFile)
            {
                string folderPath = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = Path.GetFileName(fileUploadTreatment.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                fileUploadTreatment.SaveAs(filePath);
                return "Uploads/" + fileName;
            }

            return null;
        }

        private void SaveClaim(string status, string filePath)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

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
                        TreatmentFilePath = @TreatmentFilePath,
                        Status = @Status,
                        ModifiedDate = GETDATE()
                    WHERE ClaimID = @ClaimID AND AccountID = @AccountID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@DiagnosisDate", string.IsNullOrEmpty(diagnosisDate.Value) ? (object)DBNull.Value : diagnosisDate.Value);
                cmd.Parameters.AddWithValue("@TreatmentCountry", treatmentCountry.Value);
                cmd.Parameters.AddWithValue("@CancerType", cancerType.Value);
                cmd.Parameters.AddWithValue("@FirstDiagnosis", firstYes.Checked);
                cmd.Parameters.AddWithValue("@ReceivedTreatment", treatmentYes.Checked);
                cmd.Parameters.AddWithValue("@ConfirmedBySpecialist", confirmedYes.Checked);
                cmd.Parameters.AddWithValue("@TreatmentStartDate", string.IsNullOrEmpty(treatmentDate.Value) ? (object)DBNull.Value : treatmentDate.Value);
                cmd.Parameters.AddWithValue("@Hospital", hospital.Value);
                cmd.Parameters.AddWithValue("@TherapyType", therapyType.Value);
                cmd.Parameters.AddWithValue("@UsedFreeScreening", screeningYes.Checked);
                cmd.Parameters.AddWithValue("@DeclarationConfirmed", declaration.Checked);
                cmd.Parameters.AddWithValue("@TreatmentFilePath", string.IsNullOrEmpty(filePath) ? (object)DBNull.Value : filePath);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    lblError.Text = "Error: Could not update the claim. Please try again.";
                    lblError.Visible = true;
                }
                else
                {
                    if (status == "DRAFT")
                        Response.Redirect("SaveAsDraft.aspx");
                    else
                        Response.Redirect("SubmitSucces.aspx");
                }
            }
        }
    }
}
