<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Claiming.aspx.cs" Inherits="Singlife.Claiming" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OncoShield – Advanced Cancer Care Claim</title>
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
        .btn-link.text-danger:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container py-5">
            <div class="row">
                <!-- Form Column -->
                <div class="col-lg-8 mb-4">
                    <h3 class="mb-4">OncoShield Claim Form</h3>

                    <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3 d-block" Visible="false" />

                    <!-- Claim Details -->
                    <div class="form-section">
                        <h5>Claim Details</h5>
                        <div class="mb-3">
                            <label class="form-label">Date of Diagnosis</label>
                            <input type="date" class="form-control" id="diagnosisDate" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Country of Treatment</label>
                            <input type="text" class="form-control" id="treatmentCountry" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Type of Cancer Diagnosed</label>
                            <input type="text" class="form-control" id="cancerType" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Is this your first cancer diagnosis?</label><br />
                            <div class="form-check form-check-inline">
                                <input type="radio" id="firstYes" name="firstDiagnosis" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="firstYes">Yes</label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="firstNo" name="firstDiagnosis" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="firstNo">No</label>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Have you received treatment?</label><br />
                            <div class="form-check form-check-inline">
                                <input type="radio" id="treatmentYes" name="receivedTreatment" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="treatmentYes">Yes</label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="treatmentNo" name="receivedTreatment" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="treatmentNo">No</label>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Was this diagnosis confirmed by a specialist?</label><br />
                            <div class="form-check form-check-inline">
                                <input type="radio" id="confirmedYes" name="confirmed" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="confirmedYes">Yes</label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="confirmedNo" name="confirmed" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="confirmedNo">No</label>
                            </div>
                        </div>
                    </div>

                    <!-- Treatment Information -->
                    <div class="form-section">
                        <h5>Treatment Information</h5>
                        <div class="mb-3">
                            <label class="form-label">Treatment Start Date</label>
                            <input type="date" class="form-control" id="treatmentDate" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Which clinic or hospital?</label>
                            <input type="text" class="form-control" id="hospital" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Type of Therapy</label>
                            <select class="form-select" id="therapyType" runat="server">
                                <option value="">-- Select therapy --</option>
                                <option>Chemotherapy</option>
                                <option>Targeted Therapy</option>
                                <option>Proton Beam Therapy</option>
                                <option>Gene Therapy</option>
                                <option>Cell Therapy</option>
                                <option>Other</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Attach medical reports or treatment receipts</label>
                            <asp:FileUpload ID="fileUploadTreatment" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <!-- Annual Screening -->
                    <div class="form-section">
                        <h5>Annual Screening</h5>
                        <div class="mb-3">
                            <label class="form-label">Did you use your free cancer screening this year?</label><br />
                            <div class="form-check form-check-inline">
                                <input type="radio" id="screeningYes" name="usedScreening" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="screeningYes">Yes</label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="screeningNo" name="usedScreening" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="screeningNo">No</label>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Attach screening result or receipt</label>
                            <asp:FileUpload ID="fileUploadScreening" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <!-- Other Documents -->
                    <div class="form-section">
                        <h5>Other Supporting Documents <span class="text-muted small">(Optional)</span></h5>
                        <div class="mb-3">
                            <label class="form-label">Attach additional files</label>
                            <asp:FileUpload ID="fileUploadOthers" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <!-- Declaration -->
                    <div class="form-section">
                        <h5>Declaration</h5>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="declaration" runat="server" />
                            <label class="form-check-label" for="declaration">I declare that the info is true and accurate.</label>
                        </div>
                    </div>
                </div>

                <!-- Sidebar -->
                <div class="col-lg-4">
                    <div class="sidebar-box">
                        <h6 class="fw-bold">Estimated time to complete:</h6>
                        <p class="text-muted">10–15 mins</p>
                        <ul class="list-unstyled small">
                            <li class="mb-2 border-start border-danger ps-2 fw-bold text-dark">Cancer Diagnosis</li>
                            <li class="mb-2 ps-2 text-muted">Treatment Info</li>
                            <li class="mb-2 ps-2 text-muted">Screening</li>
                            <li class="mb-2 ps-2 text-muted">Other Documents</li>
                            <li class="mb-2 ps-2 text-muted">Payment & Declaration</li>
                        </ul>
                        <div class="d-grid gap-2 mt-4">
                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save as draft" CssClass="btn btn-outline-danger" OnClick="btnSaveDraft_Click" />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit my claim" CssClass="btn btn-danger" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnDiscard" runat="server" Text="Discard submission" CssClass="btn btn-link text-danger" OnClick="btnDiscard_Click" OnClientClick="return confirm('Are you sure you want to discard this submission?');" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
