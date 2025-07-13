using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Singlife
{
    public partial class EverEditClaims : System.Web.UI.Page
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
                string sql = "SELECT * FROM EverCareClaims WHERE ClaimID = @ClaimID AND Status = 'Submitted'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtAdmissionDate.Text = reader["AdmissionDate"] is DBNull ? "" : Convert.ToDateTime(reader["AdmissionDate"]).ToString("yyyy-MM-dd");
                        txtDischargeDate.Text = reader["DischargeDate"] is DBNull ? "" : Convert.ToDateTime(reader["DischargeDate"]).ToString("yyyy-MM-dd");
                        txtHospitalName.Text = reader["HospitalName"].ToString();
                        txtWardType.Text = reader["WardType"].ToString();

                        rbTestsYes.Checked = reader["DidTestsBefore"] != DBNull.Value && Convert.ToBoolean(reader["DidTestsBefore"]);
                        rbTestsNo.Checked = !rbTestsYes.Checked;

                        rbFollowupYes.Checked = reader["DidFollowUpAfter"] != DBNull.Value && Convert.ToBoolean(reader["DidFollowUpAfter"]);
                        rbFollowupNo.Checked = !rbFollowupYes.Checked;

                        rbCpfYes.Checked = reader["CpfUsed"] != DBNull.Value && Convert.ToBoolean(reader["CpfUsed"]);
                        rbCpfNo.Checked = !rbCpfYes.Checked;

                        chkDeclaration.Checked = reader["DeclarationConfirmed"] != DBNull.Value && Convert.ToBoolean(reader["DeclarationConfirmed"]);

                        // Load existing file links
                        litHospitalDoc.Text = GenerateFileBlock(reader["HospitalDocPath"].ToString());
                        litFollowupDoc.Text = GenerateFileBlock(reader["FollowupDocPath"].ToString());
                        litOtherFiles.Text = GenerateFileBlock(reader["OtherFilesPath"].ToString());
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
                    <a class='file-link text-success' href='/{filePath}' target='_blank'>&#128206; {fileName}</a>
                    <div class='text-muted small'>You may re-upload, but this file will remain in company records.</div>
                </div>";
        }

        protected void btnUpdateClaim_Click(object sender, EventArgs e)
        {
            if (!chkDeclaration.Checked)
            {
                lblError.Text = "Please confirm the declaration.";
                lblError.Visible = true;
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT CreatedDate FROM EverCareClaims WHERE ClaimID = @ClaimID", conn);
                checkCmd.Parameters.AddWithValue("@ClaimID", claimId);
                var result = checkCmd.ExecuteScalar();

                if (result == null || (DateTime.Now - Convert.ToDateTime(result)).TotalDays > 2)
                {
                    lblError.Text = "Edit window has expired.";
                    lblError.Visible = true;
                    return;
                }
            }

            DateTime? admissionDate = DateTime.TryParse(txtAdmissionDate.Text, out var adDate) ? adDate : (DateTime?)null;
            DateTime? dischargeDate = DateTime.TryParse(txtDischargeDate.Text, out var disDate) ? disDate : (DateTime?)null;
            bool didTestsBefore = rbTestsYes.Checked;
            bool didFollowupAfter = rbFollowupYes.Checked;
            bool cpfUsed = rbCpfYes.Checked;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE EverCareClaims SET
                    AdmissionDate = @AdmissionDate,
                    DischargeDate = @DischargeDate,
                    HospitalName = @HospitalName,
                    WardType = @WardType,
                    DidTestsBefore = @DidTestsBefore,
                    DidFollowUpAfter = @DidFollowUpAfter,
                    CpfUsed = @CpfUsed,
                    DeclarationConfirmed = @DeclarationConfirmed,
                    ModifiedDate = GETDATE()
                    WHERE ClaimID = @ClaimID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@AdmissionDate", (object)admissionDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DischargeDate", (object)dischargeDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HospitalName", txtHospitalName.Text);
                cmd.Parameters.AddWithValue("@WardType", txtWardType.Text);
                cmd.Parameters.AddWithValue("@DidTestsBefore", didTestsBefore);
                cmd.Parameters.AddWithValue("@DidFollowUpAfter", didFollowupAfter);
                cmd.Parameters.AddWithValue("@CpfUsed", cpfUsed);
                cmd.Parameters.AddWithValue("@DeclarationConfirmed", chkDeclaration.Checked);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            HandleFileUpload("fuHospitalDoc", "HospitalDocPath");
            HandleFileUpload("fuFollowupDoc", "FollowupDocPath");
            HandleFileUpload("fuOtherFiles", "OtherFilesPath");

            lblError.CssClass = "text-success";
            lblError.Text = "Claim updated successfully.";
            lblError.Visible = true;

            LoadClaimData(claimId);
        }

        private void HandleFileUpload(string fileUploadId, string columnName)
        {
            FileUpload fu = (FileUpload)FindControl(fileUploadId);
            if (fu != null && fu.HasFile)
            {
                string ext = Path.GetExtension(fu.FileName);
                string filename = Guid.NewGuid() + ext;
                string folder = "~/Uploads/";
                string savePath = Server.MapPath(folder + filename);
                string dbPath = "Uploads/" + filename;

                fu.SaveAs(savePath);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"UPDATE EverCareClaims SET {columnName} = @Path WHERE ClaimID = @ClaimID", conn);
                    cmd.Parameters.AddWithValue("@Path", dbPath);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
