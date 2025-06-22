<%@ Page Title="Claim Upload Success" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="SuccessfulUploaded.aspx.cs" Inherits="Singlife.SuccessfulUploaded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .info-panel {
            background: #f8f9fa;
            border: 1px solid #ced4da;
            padding: 20px;
            border-radius: 8px;
            margin-top: 20px;
        }
        .info-panel .label {
            font-weight: bold;
            display: inline-block;
            width: 200px;
        }
        .info-panel .value {
            margin-bottom: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container mt-4">
        <h3 class="text-success">✅ Document Uploaded Successfully</h3>
        <p>Your claim has been updated. Below is a summary of your submission:</p>
        
        <asp:Literal ID="litClaimDetails" runat="server" />
        <a href="ClaimHistory.aspx" class="btn btn-outline-primary mt-3">← Back to Claim History</a>
    </div>
</asp:Content>
