<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="ThankYou.aspx.cs" Inherits="Singlife.ThankYou" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- You can add custom CSS or scripts here if needed --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5 d-flex flex-column align-items-center justify-content-center" style="min-height: 70vh;">
        <div class="text-center">
            <i class="bi bi-check-circle-fill text-success" style="font-size: 5rem;"></i>
            <h1 class="mt-3 mb-4">Thank You for Your Purchase!</h1>
            <p class="lead mb-4">We have successfully received your order and it is now being processed.</p>
            <p>If you have any questions or need assistance, please contact our support team:</p>
            <a href="mailto:support@singlife.com.sg" class="btn btn-outline-primary mb-4">support@singlife.com.sg</a>
            <br />
            <a href="HomePage.aspx" class="btn btn-primary btn-lg">Back to Home</a>
        </div>
    </div>
</asp:Content>
