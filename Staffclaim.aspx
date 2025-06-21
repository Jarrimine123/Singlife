<%@ Page Title="Staff Claim Review" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Staffclaim.aspx.cs" Inherits="Singlife.Staffclaim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .claim-card {
            margin-bottom: 20px;
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 15px 20px;
            background-color: #fff;
            box-shadow: 0 2px 6px rgba(0,0,0,0.05);
        }
        .claim-title {
            font-size: 1.2rem;
            font-weight: 700;
            margin-bottom: 12px;
            color: #333;
        }
        .claim-info-grid {
            display: grid;
            grid-template-columns: 160px 1fr;
            gap: 8px 20px;
            align-items: center;
            font-size: 0.9rem;
            color: #555;
            margin-bottom: 15px;
        }
        .claim-info-grid .row-label {
            font-weight: 600;
            color: #444;
        }
        .claim-info-grid .row-value a {
            color: #007bff;
            text-decoration: none;
            margin-right: 10px;
        }
        .claim-info-grid .row-value a:hover {
            text-decoration: underline;
        }
        .actions-row {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
            margin-top: 10px;
        }
        .actions-row .form-select,
        .actions-row .form-control {
            font-size: 0.9rem;
            padding: 6px 10px;
        }
        .actions-row .btn {
            padding: 6px 15px;
            font-size: 0.9rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2 class="mb-4">Submitted Claims (Staff Review)</h2>

    <asp:Repeater ID="rptClaims" runat="server" OnItemCommand="rptClaims_ItemCommand">
        <ItemTemplate>
            <div class="claim-card">
                <div class="claim-title">
                    Claim ID: <%# Eval("ClaimID") %> | Customer: <%# Eval("Name") %>
                </div>

                <div class="claim-info-grid">
                    <asp:Literal ID="litClaimDetails" runat="server" Text='<%# GetClaimDetails(Container.DataItem) %>'></asp:Literal>
                </div>

                <div class="actions-row">
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select" style="min-width:150px;">
                        <asp:ListItem Text="Received" />
                        <asp:ListItem Text="Approved" />
                        <asp:ListItem Text="Action Needed" />
                    </asp:DropDownList>

                    <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Enter staff comment..." style="flex-grow:1;"></asp:TextBox>

                    <asp:FileUpload ID="fuOutcomeFile" runat="server" CssClass="form-control" Style="max-width:300px;" />

                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="UpdateStatus" CommandArgument='<%# Eval("ClaimID") %>' CssClass="btn btn-danger btn-sm" />

                    <asp:HiddenField ID="hfClaimType" runat="server" Value='<%# Eval("ClaimType") %>' />
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-success fw-bold mt-3" Visible="false"></asp:Label>
</asp:Content>
