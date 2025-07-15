<%@ Page Title="Get in Touch" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="GetInTouch.aspx.cs" Inherits="Singlife.GetInTouch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Lato:wght@700&display=swap');

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

        .contact-form {
            max-width: 600px;
            margin: 4rem auto;
            padding: 2rem;
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }

        label {
            font-weight: bold;
            margin-top: 1rem;
            display: block;
            color: #333;
            font-family: 'Lato', sans-serif;
        }

        input[type="text"],
        input[type="email"] {
            width: 100%;
            padding: 0.8rem;
            margin-top: 0.5rem;
            border: 1.5px solid #ccc;
            border-radius: 8px;
            font-family: 'Lato', sans-serif;
        }

        .radio-group {
            margin-top: 1rem;
        }

        .radio-group label {
            margin-right: 1rem;
            font-weight: normal;
        }

        .btn-submit {
            background-color: #FB0202;
            color: white;
            font-weight: bold;
            font-size: 1.2rem;
            border: none;
            border-radius: 30px;
            padding: 0.8rem 2rem;
            margin-top: 2rem;
            cursor: pointer;
            font-family: 'Lato', sans-serif;
        }

        .btn-submit:hover {
            background-color: #c10000;
        }

        .custom-modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            display: none;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        }

        .custom-modal {
            background-color: #fff;
            padding: 2rem 3rem;
            border-radius: 12px;
            text-align: center;
            max-width: 400px;
            width: 100%;
            box-shadow: 0 0 20px rgba(0,0,0,0.3);
            font-family: 'Lato', sans-serif;
        }

        .custom-modal p {
            font-size: 1.3rem;
            margin-bottom: 1.5rem;
        }

        .custom-modal button {
            margin: 0.5rem 0.5rem;
            padding: 0.6rem 1.5rem;
            border: none;
            border-radius: 8px;
            background-color: #FB0202;
            color: white;
            font-weight: bold;
            cursor: pointer;
            font-size: 1rem;
        }

        .custom-modal button:hover {
            background-color: #c10000;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- Header -->
    <div class="retire-header">
        <h1>Flexi Retirement</h1>
        <h2>Interest Form</h2>
    </div>

    <!-- Contact Form -->
    <div class="contact-form">
        <asp:Label AssociatedControlID="txtName" Text="Full Name" runat="server" />
        <asp:TextBox ID="txtName" runat="server" />

        <asp:Label AssociatedControlID="txtEmail" Text="Email Address" runat="server" />
        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" />

        <asp:Label AssociatedControlID="txtPhone" Text="Phone Number (+65)" runat="server" />
        <asp:TextBox ID="txtPhone" runat="server" />

        <div class="radio-group">
            <label>Preferred Contact Method:</label><br />
            <asp:RadioButton ID="rdoEmail" runat="server" GroupName="ContactMethod" Text="Email" Checked="True" />
            <asp:RadioButton ID="rdoPhone" runat="server" GroupName="ContactMethod" Text="Phone" />
        </div>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn-submit" OnClick="btnSubmit_Click" />
    </div>

    <!-- Modal -->
    <div id="modalOverlay" class="custom-modal-overlay">
        <div class="custom-modal">
            <p>Thank you for your interest! An agent will contact you soon.</p>
            <button onclick="closeModal()">OK</button>
            <button onclick="location.href='HomePage.aspx'">Back to Home</button>
        </div>
    </div>

    <script>
        function showModal() {
            document.getElementById('modalOverlay').style.display = 'flex';
        }

        function closeModal() {
            document.getElementById('modalOverlay').style.display = 'none';
        }
    </script>
</asp:Content>
