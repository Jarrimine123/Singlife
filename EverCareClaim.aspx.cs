using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Singlife
{
    public partial class EverCareClaim : System.Web.UI.Page
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
                string query = @"INSERT INTO EverCareClaims (
                                    AccountID, PlanName, AdmissionDate, DischargeDate, HospitalName, WardType,
                                    DidTestsBefore, DidFollowUpAfter, DeclarationConfirmed, CpfUsed,
                                    HospitalDocPath, FollowupDocPath, OtherFilesPath, Status
                                ) VALUES (
                                    @AccountID, @PlanName, @AdmissionDate, @DischargeDate, @HospitalName, @WardType,
                                    @DidTestsBefore, @DidFollowUpAfter, @DeclarationConfirmed, @CpfUsed,
                                    @HospitalDocPath, @FollowupDocPath, @OtherFilesPath, @Status
                                )";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountID", accountId);
                cmd.Parameters.AddWithValue("@PlanName", "EverCare Plan");
                cmd.Parameters.AddWithValue("@AdmissionDate", string.IsNullOrEmpty(admissionDate.Value) ? (object)DBNull.Value : admissionDate.Value);
                cmd.Parameters.AddWithValue("@DischargeDate", string.IsNullOrEmpty(dischargeDate.Value) ? (object)DBNull.Value : dischargeDate.Value);
                cmd.Parameters.AddWithValue("@HospitalName", string.IsNullOrWhiteSpace(hospitalName.Value) ? (object)DBNull.Value : hospitalName.Value);
                cmd.Parameters.AddWithValue("@WardType", string.IsNullOrWhiteSpace(wardType.Value) ? (object)DBNull.Value : wardType.Value);
                cmd.Parameters.AddWithValue("@DidTestsBefore", testsYes.Checked);
                cmd.Parameters.AddWithValue("@DidFollowUpAfter", followUpYes.Checked);
                cmd.Parameters.AddWithValue("@DeclarationConfirmed", declaration.Checked);
                cmd.Parameters.AddWithValue("@CpfUsed", cpfYes.Checked);

                string hospitalDocPath = SaveFile(fileUploadHospital);
                string followupDocPath = SaveFile(fileUploadFollowup);
                string othersPath = SaveFile(fileUploadOthers);

                cmd.Parameters.AddWithValue("@HospitalDocPath", string.IsNullOrEmpty(hospitalDocPath) ? (object)DBNull.Value : hospitalDocPath);
                cmd.Parameters.AddWithValue("@FollowupDocPath", string.IsNullOrEmpty(followupDocPath) ? (object)DBNull.Value : followupDocPath);
                cmd.Parameters.AddWithValue("@OtherFilesPath", string.IsNullOrEmpty(othersPath) ? (object)DBNull.Value : othersPath);

                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (status == "DRAFT")
                Response.Redirect("SaveAsDraft.aspx");
            else
                Response.Redirect("SubmitSucces.aspx");
        }

        private bool IsValidSubmission()
        {
            if (string.IsNullOrWhiteSpace(admissionDate.Value) ||
                string.IsNullOrWhiteSpace(dischargeDate.Value) ||
                string.IsNullOrWhiteSpace(hospitalName.Value) ||
                string.IsNullOrWhiteSpace(wardType.Value))
                return false;

            if (!(testsYes.Checked || testsNo.Checked))
                return false;
            if (!(followUpYes.Checked || followUpNo.Checked))
                return false;
            if (!(cpfYes.Checked || cpfNo.Checked))
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
