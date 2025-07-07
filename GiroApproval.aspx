<%@ Page Title="GIRO Approval" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="GiroApproval.aspx.cs" Inherits="Singlife.GiroApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2>GIRO Approvals and History</h2>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-success mb-3"></asp:Label>

        <!-- Filters -->
        <div class="mb-3">
            <asp:Label runat="server" Text="Account ID: " AssociatedControlID="txtAccountFilter" />
            <asp:TextBox ID="txtAccountFilter" runat="server" CssClass="form-control d-inline-block w-auto" />

            <asp:Label runat="server" Text="From: " AssociatedControlID="txtDateFrom" CssClass="ms-3" />
            <asp:TextBox ID="txtDateFrom" runat="server" TextMode="Date" CssClass="form-control d-inline-block w-auto" />

            <asp:Label runat="server" Text="To: " AssociatedControlID="txtDateTo" CssClass="ms-3" />
            <asp:TextBox ID="txtDateTo" runat="server" TextMode="Date" CssClass="form-control d-inline-block w-auto" />

            <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-primary ms-3" OnClick="btnFilter_Click" />
        </div>

        <asp:Repeater ID="rptGiroPending" runat="server" OnItemCommand="rptGiroPending_ItemCommand">
            <HeaderTemplate>
                <table class="table table-bordered table-hover">
                    <thead class="table-light">
                        <tr>
                            <th>Purchase ID</th>
                            <th>Account ID</th>
                            <th>Customer Name</th>
                            <th>Amount</th>
                            <th>Next Billing Date</th>
                            <th>Status</th>
                            <th>GIRO Form</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("PurchaseID") %></td>
                    <td><%# Eval("AccountID") %></td>
                    <td><%# Eval("CustomerName") %></td>
                    <td><%# Eval("Amount", "{0:C}") %></td>
                    <td><%# Eval("NextBillingDate", "{0:yyyy-MM-dd}") %></td>
                    <td><%# Eval("Status") %></td>
                    <td>
                        <%# 
                            Eval("GiroFormPath") != DBNull.Value && 
                            !string.IsNullOrEmpty(Eval("GiroFormPath").ToString()) 
                            ? $"<a href='{ResolveUrl("~/GiroForms/" + Eval("GiroFormPath"))}' target='_blank'>Download</a>" 
                            : "No File" 
                        %>
                    </td>
                    <td>
                        <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-success btn-sm me-2"
                            CommandArgument='<%# Eval("RecurringPaymentID") %>' CommandName="Approve"
                            Visible='<%# Eval("Status").ToString().Equals("Pending", StringComparison.OrdinalIgnoreCase) %>' />

                        <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn btn-danger btn-sm"
                            CommandArgument='<%# Eval("RecurringPaymentID") %>' CommandName="Reject"
                            Visible='<%# Eval("Status").ToString().Equals("Pending", StringComparison.OrdinalIgnoreCase) %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
