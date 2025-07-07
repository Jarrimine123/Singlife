<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="LifeGrowQuote.aspx.cs" Inherits="Singlife.LifeGrowQuote" %>

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
        <h2>LifeGrow Retirement Plus</h2>
        <div class="plan-info">
            <strong>Premium Rate Guide:</strong> Starting from as low as S$2.10/day for S$300,000 coverage.<br />
            This plan helps you grow your wealth while being protected up to age 100 — with capital guarantee and retirement income options.
        </div>

        <div class="min-coverage-info">
            * Minimum coverage amount is <strong>SGD 50,000</strong>.
        </div>

        <h3>Quote Calculator</h3>

        <div class="form-group">
            <label for="txtCoverage">Desired Coverage Amount (SGD):</label>
            <asp:TextBox ID="txtCoverage" runat="server" />
        </div>

        <div class="form-group">
            <label for="txtAge">Your Current Age:</label>
            <asp:TextBox ID="txtAge" runat="server" />
        </div>

        <div class="form-group">
            <label for="ddlGoal">Main Retirement Goal:</label>
            <asp:DropDownList ID="ddlGoal" runat="server">
                <asp:ListItem Text="Monthly Income" Value="Income" />
                <asp:ListItem Text="Lump Sum at Retirement" Value="LumpSum" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label for="ddlRetireAge">Planned Retirement Age:</label>
            <asp:DropDownList ID="ddlRetireAge" runat="server">
                <asp:ListItem Text="55" Value="55" />
                <asp:ListItem Text="60" Value="60" />
                <asp:ListItem Text="65" Value="65" />
                <asp:ListItem Text="70" Value="70" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label for="ddlFrequency">Payment Frequency:</label>
            <asp:DropDownList ID="ddlFrequency" runat="server">
                <asp:ListItem Text="Annual" Value="Annual" />
                <asp:ListItem Text="Monthly" Value="Monthly" />
            </asp:DropDownList>
        </div>

        <asp:Label ID="lblValidationMessage" runat="server" CssClass="validation-message" Visible="false"></asp:Label>

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
