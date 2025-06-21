<%@ Page Title="EverCare Approved Claim Details" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="EverApproveClaim.aspx.cs" Inherits="Singlife.EverApproveClaim" %>

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

        .outcome-link {
            font-size: 1rem;
            color: #198754;
            text-decoration: none;
        }

        .outcome-link:hover {
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container mt-4">
        <h3 class="text-primary mb-4">EverCare Approved Claim Details</h3>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false" />

        <asp:Panel ID="pnlOutcome" runat="server" CssClass="outcome-section" Visible="false">
            <h5>Staff Review Outcome</h5>
            <p><strong>Comment:</strong> <asp:Literal ID="litComment" runat="server" /></p>
            <p>
                <strong>Outcome File:</strong>
                <asp:HyperLink ID="lnkOutcomeFile" runat="server" CssClass="outcome-link" Target="_blank" />
                <i class="bi bi-download ms-1"></i>
            </p>
        </asp:Panel>

        <asp:Panel ID="pnlClaimDetails" runat="server" CssClass="claim-detail">
            <h4>Claim Information</h4>
            <asp:Literal ID="litClaimInfo" runat="server" />
        </asp:Panel>
    </div>
</asp:Content>
