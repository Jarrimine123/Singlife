<%@ Page Title="Create Account" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="CreateAccount.aspx.cs" Inherits="Singlife.CreateAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Lato:wght@400;500;700&display=swap');

        body, html {
            height: 100%;
            margin: 0;
            font-family: 'Lato', sans-serif;
        }

        .create-container {
            width: 100%;
            max-width: 480px;
            margin: 4rem auto;
            padding: 2rem 2.5rem;
            border-radius: 12px;
            background-color: #fff;
            text-align: center;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);
        }

        h1 {
            font-weight: 700;
            color: #B90000;
            font-size: 2.2rem;
            margin-bottom: 0.75rem;
        }

        h3.subtitle {
            font-weight: 600;
            font-size: 1.1rem;
            margin-bottom: 1.8rem;
            color: #333;
            text-align: left;
        }

        label {
            display: block;
            text-align: left;
            font-weight: 500;
            margin-bottom: 0.4rem;
            font-size: 1rem;
            color: #333;
            font-family: 'Lato', sans-serif;
        }

        input[type="text"],
        input[type="password"],
        input[type="date"],
        .form-control {
            width: 100%;
            padding: 0.7rem 1rem;
            font-size: 1.1rem;
            border: 1.5px solid #ccc;
            border-radius: 8px;
            margin-bottom: 1.4rem;
            font-family: 'Lato', sans-serif;
            box-sizing: border-box;
            font-weight: 400;
        }

            input[type="text"]:focus,
            input[type="password"]:focus,
            input[type="date"]:focus,
            .form-control:focus {
                outline: none;
                border-color: #FB0202;
                box-shadow: 0 0 5px #FB0202;
            }

        .btn-create {
            background-color: #FB0202;
            border: none;
            color: white;
            font-weight: 600;
            font-size: 1.4rem;
            padding: 0.9rem 2rem;
            border-radius: 30px;
            cursor: pointer;
            width: 100%;
            transition: background-color 0.3s ease;
            font-family: 'Lato', sans-serif;
        }

            .btn-create:hover {
                background-color: #d50101;
            }

        .text-danger {
            color: #e60012;
            font-size: 1rem;
            text-align: left;
            display: block;
            margin-top: -0.6rem; /* reduced from -1rem */
            margin-bottom: 0.6rem; /* reduced from 1.2rem */
            font-family: 'Lato', sans-serif;
            font-weight: 500;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="create-container">
        <h1>Create Account</h1>
        <h3 class="subtitle">Welcome! Please enter your details.</h3>

        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="text-danger" HeaderText="Please correct the following:" />

        <label for="txtName">Name</label>
        <asp:TextBox ID="txtName" runat="server" />

        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Name is required." CssClass="text-danger" Display="Dynamic" />

        <label for="txtEmail">Email</label>
        <asp:TextBox ID="txtEmail" runat="server" />

        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
            ErrorMessage="Invalid email format." ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" CssClass="text-danger" Display="Dynamic" />

        <label for="txtDOB">Date of Birth</label>
        <asp:TextBox ID="txtDOB" runat="server" TextMode="Date" />

        <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="txtDOB" ErrorMessage="Date of Birth is required." CssClass="text-danger" Display="Dynamic" />
        <asp:CustomValidator ID="cvDOB" runat="server" ControlToValidate="txtDOB" OnServerValidate="cvDOB_ServerValidate"
            ErrorMessage="You must be at least 21 years old." CssClass="text-danger" Display="Dynamic" />

        <label for="txtNRIC">NRIC</label>
        <asp:TextBox ID="txtNRIC" runat="server" />

        <asp:RequiredFieldValidator ID="rfvNRIC" runat="server" ControlToValidate="txtNRIC" ErrorMessage="NRIC is required." CssClass="text-danger" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revNRIC" runat="server" ControlToValidate="txtNRIC"
            ValidationExpression="^[STFGstfg]\d{7}[A-Za-z]$" ErrorMessage="Invalid NRIC format." CssClass="text-danger" Display="Dynamic" />

        <label for="txtPassword">Password</label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />

        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword"
            ValidationExpression=".{8,}" ErrorMessage="Password must be at least 8 characters." CssClass="text-danger" Display="Dynamic" />

        <label for="txtConfirmPassword">Confirm Password</label>
        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />

        <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm your password." CssClass="text-danger" Display="Dynamic" />
        <asp:CompareValidator ID="cvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword"
            ErrorMessage="Passwords do not match." CssClass="text-danger" Display="Dynamic" />

        <asp:Label ID="lblError" runat="server" CssClass="text-danger" />
        <br />
        <asp:Button ID="btnCreate" runat="server" Text="Register" CssClass="btn-create" OnClick="btnCreate_Click" />
    </div>
</asp:Content>
