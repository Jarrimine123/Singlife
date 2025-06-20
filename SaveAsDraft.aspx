<%@ Page Title="Saved Drafts" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="SaveAsDraft.aspx.cs" Inherits="Singlife.SaveAsDraft" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="container py-5">
        <h2 class="mb-4 fw-semibold text-dark">Saved Drafts</h2>

        <asp:Repeater ID="rptDrafts" runat="server" OnItemCommand="rptDrafts_ItemCommand">
            <ItemTemplate>
                <div class="card mb-3 shadow-sm border-0">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <h5 class="card-title mb-1 text-dark">
                                <%# Eval("PlanName") %> <span class="badge bg-light text-secondary fw-normal">Draft</span>
                            </h5>
                            <p class="card-subtitle text-muted small">
                                Autosaved • <%# Eval("ModifiedDate", "{0:dd MMM} • {0:hh:mmtt}") %>
                            </p>
                        </div>
                        <div class="d-flex">
                            <asp:HyperLink ID="lnkContinue" runat="server"
                                CssClass="btn btn-outline-danger me-2"
                                NavigateUrl='<%# Eval("ClaimID", "~/ContinueDraft.aspx?claimId={0}") %>'>
                                Continue submission <i class="bi bi-arrow-right ms-1"></i>
                            </asp:HyperLink>
                            <asp:LinkButton ID="btnDelete" runat="server"
                                CssClass="btn btn-outline-secondary"
                                CommandName="Delete"
                                CommandArgument='<%# Eval("ClaimID") %>'>
                                <i class="bi bi-trash"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Label ID="lblNoDrafts" runat="server" Text="No saved drafts found." CssClass="text-muted fs-5" Visible="false" />
    </div>
</asp:Content>
