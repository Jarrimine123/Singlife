<%@ Page Title="Claim History" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="ClaimHistory.aspx.cs" Inherits="Singlife.ClaimHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .claim-box {
            background: #ffffff;
            border-radius: 1rem;
            padding: 1rem 1.25rem;
            margin-bottom: 1rem;
            box-shadow: 0 0.25rem 0.75rem rgba(0,0,0,0.04);
            border: 1px solid #e3e6f0;
            transition: transform 0.2s ease-in-out;
            height: 100%;
        }

        .claim-box:hover {
            transform: scale(1.01);
        }

        .claim-header {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            align-items: center;
            margin-bottom: 0.25rem;
        }

        .claim-plan-name {
            font-weight: 600;
            font-size: 1.1rem;
            color: #343a40;
        }

        .claim-status-label {
            font-size: 0.95rem;
        }

        .claim-date {
            font-size: 0.85rem;
            color: #6c757d;
            margin-bottom: 0.25rem;
        }

        .status-received {
            color: #0d6efd;
            font-weight: 500;
        }

        .status-approved {
            color: #198754;
            font-weight: 500;
        }

        .status-action {
            color: #dc3545;
            font-weight: 500;
        }

        .claim-comment {
            margin-top: 0.5rem;
            font-size: 0.9rem;
            background-color: #f8d7da;
            padding: 0.5rem 0.75rem;
            border-left: 4px solid #dc3545;
            border-radius: 0.375rem;
            color: #842029;
        }

        .edit-link {
            display: inline-block;
            margin-top: 0.5rem;
            font-size: 0.85rem;
            color: #0d6efd;
            font-weight: 500;
            text-decoration: none;
        }

        .edit-link:hover {
            text-decoration: underline;
        }

        .info-note {
            background-color: #e9f7ef;
            border-left: 5px solid #198754;
            padding: 0.75rem 1rem;
            border-radius: 0.375rem;
            font-size: 0.9rem;
            margin-bottom: 1rem;
            color: #155724;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container mt-4">
        <h3 class="mb-4 text-primary">Your Claim History</h3>

        <div class="info-note">
            You can only <strong>edit claims</strong> within <strong>2 days</strong> from submission, and only if the claim status is <strong>Received</strong>.
        </div>

        <asp:Panel ID="pnlSearchForm" runat="server" CssClass="row g-2 align-items-center mb-4">
            <div class="col-md-4">
                <asp:TextBox ID="txtSearchPlan" runat="server" CssClass="form-control" placeholder="Search by Plan Name" />
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="All Statuses" Value="" />
                    <asp:ListItem Text="Received" Value="Received" />
                    <asp:ListItem Text="Approved" Value="Approved" />
                    <asp:ListItem Text="Action Needed" Value="Action Needed" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </asp:Panel>

        <div class="row">
            <asp:Repeater ID="rptClaimHistory" runat="server">
                <ItemTemplate>
                    <div class="col-md-6 mb-3">
                        <div class="claim-box">
                            <div class="claim-header">
                                <div class="claim-plan-name"><%# Eval("PlanName") %></div>
                                <div class="claim-status-label">
                                    <strong>Status:</strong>
                                    <span class='<%# ((Singlife.ClaimHistory)Page).GetStatusClass(Eval("ReviewStatus")) %>'>
                                        <%# Eval("ReviewStatus") ?? "Received" %>
                                    </span>
                                </div>
                            </div>
                            <div class="claim-date">Date received: <%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %></div>
                            <%# ((Singlife.ClaimHistory)Page).ShowCommentIfNeeded(Eval("ReviewStatus"), Eval("Comment")) %>
                            <%# ((Singlife.ClaimHistory)Page).ShowEditButton(Eval("ClaimID"), Eval("CreatedDate"), Eval("ReviewStatus")) %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
