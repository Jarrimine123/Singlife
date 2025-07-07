<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="StaffLogin.aspx.cs" Inherits="Singlife.StaffLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container mt-5" style="max-width: 500px;">
        <h3 class="text-center mb-4">Staff Login</h3>

        <asp:Label ID="lblError" runat="server" CssClass="text-danger" EnableViewState="false" />

        <div class="mb-3">
            <label for="txtUsername" class="form-label">Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                ErrorMessage="Username is required" CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="mb-3">
            <label for="txtPassword" class="form-label">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />
    </div>
</asp:Content>
