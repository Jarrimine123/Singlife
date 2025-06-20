<%@ Page Title="Choose Claim" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="ChooseClaim.aspx.cs" Inherits="Singlife.ChooseClaim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            background-color: #f1f3f5;
        }

        .claim-header {
            padding: 60px 20px 20px 20px;
            text-align: center;
            background-color: #ffffff;
            border-bottom: 1px solid #dee2e6;
        }

        .claim-header h2 {
            font-weight: 700;
            color: #e60012;
        }

        .claim-tabs {
            border-bottom: 1px solid #dee2e6;
            gap: 20px;
        }

        .claim-tabs .nav-link {
            color: #6c757d;
            font-weight: 600;
            border: none;
            border-radius: 0;
            position: relative;
            background-color: transparent;
            padding-bottom: 8px;
            font-size: 16px;
        }

        .claim-tabs .nav-link.active {
            color: #e60012;
            border-bottom: 3px solid #e60012;
        }

        .claim-section {
            background-color: #f9f9f9;
            border-radius: 20px;
            padding: 40px 30px;
        }

        .claim-card {
            border: none;
            border-radius: 16px;
            padding: 25px;
            height: 100%;
            background: #fff;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            position: relative;
        }

        .claim-card:hover {
            transform: translateY(-8px);
            box-shadow: 0 12px 25px rgba(0, 0, 0, 0.1);
        }

        .claim-icon {
            font-size: 36px;
            color: #fff;
            background-color: #e60012;
            width: 70px;
            height: 70px;
            line-height: 70px;
            border-radius: 50%;
            margin: 0 auto 15px auto;
        }

        .claim-btn a {
            margin-top: 20px;
            font-weight: 600;
            padding: 10px 24px;
            font-size: 15px;
            border-radius: 30px;
        }

        .claim-text h5 {
            font-size: 18px;
            margin-bottom: 5px;
        }

        .claim-text p {
            font-size: 13.5px;
            margin-bottom: 5px;
        }

        .plan-badge {
            display: inline-block;
            font-size: 12px;
            background-color: #e60012;
            color: white;
            padding: 3px 10px;
            border-radius: 30px;
            margin-top: 6px;
            font-weight: 500;
        }

        .small-desc {
            font-size: 13.5px;
            font-weight: 500;
            color: #555;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5">
        <div class="claim-header mb-4">
            <h2>We're here to help — what would you like to do today?</h2>
            <p class="text-muted">Select a purchased plan below to start your claim.</p>
        </div>

        <!-- Tabs -->
        <div class="d-flex justify-content-center mb-4">
            <ul class="nav nav-pills claim-tabs" id="claimTab" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="new-claim-tab" data-bs-toggle="pill" type="button" role="tab">
                        New Claim
                    </button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="draft-claim-tab" onclick="window.location.href='SaveAsDraft.aspx'" type="button" role="tab">
                        Continue Draft Claims
                    </button>
                </li>
            </ul>
        </div>

        <!-- Cards Section -->
        <div class="claim-section mt-4">
            <div class="row row-cols-1 row-cols-md-3 g-4">
                <asp:Repeater ID="rptPlans" runat="server" OnItemDataBound="rptPlans_ItemDataBound">
                    <ItemTemplate>
                        <div class="col">
                            <div class="claim-card text-center d-flex flex-column justify-content-between">
                                <div>
                                    <div class="claim-icon d-flex align-items-center justify-content-center">
                                        <i class="bi bi-heart-pulse-fill"></i>
                                    </div>
                                    <div class="claim-text mt-2">
                                        <h5 class="fw-bold text-dark"><%# Eval("ProductName") %></h5>
                                        <div class="plan-badge"><%# Eval("PlanName") %></div>
                                        <p class="small-desc mt-2" runat="server" id="lblShortDesc"></p>
                                    </div>
                                </div>
                                <div class="claim-btn">
                                    <a href='<%# "OncoShieldTimeClaim.aspx?plan=" + Eval("PlanName") %>' class="btn btn-danger">File a Claim</a>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
