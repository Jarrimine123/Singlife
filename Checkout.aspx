<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Singlife.Checkout" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <div class="container py-5">
        <h2 class="mb-4">Checkout</h2>

        <asp:GridView ID="gvOrderSummary" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False" EnableViewState="true">
            <Columns>
                <asp:BoundField DataField="ProductName" HeaderText="Product" />
                <asp:BoundField DataField="PlanName" HeaderText="Plan" />
                <asp:TemplateField HeaderText="Coverage">
                    <ItemTemplate>
                        <%# ((Singlife.Checkout)Page).GetCoverageDisplay(Container.DataItem) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PaymentFrequency" HeaderText="Payment Frequency" />
                <asp:TemplateField HeaderText="Premium">
                    <ItemTemplate>
                        <%# ((Singlife.Checkout)Page).GetPremiumDisplay(Container.DataItem) %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="text-end mt-2">
            <strong>Total Premium: </strong><asp:Label ID="lblTotalMonthly" runat="server" CssClass="fw-bold fs-5" />
        </div>

        <hr />

        <h4 class="mt-5">Customer Information</h4>
        <div class="row g-3">
            <div class="col-md-6">
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Full Name" />
            </div>
            <div class="col-md-6">
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email" />
            </div>
            <div class="col-md-6">
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Phone Number" />
            </div>
            <div class="col-md-6">
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Address" />
            </div>
        </div>

        <div class="mt-4">
            <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3 text-danger" Visible="false" />
            <asp:Button ID="btnPlaceOrder" runat="server" CssClass="btn btn-primary" Text="Pay with Stripe" OnClick="btnPlaceOrder_Click" />
        </div>
    </div>

</asp:Content>
