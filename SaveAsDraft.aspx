<%@ Page Title="Saved Drafts" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="SaveAsDraft.aspx.cs" Inherits="Singlife.SaveAsDraft" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            background-color: #f8f9fa;
        }

        .claim-header {
            padding: 3.5rem 1rem 2rem;
            text-align: center;
            background-color: #fff;
            border-bottom: 1px solid #dee2e6;
            margin-bottom: 3rem;
        }

        .claim-header h2 {
            font-weight: 700;
            color: #e60012;
            font-size: 2.5rem;
        }

        .claim-header p {
            font-size: 1.1rem;
            color: #6c757d;
            margin-top: 0.5rem;
        }

        /* Tabs styling to match ChooseClaim */
        .nav-pills .nav-link {
            font-weight: 600;
            font-size: 1.125rem;
            color: #6c757d;
            border-radius: 50px;
            padding: 0.5rem 1.5rem;
            transition: all 0.3s ease;
            border: 2px solid transparent;
        }

        .nav-pills .nav-link.active,
        .nav-pills .nav-link:hover {
            color: #fff;
            background-color: #e60012;
            border-color: #e60012;
            box-shadow: 0 4px 10px rgb(230 0 18 / 0.4);
        }

        .claim-section {
            max-width: 900px;
            margin-left: auto;
            margin-right: auto;
        }

        /* Cards updated to be consistent with ChooseClaim */
        .card {
            border-radius: 1rem;
            box-shadow: 0 5px 15px rgb(0 0 0 / 0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .card:hover {
            transform: translateY(-6px);
            box-shadow: 0 12px 25px rgb(230 0 18 / 0.25);
        }

        .card-body {
            padding: 1.5rem 2rem;
        }

        .card-title {
            font-weight: 700;
            font-size: 1.3rem;
            color: #212529;
        }

        .badge {
            font-weight: 500;
            font-size: 0.85rem;
            padding: 0.4em 0.7em;
            border-radius: 50px;
        }

        .btn-outline-danger {
            font-weight: 600;
            font-size: 1rem;
            border-radius: 50px;
            padding: 0.5rem 1.5rem;
            transition: background-color 0.3s ease, color 0.3s ease;
        }

        .btn-outline-danger:hover {
            background-color: #e60012;
            color: white;
            border-color: #e60012;
        }

        .btn-outline-secondary {
            font-weight: 600;
            font-size: 1rem;
            border-radius: 50px;
            padding: 0.5rem 1.5rem;
        }

        /* Adjust spacing between buttons */
        .btn + .btn {
            margin-left: 0.75rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="claim-header">
        <h2>Saved Drafts</h2>
        <p>Manage and continue your saved draft claims.</p>
    </div>

    <!-- Tabs -->
    <ul class="nav nav-pills justify-content-center mb-5" id="claimTab" role="tablist">
        <li class="nav-item" role="presentation">
            <a class="nav-link" href="ChooseClaim.aspx" role="tab" aria-selected="false">New Claim</a>
        </li>
        <li class="nav-item" role="presentation">
            <a class="nav-link active" href="SaveAsDraft.aspx" role="tab" aria-selected="true">Continue Draft Claims</a>
        </li>
    </ul>

    <!-- Draft Cards -->
    <div class="claim-section">
        <asp:Repeater ID="rptDrafts" runat="server" OnItemCommand="rptDrafts_ItemCommand" OnItemDataBound="rptDrafts_ItemDataBound">
            <ItemTemplate>
                <div class="card mb-4 shadow-sm border-0">
                    <div class="card-body d-flex justify-content-between align-items-center flex-wrap">
                        <div>
                            <h5 class="card-title mb-1">
                                <%# Eval("PlanName") %>
                                <span class="badge bg-light text-secondary fw-normal">Draft</span>
                            </h5>
                            <p class="card-subtitle text-muted small mb-0">
                                Autosaved • <%# Eval("ModifiedDate", "{0:dd MMM} • {0:hh:mmtt}") %>
                            </p>
                        </div>
                        <div class="d-flex flex-wrap gap-2">
                            <asp:HyperLink ID="lnkContinue" runat="server"
                                CssClass="btn btn-outline-danger"
                                NavigateUrl='<%# Eval("Source").ToString() == "Claims" ? 
                                              Eval("ClaimID", "~/ContinueDraft.aspx?claimId={0}") : 
                                              Eval("ClaimID", "~/EverContinueDraft.aspx?claimId={0}") %>'>
                                Continue submission <i class="bi bi-arrow-right ms-1"></i>
                            </asp:HyperLink>
                            <asp:LinkButton ID="btnDelete" runat="server"
                                CssClass="btn btn-outline-secondary"
                                CommandName="Delete"
                                CommandArgument='<%# Eval("ClaimID") + "|" + Eval("Source") %>'>
                                <i class="bi bi-trash"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Label ID="lblNoDrafts" runat="server" Text="No saved drafts found." CssClass="text-muted fs-5 text-center mt-5" Visible="false" />
    </div>
</asp:Content>
