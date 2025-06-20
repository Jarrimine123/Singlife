<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="StaffPurchase.aspx.cs" Inherits="Singlife.StaffPurchase" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <h2 class="mb-4">Customer Purchases</h2>

        <!-- Filter Form -->
        <div class="row mb-4">
            <div class="col-md-4">
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Customer Name" />
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtPlan" runat="server" CssClass="form-control" placeholder="Plan Name" />
            </div>
            <div class="col-md-4">
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-primary me-2" OnClick="btnFilter_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
            </div>
        </div>

        <!-- GridView -->
        <asp:GridView ID="gvPurchases" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Customer Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" />
                <asp:BoundField DataField="ProductName" HeaderText="Product" />
                <asp:BoundField DataField="PlanName" HeaderText="Plan" />
                <asp:BoundField DataField="CoverageAmount" HeaderText="Coverage" DataFormatString="{0:C}" />
                <asp:BoundField DataField="MonthlyPremium" HeaderText="Monthly Premium" DataFormatString="{0:C}" />
                <asp:BoundField DataField="CardLast4" HeaderText="Card Last 4 Digits" />
                <asp:BoundField DataField="PurchaseDate" HeaderText="Date" DataFormatString="{0:g}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>



