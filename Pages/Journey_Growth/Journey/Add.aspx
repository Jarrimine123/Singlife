<%@ Page Title="Add Journey Mapping" Language="C#" MasterPageFile="~/Pages/Master/Journey.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Singlife.Pages.Journey_Growth.Journey.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- You can add custom head content here if needed -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">
        <h2 class="mb-4">Add Journey Mapping</h2>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-success"></asp:Label>

        <div class="mb-3">
            <label for="txtTitle" class="form-label">Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter title"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label for="txtYear" class="form-label">Year</label>
            <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" placeholder="Enter year"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label for="txtDescription" class="form-label">Description</label>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Enter description"></asp:TextBox>
        </div>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />


         <hr class="my-5" />

        <h3>My Journey Mappings</h3>
        <asp:GridView ID="gvJourneyMappings" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="MilestoneID" HeaderText="ID" />
                <asp:BoundField DataField="Year" HeaderText="Year" />
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>


    </div>

</asp:Content>
