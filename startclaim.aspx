<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="startclaim.aspx.cs" Inherits="Singlife.startclaim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5">
        <div class="row">
            <!-- Left Side: Form -->
            <div class="col-lg-8 mb-4">
                <h3 class="mb-4">Personal Accident Claim</h3>

                <!-- Incident Details -->
                <div class="mb-4">
                    <h5>Tell us what happened</h5>

                    <div class="mb-3">
                        <label for="incidentDate" class="form-label">Date of Incident</label>
                        <input type="date" class="form-control" id="incidentDate" runat="server" />
                    </div>

                    <div class="mb-3">
                        <label for="country" class="form-label">Which country did it take place?</label>
                        <input type="text" class="form-control" id="country" runat="server" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Was the incident caused by:</label><br />
                        <div class="form-check form-check-inline">
                            <input type="radio" id="accident" name="cause" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="accident">Accident</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input type="radio" id="sickness" name="cause" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="sickness">Sickness</label>
                        </div>
                    </div>

                    <div class="mb-3">
                        <label for="diagnosis" class="form-label">Closest diagnosis</label>
                        <input type="text" class="form-control" id="diagnosis" runat="server" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Is the claim related to self-inflicted injuries or war?</label><br />
                        <div class="form-check form-check-inline">
                            <input type="radio" id="selfYes" name="selfInjury" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="selfYes">Yes</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input type="radio" id="selfNo" name="selfInjury" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="selfNo">No</label>
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Was there any loss of body parts or bone fractures?</label><br />
                        <div class="form-check form-check-inline">
                            <input type="radio" id="lossYes" name="loss" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="lossYes">Yes</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input type="radio" id="lossNo" name="loss" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="lossNo">No</label>
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Was there any hospitalisation due to the accident?</label><br />
                        <div class="form-check form-check-inline">
                            <input type="radio" id="hospYes" name="hospitalised" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="hospYes">Yes</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input type="radio" id="hospNo" name="hospitalised" class="form-check-input" runat="server" />
                            <label class="form-check-label" for="hospNo">No</label>
                        </div>
                    </div>
                </div>

                <!-- Medical Expense -->
                <div class="mb-4">
                    <h5>Medical Expense 1</h5>

                    <div class="mb-3">
                        <label for="receiptDate" class="form-label">Date of Receipt</label>
                        <input type="date" class="form-control" id="receiptDate" runat="server" />
                    </div>

                    <div class="mb-3">
                        <label for="cost" class="form-label">How much did it cost?</label>
                        <input type="number" class="form-control" id="cost" runat="server" />
                    </div>

                    <div class="mb-3">
                        <label for="clinic" class="form-label">Which clinic or hospital?</label>
                        <input type="text" class="form-control" id="clinic" runat="server" />
                    </div>

                    <div class="mb-3">
                        <label for="fileUpload1" class="form-label">Attach file</label>
                        <asp:FileUpload ID="fileUpload1" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <!-- Others -->
                <div class="mb-4">
                    <h5>Others</h5>
                    <div class="mb-3">
                        <label for="fileUpload2" class="form-label">Attach file</label>
                        <asp:FileUpload ID="fileUpload2" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <!-- Payment Info -->
                <div class="mb-4">
                    <h5>Payment Details</h5>
                    <p class="text-muted">If the claim is approved, it will be paid to the policyholder via PayNow NRIC bank account.</p>
                </div>

                <!-- Declaration -->
                <div class="mb-4">
                    <h5>Declaration</h5>
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" id="declaration1" runat="server" />
                        <label class="form-check-label" for="declaration1">
                            I declare that the information provided is true and accurate to the best of my knowledge.
                        </label>
                    </div>
                </div>
            </div>

            <!-- Right Side: Sidebar -->
            <div class="col-lg-4">
                <div class="bg-white p-4 rounded shadow-sm mb-4">
                    <h6 class="fw-bold">Estimated time to complete:</h6>
                    <p class="text-muted mb-3">15–20 mins</p>

                    <ul class="list-unstyled">
                        <li class="mb-2 border-start border-danger ps-2 fw-bold text-dark">Policy & Life Assured</li>
                        <li class="mb-2 ps-2 text-muted">Claim Details</li>
                        <li class="mb-2 ps-2 text-muted">Additional Details</li>
                        <li class="mb-2 ps-2 text-muted">Upload Supporting Documents</li>
                        <li class="mb-2 ps-2 text-muted">Payment Info & Declaration</li>
                    </ul>

                    <!-- Buttons -->
                    <div class="d-grid gap-2 mt-4">
                        <asp:Button ID="btnSaveDraft" runat="server" Text="Save as draft" CssClass="btn btn-outline-danger" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit my claim" CssClass="btn btn-danger" />
                        <asp:Button ID="btnDiscard" runat="server" Text="Discard submission" CssClass="btn btn-link text-danger" OnClientClick="return confirm('Are you sure you want to discard this submission?');" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
