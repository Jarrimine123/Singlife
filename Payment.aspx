<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="Singlife.Payment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <style>
        .hero-container {
            position: relative;
            width: 100vw;
            max-height: 500px;
            overflow: hidden;
            box-shadow: 0 3px 10px rgba(0, 0, 0, 0.25);
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

        .payment-section {
            padding: 50px 20px;
        }

        .card-method {
            border: none;
            border-radius: 12px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.05);
            transition: 0.3s;
        }

        .card-method:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 12px rgba(0,0,0,0.1);
        }

        .faq-item {
            border-bottom: 1px solid #ddd;
        }

        .faq-header {
            font-weight: bold;
            cursor: pointer;
        }

        .terms-section {
            background: #f9f9f9;
            padding: 40px 20px;
            border-radius: 12px;
        }
    </style>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            setTimeout(() => {
                const img = document.querySelector('.hero-container img');
                img?.classList.add('loaded');
            }, 100);
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <!-- HERO SECTION -->
    <div class="hero-container shadow-sm rounded-bottom">
        <img src="Images/Paymentbackground.png" alt="Paying for your policy" class="loaded" />
    </div>

    <!-- PAYMENT OPTIONS -->
    <section class="container payment-section text-center">
        <h2 class="text-danger fw-bold mb-4">Paying for Your Policy</h2>
        <div class="row g-4">
            <div class="col-md-4">
                <div class="card card-method p-4">
                    <i class="fas fa-mobile-alt fa-2x text-danger mb-3"></i>
                    <h5 class="fw-bold">PayNow</h5>
                    <p>Quick and easy payment via PayNow QR.</p>
                    <button class="btn btn-outline-danger btn-sm">Show QR</button>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card card-method p-4">
                    <i class="fas fa-university fa-2x text-danger mb-3"></i>
                    <h5 class="fw-bold">GIRO</h5>
                    <p>Set up auto deductions from your bank account.</p>
                    <button class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#giroModal">Setup GIRO</button>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card card-method p-4">
                    <i class="fas fa-credit-card fa-2x text-danger mb-3"></i>
                    <h5 class="fw-bold">Credit/Debit Card</h5>
                    <p>Secure one-time payment via card.</p>
                    <button class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#cardModal">Pay Now</button>
                </div>
            </div>
        </div>
    </section>

    <!-- MODALS -->
    <div class="modal fade" id="giroModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">GIRO Setup</h5></div>
                <div class="modal-body">
                    <p>Upload your GIRO form or submit your bank details to begin processing.</p>
                    <input type="file" class="form-control" />
                    <input type="text" class="form-control mt-2" placeholder="Bank Account Number" />
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button class="btn btn-danger">Submit</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="cardModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Card Payment</h5></div>
                <div class="modal-body">
                    <input type="text" class="form-control mb-2" placeholder="Card Number" />
                    <div class="row g-2">
                        <div class="col"><input type="text" class="form-control" placeholder="MM/YY" /></div>
                        <div class="col"><input type="text" class="form-control" placeholder="CVV" /></div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <button class="btn btn-danger">Pay</button>
                </div>
            </div>
        </div>
    </div>

    <!-- FAQ SECTION -->
    <section class="container my-5">
        <h3 class="text-danger fw-bold mb-3">Frequently Asked Questions</h3>
        <div class="accordion" id="faqAccordion">
            <div class="accordion-item">
                <h2 class="accordion-header" id="headingOne">
                    <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne">
                        How do I know my payment was successful?
                    </button>
                </h2>
                <div id="collapseOne" class="accordion-collapse collapse show" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">You'll receive a confirmation email and can check the payment status in your customer portal.</div>
                </div>
            </div>
            <div class="accordion-item">
                <h2 class="accordion-header" id="headingTwo">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseTwo">
                        Can I change my payment method later?
                    </button>
                </h2>
                <div id="collapseTwo" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">Yes. You can update your payment method anytime in the policy servicing section.</div>
                </div>
            </div>
        </div>
    </section>

    <!-- TERMS & CONDITIONS -->
    <section class="container terms-section">
        <h5 class="text-dark fw-bold mb-3">Terms & Conditions</h5>
        <ul>
            <li>All payments are subject to verification and processing timelines.</li>
            <li>GIRO deductions may take up to 14 business days to activate.</li>
            <li>For any disputes or refund queries, contact our customer care team.</li>
        </ul>
    </section>

</asp:Content>
