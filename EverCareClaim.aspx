<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EverCareClaim.aspx.cs" Inherits="Singlife.EverCareClaim" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EverCare – Hospital & Specialist Claim</title>
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
                <div class="col-lg-8 mb-4">
                    <h3 class="mb-4">EverCare Claim Form</h3>

                    <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3 d-block" Visible="false" />

                    <div class="form-section">
                        <h5>Hospitalisation Details</h5>
                        <div class="mb-3">
                            <label class="form-label">Admission Date</label>
                            <input type="date" class="form-control" id="admissionDate" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Discharge Date</label>
                            <input type="date" class="form-control" id="dischargeDate" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Hospital Name</label>
                            <input type="text" class="form-control" id="hospitalName" runat="server" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Ward Type</label>
                            <select class="form-select" id="wardType" runat="server">
                                <option value="">-- Select ward type --</option>
                                <option>General (B2/C)</option>
                                <option>Private</option>
                                <option>ICU</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Attach hospital bill / discharge summary</label>
                            <asp:FileUpload ID="fileUploadHospital" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Tests & Follow-up</h5>
                        <div class="mb-3">
                            <label class="form-label">Did you do any tests before hospitalisation?</label><br />
                            <input type="radio" id="testsYes" name="tests" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="testsYes">Yes</label>
                            <input type="radio" id="testsNo" name="tests" class="form-check-input ms-4" runat="server" />
                            <label class="form-check-label" for="testsNo">No</label>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Any follow-up after discharge?</label><br />
                            <input type="radio" id="followUpYes" name="followup" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="followUpYes">Yes</label>
                            <input type="radio" id="followUpNo" name="followup" class="form-check-input ms-4" runat="server" />
                            <label class="form-check-label" for="followUpNo">No</label>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Upload supporting documents</label>
                            <asp:FileUpload ID="fileUploadFollowup" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>CPF Payment</h5>
                        <div class="mb-3">
                            <label class="form-label">Did you use CPF to pay for your EverCare premium?</label><br />
                            <div class="form-check form-check-inline">
                                <input type="radio" id="cpfYes" name="cpfUsed" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="cpfYes">Yes</label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input type="radio" id="cpfNo" name="cpfUsed" class="form-check-input" runat="server" />
                                <label class="form-check-label" for="cpfNo">No</label>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Other Supporting Documents <span class="text-muted small">(Optional)</span></h5>
                        <div class="mb-3">
                            <label class="form-label">Attach additional files</label>
                            <asp:FileUpload ID="fileUploadOthers" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="form-section">
                        <h5>Declaration</h5>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="declaration" runat="server" />
                            <label class="form-check-label" for="declaration">I declare that the info is true and accurate.</label>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4">
                    <div class="sidebar-box">
                        <h6 class="fw-bold">Estimated time to complete:</h6>
                        <p class="text-muted">10–15 mins</p>
                        <ul class="list-unstyled small">
                            <li class="mb-2 border-start border-danger ps-2 fw-bold text-dark">Hospitalisation</li>
                            <li class="mb-2 ps-2 text-muted">Follow-up & Tests</li>
                            <li class="mb-2 ps-2 text-muted">CPF & Other Docs</li>
                            <li class="mb-2 ps-2 text-muted">Declaration</li>
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
