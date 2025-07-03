<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="GlobalEase.aspx.cs" Inherits="Singlife.GlobalEase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        /* ... same CSS as before ... */
        /* Hero Section */
        .hero-container {
            position: relative;
            width: 100vw;
            max-height: 500px;
            overflow: hidden;
            box-shadow: 0 3px 10px rgba(0,0,0,0.25);
            border-radius: 0 0 30px 30px;
        }

            .hero-container img {
                width: 100%;
                height: 500px;
                object-fit: cover;
                filter: brightness(0.85);
                display: block;
                transform: scale(1.05);
                opacity: 0;
                transition: transform 1.5s ease, opacity 1.5s ease;
            }

                .hero-container img.loaded {
                    transform: scale(1);
                    opacity: 1;
                }

            .hero-container:hover img {
                transform: scale(1.03);
                filter: brightness(1);
                transition: transform 0.6s ease, filter 0.6s ease;
            }

        .hero-buttons {
            position: absolute;
            bottom: 150px;
            left: 20%;
            transform: translateX(-50%);
            display: flex;
            gap: 14px;
            flex-wrap: wrap;
            z-index: 10;
            justify-content: flex-start;
            padding: 0 10px;
            max-width: 90vw;
        }

            .hero-buttons a {
                background: linear-gradient(135deg, #d80027 0%, #a6001e 100%);
                color: white;
                padding: 10px 22px;
                font-weight: 700;
                font-size: 0.9rem;
                border-radius: 25px;
                box-shadow: 0 3px 10px rgba(216,0,39,0.5);
                text-transform: uppercase;
                letter-spacing: 0.8px;
                transition: background 0.4s ease, box-shadow 0.4s ease;
            }

                .hero-buttons a:hover {
                    background: linear-gradient(135deg, #a6001e 0%, #d80027 100%);
                    box-shadow: 0 5px 15px rgba(166,0,30,0.7);
                    text-decoration: none;
                }

        /* Intro Section */
        /* Intro Section with Table Layout */
        .intro-table {
            width: 100%;
            max-width: 950px;
            margin: 50px auto;
            background-color: #fff4f6;
            border-radius: 16px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.08);
        }

        .intro-table td {
            padding: 20px;
            vertical-align: middle;
        }

        .intro-img-cell {
            width: 25%;
            max-width: 250px;
        }

        .intro-img-cell img {
            width: 100%;
            height: auto;
            max-height: 280px;
            object-fit: contain;
            border-radius: 12px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
        }

        .intro-text h1 {
            font-size: 2.2rem;
            color: #d80027;
            font-weight: 800;
            margin-bottom: 14px;
        }

        .intro-text p {
            font-size: 1rem;
            color: #444;
            line-height: 1.5;
        }


        .review-section {
            background-color: #fff4f6;
            padding: 40px 15px;
            max-width: 900px;
            margin: 60px auto;
            border-radius: 20px;
            box-shadow: 0 6px 20px rgba(216,0,39,0.12);
        }

        .review-title {
            font-size: 1.8rem;
            font-weight: 800;
            color: #d80027;
            margin-bottom: 30px;
            text-align: center;
        }

        .review-card {
            background: #fff;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            margin-bottom: 20px;
        }

        .review-stars i {
            color: #f1c40f;
        }

        .review-form {
            margin-top: 30px;
        }

        .filter-bar {
            margin-bottom: 20px;
            display: flex;
            justify-content: end;
        }

        /* Features Section */
        .features-wrapper {
            background-color: #fff4f6;
            padding: 40px 15px 60px;
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 20px;
            max-width: 900px;
            margin: 0 auto 60px;
            border-radius: 20px;
            box-shadow: 0 6px 25px rgba(216,0,39,0.1);
        }

        .feature-card {
            border-radius: 14px;
            padding: 20px 16px;
            text-align: center;
            width: 220px;
            min-height: 210px;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            display: flex;
            flex-direction: column;
            justify-content: flex-start;
            align-items: center;
            color: #444;
            box-shadow: 0 6px 15px rgba(216,0,39,0.12);
            font-weight: 600;
        }

            .feature-card:hover {
                transform: translateY(-6px);
                box-shadow: 0 12px 28px rgba(216,0,39,0.25);
            }

        .feature-emoji {
            font-size: 36px;
            margin-bottom: 10px;
        }

        .feature-title {
            font-size: 1rem;
            font-weight: 700;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 0.03em;
        }

        .feature-desc {
            font-size: 0.85rem;
            line-height: 1.4;
        }

        .feature-card:nth-child(1) {
            background: #ffe1e6;
            color: #b32e3a;
        }

        .feature-card:nth-child(2) {
            background: #ffe9d6;
            color: #b36a2e;
        }

        .feature-card:nth-child(3) {
            background: #d6f0ff;
            color: #2e5ab3;
        }

        .feature-card:nth-child(4) {
            background: #d6ffe9;
            color: #2eb37e;
        }

        .feature-card:nth-child(5) {
            background: #fff6d6;
            color: #b3a72e;
        }

        h2.section-title {
            text-align: center;
            font-size: 2rem;
            color: #d80027;
            margin-bottom: 40px;
            font-weight: 800;
        }

        .scroll-arrow {
            width: 70px;
            animation: bounce 2s infinite;
            cursor: pointer;
        }

        .how-to-use-img {
            max-width: 60%;
            max-height: 400px;
            margin: 0 auto;
            display: block;
        }



        @keyframes bounce {
            0%, 20%, 50%, 80%, 100% {
                transform: translateY(0);
            }

            40% {
                transform: translateY(-15px);
            }

            60% {
                transform: translateY(-8px);
            }
        }

        @media (max-width: 767.98px) {
            .hero-buttons {
                bottom: 40px;
                left: 50%;
                transform: translateX(-50%);
                flex-direction: column;
                align-items: center;
                padding: 0 15px;
            }

             .intro-text-cell {
                padding: 20px 20px;
                text-align: center;
            }

            .feature-card {
                width: 100%;
            }

            .how-to-use-img {
                max-width: 80%;
                max-height: 250px;
            }
        }
        .auto-style1 {
            width: 86%;
            height: 774px;
            margin-bottom: 0px;
        }
    </style>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const heroImage = document.querySelector('.hero-container img');
            setTimeout(() => heroImage.classList.add('loaded'), 100);
            const arrow = document.getElementById('scrollDownArrow');
            if (arrow) {
                arrow.addEventListener('click', function (event) {
                    event.preventDefault();
                    document.querySelector('#nextSection').scrollIntoView({ behavior: 'smooth' });
                });
            }
        });
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- Hero Banner -->
    <div class="hero-container shadow-sm rounded-bottom">
        <img src="Images/Globalease.PNG" alt="GlobeEase Banner" />
        <div class="hero-buttons">
            <a href="GlobalEaseQuote.aspx" class="btn btn-danger btn-lg shadow">Get a Quote</a>
            <a href="Documents/OncoShield_Brochure.pdf" target="_blank" class="btn btn-outline-danger btn-lg shadow">View Brochure</a>
        </div>
    </div>

    <!-- Intro Section with Table -->
    <table class="intro-table">
        <tr>
            <td class="intro-img-cell">
                <img src="Images/Screenshot%202025-06-18%20021919.jpg" alt="GlobeEase Plan Illustration" class="auto-style1" />
            </td>
            <td class="intro-text-cell">
                <h1> GlobeEase Retirement-Ready Travel Plan</h1>
                <p>
                    A travel insurance plan for people who love to explore, now and during retirement. 
                    It protects your health and belongings while you travel and supports long trips or remote work overseas.
                </p>
            </td>
        </tr>
    </table>

    <!-- Features Section -->
    <h2 class="section-title">Key Features and Benefits</h2>
    <section class="features-wrapper">
        <div class="feature-card">
            <div class="feature-emoji">✈</div>
            <div class="feature-title">Travel Worry-Free</div>
            <div class="feature-desc">Comprehensive medical and emergency coverage when you travel — even up to 90 days.</div>
        </div>
        <div class="feature-card">
            <div class="feature-emoji">💆</div>
            <div class="feature-title">Wellness On the Go</div>
            <div class="feature-desc">Use up to S$300/year for massages, yoga retreats, or preventive travel health services.</div>
        </div>
        <div class="feature-card">
            <div class="feature-emoji">📱</div>
            <div class="feature-title">Instant Help Anywhere</div>
            <div class="feature-desc">24/7 concierge services from lost passports to emergency medical care while overseas.</div>
        </div>
        <div class="feature-card">
            <div class="feature-emoji">🧳</div>
            <div class="feature-title">Protect Your Gear</div>
            <div class="feature-desc">Covers personal items like baggage, electronics, and more — so you travel light and stress-free.</div>
        </div>
        <div class="feature-card">
            <div class="feature-emoji">🌍</div>
            <div class="feature-title">Retirement-Ready Flexibility</div>
            <div class="feature-desc">Perfect for digital nomads or early retirees exploring long-stay travel or relocation plans.</div>
        </div>
    </section>

    <!-- Scroll Arrow -->
    <div class="text-center my-4">
        <a href="#nextSection" id="scrollDownArrow">
            <img src="Images/pngtree-down-arrow-red-png-image_4376823-removebg-preview.png" alt="Scroll Down" class="scroll-arrow" />
        </a>
    </div>

    <!-- How to Use Section -->
    <section id="nextSection" class="text-center">
        <h2 class="section-title mb-3">How to Use the GlobeEase Travel Plan</h2>
        <img src="Images/GAIr.jpg" alt="How to Use GlobeEase Plan" class="how-to-use-img rounded shadow-sm" />
    </section>

    <!-- Review Section -->
    <section class="review-section my-5 px-3">
        <h2 class="section-title text-center mb-4">Customer Reviews</h2>

        <!-- Filter Dropdown -->
        <div class="d-flex justify-content-center mb-4">
            <asp:DropDownList ID="ddlFilterStars" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="ddlFilterStars_SelectedIndexChanged"
                CssClass="form-select w-auto shadow-sm">
                <asp:ListItem Text="All Ratings" Value="all" />
                <asp:ListItem Text="5 Stars" Value="5" />
                <asp:ListItem Text="4 Stars & Up" Value="4" />
                <asp:ListItem Text="3 Stars & Up" Value="3" />
            </asp:DropDownList>
        </div>

        <!-- Reviews List -->
        <asp:Repeater ID="ReviewsRepeater" runat="server">
            <ItemTemplate>
                <div class="card mb-3 mx-auto shadow-sm" style="max-width: 700px;">
                    <div class="card-body">
                        <div class="d-flex justify-content-between mb-2">
                            <h6 class="mb-0 text-dark fw-bold"><%# Eval("Name") %></h6>
                            <div class="review-stars text-warning">
                               <%# new System.Web.HtmlString(((Singlife.GlobalEase)Page).GetStarHtml(Convert.ToInt32(Eval("Rating")))) %>
                            </div>
                        </div>
                        <p class="card-text mt-2" style="font-size: 1rem; color: #333;">
                            <i class="fas fa-quote-left me-2 text-danger"></i>
                            <%# Eval("ReviewText") %>
                        </p>
                        <div class="text-end text-muted small mt-1">
                            <%# Convert.ToDateTime(Eval("ReviewDate")).ToString("dd MMM yyyy") %>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Message Label -->
        <asp:Label ID="lblMessage" runat="server" CssClass="text-success d-block text-center my-3 fw-bold" EnableViewState="false" />

        <!-- Submit Form -->
        <asp:Panel ID="reviewForm" runat="server" CssClass="card mx-auto mt-5 shadow-sm p-4" style="max-width: 600px;">
            <h5 class="mb-4 text-danger">Leave a Review</h5>

            <div class="mb-3">
                <label for="ddlRating" class="form-label">Your Rating</label>
                <asp:DropDownList ID="ddlRating" runat="server" CssClass="form-select">
                    <asp:ListItem Text="5 - Excellent" Value="5" />
                    <asp:ListItem Text="4 - Good" Value="4" />
                    <asp:ListItem Text="3 - Average" Value="3" />
                    <asp:ListItem Text="2 - Poor" Value="2" />
                    <asp:ListItem Text="1 - Terrible" Value="1" />
                </asp:DropDownList>
            </div>

            <div class="mb-3">
                <label for="txtReview" class="form-label">Your Review</label>
                <asp:TextBox ID="txtReview" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control" />
            </div>

            <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review"
                CssClass="btn btn-danger w-100" OnClick="btnSubmitReview_Click" />
        </asp:Panel>
    </section>

    <!-- FontAwesome for star icons -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
</asp:Content>