<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="Singlife.Cart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5">
        <h2 class="text-center mb-4">🛒 Your Cart</h2>

        <asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped"
            DataKeyNames="CartID" OnRowCommand="gvCart_RowCommand" HeaderStyle-CssClass="table-primary" EmptyDataText="Your cart is empty.">
            <Columns>
                <asp:BoundField DataField="PlanName" HeaderText="Plan Name" />
                <asp:BoundField DataField="CoverageAmount" HeaderText="Coverage (SGD)" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="AnnualPremium" HeaderText="Annual Premium (SGD)" DataFormatString="{0:F2}" />
                <asp:BoundField DataField="MonthlyPremium" HeaderText="Monthly Premium (SGD)" DataFormatString="{0:F2}" />
                <asp:BoundField DataField="DateAdded" HeaderText="Date Added" DataFormatString="{0:dd MMM yyyy}" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button runat="server" Text="🗑 Remove" CommandName="DeleteItem" CommandArgument="<%# Container.DataItemIndex %>" CssClass="btn btn-danger btn-sm" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="text-center mt-4">
            <asp:Button ID="btnCheckout" runat="server" Text="🛍 Proceed to Checkout" CssClass="btn btn-success px-4 py-2" OnClick="btnCheckout_Click" />
        </div>
    </div>
</asp:Content>

