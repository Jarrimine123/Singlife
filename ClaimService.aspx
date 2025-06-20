<%@ Page Title="Claim Services" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="ClaimService.aspx.cs" Inherits="Singlife.ClaimService" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .claim-card {
            border-radius: 16px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            padding: 24px;
            transition: box-shadow 0.3s;
            height: 100%;
            background: #fff;
        }

        .claim-card:hover {
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }

        .claim-icon {
            font-size: 2rem;
            margin-bottom: 12px;
        }

        .claim-title {
            font-weight: 600;
            font-size: 1.25rem;
        }

        .claim-desc {
            color: #555;
            font-size: 0.95rem;
        }

        .claim-section-header {
            font-size: 1.5rem;
            font-weight: 700;
            margin-bottom: 20px;
        }

        .claim-wrapper {
            max-width: 1000px;
            margin: auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5 claim-wrapper">
        <h2 class="claim-section-header text-center mb-4">Manage Your Claims</h2>
        <p class="text-center mb-5 text-muted">Easily submit new claims or review your claim history. Our team is here to help you every step of the way.</p>

        <div class="row g-4">
            <!-- Submit a Claim -->
            <div class="col-md-6">
                <div class="claim-card h-100 text-start">
                    <div class="text-warning claim-icon">
                        <i class="bi bi-shield-fill-exclamation"></i>
                    </div>
                    <div class="claim-title">Submit a Claim</div>
                    <p class="claim-desc">Start a new claim submission and get connected with our experts to help process your claim efficiently.</p>
                    <a href="ChooseClaim.aspx" class="btn btn-warning mt-3">Start Claim</a>
                </div>
            </div>

            <!-- Claim History -->
            <div class="col-md-6">
                <div class="claim-card h-100 text-start">
                    <div class="text-primary claim-icon">
                        <i class="bi bi-clock-history"></i>
                    </div>
                    <div class="claim-title">Claim History</div>
                    <p class="claim-desc">View your past claims, check statuses, and download payout invoices anytime you need.</p>
                    <a href="ClaimHistory.aspx" class="btn btn-primary mt-3">View History</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
