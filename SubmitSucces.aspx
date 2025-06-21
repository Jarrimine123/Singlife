<%@ Page Title="Submission Successful" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="SubmitSucces.aspx.cs" Inherits="Singlife.SubmitSucces" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
        <div class="container py-5">
        <div class="row justify-content-center">
            <div class="col-lg-8">
                <div class="card shadow-lg border-0">
                    <div class="card-body text-center py-5">
                        <div class="mb-4">
                            <i class="bi bi-check-circle-fill text-success" style="font-size: 4rem;"></i>
                        </div>
                        <h2 class="fw-bold text-success">Claim Submitted Successfully!</h2>
                        <p class="fs-5 mt-3">Thank you for submitting your OncoShield claim.</p>
                        <p class="fs-6 text-muted">
                            You may still <strong>edit your submitted claim</strong> within the next <strong>2 days</strong> from today.
                            After 2 days, editing will be disabled.
                        </p>
                        <div class="mt-4 d-flex justify-content-center gap-3">
                            <a href="ClaimHistory.aspx" class="btn btn-success px-4">View My Claims</a>
                            <a href="HomePage.aspx" class="btn btn-outline-secondary px-4">Return to Dashboard</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
