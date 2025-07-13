<%@ Page Title="Retirement Information" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="RetirementInfo.aspx.cs" Inherits="Singlife.RetirementInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Lato:wght@400;600;700&display=swap');

        body {
            font-family: 'Lato', sans-serif;
            text-align: center;
            font-size: 1.25rem;
            font-weight: 400;
        }

        p {
            font-weight: 400;
        }

        strong {
            font-family: 'Lato', sans-serif;
            font-weight: 700; /* Lato Bold */
        }

        h1, h2, h3 {
            font-family: 'Lato', sans-serif;
            font-weight: 700;
        }

        h1 {
            color: #B90000;
            font-size: 3rem;
            margin-bottom: 2rem;
        }

        h2 {
            font-size: 2.5rem;
            margin-top: 2.5rem;
            margin-bottom: 1.5rem;
        }

        h3 {
            font-size: 2rem;
            margin-top: 2rem;
            margin-bottom: 1rem;
        }

        .container {
            padding: 2rem 1rem;
            max-width: 1200px;
            margin: 0 auto;
        }

        .row-centered {
            justify-content: center;
            gap: 1rem; /* spacing between feature boxes */
        }

        .feature-card {
            border: 2px solid #FB0202;
            border-radius: 12px;
            background-color: #FB0202;
            color: white;
            padding: 1rem; /* ↓ Reduced from 1.5rem */
            min-height: 260px;
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
            padding: 10rem 10rem;
            font-size: 30px;
        }

            .btn-primary:hover {
                background-color: #d50101 !important;
            }

        input.btn.btn-primary {
            background-color: #FB0202 !important;
            border: none !important;
            border-radius: 20px !important;
            font-weight: 700 !important;
            color: white !important;
            padding: 1.2rem 3rem !important;
            font-size: 1.5rem !important;
            line-height: 1.3 !important;
            text-align: center !important;
            display: inline-block !important;
            width: auto !important;
        }

        .bg-image-section {
            position: relative;
            width: 100%;
            max-width: 580px;
            height: 386px; /* directly set the height */
            margin: 0 auto 2rem;
            background-image: url('Images/Retireconsult.jpg');
            background-size: cover;
            background-position: center;
            border-radius: 12px;
            overflow: hidden;
        }

            .bg-image-section::before {
                content: '';
                position: absolute;
                inset: 0;
                background: rgba(70, 70, 70, 0.5); /* translucent overlay */
                z-index: 1;
            }

        .bg-image-section-content {
            position: absolute;
            inset: 0;
            z-index: 2;
            color: white;
            padding: 2rem;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
        }



        .why-choose-us-wrapper {
            background-color: #464646;
            color: white;
            padding: 1rem 0 3rem 0;;
            width: 100%;
        }

        .why-choose-us {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 1rem;
        }

            .why-choose-us .table {
                background-color: white;
                color: black;
                border-collapse: collapse;
                width: 100%;
                margin-top: 2rem;
                border: 3px solid #FB0202;
                border-radius: 12px;
                overflow: hidden;
            }

                .why-choose-us .table th,
                .why-choose-us .table td {
                    border: 1px solid black;
                    padding: 1rem;
                    text-align: center;
                }

                    .why-choose-us .table thead th,
                    .why-choose-us .table td:first-child {
                        font-weight: 600;
                    }

        .interested-section-wrapper {
            background-color: #FFE4E7;
            color: #FB0202;
            margin-top: -20px;
            padding: 1rem 0 3rem 0;
            width: 100%;
            position: relative;
            z-index: 2;
        }

        .interested-section {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 1rem;
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

        .table + .interested-section-wrapper {
            margin-top: 2rem !important;
        }
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container">
        <h1>Flexi Retirement Savings Plan</h1>

        <div class="bg-image-section">
            <div class="bg-image-section-content">
                <h2>What It Is ✨</h2>
                <p>SingLife Flexi Retirement Savings Plan is a high-interest, long-term savings plan with built-in insurance protection.</p>
            </div>
        </div>

        <h2 class="mb-3">Features 📝</h2>
        <div class="row g-3 justify-content-center">
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>💰 Low Minimum Deposit</h5>
                    <p>No need for a big deposit — begin your retirement plan with as little as $1000 per year.</p>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>📈 Earn 4% Interest p.a.</h5>
                    <p>Grow your savings faster with 4% compound interest per year, much higher than most bank accounts.</p>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>💸 Savings + Insurance in One</h5>
                    <p>Your money grows over time, and you also get basic life insurance to protect your loved ones.</p>
                </div>
            </div>
            <div class="col-12 col-sm-6 col-md-3 d-flex">
                <div class="feature-card w-100">
                    <h5>👨‍👩‍👧 Helping you to Plan Ahead</h5>
                    <p>Use our calculator to find out how much payout you will receive or premium amount to contribute!</p>
                </div>
            </div>
        </div>

        <div class="learn-more-section">
            <h2>Learn More</h2>
            <p>To know the full information on benefits and policy terms, please click the button below.</p>
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
            <p>Get in touch with our Financial Consultants to sign up!</p>
            <asp:Button ID="btnContact" runat="server" Text="Get in touch" CssClass="btn btn-primary" OnClick="btnContact_Click" />
        </div>
    </div>
</asp:Content>
