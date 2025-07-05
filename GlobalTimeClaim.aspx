<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="GlobalTimeClaim.aspx.cs" Inherits="Singlife.GlobalClaim" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="d-flex justify-content-center align-items-start py-5">
    <div class="bg-white p-4 p-md-5 rounded shadow-sm" style="max-width: 700px; width: 100%;">
        <h2 class="mb-3 text-center">Let's start your claim</h2>

        <div class="alert alert-warning d-flex align-items-center mb-4" role="alert">
            <i class="bi bi-clock-fill me-2"></i>
            <div>
                <strong>Estimated time to complete:</strong> 10–15 mins<br />
                <small>You can save your submission as a draft anytime and continue later.</small>
            </div>
        </div>

        <!-- Step 1 -->
        <div class="d-flex align-items-start mb-3">
            <span class="badge rounded-circle bg-danger text-white me-3" style="width: 28px; height: 28px; line-height: 20px; padding-top: 4px; text-align: center;">1</span>
            <div class="flex-grow-1">
                <div class="d-flex justify-content-between align-items-center">
                    <h6 class="mb-1 fw-bold">View policy and life assured</h6>
                    <span class="text-muted small">~ 1 min</span>
                </div>
            </div>
        </div>

        <!-- Step 2 -->
        <div class="d-flex align-items-start mb-3">
            <span class="badge rounded-circle bg-danger text-white me-3" style="width: 28px; height: 28px; line-height: 20px; padding-top: 4px; text-align: center;">2</span>
            <div class="flex-grow-1">
                <div class="d-flex justify-content-between align-items-center">
                    <h6 class="mb-1 fw-bold">Fill in claim details</h6>
                    <span class="text-muted small">~ 10 min</span>
                </div>
                <ul class="mb-0 text-muted small ps-4">
                    <li>Incident details</li>
                    <li>Additional details</li>
                    <li>
                        Supporting documents –
                        <a href="#" class="text-danger text-decoration-none">Find out what documents</a>
                    </li>
                </ul>
            </div>
        </div>

        <!-- Step 3 -->
        <div class="d-flex align-items-start mb-3">
            <span class="badge rounded-circle bg-danger text-white me-3" style="width: 28px; height: 28px; line-height: 20px; padding-top: 4px; text-align: center;">3</span>
            <div class="flex-grow-1">
                <div class="d-flex justify-content-between align-items-center">
                    <h6 class="mb-1 fw-bold">Claim payment method</h6>
                    <span class="text-muted small">~ 1 min</span>
                </div>
                <p class="text-muted mb-0 small">For receiving claim payment</p>
            </div>
        </div>

        <!-- Step 4 -->
        <div class="d-flex align-items-start mb-3">
            <span class="badge rounded-circle bg-danger text-white me-3" style="width: 28px; height: 28px; line-height: 20px; padding-top: 4px; text-align: center;">4</span>
            <div class="flex-grow-1">
                <div class="d-flex justify-content-between align-items-center">
                    <h6 class="mb-1 fw-bold">Review and declare</h6>
                    <span class="text-muted small">~ 2 min</span>
                </div>
            </div>
        </div>

        <!-- Button -->
        <div class="text-center mt-4">
            <a href="EverCareClaim.aspx" class="btn btn-danger btn-lg px-5">Get started</a>
        </div>
    </div>
</div>
</asp:Content>
