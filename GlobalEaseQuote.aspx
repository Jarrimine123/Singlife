<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="GlobalEaseQuote.aspx.cs" Inherits="Singlife.GlobalEaseQuote" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .quote-container {
            max-width: 650px;
            margin: 40px auto;
            padding: 30px;
            border-radius: 10px;
            background-color: #f9f9f9;
            font-family: Arial, sans-serif;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }

        h2, h3 {
            text-align: center;
            margin-bottom: 10px;
        }

        .plan-info {
            background-color: #d4edda;
            border-left: 5px solid #28a745;
            padding: 15px;
            margin-bottom: 25px;
            font-size: 14px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        label {
            font-weight: bold;
            display: block;
            margin-bottom: 5px;
        }

        input[type="text"], select {
            width: 100%;
            padding: 8px;
            font-size: 14px;
        }

        .btn-container {
            margin-top: 20px;
            text-align: center;
        }

        .btn {
            padding: 10px 20px;
            margin: 5px;
            font-size: 14px;
            cursor: pointer;
        }

        .btn-primary {
            background-color: #28a745;
            border: none;
            color: white;
        }

        .btn-secondary {
            background-color: #6c757d;
            border: none;
            color: white;
        }

        .result-box {
            margin-top: 20px;
            background-color: #e8f5e9;
            padding: 15px;
            border-left: 5px solid #28a745;
        }

        .validation-message {
            color: red;
            font-weight: bold;
            margin-top: 10px;
            text-align: center;
        }

        .min-coverage-info {
            font-style: italic;
            font-size: 13px;
            color: #555;
            margin-bottom: 15px;
            text-align: center;
        }
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="quote-container">
        <h2>GlobeEase Travel Plan</h2>
        <h3>Get a Quick Quote</h3>

        <div class="form-group">
            <label for="ddlPlanType">Type of Plan:</label>
            <asp:DropDownList ID="ddlPlanType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPlanType_SelectedIndexChanged">
                <asp:ListItem Text="Individual" Value="Individual" />
                <asp:ListItem Text="Family (Multi-Generational)" Value="Family" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <asp:PlaceHolder ID="phIndividualAge" runat="server" Visible="true">
                <label for="txtAge">Traveller's Age:</label>
                <asp:TextBox ID="txtAge" runat="server" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phFamilyAges" runat="server" Visible="false">
                <label>Family Member Ages (comma-separated):</label>
                <asp:TextBox ID="txtFamilyAges" runat="server" />
            </asp:PlaceHolder>
        </div>

        <div class="form-group">
            <label for="ddlDestination">Trip Destination:</label>
            <asp:DropDownList ID="ddlDestination" runat="server">
                <asp:ListItem Text="Asia" Value="Asia" />
                <asp:ListItem Text="Europe" Value="Europe" />
                <asp:ListItem Text="Worldwide" Value="Worldwide" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label for="txtDuration">Trip Duration (in days):</label>
            <asp:TextBox ID="txtDuration" runat="server" />
        </div>

        <div class="form-group">
            <label for="ddlPurpose">Travel Purpose:</label>
            <asp:DropDownList ID="ddlPurpose" runat="server">
                <asp:ListItem Text="Leisure / Holiday" Value="Leisure" />
                <asp:ListItem Text="Business" Value="Business" />
                <asp:ListItem Text="Study Abroad" Value="Study" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label for="ddlMedical">Pre-existing Medical Condition:</label>
            <asp:DropDownList ID="ddlMedical" runat="server">
                <asp:ListItem Text="No" Value="No" />
                <asp:ListItem Text="Yes" Value="Yes" />
            </asp:DropDownList>
        </div>

        <asp:Label ID="lblValidationMessage" runat="server" CssClass="validation-message" Visible="false" />

        <div class="btn-container">
            <asp:Button ID="btnCalculate" runat="server" Text="Get Quote" CssClass="btn btn-primary" OnClick="btnCalculate_Click" />
        </div>

        <asp:Panel ID="pnlResult" runat="server" Visible="false" CssClass="result-box">
            <asp:Label ID="lblResult" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlActions" runat="server" Visible="false" CssClass="btn-container">
            <asp:Button ID="btnBuyNow" runat="server" Text="Buy Now" CssClass="btn btn-primary" OnClick="btnBuyNow_Click" />
            <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn btn-secondary" OnClick="btnAddToCart_Click" />
        </asp:Panel>
    </div>
</asp:Content>