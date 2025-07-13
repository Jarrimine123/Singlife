<%@ Page Title="Retirement Information" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="RetirementInfo.aspx.cs" Inherits="Singlife.RetirementInfo" %>

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

        h2 {
            font-size: 2.5rem;
            font-family: 'Lato', sans-serif;
            font-weight: 700;
            margin-top: 2.5rem;
            margin-bottom: 1.5rem;
        }

        h3 {
            font-size: 2rem;
            font-family: 'Lato', sans-serif;
            font-weight: 700;
            margin-top: 2rem;
            margin-bottom: 1rem;
        }

        h2.mb-3 {
            margin-bottom: 2.5rem; /* increase from default 1.5rem */
        }

        .row.g-3.justify-content-center {
            padding-top: 1rem; /* optional: add some top padding */
        }

        .feature-card {
            border: 2px solid #FB0202;
            border-radius: 12px;
            background-color: #FB0202;
            color: white;
            padding: 0.75rem;
            min-height: 200px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            overflow: hidden;
            word-wrap: break-word;
        }

        .feature-card h5 {
            font-weight: 700;
            margin-bottom: 0.5rem;
            font-size: 1.5rem;
        }


        .btn-primary {
            background-color: #FB0202 !important;
            border: none;
            border-radius: 30px;
            font-weight: bold;
            color: white;
            padding: 1rem 2rem;
            font-size: 1.5rem;
            cursor: pointer;
        }

        .btn-primary:hover {
            background-color: #d50101 !important;
        }

        /* Full width dark grey background */
        .why-choose-us-wrapper {
            background-color: #464646;
            color: white;
            padding: 1rem 0 3rem 0;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            margin-top: 4rem; /* extra spacing */
            text-align: center;  /* center content */
        }

        .why-choose-us {
            max-width: 1140px;
            margin: 0 auto;
            padding: 0 15px;
            text-align: center;  /* center text and children */
        }

        .why-choose-us h2,
        .why-choose-us h3 {
            margin-left: auto;
            margin-right: auto;
        }

        .why-choose-us p {
            font-size: 1.25rem;  /* bigger paragraphs */
            line-height: 1.6;
            max-width: 700px;
            margin-left: auto;
            margin-right: auto;
        }

        /* Table */
        .why-choose-us .table {
            background-color: white;
            color: black;
            border-collapse: separate;
            border-spacing: 0;
            width: 100%;
            margin-top: 2rem;
            border: 3px solid #FB0202;
            border-radius: 20px; /* more rounded */
            overflow: hidden; /* clip corners */
            margin-left: auto;
            margin-right: auto;
        }

        .why-choose-us .table th,
        .why-choose-us .table td {
            border: 1px solid black;
            border-width: 0.5px; /* thinner lines */
            padding: 1rem;
            text-align: center;
        }

        .why-choose-us .table thead th,
        .why-choose-us .table td:first-child {
            font-weight: 600;
        }


        /* Full width pink background for interested section */
        .interested-section-wrapper {
            background-color: #FFE4E7;
            color: #FB0202;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            padding: 1rem 0 3rem 0;
            text-align: center;
        }

        .interested-section h2,
        .interested-section p {
            color: #FB0202;
        }

        .section-padding {
            margin-top: 4rem;
            margin-bottom: 2rem;
        }

        .learn-more-section {
            margin-top: 4rem;
            margin-bottom: 3rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- Header Section -->
    <div class="retire-header">
        <h1>Flexi Retirement</h1>
        <h2>Savings Plan</h2>
    </div>

    <div class="container text-center mt-5">
        <h2>What It Is ✨</h2>
        <p>SingLife Flexi Retirement Savings Plan is a high-interest, long-term savings plan with built-in insurance protection.</p>

        <h2 class="mb-3">Features 📝</h2>
        <div class="row g-3 justify-content-center">
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>💰 Low Minimum Deposit</h5>
                    <p>Begin your plan with just $1000 per year.</p>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>📈 4% Interest p.a.</h5>
                    <p>Earn compound interest much higher than banks.</p>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>💸 Savings + Insurance</h5>
                    <p>Grow your money and get basic life protection.</p>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>👨‍👩‍👧 Plan Ahead</h5>
                    <p>Use our calculator to find out your payout or contribution.</p>
                </div>
            </div>
        </div>

        <div class="learn-more-section">
            <h2>Learn More</h2>
            <p>Click below to view the full plan details and terms.</p>
            <asp:Button ID="btnLearnMore" runat="server" Text="Click Here" CssClass="btn btn-primary" OnClick="btnLearnMore_Click" />
        </div>
    </div>

    <div class="why-choose-us-wrapper">
        <div class="why-choose-us">
            <h2>Why Choose Us?</h2>
            <p>At SingLife, we have curated the ideal retirement account for those planning for retirement. Start saving early so your future self will thank you!</p>

            <h3>Retirement Calculator</h3>
            <p>Try out our premium and payout calculator!</p>
            <asp:Button ID="btnCalculator" runat="server" Text="Click here" CssClass="btn btn-primary" OnClick="btnCalculator_Click" />

            <h3 class="section-padding">Why We’re Better</h3>
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Feature</th>
                            <th>Our Flexi Retirement</th>
                            <th>Other Retirement Accounts</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Interest Rate (p.a.)</td>
                            <td>4%</td>
                            <td>2.5-3%</td>
                        </tr>
                        <tr>
                            <td>Minimum Deposit</td>
                            <td>$1,000</td>
                            <td>$10,000</td>
                        </tr>
                        <tr>
                            <td>Lock-in Period</td>
                            <td>10 years</td>
                            <td>20 years</td>
                        </tr>
                        <tr>
                            <td>Insurance Coverage</td>
                            <td>Life coverage (up to $20k)</td>
                            <td>No</td>
                        </tr>
                        <tr>
                            <td>Retirement Simulator tool</td>
                            <td>Yes</td>
                            <td>No</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <div class="interested-section-wrapper">
        <div class="interested-section">
            <h2>Interested?</h2>
            <p>Contact our Financial Consultants to sign up!</p>
            <asp:Button ID="btnContact" runat="server" Text="Get in Touch" CssClass="btn btn-primary" OnClick="btnContact_Click" />
        </div>
    </div>
</asp:Content>
