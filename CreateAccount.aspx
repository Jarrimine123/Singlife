<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="CreateAccount.aspx.cs" Inherits="Singlife.CreateAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5 col-md-6 mx-auto">
        <h2>Create Account</h2>

        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="text-danger" HeaderText="Please correct the following:" />

        <div class="form-group mb-3">
            <label>Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Name is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group mb-3">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="Invalid email format." ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group mb-3">
            <label>Date of Birth</label>
            <asp:TextBox ID="txtDOB" runat="server" TextMode="Date" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="txtDOB" ErrorMessage="Date of Birth is required." CssClass="text-danger" Display="Dynamic" />
            <asp:CustomValidator ID="cvDOB" runat="server" ControlToValidate="txtDOB" OnServerValidate="cvDOB_ServerValidate"
                ErrorMessage="You must be at least 21 years old." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group mb-3">
            <label>NRIC</label>
            <asp:TextBox ID="txtNRIC" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvNRIC" runat="server" ControlToValidate="txtNRIC" ErrorMessage="NRIC is required." CssClass="text-danger" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revNRIC" runat="server" ControlToValidate="txtNRIC"
                ValidationExpression="^[STFGstfg]\d{7}[A-Za-z]$" ErrorMessage="Invalid NRIC format." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group mb-3">
            <label>Password</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword"
                ValidationExpression=".{8,}" ErrorMessage="Password must be at least 8 characters." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group mb-3">
            <label>Confirm Password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm your password." CssClass="text-danger" Display="Dynamic" />
            <asp:CompareValidator ID="cvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword"
                ErrorMessage="Passwords do not match." CssClass="text-danger" Display="Dynamic" />
        </div>

        <asp:Label ID="lblError" runat="server" CssClass="text-danger" />
        <br />
        <asp:Button ID="btnCreate" runat="server" Text="Register" CssClass="btn btn-success" OnClick="btnCreate_Click" />
    </div>
</asp:Content>


