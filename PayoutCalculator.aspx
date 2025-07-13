<%@ Page Title="Payout Calculator" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="PayoutCalculator.aspx.cs" Inherits="Singlife.PayoutCalculator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        /* Center container vertically and horizontally */
        .calculator-panel {
            max-width: 400px;
            margin: 60px auto;
            padding: 30px 25px;
            background-color: #fff0f0;
            border: 1px solid #e60012;
            border-radius: 10px;
            box-shadow: 0 0 15px rgba(230, 0, 18, 0.3);
            text-align: center;
            font-family: Arial, sans-serif;
        }

        h2 {
            color: #e60012;
            margin-bottom: 25px;
            font-weight: 700;
        }

        .form-control {
            border: 2px solid #e60012;
            border-radius: 5px;
            margin-bottom: 15px;
            padding: 10px;
            font-size: 1rem;
        }

        .btn-primary {
            background-color: #e60012;
            border-color: #e60012;
            font-weight: 600;
            padding: 10px 20px;
        }

        .btn-primary:hover,
        .btn-primary:focus {
            background-color: #b4000f;
            border-color: #b4000f;
        }

        .result-label {
            font-weight: 700;
            font-size: 1.1rem;
            margin-top: 20px;
            display: block;
            color: #e60012;
        }

        label {
            font-weight: 600;
            color: #a3000f;
            display: block;
            margin-bottom: 5px;
            text-align: left;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <h2>Payout Calculator</h2>
    <asp:Panel runat="server" CssClass="calculator-panel">
        <asp:Label ID="lblContribution" runat="server" Text="Annual Premium Contribution ($):" AssociatedControlID="txtContribution" />
        <asp:TextBox ID="txtContribution" runat="server" CssClass="form-control" />

        <asp:Label ID="lblPayoutYears" runat="server" Text="Payout Period (years):" AssociatedControlID="txtPayoutYears" />
        <asp:TextBox ID="txtPayoutYears" runat="server" CssClass="form-control" />

        <asp:Label ID="lblPremiumYears" runat="server" Text="Premium Payment Term (years):" AssociatedControlID="txtPremiumYears" />
        <asp:TextBox ID="txtPremiumYears" runat="server" CssClass="form-control" />

        <asp:Button ID="btnCalculate" runat="server" Text="Calculate Payout" OnClick="btnCalculate_Click" CssClass="btn btn-primary mt-2" />

        <asp:Label ID="lblResult" runat="server" CssClass="result-label mt-3" ForeColor="Green" />
    </asp:Panel>
</asp:Content>
