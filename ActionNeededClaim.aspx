<%@ Page Title="Action Needed Claim" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="ActionNeededClaim.aspx.cs" Inherits="Singlife.ActionNeededClaim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .claim-detail {
            background: #fff;
            border: 1px solid #dee2e6;
            border-radius: 0.5rem;
            padding: 1.5rem;
            margin-bottom: 1rem;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
        }

        .claim-detail h4 {
            margin-bottom: 1rem;
            color: #0d6efd;
        }

        .claim-detail .label {
            font-weight: 600;
            color: #212529;
        }

        .claim-detail .value {
            margin-bottom: 0.75rem;
            color: #495057;
        }

        .outcome-section {
            background-color: #e9f7ef;
            border-left: 5px solid #198754;
            padding: 1rem;
            border-radius: 0.5rem;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.03);
        }

        .upload-section {
            background-color: #fff3cd;
            border-left: 5px solid #ffc107;
            padding: 1rem;
            border-radius: 0.5rem;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.03);
        }

        .upload-section h5 {
            margin-bottom: 1rem;
            color: #856404;
        }

        .btn-submit {
            margin-bottom: 1.5rem;
        }

        .text-success {
            color: #198754 !important;
        }

        .text-danger {
            color: #dc3545 !important;
        }

        .btn-back {
            font-size: 0.9rem;
            margin-top: 0.75rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container mt-4">
        <h3 class="text-primary mb-4">Action Needed: Respond to Claim Review</h3>

        <asp:Label ID="lblMessage" runat="server" Visible="false" EnableViewState="false" />

        <!-- Staff Review -->
        <asp:Panel ID="pnlOutcome" runat="server" CssClass="outcome-section" Visible="false">
            <h5>Staff Review Outcome</h5>
            <p><strong>Comment:</strong> <asp:Literal ID="litComment" runat="server" /></p>
        </asp:Panel>

        <!-- File Upload Panel -->
        <asp:Panel ID="pnlUpload" runat="server" CssClass="upload-section">
            <h5>Upload Required Document</h5>
            <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control mb-3" />
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-warning btn-submit" OnClick="btnSubmit_Click" />
        </asp:Panel>

        <!-- Claim Info -->
        <asp:Panel ID="pnlClaimDetails" runat="server" CssClass="claim-detail">
            <h4>Claim Information</h4>
            <asp:Literal ID="litClaimInfo" runat="server" />
        </asp:Panel>
    </div>
</asp:Content>
