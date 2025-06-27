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
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            text-align: center;
        }

            .hero-container img {
                position: absolute;
                top: 0;
                left: 0;
                width: 100%;
                height: 500px;
                object-fit: cover;
                filter: brightness(0.45);
                transition: transform 1.5s ease, opacity 1.5s ease;
                z-index: 0;
                opacity: 0;
                transform: scale(1.05);
            }

                .hero-container img.loaded {
                    opacity: 1;
                    transform: scale(1);
                }

        .hero-text {
            position: relative;
            z-index: 1;
            max-width: 600px;
            padding: 20px;
            background: rgba(0, 0, 0, 0.35);
            border-radius: 15px;
            box-shadow: 0 0 15px rgba(0,0,0,0.5);
        }

        .payment-section {
            padding: 60px 20px 40px;
            background: #fff;
        }

        .card-method {
            border: none;
            border-radius: 16px;
            box-shadow: 0 6px 16px rgba(0,0,0,0.05);
            transition: 0.3s;
            background-color: #fefefe;
        }

            .card-method:hover {
                transform: translateY(-5px);
                box-shadow: 0 8px 20px rgba(0,0,0,0.1);
            }

            .card-method i {
                font-size: 2.5rem;
                color: #dc3545;
                margin-bottom: 15px;
            }

        #payNowQR img {
            max-width: 250px;
            display: block;
            margin: 0 auto 15px;
        }

        .terms-section {
            margin-bottom: 80px;
        }
    </style>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            setTimeout(() => {
                const img = document.querySelector('.hero-container img');
                if (img) img.classList.add('loaded');
            }, 100);
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <!-- Hero Section -->
    <div class="hero-container shadow-sm rounded-bottom">
        <img src="Images/Paymentbackground.png" alt="Paying for your policy" class="loaded" />
        <div class="hero-text">
            <h1>Pay for Your Insurance Plan</h1>
            <asp:Label ID="lblPlanName" runat="server" CssClass="h3 fw-bold"></asp:Label><br />
            <asp:Label ID="lblAmountDue" runat="server" CssClass="h4 text-warning"></asp:Label><br />
            <asp:Label ID="lblNextBillingDate" runat="server" CssClass="h5 text-info"></asp:Label>
        </div>
    </div>

    <!-- Show if GIRO not active -->
    <asp:PlaceHolder ID="phPaymentMethods" runat="server" Visible="true">
        <section class="container payment-section text-center">
            <h2 class="text-danger fw-bold mb-4">Choose Your Payment Method</h2>
            <p class="mb-5 text-muted">Select one of the options below to proceed with your payment.</p>
            <div class="row g-4 justify-content-center">

                <!-- PayNow Card -->
                <div class="col-md-4">
                    <div class="card card-method p-4">
                        <i class="fas fa-mobile-alt"></i>
                        <h5 class="fw-bold">PayNow</h5>
                        <p>Quick and easy payment via PayNow QR.</p>
                        <button type="button" class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#payNowQR">Show QR</button>
                    </div>
                </div>

                <!-- Credit/Debit Card -->
                <div class="col-md-4">
                    <div class="card card-method p-4">
                        <i class="fas fa-credit-card"></i>
                        <h5 class="fw-bold">Credit/Debit Card</h5>
                        <p>Secure one-time payment via card.</p>
                        <button type="button" class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#cardModal">Pay Now</button>
                    </div>
                </div>

                <!-- GIRO Payment Card -->
                <div class="col-md-4">
                    <div class="card card-method p-4">
                        <i class="fas fa-file-invoice-dollar"></i>
                        <h5 class="fw-bold">GIRO Payment</h5>
                        <p>Setup automatic payments via GIRO.</p>
                        <button type="button" class="btn btn-outline-primary btn-sm" data-bs-toggle="modal" data-bs-target="#giroUploadModal">Setup GIRO</button>
                    </div>
                </div>

            </div>
        </section>
    </asp:PlaceHolder>

    <!-- Show if GIRO is active -->
    <asp:PlaceHolder ID="phGiroActive" runat="server" Visible="false">
        <section class="container payment-section text-center">
            <h4 class="text-success fw-bold mb-4">GIRO Payment Active</h4>
            <p>Your payments will be automatically deducted monthly/annually from your bank account.</p>
            <p>No further action is needed on your part at this time.</p>
        </section>
    </asp:PlaceHolder>

    <!-- Modals -->

    <!-- PayNow QR Modal -->
    <div class="modal fade" id="payNowQR" tabindex="-1" aria-labelledby="payNowQRLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content text-center p-4">
                <h5 class="modal-title mb-3" id="payNowQRLabel">PayNow QR Code</h5>
                <img src="Images/UM_bank_OCBC_paynow_qr-code_480x480.jpg" alt="PayNow QR Code" />
                <p>Scan this QR code with your banking app to pay.</p>
                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>

    <!-- Card Payment Modal -->
    <div class="modal fade" id="cardModal" tabindex="-1" aria-labelledby="cardModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="cardModalLabel">Card Payment</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblCardMessage" runat="server" CssClass="text-danger"></asp:Label>
                    <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control mb-2" Placeholder="Card Number" MaxLength="16" />
                    <div class="row g-2 mb-2">
                        <div class="col">
                            <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control" Placeholder="MM/YY" MaxLength="5" />
                        </div>
                        <div class="col">
                            <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" Placeholder="CVV" MaxLength="4" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmitCard" runat="server" CssClass="btn btn-danger" Text="Pay" OnClick="btnSubmitCard_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <!-- GIRO Upload Modal -->
    <div class="modal fade" id="giroUploadModal" tabindex="-1" aria-labelledby="giroUploadModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="giroUploadModalLabel">Upload GIRO Authorization Form (PDF)</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblGiroUploadMessage" runat="server" CssClass="text-danger"></asp:Label>
                    <asp:FileUpload ID="fuGiroForm" runat="server" CssClass="form-control" Accept=".pdf" />
                    <small class="form-text text-muted mt-2">Please upload a scanned GIRO authorization PDF.</small>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnUploadGiro" runat="server" CssClass="btn btn-primary" Text="Submit GIRO" OnClick="btnUploadGiro_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <!-- FAQ -->
    <section class="container my-5">
        <h3 class="text-danger fw-bold mb-4">Frequently Asked Questions</h3>
        <div class="accordion" id="faqAccordion">
            <div class="accordion-item">
                <h2 class="accordion-header" id="headingOne">
                    <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">
                        How do I know my payment was successful?
                    </button>
                </h2>
                <div id="collapseOne" class="accordion-collapse collapse show" aria-labelledby="headingOne" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">You'll receive a confirmation email and can check the payment status in your customer portal.</div>
                </div>
            </div>
            <div class="accordion-item">
                <h2 class="accordion-header" id="headingTwo">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                        Can I change my payment method later?
                    </button>
                </h2>
                <div id="collapseTwo" class="accordion-collapse collapse" aria-labelledby="headingTwo" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">Yes. You can update your payment method anytime in the policy servicing section.</div>
                </div>
            </div>
        </div>
    </section>

    <!-- Terms -->
    <section class="container terms-section">
        <h5>Terms & Conditions</h5>
        <ul>
            <li>All payments are subject to verification and processing timelines.</li>
            <li>GIRO deductions may take up to 14 business days to activate.</li>
            <li>For any disputes or refund queries, contact our customer care team.</li>
        </ul>
    </section>

</asp:Content>

