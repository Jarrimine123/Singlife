<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Singlife.Checkout" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script type="text/javascript">
        function formatExpiry(input) {
            let value = input.value.replace(/\D/g, '');
            if (value.length > 4) value = value.substring(0, 4);
            if (value.length >= 3) {
                input.value = value.substring(0, 2) + '/' + value.substring(2);
            } else {
                input.value = value;
            }
        }
    </script>

    <div class="container py-5">
        <h2 class="mb-4">Checkout</h2>

        <asp:GridView ID="gvOrderSummary" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="ProductName" HeaderText="Product" />
                <asp:BoundField DataField="PlanName" HeaderText="Plan" />
                <asp:TemplateField HeaderText="Coverage">
                    <ItemTemplate>
                        <%# GetCoverageDisplay(Container.DataItem) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PaymentFrequency" HeaderText="Payment Frequency" />
                <asp:TemplateField HeaderText="Premium">
                    <ItemTemplate>
                        <%# GetPremiumDisplay(Container.DataItem) %>
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

        <h4 class="mt-5">Card Details</h4>
        <div class="row g-3">
            <div class="col-md-6">
                <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control" placeholder="Card Number" MaxLength="16" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control" placeholder="MM/YY" MaxLength="5" onkeyup="formatExpiry(this)" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" placeholder="CVV" MaxLength="4" />
            </div>
        </div>

        <div class="mt-4">
            <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" ForeColor="Red" Visible="false" />
            <asp:Button ID="btnPlaceOrder" runat="server" CssClass="btn btn-primary" Text="Place Order" OnClick="btnPlaceOrder_Click" />
        </div>
    </div>
</asp:Content>
