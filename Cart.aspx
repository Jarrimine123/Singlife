<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="Singlife.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Bootstrap CSS is assumed to be included in the MasterPage -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5">
        <h2 class="text-center mb-4">🛒 Your Cart</h2>

        <asp:Panel ID="pnlCart" runat="server" Visible="true">
<asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="False"
    CssClass="table table-striped table-hover align-middle"
    DataKeyNames="CartID"
    OnRowCommand="gvCart_RowCommand"
    HeaderStyle-CssClass="table-dark"
    EmptyDataText="Your cart is currently empty.">
    <Columns>
      
        <asp:BoundField DataField="PlanName" HeaderText="Plan Name" />
        <asp:TemplateField HeaderText="Coverage">
            <ItemTemplate>
                <%# GetCoverageOrDuration(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Premium">
            <ItemTemplate>
                <%# GetPremiumDisplay(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

     
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:LinkButton runat="server"
                    CommandName="DeleteItem"
                    CommandArgument='<%# Container.DataItemIndex %>'
                    CssClass="btn btn-danger btn-sm"
                    OnClientClick="return confirm('Are you sure you want to remove this item?');">
                    Delete
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>



            <div class="text-center mt-4">
                <asp:Button ID="btnCheckout" runat="server"
                    Text="🛍 Proceed to Checkout"
                    CssClass="btn btn-success btn-lg px-4"
                    OnClick="btnCheckout_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="text-center py-5">
            <i class="bi bi-cart-x" style="font-size: 4rem; color: #dc3545;"></i>
            <h3 class="mt-3 mb-4">Your cart is empty.</h3>
            <a href="Products.aspx" class="btn btn-primary btn-lg">Continue Shopping</a>
        </asp:Panel>
    </div>

    <!-- Bootstrap Icons CDN -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" />
</asp:Content>
