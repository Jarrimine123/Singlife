<%@ Page Title="Staff Claim Review" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Staffclaim.aspx.cs" Inherits="Singlife.Staffclaim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-box {
            background-color: #fff;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
        }

        .claim-card {
            margin-bottom: 25px;
            border: 1px solid #ddd;
            border-radius: 12px;
            padding: 20px;
            background-color: #fff;
            box-shadow: 0 1px 6px rgba(0,0,0,0.03);
        }

        .claim-title {
            font-size: 1.2rem;
            font-weight: 600;
            margin-bottom: 16px;
            color: #222;
        }

        .claim-info-grid {
            display: grid;
            grid-template-columns: 180px 1fr;
            gap: 6px 20px;
            font-size: 0.92rem;
            color: #444;
            margin-bottom: 15px;
        }

        .row-label {
            font-weight: 600;
            color: #333;
        }

        .row-value a {
            color: #007bff;
            text-decoration: none;
        }

        .actions-row {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            margin-top: 10px;
            align-items: flex-start;
        }

        .actions-row .form-control,
        .actions-row .form-select {
            font-size: 0.9rem;
            padding: 6px 10px;
        }

        .actions-row .btn {
            font-size: 0.9rem;
            padding: 6px 15px;
        }

        .review-panel {
            margin-top: 15px;
            border-top: 1px solid #ccc;
            padding-top: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2 class="mb-4">Submitted Claims (Staff Review)</h2>

    <div class="filter-box mb-4">
        <div class="row g-3 align-items-end">
            <div class="col-md-4">
                <label for="txtSearchName" class="form-label">Filter by Customer Name</label>
                <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" placeholder="Enter name..." />
            </div>
            <div class="col-md-4">
                <label for="txtSearchPlan" class="form-label">Filter by Plan Name</label>
                <asp:TextBox ID="txtSearchPlan" runat="server" CssClass="form-control" placeholder="Enter plan name..." />
            </div>
            <div class="col-md-4 d-grid">
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-danger" OnClick="btnFilter_Click" />
            </div>
        </div>
    </div>

    <asp:Repeater ID="rptClaims" runat="server" OnItemCommand="rptClaims_ItemCommand">
        <ItemTemplate>
            <div class="claim-card">
                <div class="claim-title">
                    Claim ID: <%# Eval("ClaimID") %> | Customer: <%# Eval("Name") %>
                </div>

                <div class="claim-info-grid">
                    <asp:Literal ID="litClaimDetails" runat="server" Text='<%# GetClaimDetails(Container.DataItem) %>' />
                </div>

                <div class="actions-row">
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select" style="min-width:150px;">
                        <asp:ListItem Text="Received" />
                        <asp:ListItem Text="Approved" />
                        <asp:ListItem Text="Action Needed" />
                    </asp:DropDownList>

                    <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Enter staff comment..." style="flex-grow:1;" />

                    <asp:FileUpload ID="fuOutcomeFile" runat="server" CssClass="form-control" style="max-width:300px;" />

                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="UpdateStatus" CommandArgument='<%# Eval("ClaimID") %>' CssClass="btn btn-danger btn-sm" />

                    <asp:HiddenField ID="hfClaimType" runat="server" Value='<%# Eval("ClaimType") %>' />
                </div>

                <asp:Panel ID="pnlReviewSummary" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("ReviewStatus").ToString()) %>' CssClass="review-panel">
                    <div class="claim-info-grid">
                        <div class="row-label">Review Status:</div>
                        <div class="row-value"><%# Eval("ReviewStatus") %></div>

                        <div class="row-label">Staff Comment:</div>
                        <div class="row-value"><%# Eval("ReviewComment") %></div>

                        <asp:Panel ID="pnlOutcomeFile" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("OutcomeFilePath").ToString()) %>'>
                            <div class="row-label">Outcome File:</div>
                            <div class="row-value">
                                <a href='<%# ResolveUrl(Eval("OutcomeFilePath").ToString()) %>' target="_blank">View File</a>
                            </div>
                        </asp:Panel>
                    </div>
                </asp:Panel>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-success fw-bold mt-3" Visible="false" />
</asp:Content>
