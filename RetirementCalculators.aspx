<%@ Page Title="Retirement Calculators" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="RetirementCalculators.aspx.cs" Inherits="Singlife.RetirementCalculators" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .retirement-header {
            background: url('Images/RedBackground.png') no-repeat center center;
            background-size: cover;
            min-height: 300px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            color: white;
        }

        .retirement-header h1 {
            font-size: 3.2rem;
            margin-bottom: 10px;
        }

        .retirement-header h2 {
            font-size: 2rem;
        }

        .retirement-info {
            text-align: center;
            padding: 60px 20px;
        }

        .retirement-info .intro {
            color: #e60012;
            font-weight: bold;
            font-size: 1.5rem;
        }

        .retirement-info h2 {
            margin-top: 25px;
            color: #000;
        }

        .retirement-buttons {
            display: flex;
            justify-content: center;
            gap: 40px; /* increased spacing between buttons */
            margin-top: 40px;
            flex-wrap: wrap;
        }

        .calc-button {
            background-color: #FFD8DC;
            border: 4px solid #FF2261;
            border-radius: 20px;
            width: 210px;
            height: 210px;
            display: flex;
            flex-direction: column;
            align-items: center;
            padding-top: 10px;
        }

        .calc-button h2 {
            margin-bottom: 8px;
            padding: 6px 14px;
            background-color: #FF5C7F;
            border: 2px solid #E7436C;
            border-radius: 20px;
            color: white;
            font-size: 1.625rem; /* 25% bigger than previous 1.3rem */
        }

        .calc-button p {
            color: black;
            font-size: 1.44rem; /* 25% bigger than previous 1.15rem */
            margin: 0;
            line-height: 1.4;
            text-align: center;
            white-space: pre-line;
        }

        .disclaimer {
            text-align: center;
            margin-top: 50px;
            color: #e60012;
            font-size: 1.3rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="retirement-header">
        <h1>Flexi Retirement</h1>
        <h2>Retirement Calculator</h2>
    </div>

    <div class="retirement-info">
        <p class="intro">Not sure how to start planning? Don’t worry, we’re here to help!</p>
        <h2>I want to calculate:</h2>

        <div class="retirement-buttons">
            <a href="PremiumCalculator.aspx" style="text-decoration: none;">
                <div class="calc-button">
                    <h2>Premium</h2>
                    <p>Annual<br />contribution<br />amount</p>
                </div>
            </a>

            <a href="PayoutCalculator.aspx" style="text-decoration: none;">
                <div class="calc-button">
                    <h2>Payout</h2>
                    <p>Annual<br />payout<br />amount</p>
                </div>
            </a>
        </div>

        <h3 class="disclaimer">Our calculations assume a 4% annual interest rate compounded yearly.</h3>
    </div>
</asp:Content>

