<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Medical.aspx.cs" Inherits="Singlife.Medical" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        /* Full width background for cards */
        .plans-bg {
            width: 100vw; /* full viewport width */
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            background: linear-gradient(to bottom right, #f9f3f6, #ffffff);
            padding-top: 2rem;
            padding-bottom: 2rem;
        }

        /* Inner container to center and constrain cards */
        .plans-container {
            max-width: 1140px;
            margin-left: auto;
            margin-right: auto;
            padding-left: 2rem; /* padding inside container */
            padding-right: 2rem;
        }

        /* Cards styling remains the same */
        .card {
            border: none;
            border-radius: 16px;
            box-shadow: 0 8px 20px rgba(0,0,0,0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            display: flex;
            flex-direction: column;
            height: 100%;
            max-height: 420px;
        }

        .card:hover {
            transform: translateY(-6px);
            box-shadow: 0 14px 30px rgba(0,0,0,0.15);
        }

        .card-body {
            padding: 1.25rem 1rem;
            display: flex;
            flex-direction: column;
            flex-grow: 1;
        }

        .card-title {
            font-weight: 700;
            font-size: 1.25rem;
            margin-bottom: 0.4rem;
            color: #3c1053;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .card-text {
            font-size: 0.95rem;
            color: #6c757d;
            margin-bottom: 1rem;
            font-weight: 500;
        }

        ul.list-unstyled li {
            font-size: 0.9rem;
            margin-bottom: 0.6rem;
            line-height: 1.3;
        }

        .btn-learn-more {
            padding: 0.5rem 1.2rem;
            font-size: 0.9rem;
            border-radius: 30px;
            box-shadow: 0 5px 10px rgba(111, 66, 193, 0.35);
            align-self: flex-start;
        }

        .badge-purple {
            background-color: #800080;
            color: white;
            font-weight: 600;
            font-size: 0.75rem;
            border-radius: 1rem;
            padding: 0.25rem 0.75rem;
            white-space: nowrap;
        }

        .brochure-link {
            color: #d80027;
            font-weight: 600;
            text-decoration: none;
        }

        .brochure-link:hover {
            text-decoration: underline;
        }

        @media (max-width: 992px) {
            .plans-container {
                padding-left: 1rem;
                padding-right: 1rem;
            }

            .card {
                max-height: none;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- Hero image -->
    <div>
        <img src="Images/Medicalind.png" alt="Medical Insurance Banner" class="img-fluid w-100" />
    </div>

    <!-- Full width background wrapper -->
    <div class="plans-bg">
        <!-- Centered container with padding -->
        <div class="plans-container">
            <!-- Your card rows here exactly as before -->

            <div class="row justify-content-center mb-4 gx-4 gy-4">
                <!-- Card 1 -->
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">SingLife OncoShield Plan 🧬</h5>
                            </div>
                            <p class="card-text text-muted">Cancer Protection & Advanced Therapy Access</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Up to S$15,000/month for chemo and targeted therapy 20× MediShield Life limits.</li>
                                <li>✔ Full coverage for proton beam, cell & gene therapies (worth S$60K–100K per course).</li>
                                <li>✔ Includes 1 free screening/year and second opinions from top oncologists worldwide.</li>
                            </ul>
                            <a href="Documents/OncoShield_Brochure.pdf" target="_blank" class="brochure-link mb-3 d-inline-block">View Brochure</a>
                            <a href="OncoShieldPlan.aspx" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>

                <!-- Card 2 -->
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">Singlife EverCare Plan 🌿</h5>
                                <span class="badge badge-purple">POPULAR</span>
                            </div>
                            <p class="card-text text-muted">Comprehensive Hospitalisation & Specialist Care</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Full reimbursement for B1/B2+ ward and ICU (up to S$1,000/day)</li>
                                <li>✔ Priority access and up to 95% bill coverage with panel specialists</li>
                                <li>✔ 100% coverage for outpatient procedures like scopes or biopsies</li>
                            </ul>
                            <a href="#" class="brochure-link mb-3 d-inline-block">↓ View brochure</a>
                            <a href="EverCarePlan.aspx" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>

                <!-- Card 3 -->
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">Singlife SecureFuture Benefits 💎</h5>
                            </div>
                            <p class="card-text text-muted">Critical Illness + Family Coverage + Retirement Bonus</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Premiums can be paid using CPF Medisave (within limits)</li>
                                <li>✔ S$500 CPF top-up for early signup (before age 35)</li>
                                <li>✔ Rebates returned to CPF Medisave after claim-free years</li>
                            </ul>
                            <a href="#" class="brochure-link mb-3 d-inline-block">↓ View brochure</a>
                            <a href="#" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Second Row -->
            <div class="row justify-content-center gx-4 gy-4">
                <div class="col-md-6 col-lg-5">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">Singlife GlobalGuard Plan 🌍</h5>
                            </div>
                            <p class="card-text text-muted">Worldwide Health Coverage for Travellers & Expats</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Up to S$1 million/year coverage (excluding elective cosmetic surgery)</li>
                                <li>✔ Covered up to S$250,000 per incident — includes air ambulance</li>
                                <li>✔ Annual reimbursement of S$300 for vaccines and travel meds</li>
                            </ul>
                            <a href="#" class="brochure-link mb-3 d-inline-block">↓ View brochure</a>
                            <a href="#" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>

                <div class="col-md-6 col-lg-5">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">Singlife MindBalance Plan 🧠</h5>
                                <span class="badge badge-purple">NEW</span>
                            </div>
                            <p class="card-text text-muted">Mental Health & Emotional Wellness</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Covers up to S$120/visit (capped at S$1,200/year) Psychiatric care</li>
                                <li>✔ Up to 6 sessions per year with certified therapists</li>
                                <li>✔ Up to S$500/year for psychiatric medications</li>
                            </ul>
                            <a href="#" class="brochure-link mb-3 d-inline-block">↓ View brochure</a>
                            <a href="#" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


