<%@ Page Title="" Language="C#" MasterPageFile="~/Customer.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="Singlife.Payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f9fafb;
            margin: 0;
            padding: 0;
        }

        .hero-container {
            background: linear-gradient(135deg, #e0e7ff, #a5b4fc);
            height: auto;
            border-radius: 0 0 32px 32px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            box-shadow: 0 10px 30px rgba(99, 102, 241, 0.2);
            padding: 2rem;
        }

            .hero-container img {
                max-height: 180px;
                object-fit: contain;
                margin-bottom: 1rem;
            }

            .hero-container h1 {
                font-weight: 900;
                font-size: 3rem;
                color: #3730a3;
                margin-bottom: 0.2rem;
                letter-spacing: -0.02em;
            }

        .hero-text .h3 {
            color: #4338ca;
            font-weight: 700;
            font-size: 1.5rem;
        }

        .hero-text .h4 {
            color: #6b7280;
            font-weight: 500;
            font-size: 1.2rem;
        }

        .payment-section {
            max-width: 960px;
            margin: 3rem auto 5rem;
            padding: 0 1rem;
            text-align: center;
        }

        .payment-methods {
            display: flex;
            justify-content: center;
            gap: 2rem;
            flex-wrap: wrap;
        }

        .card-method {
            background: white;
            flex: 1 1 280px;
            border-radius: 24px;
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
            padding: 2rem 1.5rem;
            transition: all 0.3s ease;
        }

            .card-method:hover {
                box-shadow: 0 14px 48px rgba(67, 56, 202, 0.25);
                transform: translateY(-6px);
            }

            .card-method i {
                font-size: 2.8rem;
                color: #4338ca;
                margin-bottom: 1rem;
            }

            .card-method h5 {
                font-weight: 700;
                font-size: 1.3rem;
                color: #3730a3;
                margin-bottom: 0.5rem;
            }

            .card-method p {
                font-size: 1rem;
                color: #6b7280;
                margin-bottom: 1.5rem;
            }

            .card-method button {
                border: 2px solid #4338ca;
                color: #4338ca;
                padding: 0.5rem 1.5rem;
                border-radius: 30px;
                font-weight: 600;
                background-color: transparent;
                transition: all 0.3s ease;
            }

                .card-method button:hover {
                    background-color: #4338ca;
                    color: white;
                }

        .accordion-button {
            font-weight: 600;
            font-size: 1.1rem;
            background-color: #f3f4f6;
        }

            .accordion-button:not(.collapsed) {
                background-color: #ede9fe;
                color: #3730a3;
            }

        .accordion-body {
            font-size: 1rem;
            color: #4b5563;
        }

        .terms-section ul {
            list-style: disc;
            padding-left: 1.2rem;
        }

        .btn-outline-primary,
        .btn-outline-danger {
            border-radius: 30px;
            font-weight: 600;
        }

        .modal-content {
            border-radius: 24px;
            box-shadow: 0 20px 60px rgba(67, 56, 202, 0.15);
        }

        @media (max-width: 768px) {
            .hero-container {
                padding: 2rem 1rem;
            }

                .hero-container h1 {
                    font-size: 2.2rem;
                }

            .hero-text .h3 {
                font-size: 1.25rem;
            }

            .hero-text .h4 {
                font-size: 1rem;
            }

            .card-method {
                padding: 1.5rem 1rem;
            }

            .payment-section h2 {
                font-size: 1.6rem;
            }
        }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

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
    <div class="hero-container shadow-sm rounded-bottom">
        <img src="Images/Paymentbackground.png" alt="Paying for your policy" class="loaded" />
        <div class="hero-text">
            <h1>Pay for Your Insurance Plan</h1>
            <asp:Label ID="lblPlanName" runat="server" CssClass="h3 fw-bold d-block mb-1" />
            <asp:Label ID="lblAmountDue" runat="server" CssClass="h4 text-warning d-block mb-2" />
        </div>
    </div>

    <asp:PlaceHolder ID="phPaymentMethods" runat="server" Visible="true">
        <section class="container payment-section text-center">
            <h2 class="text-danger fw-bold mb-4">Choose Your Payment Method</h2>
            <p class="mb-5 text-muted">Select one of the options below to proceed with your payment.</p>
            <div class="row g-4 justify-content-center">
                <div class="col-md-4">
                    <div class="card card-method p-4">
                        <i class="fas fa-mobile-alt"></i>
                        <h5 class="fw-bold">PayNow</h5>
                        <p>Quick and easy payment via PayNow QR.</p>
                        <button type="button" class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#payNowQR">Learn More</button>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card card-method p-4">
                        <i class="fas fa-credit-card"></i>
                        <h5 class="fw-bold">Credit/Debit Card</h5>
                        <p>Secure one-time payment via card.</p>
                        <button type="button" class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#cardModalStatic">Learn More</button>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card card-method p-4">
                        <i class="fas fa-file-invoice-dollar"></i>
                        <h5 class="fw-bold">GIRO Payment</h5>
                        <p>Setup automatic payments via GIRO.</p>
                        <button type="button" class="btn btn-outline-primary btn-sm" data-bs-toggle="modal" data-bs-target="#giroUploadModalStatic">Learn More</button>
                    </div>
                </div>
            </div>
        </section>
    </asp:PlaceHolder>

    <!-- Static Modals (General Info about payment methods) -->
    <!-- PayNow Modal -->
    <div class="modal fade" id="payNowQR" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content text-center p-4">
                <h5 class="modal-title">PayNow Payment</h5>
                <p>PayNow lets you quickly pay via scanning the QR code using your banking app.</p>
                <p>Simply scan the QR code at payment time to complete the transaction instantly.</p>
                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>

    <!-- Card Payment Static Modal -->
    <div class="modal fade" id="cardModalStatic" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content p-4">
                <div class="modal-header">
                    <h5 class="modal-title">Credit/Debit Card Payment</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>You can make a secure one-time payment using your credit or debit card.</p>
                    <p>We accept Visa, MasterCard, and American Express cards.</p>
                    <p>All transactions are encrypted and processed securely.</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <!-- GIRO Static Modal -->
    <div class="modal fade" id="giroUploadModalStatic" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content p-4">
                <div class="modal-header">
                    <h5 class="modal-title">GIRO Payment</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>GIRO allows you to set up automatic monthly or annual payments directly from your bank account.</p>
                    <p>Once authorized, payments will be deducted automatically without any further action needed from you.</p>
                    <p>Please upload a GIRO authorization form to get started (this can be done from your account page).</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Payment Plans Repeater -->
    <!-- Payment Plans Repeater -->
    <asp:Repeater ID="rptPlans" runat="server" OnItemCommand="rptPlans_ItemCommand" OnItemDataBound="rptPlans_ItemDataBound">
        <ItemTemplate>
            <div class="card mb-3">
                <div class="card-body">
                    <h5 class="card-title"><%# Eval("PlanName") %></h5>
                    <p class="card-text">
                        <strong>Payment Frequency:</strong> <%# Eval("PaymentFrequency") %><br />
                        <strong>Next Billing Date:</strong> <%# Eval("NextBillingDate", "{0:yyyy-MM-dd}") %><br />
                        <strong>Amount Due:</strong> $<%# Eval("AmountDue") %><br />
                        <strong>Total Payments Made:</strong> <%# Eval("TotalPaymentCount") %><br />
                    </p>

                    <div class="d-flex flex-wrap gap-2">
                        <asp:Button ID="btnPayNow" runat="server" Text="PayNow" CssClass="btn btn-warning"
                            CommandName="ShowPayNowModal" CommandArgument='<%# Eval("PurchaseID") %>' />

                        <asp:Button ID="btnCard" runat="server" Text="Pay with Card" CssClass="btn btn-primary"
                            CommandName="ShowCardModal" CommandArgument='<%# Eval("PurchaseID") %>' />

                        <asp:Button ID="btnGiroUpload" runat="server" Text="Upload GIRO Form" CssClass="btn btn-success"
                            CommandName="ShowGiroModal" CommandArgument='<%# Eval("PurchaseID") %>' />

                        <%-- Show Simulate GIRO Deduction only if GIRO is active --%>
                        <asp:PlaceHolder ID="phSimulateGiro" runat="server"
                            Visible='<%# Eval("PaymentMethod").ToString() == "GIRO" && Eval("GiroStatus").ToString().ToLower() == "active" %>'>
                            <asp:Button ID="btnDeductGiro" runat="server"
                                CommandName="ProcessGiroPayment"
                                CommandArgument='<%# Eval("PurchaseID") %>'
                                Text="Simulate GIRO Deduction"
                                CssClass="btn btn-success" />
                        </asp:PlaceHolder>
                    </div>

                    <%-- GIRO Status Info & Cancel Button --%>
                    <asp:PlaceHolder ID="phGiroStatus" runat="server"
                        Visible='<%# Eval("PaymentMethod").ToString().ToLower() == "giro" 
                          && (Eval("GiroStatus").ToString().ToLower() == "active" || Eval("GiroStatus").ToString().ToLower() == "pending") %>'>
                        <div class="alert alert-info mt-3">
                            <asp:Label ID="lblGiroStatusText" runat="server"
                                Text='<%# Eval("GiroStatus").ToString().ToUpper() == "ACTIVE" 
                                   ? "GIRO Payment Active - Auto deduction enabled" 
                                   : "GIRO Pending - Awaiting approval" %>' />

                            <asp:Button ID="btnCancelGiro" runat="server" Text="Cancel GIRO"
                                CssClass="btn btn-outline-danger btn-sm ms-2"
                                CommandName="CancelGiro"
                                CommandArgument='<%# Eval("PurchaseID") %>' />

                            <asp:Label ID="lblGiroCancelMessage" runat="server" CssClass="ms-2 text-danger" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>

            <!-- PayNow Modal -->
            <div class="modal fade" id='payNowModal_<%# Eval("PurchaseID") %>' tabindex="-1" aria-labelledby="payNowModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content p-4">
                        <div class="modal-header">
                            <h5 class="modal-title">PayNow Confirmation</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>

                        <div class="modal-body">
                            <div class="text-center mb-3">
                                <img src="Images/UM_bank_OCBC_paynow_qr-code_480x480.jpg" alt="PayNow QR Code" class="img-fluid" style="max-width: 200px;" />
                                <p class="mt-2">Scan this QR code with your banking app to pay.</p>
                            </div>

                            <p>Please fill in the details below after completing your PayNow transfer:</p>

                            <asp:TextBox ID="txtPayNowRef" runat="server" CssClass="form-control mb-3"
                                placeholder="Enter PayNow Reference Number" />

                            <asp:FileUpload ID="fuPayNowReceipt" runat="server" CssClass="form-control mb-3" />

                            <asp:Label ID="lblPayNowMessage" runat="server" CssClass="text-danger" />
                        </div>

                        <div class="modal-footer">
                            <asp:Button ID="btnConfirmPayNow" runat="server" Text="Confirm PayNow Payment"
                                CssClass="btn btn-success"
                                CommandArgument='<%# Eval("PurchaseID") %>'
                                OnClick="btnSubmitPayNow_Click" />
                        </div>
                    </div>
                </div>
            </div>


            <!-- Card Modal -->
            <div class="modal fade" id='cardModal_<%# Eval("PurchaseID") %>' tabindex="-1" aria-labelledby="cardModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Card Payment</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:TextBox ID="txtCardholderName" runat="server" CssClass="form-control mb-2" placeholder="Cardholder Name" />
                            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control mb-2" placeholder="Card Number" MaxLength="16" />
                            <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control mb-2" placeholder="MM/YY" />
                            <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control mb-2" placeholder="CVV" MaxLength="4" />
                            <asp:Label ID="lblCardMessage" runat="server" CssClass="text-danger" />
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSubmitCard" runat="server" Text="Submit Payment" CssClass="btn btn-primary"
                                CommandArgument='<%# Eval("PurchaseID") %>' OnClick="btnSubmitCard_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- GIRO Modal -->
            <div class="modal fade" id='giroModal_<%# Eval("PurchaseID") %>' tabindex="-1" aria-labelledby="giroModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Upload GIRO Form</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:FileUpload ID="fuGiroForm" runat="server" CssClass="form-control mb-2" />
                            <asp:Label ID="lblGiroUploadMessage" runat="server" CssClass="text-danger" />
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnUploadGiro" runat="server" Text="Upload" CssClass="btn btn-success"
                                CommandArgument='<%# Eval("PurchaseID") %>' OnClick="btnUploadGiro_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>




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

    <section class="container terms-section mb-5">
        <h5>Terms & Conditions</h5>
        <ul>
            <li>All payments are subject to verification and processing timelines.</li>
            <li>GIRO deductions may take up to 14 business days to activate.</li>
            <li>For any disputes or refund queries, contact our customer care team.</li>
        </ul>
    </section>
</asp:Content>
