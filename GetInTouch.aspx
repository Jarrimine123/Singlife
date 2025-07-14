<%@ Page Title="Get in Touch" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="GetInTouch.aspx.cs" Inherits="Singlife.GetInTouch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .retire-header {
            position: relative;
            background-image: url('Images/RedBackground.png');
            background-size: cover;
            background-position: center;
            height: 320px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
        }

        .retire-header h1 {
            color: white;
            font-size: 3.5rem;
            font-family: 'Lato', sans-serif;
            font-weight: 700;
            margin-bottom: 12px;
        }

        .retire-header h2 {
            color: white;
            font-size: 2.3rem;
            font-family: 'Lato', sans-serif;
            font-weight: 700;
            margin-top: 0;
        }

        .form-section {
            padding: 3rem 1rem;
            max-width: 700px;
            margin: 0 auto;
            font-family: 'Lato', sans-serif;
        }

        .form-section h2 {
            font-size: 2.2rem;
            text-align: center;
            color: #B90000;
            margin-bottom: 2rem;
        }

        .form-group {
            margin-bottom: 1.5rem;
        }

        .form-group label {
            display: block;
            font-weight: 700;
            margin-bottom: 0.5rem;
        }

        .form-control {
            width: 100%;
            padding: 0.75rem;
            font-size: 1rem;
            border: 1px solid #ccc;
            border-radius: 8px;
        }

        .radio-group {
            display: flex;
            gap: 1.5rem;
            margin-top: 0.5rem;
        }

        .btn-submit {
            background-color: #FB0202;
            color: white;
            border: none;
            border-radius: 30px;
            padding: 0.75rem 2rem;
            font-size: 1.3rem;
            font-weight: bold;
            cursor: pointer;
            display: block;
            margin: 2rem auto 0 auto;
        }

        .btn-submit:hover {
            background-color: #d50101;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- Header -->
    <div class="retire-header">
        <h1>Flexi Retirement</h1>
        <h2>Interest Form</h2>
    </div>

    <!-- Form Section -->
    <div class="form-section">
        <h2>Contact Us</h2>

        <div class="form-group">
            <label for="txtName">Full Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Your name" />
        </div>

        <div class="form-group">
            <label for="txtEmail">Email Address</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="example@email.com" />
        </div>

        <div class="form-group">
            <label for="txtPhone">Phone Number (+65)</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="+65 9123 4567" />
        </div>

        <div class="form-group">
            <label>Preferred Contact Method</label>
            <div class="radio-group">
                <asp:RadioButton ID="rdoEmail" runat="server" GroupName="contactMethod" Text="Email" />
                <asp:RadioButton ID="rdoPhone" runat="server" GroupName="contactMethod" Text="Phone Number" />
            </div>
        </div>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit" OnClick="btnSubmit_Click" />
    </div>
</asp:Content>
