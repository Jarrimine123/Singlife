<%@ Page Title="Login" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Singlife.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Lato:wght@400;500;700&display=swap');

        body, html {
            height: 100%;
            margin: 0;
            font-family: 'Lato', sans-serif;
        }

        .full-height-center {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 80vh;
            padding: 1rem;
        }

        .login-container {
            width: 100%;
            max-width: 480px;
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

        p.subtitle {
            font-weight: 600;
            font-size: 1.1rem;
            margin-bottom: 1.8rem;
            color: #333;
        }

        label {
            display: block;
            text-align: left;
            font-weight: 500;
            margin-bottom: 0.4rem;
            font-size: 1rem;
            color: #333;
        }

        input[type="text"],
        input[type="password"] {
            width: 100%;
            padding: 0.7rem 1rem;
            font-size: 1.1rem;
            border: 1.5px solid #ccc;
            border-radius: 8px;
            margin-bottom: 1.4rem;
            font-family: 'Lato', sans-serif;
            box-sizing: border-box;
        }

            input[type="text"]:focus,
            input[type="password"]:focus {
                outline: none;
                border-color: #FB0202;
                box-shadow: 0 0 5px #FB0202;
            }

        .btn-login {
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

            .btn-login:hover {
                background-color: #d50101;
            }

        .error-label {
            color: red;
            margin-top: 0.8rem;
            font-size: 1rem;
            font-weight: 500;
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

            .custom-modal button {
                margin-top: 1.5rem;
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
    <div class="full-height-center">
        <div class="login-container">
            <h1>Log in to your account</h1>
            <p class="subtitle">Welcome back! Please enter your details.</p>

            <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email" />
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />

            <asp:Label runat="server" AssociatedControlID="txtPassword" Text="Password" />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />

            <asp:Label ID="lblError" runat="server" CssClass="error-label" Visible="false" />
        </div>
    </div>

    <div id="modalOverlay" class="custom-modal-overlay">
    <div class="custom-modal">
        <p id="modalMessage">Message here</p>
        <button onclick="closeModal()">OK</button>
    </div>
</div>

<script>
    function showModal(message) {
        document.getElementById('modalMessage').innerText = message;
        document.getElementById('modalOverlay').style.display = 'flex';
    }

    function closeModal() {
        document.getElementById('modalOverlay').style.display = 'none';
    }
</script>

</asp:Content>
