<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="LifeInsurance.aspx.cs" Inherits="Singlife.LifeInsurance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .plans-bg {
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
            background: linear-gradient(to bottom right, #f9f3f6, #ffffff);
            padding-top: 2rem;
            padding-bottom: 2rem;
        }

        .plans-container {
            max-width: 1140px;
            margin-left: auto;
            margin-right: auto;
            padding-left: 2rem;
            padding-right: 2rem;
        }

        .card {
            border: none;
            border-radius: 16px;
            box-shadow: 0 8px 20px rgba(0,0,0,0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            display: flex;
            flex-direction: column;
            height: 100%;
            max-height: 500px;
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
            font-size: 0.85rem; /* increased from 0.75rem */
            border-radius: 1.25rem; /* slightly more rounded */
            padding: 0.35rem 1rem; /* more padding = bigger badge */
            white-space: nowrap;
            /* Position it slightly higher */
            position: relative;
            top: -45px; /* adjust up position */
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
    <div>
        <img src="Images/Screenshot%202025-06-27%20204318.png" alt="Life Insurance Banner" class="img-fluid w-100" />
    </div>

    <div class="plans-bg">
        <div class="plans-container">
            <div class="row justify-content-center mb-4 gx-4 gy-4">
                <!-- LifeGrow Retirement Plus -->
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">LifeGrow Retirement Plus 🌱</h5>
                                <span class="badge badge-purple">POPULAR</span>
                            </div>
                            <p class="card-text text-muted">Grow protection and wealth together with life-long coverage and bonuses.</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Life-long peace of mind – Protection up to age 100 with S$300,000 coverage from just S$2.10/day.</li>
                                <li>✔ Build wealth as you protect – Projected returns of 4–6%, growing to S$180,000 over 20 years.</li>
                                <li>✔ Get rewarded for reaching retirement – Receive S$5,000 every 5 years from age 55.</li>
                            </ul>
                            <a href="#" class="brochure-link mb-3 d-inline-block">↓ View brochure</a>
                            <a href="#" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>

                <!-- FlexiCare Life Plan -->
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">FlexiCare Life Plan 🛡 </h5>
                            </div>
                            <p class="card-text text-muted">Start smart and stay covered with flexibility and affordability.</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ Easy on your wallet – Premiums start from S$0.37/day for S$500,000 coverage.</li>
                                <li>✔ Grow with your life – Convert to lifelong coverage between age 50–55, no checks needed.</li>
                                <li>✔ Care if you lose independence – Receive S$2,000/month for daily living assistance.</li>
                            </ul>
                            <a href="#" class="brochure-link mb-3 d-inline-block">↓ View brochure</a>
                            <a href="#" class="btn btn-danger rounded-pill fw-bold btn-learn-more">Learn more</a>
                        </div>
                    </div>
                </div>

                <!-- IncomeShield Legacy Plan -->
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm h-100 rounded-4">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <h5 class="card-title mb-0">IncomeShield Legacy Plan 💼 </h5>
                            </div>
                            <p class="card-text text-muted">Retire well and leave more behind with a legacy-focused plan.</p>
                            <ul class="list-unstyled flex-grow-1">
                                <li>✔ For a legacy that grows with you – S$200,000 coverage grows to S$270,000 by your 70s.</li>
                                <li>✔ Income you can count on – S$500/month for 20 years from age 60 (S$120,000 total).</li>
                                <li>✔ Pass on your protection – Gift a policy to your child to secure their future.</li>
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
