using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Singlife
{
    public partial class EverContinueDraft : Page
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
                string sql = "SELECT * FROM EverCareClaims WHERE ClaimID = @ClaimID AND AccountID = @AccountID AND Status = 'DRAFT'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);
                cmd.Parameters.AddWithValue("@AccountID", accountId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    admissionDate.Value = reader["AdmissionDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["AdmissionDate"]).ToString("yyyy-MM-dd");
                    dischargeDate.Value = reader["DischargeDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DischargeDate"]).ToString("yyyy-MM-dd");
                    hospitalName.Value = reader["HospitalName"]?.ToString() ?? "";
                    wardType.Value = reader["WardType"]?.ToString() ?? "";

                    bool didTestsBefore = reader["DidTestsBefore"] != DBNull.Value && Convert.ToBoolean(reader["DidTestsBefore"]);
                    testsYes.Checked = didTestsBefore;
                    testsNo.Checked = !didTestsBefore;

                    bool didFollowUpAfter = reader["DidFollowUpAfter"] != DBNull.Value && Convert.ToBoolean(reader["DidFollowUpAfter"]);
                    followUpYes.Checked = didFollowUpAfter;
                    followUpNo.Checked = !didFollowUpAfter;

                    bool cpfUsed = reader["CpfUsed"] != DBNull.Value && Convert.ToBoolean(reader["CpfUsed"]);
                    cpfYes.Checked = cpfUsed;
                    cpfNo.Checked = !cpfUsed;

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
            SaveClaim("DRAFT");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!IsValidSubmission())
            {
                lblError.Text = "Please fill in all required fields before submitting.";
                lblError.Visible = true;
                return;
            }

            SaveClaim("SUBMITTED");
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            Response.Redirect("SaveAsDraft.aspx");
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
                    Directory.CreateDirectory(folderPath);

                string fileName = Path.GetFileName(fileUpload.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                fileUpload.SaveAs(filePath);
                return "Uploads/" + fileName;
            }
            return null;
        }

        private void SaveClaim(string status)
        {
            string hospitalFile = SaveFile(fileUploadHospital);
            string followUpFile = SaveFile(fileUploadFollowup);
            string otherFile = SaveFile(fileUploadOthers);

            string connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    UPDATE EverCareClaims SET
                        AdmissionDate = @AdmissionDate,
                        DischargeDate = @DischargeDate,
                        HospitalName = @HospitalName,
                        WardType = @WardType,
                        DidTestsBefore = @DidTestsBefore,
                        DidFollowUpAfter = @DidFollowUpAfter,
                        CpfUsed = @CpfUsed,
                        DeclarationConfirmed = @DeclarationConfirmed,
                        HospitalDocPath = @HospitalDocPath,
                        FollowupDocPath = @FollowupDocPath,
                        OtherFilesPath = @OtherFilesPath,
                        Status = @Status,
                        ModifiedDate = GETDATE()
                    WHERE ClaimID = @ClaimID AND AccountID = @AccountID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@AdmissionDate", string.IsNullOrEmpty(admissionDate.Value) ? (object)DBNull.Value : admissionDate.Value);
                cmd.Parameters.AddWithValue("@DischargeDate", string.IsNullOrEmpty(dischargeDate.Value) ? (object)DBNull.Value : dischargeDate.Value);
                cmd.Parameters.AddWithValue("@HospitalName", hospitalName.Value);
                cmd.Parameters.AddWithValue("@WardType", wardType.Value);
                cmd.Parameters.AddWithValue("@DidTestsBefore", testsYes.Checked);
                cmd.Parameters.AddWithValue("@DidFollowUpAfter", followUpYes.Checked);
                cmd.Parameters.AddWithValue("@CpfUsed", cpfYes.Checked);
                cmd.Parameters.AddWithValue("@DeclarationConfirmed", declaration.Checked);
                cmd.Parameters.AddWithValue("@HospitalDocPath", string.IsNullOrEmpty(hospitalFile) ? (object)DBNull.Value : hospitalFile);
                cmd.Parameters.AddWithValue("@FollowupDocPath", string.IsNullOrEmpty(followUpFile) ? (object)DBNull.Value : followUpFile);
                cmd.Parameters.AddWithValue("@OtherFilesPath", string.IsNullOrEmpty(otherFile) ? (object)DBNull.Value : otherFile);
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
