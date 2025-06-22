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
                    <h3 class="mb-4 text-danger">Edit Submitted OncoShield Claim</h3>

                    <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3 d-block" Visible="false" />

                    <div class="form-section">
                        <h5>Claim Details</h5>
                        <div class="mb-3">
                            <label class="form-label" for="diagnosisDate">Date of Diagnosis</label>
                            <input type="date" id="diagnosisDate" runat="server" class="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="treatmentCountry">Country of Treatment</label>
                            <input type="text" id="treatmentCountry" runat="server" class="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="cancerType">Type of Cancer Diagnosed</label>
                            <input type="text" id="cancerType" runat="server" class="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Is this your first cancer diagnosis?</label><br />
                            <input type="radio" id="firstYes" runat="server" name="firstDiagnosis" class="form-check-input" />
                            <label for="firstYes" class="form-check-label me-3">Yes</label>
                            <input type="radio" id="firstNo" runat="server" name="firstDiagnosis" class="form-check-input" />
                            <label for="firstNo" class="form-check-label">No</label>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Have you received treatment?</label><br />
                            <input type="radio" id="treatmentYes" runat="server" name="receivedTreatment" class="form-check-input" />
                            <label for="treatmentYes" class="form-check-label me-3">Yes</label>
                            <input type="radio" id="treatmentNo" runat="server" name="receivedTreatment" class="form-check-input" />
                            <label for="treatmentNo" class="form-check-label">No</label>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Was this diagnosis confirmed by a specialist?</label><br />
                            <input type="radio" id="confirmedYes" runat="server" name="confirmed" class="form-check-input" />
                            <label for="confirmedYes" class="form-check-label me-3">Yes</label>
                            <input type="radio" id="confirmedNo" runat="server" name="confirmed" class="form-check-input" />
                            <label for="confirmedNo" class="form-check-label">No</label>
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Treatment Information</h5>
                        <div class="mb-3">
                            <label class="form-label" for="treatmentDate">Treatment Start Date</label>
                            <input type="date" id="treatmentDate" runat="server" class="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="hospital">Which clinic or hospital?</label>
                            <input type="text" id="hospital" runat="server" class="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="therapyType">Type of Therapy</label>
                            <select id="therapyType" runat="server" class="form-select">
                                <option value="">-- Select therapy --</option>
                                <option>Chemotherapy</option>
                                <option>Targeted Therapy</option>
                                <option>Proton Beam Therapy</option>
                                <option>Gene Therapy</option>
                                <option>Cell Therapy</option>
                                <option>Other</option>
                            </select>
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Uploaded Documents</h5>

                        <div class="file-section">
                            <label class="form-label">Treatment File</label>
                            <asp:Literal ID="litTreatmentFile" runat="server" />
                            <asp:FileUpload ID="fuTreatment" runat="server" CssClass="form-control mt-2" />
                            <asp:Button ID="btnDeleteTreatment" runat="server" Text="Delete Treatment File" CssClass="btn btn-outline-danger btn-sm mt-2" OnClick="btnDeleteTreatment_Click" />
                        </div>

                        <div class="file-section">
                            <label class="form-label">Screening File</label>
                            <asp:Literal ID="litScreeningFile" runat="server" />
                            <asp:FileUpload ID="fuScreening" runat="server" CssClass="form-control mt-2" />
                            <asp:Button ID="btnDeleteScreening" runat="server" Text="Delete Screening File" CssClass="btn btn-outline-danger btn-sm mt-2" OnClick="btnDeleteScreening_Click" />
                        </div>

                        <div class="file-section">
                            <label class="form-label">Other Documents</label>
                            <asp:Literal ID="litOtherFiles" runat="server" />
                            <asp:FileUpload ID="fuOthers" runat="server" CssClass="form-control mt-2" />
                            <asp:Button ID="btnDeleteOthers" runat="server" Text="Delete Other File" CssClass="btn btn-outline-danger btn-sm mt-2" OnClick="btnDeleteOthers_Click" />
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Annual Screening</h5>
                        <div class="mb-3">
                            <label class="form-label">Did you use your free cancer screening this year?</label><br />
                            <input type="radio" id="screeningYes" runat="server" name="usedScreening" class="form-check-input" />
                            <label for="screeningYes" class="form-check-label me-3">Yes</label>
                            <input type="radio" id="screeningNo" runat="server" name="usedScreening" class="form-check-input" />
                            <label for="screeningNo" class="form-check-label">No</label>
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Declaration</h5>
                        <input type="checkbox" id="declaration" runat="server" class="form-check-input" />
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
