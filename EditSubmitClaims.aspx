<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSubmitClaims.aspx.cs" Inherits="Singlife.EditSubmitClaims" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OncoShield – Edit Submitted Claim</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
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
        .file-note {
            font-size: 0.85rem;
            color: #6c757d;
            margin-top: 4px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="container py-5">
            <div class="row">
                <div class="col-lg-8 mb-4">
                    <h3 class="mb-4 text-danger">Edit Submitted OncoShield Claim</h3>

                    <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3 d-block" Visible="false" />

                    <div class="form-section">
                        <h5>Claim Details</h5>
                        <div class="mb-3">
                            <label for="diagnosisDate" class="form-label">Diagnosis Date</label>
                           <asp:TextBox ID="diagnosisDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="mb-3">
                            <label for="treatmentCountry" class="form-label">Country of Treatment</label>
                            <asp:TextBox ID="treatmentCountry" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label for="cancerType" class="form-label">Type of Cancer Diagnosed</label>
                            <asp:TextBox ID="cancerType" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Is this your first cancer diagnosis?</label><br />
                            <asp:RadioButton ID="firstYes" GroupName="firstDiagnosis" runat="server" Text="Yes" CssClass="form-check-input" />
                            <asp:RadioButton ID="firstNo" GroupName="firstDiagnosis" runat="server" Text="No" CssClass="form-check-input" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Have you received treatment?</label><br />
                            <asp:RadioButton ID="treatmentYes" GroupName="receivedTreatment" runat="server" Text="Yes" CssClass="form-check-input" />
                            <asp:RadioButton ID="treatmentNo" GroupName="receivedTreatment" runat="server" Text="No" CssClass="form-check-input" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Was this diagnosis confirmed by a specialist?</label><br />
                            <asp:RadioButton ID="confirmedYes" GroupName="confirmed" runat="server" Text="Yes" CssClass="form-check-input" />
                            <asp:RadioButton ID="confirmedNo" GroupName="confirmed" runat="server" Text="No" CssClass="form-check-input" />
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Treatment Information</h5>
                        <div class="mb-3">
                            <label for="treatmentDate" class="form-label">Treatment Start Date</label>
                            <asp:TextBox ID="treatmentDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="mb-3">
                            <label for="hospital" class="form-label">Which clinic or hospital?</label>
                            <asp:TextBox ID="hospital" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label for="therapyType" class="form-label">Type of Therapy</label>
                            <asp:DropDownList ID="therapyType" runat="server" CssClass="form-select">
                                <asp:ListItem Value="" Text="-- Select therapy --" />
                                <asp:ListItem Text="Chemotherapy" />
                                <asp:ListItem Text="Targeted Therapy" />
                                <asp:ListItem Text="Proton Beam Therapy" />
                                <asp:ListItem Text="Gene Therapy" />
                                <asp:ListItem Text="Cell Therapy" />
                                <asp:ListItem Text="Other" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Uploaded Documents</h5>

                        <div class="file-section">
                            <label class="form-label">Treatment File</label>
                            <asp:Literal ID="litTreatmentFile" runat="server" />
                            <asp:FileUpload ID="fuTreatment" runat="server" CssClass="form-control mt-2" />
                            <div class="file-note">You may re-upload. Previously uploaded files are kept in company records.</div>
                        </div>

                        <div class="file-section">
                            <label class="form-label">Screening File</label>
                            <asp:Literal ID="litScreeningFile" runat="server" />
                            <asp:FileUpload ID="fuScreening" runat="server" CssClass="form-control mt-2" />
                            <div class="file-note">You may re-upload. Previously uploaded files are kept in company records.</div>
                        </div>

                        <div class="file-section">
                            <label class="form-label">Other Documents</label>
                            <asp:Literal ID="litOtherFiles" runat="server" />
                            <asp:FileUpload ID="fuOthers" runat="server" CssClass="form-control mt-2" />
                            <div class="file-note">You may re-upload. Previously uploaded files are kept in company records.</div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Annual Screening</h5>
                        <div class="mb-3">
                            <label class="form-label">Did you use your free cancer screening this year?</label><br />
                            <asp:RadioButton ID="screeningYes" GroupName="usedScreening" runat="server" Text="Yes" CssClass="form-check-input" />
                            <asp:RadioButton ID="screeningNo" GroupName="usedScreening" runat="server" Text="No" CssClass="form-check-input" />
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Declaration</h5>
                        <asp:CheckBox ID="declaration" runat="server" CssClass="form-check-input" />
                        <label for="declaration" class="form-check-label ms-2">I declare that the information is true and accurate.</label>
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
