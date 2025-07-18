﻿<%@ Page Title="Choose Claim" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="ChooseClaim.aspx.cs" Inherits="Singlife.ChooseClaim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            background-color: #f8f9fa;
        }

        .claim-header {
            padding: 4rem 1rem 2rem;
            text-align: center;
            background-color: #fff;
            border-bottom: 1px solid #dee2e6;
            margin-bottom: 3rem;
        }

            .claim-header h2 {
                font-weight: 700;
                color: #e60012;
                font-size: 2.75rem;
                line-height: 1.2;
            }

            .claim-header p {
                font-size: 1.1rem;
                color: #6c757d;
                margin-top: 0.75rem;
            }

        .nav-pills .nav-link {
            font-weight: 600;
            font-size: 1.125rem;
            color: #6c757d;
            border-radius: 50px;
            padding: 0.5rem 1.5rem;
            transition: all 0.3s ease;
            border: 2px solid transparent;
        }

            .nav-pills .nav-link.active,
            .nav-pills .nav-link:hover {
                color: #fff;
                background-color: #e60012;
                border-color: #e60012;
                box-shadow: 0 4px 10px rgb(230 0 18 / 0.4);
            }

        .claim-section {
            max-width: 1140px;
            margin-left: auto;
            margin-right: auto;
            margin-bottom: 2rem; /* 👈 Add this line */
        }


        .claim-card {
            background: #fff;
            border-radius: 1rem;
            box-shadow: 0 5px 15px rgb(0 0 0 / 0.1);
            padding: 2rem 1.5rem;
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            height: 100%;
        }

            .claim-card:hover {
                transform: translateY(-8px);
                box-shadow: 0 15px 30px rgb(230 0 18 / 0.25);
            }

        .claim-icon {
            font-size: 4.5rem;
            color: #fff;
            width: 90px;
            height: 90px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 1.25rem;
            box-shadow: 0 4px 12px rgb(230 0 18 / 0.5);
        }

        .small-desc {
            font-size: 1rem;
            font-weight: 500;
            color: #6c757d;
            min-height: 3rem;
            margin-bottom: 1.5rem;
        }

        .claim-btn a {
            font-weight: 700;
            padding: 0.625rem 2rem;
            font-size: 1.125rem;
            border-radius: 50px;
            color: #fff !important;
            background-color: #e60012;
            text-decoration: none;
            box-shadow: 0 6px 15px rgb(230 0 18 / 0.4);
            transition: background-color 0.3s ease, box-shadow 0.3s ease;
            display: inline-block;
        }

            .claim-btn a:hover {
                background-color: #b4000e;
                box-shadow: 0 8px 20px rgb(180 0 14 / 0.6);
                color: #fff !important;
                text-decoration: none;
            }

        /* Color highlight styles */
        .card-red {
            border-top: 6px solid #e60012;
        }

        .card-blue {
            border-top: 6px solid #007bff;
        }

        .card-green {
            border-top: 6px solid #28a745;
        }

        .card-default {
            border-top: 6px solid #adb5bd;
        }

        .icon-red {
            background-color: #e60012;
        }

        .icon-blue {
            background-color: #007bff;
        }

        .icon-green {
            background-color: #28a745;
        }

        .icon-default {
            background-color: #adb5bd;
        }

        .plan-name {
            font-size: 2rem;
            font-weight: 800;
            color: #212529;
            margin-bottom: 0.25rem;
        }

        .product-badge {
            font-size: 0.95rem;
            color: #6c757d;
            background-color: #f1f3f5;
            padding: 0.3rem 0.8rem;
            border-radius: 30px;
            font-weight: 600;
            margin-bottom: 1rem;
            display: inline-block;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="claim-header">
        <h2>We're here to help — what would you like to do today?</h2>
        <p>Select a purchased plan below to start your claim.</p>
    </div>

    <ul class="nav nav-pills justify-content-center mb-5" id="claimTab" role="tablist">
        <li class="nav-item" role="presentation">
            <a class="nav-link active" href="ChooseClaim.aspx" role="tab" aria-selected="true">New Claim</a>
        </li>
        <li class="nav-item" role="presentation">
            <a class="nav-link" href="SaveAsDraft.aspx" role="tab" aria-selected="false">Continue Draft Claims</a>
        </li>
    </ul>

    <div class="claim-section">
        <div class="row row-cols-1 row-cols-md-3 g-5">
            <asp:Repeater ID="rptPlans" runat="server" OnItemDataBound="rptPlans_ItemDataBound">
                <ItemTemplate>
                    <div class="col d-flex align-items-stretch">
                        <div class='claim-card <%# GetCardColorClass(Eval("PlanName").ToString()) %>'>
                            <div class='claim-icon <%# GetIconColorClass(Eval("PlanName").ToString()) %>'>
                                <i class="bi bi-heart-pulse-fill"></i>
                            </div>
                            <div class="claim-text">
                                <h5 class="plan-name"><%# Eval("PlanName") %></h5>
                                <div class="product-badge"><%# Eval("ProductName") %></div>
                                <p class="small-desc" runat="server" id="lblShortDesc"></p>
                            </div>
                            <div class="claim-btn mt-auto">
                                <a href='<%# GetClaimPageUrl(Eval("PlanName").ToString()) %>'>File a Claim</a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
