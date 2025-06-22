<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EverEditClaims.aspx.cs" Inherits="Singlife.EverEditClaims" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit EverCare Claim</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body { background-color: #f5f7fa; }
        .form-section {
            background-color: #ffffff;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.03);
            margin-bottom: 30px;
        }
        h3, h5 { font-weight: 600; }
        .sidebar-box {
            position: sticky;
            top: 100px;
            background-color: #fff;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.05);
        }
        .file-link {
            margin-top: 10px;
            display: block;
            font-size: 0.9rem;
        }
        .file-section {
            border: 1px dashed #ccc;
            padding: 15px;
            border-radius: 10px;
            margin-top: 10px;
            background-color: #f8f9fa;
        }
        .file-actions {
            margin-top: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="container py-5">
            <div class="row">
                <div class="col-lg-8 mb-4">
                    <h3 class="mb-4 text-danger">Edit Submitted EverCare Claim</h3>

                    <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3 d-block" Visible="false" />

                    <div class="form-section">
                        <h5>Claim Details</h5>

                        <div class="mb-3">
                            <label for="txtAdmissionDate" class="form-label">Admission Date</label>
                            <asp:TextBox ID="txtAdmissionDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="mb-3">
                            <label for="txtDischargeDate" class="form-label">Discharge Date</label>
                            <asp:TextBox ID="txtDischargeDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="mb-3">
                            <label for="txtHospitalName" class="form-label">Hospital Name</label>
                            <asp:TextBox ID="txtHospitalName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label for="txtWardType" class="form-label">Ward Type</label>
                            <asp:TextBox ID="txtWardType" runat="server" CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Did Tests Before?</label><br />
                            <asp:RadioButton ID="rbTestsYes" GroupName="TestsBefore" runat="server" Text="Yes" />
                            <asp:RadioButton ID="rbTestsNo" GroupName="TestsBefore" runat="server" Text="No" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Did Follow-up After?</label><br />
                            <asp:RadioButton ID="rbFollowupYes" GroupName="FollowUpAfter" runat="server" Text="Yes" />
                            <asp:RadioButton ID="rbFollowupNo" GroupName="FollowUpAfter" runat="server" Text="No" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Used CPF?</label><br />
                            <asp:RadioButton ID="rbCpfYes" GroupName="CpfUsed" runat="server" Text="Yes" />
                            <asp:RadioButton ID="rbCpfNo" GroupName="CpfUsed" runat="server" Text="No" />
                        </div>

                        <div class="form-section">
                            <h5>Declaration</h5>
                            <asp:CheckBox ID="chkDeclaration" runat="server" CssClass="form-check-input" />
                            <label for="chkDeclaration" class="form-check-label ms-2">I declare that the information is true and accurate.</label>
                        </div>
                    </div>

                    <!-- Uploaded Files -->
                    <div class="form-section">
                        <h5>Uploaded Documents</h5>

                        <div class="file-section">
                            <label class="form-label">Hospital Document</label>
                            <asp:Literal ID="litHospitalDoc" runat="server" />
                            <div class="file-actions">
                                <asp:FileUpload ID="fuHospitalDoc" runat="server" CssClass="form-control" />
                                <asp:Button ID="btnDeleteHospitalDoc" runat="server" Text="Delete Hospital Document" CssClass="btn btn-outline-danger btn-sm mt-2" OnClick="btnDeleteHospitalDoc_Click" />
                            </div>
                        </div>

                        <div class="file-section">
                            <label class="form-label">Follow-up Document</label>
                            <asp:Literal ID="litFollowupDoc" runat="server" />
                            <div class="file-actions">
                                <asp:FileUpload ID="fuFollowupDoc" runat="server" CssClass="form-control" />
                                <asp:Button ID="btnDeleteFollowupDoc" runat="server" Text="Delete Follow-up Document" CssClass="btn btn-outline-danger btn-sm mt-2" OnClick="btnDeleteFollowupDoc_Click" />
                            </div>
                        </div>

                        <div class="file-section">
                            <label class="form-label">Other Documents</label>
                            <asp:Literal ID="litOtherFiles" runat="server" />
                            <div class="file-actions">
                                <asp:FileUpload ID="fuOtherFiles" runat="server" CssClass="form-control" />
                                <asp:Button ID="btnDeleteOtherFiles" runat="server" Text="Delete Other Documents" CssClass="btn btn-outline-danger btn-sm mt-2" OnClick="btnDeleteOtherFiles_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4">
                    <div class="sidebar-box">
                        <h6 class="fw-bold">Reminder</h6>
                        <p class="text-muted">You can only update your claim within <strong>2 days</strong> from the initial submission.</p>

                        <div class="d-grid gap-2 mt-4">
                            <asp:Button ID="btnUpdateClaim" runat="server" Text="Update Claim" CssClass="btn btn-danger" OnClick="btnUpdateClaim_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary" PostBackUrl="~/ClaimHistory.aspx" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
