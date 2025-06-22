<%@ Page Title="Staff Review Management" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="StaffReview.aspx.cs" Inherits="Singlife.StaffReview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2>Customer Reviews Management</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="text-success mb-3"></asp:Label>

        <asp:Repeater ID="rptReviews" runat="server">
            <HeaderTemplate>
                <table class="table table-bordered table-hover">
                    <thead class="table-light">
                        <tr>
                            <th>Review ID</th>
                            <th>Account ID</th>
                            <th>Plan Name</th>
                            <th>Rating</th>
                            <th>Review Text</th>
                            <th>Review Date</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("ReviewID") %></td>
                    <td><%# Eval("AccountID") %></td>
                    <td><%# Eval("PlanName") %></td>
                    <td><%# Eval("Rating") %></td>
                    <td style="max-width: 300px; word-wrap: break-word;"><%# Eval("ReviewText") %></td>
                    <td><%# Eval("ReviewDate", "{0:yyyy-MM-dd HH:mm}") %></td>
                    <td>
                        <%# (Convert.ToBoolean(Eval("IsApproved")) ? "Approved" : "Pending Approval") %>
                    </td>
                    <td>
                        <%-- Approve button only if not approved --%>
                        <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-success btn-sm me-2"
                            CommandArgument='<%# Eval("ReviewID") %>' OnClick="btnApprove_Click"
                            Visible='<%# !Convert.ToBoolean(Eval("IsApproved")) %>' />

                        <%-- Reject button (delete) only if not approved --%>
                        <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn btn-danger btn-sm me-2"
                            CommandArgument='<%# Eval("ReviewID") %>' OnClick="btnReject_Click"
                            Visible='<%# !Convert.ToBoolean(Eval("IsApproved")) %>' />

                        <%-- Delete button only if approved --%>
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm"
                            CommandArgument='<%# Eval("ReviewID") %>' OnClick="btnDelete_Click"
                            Visible='<%# Convert.ToBoolean(Eval("IsApproved")) %>' />
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
