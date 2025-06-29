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
            height: 440px;
            border-radius: 0 0 32px 32px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            box-shadow: 0 10px 30px rgba(99, 102, 241, 0.2);
            padding: 0 2rem;
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
    <div class="hero-container shadow-sm rounded-bottom">
        <img src="Images/Paymentbackground.png" alt="Paying for your policy" class="loaded" />
        <div class="hero-text">
            <h1>Pay for Your Insurance Plan</h1>
            <asp:Label ID="lblPlanName" runat="server" CssClass="h3 fw-bold" />
            <br />
            <asp:Label ID="lblAmountDue" runat="server" CssClass="h4 text-warning" />
            <br />
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
    <asp:Repeater ID="rptPlans" runat="server" OnItemDataBound="rptPlans_ItemDataBound">
        <ItemTemplate>
            <div class="card mb-4 shadow-sm mx-auto" style="max-width: 500px;">
                <div class="card-body">
                    <h4 class="card-title"><%# Eval("PlanName") %></h4>
                    <p><strong>Amount Due: <%# Eval("AmountDue", "{0:C2}") %></strong></p>
                    <p class="card-text">Payment Frequency: <%# Eval("PaymentFrequency") %></p>
                    <p class="card-text">Next Billing Date: <%# Eval("NextBillingDate", "{0:dd MMM yyyy}") %></p>
                    <p class="card-text fw-semibold">Total Payments Made: <%# Eval("TotalPaymentCount") %></p>

                    <div class="d-flex flex-wrap gap-2 justify-content-center mt-3">
                        <asp:Button ID="btnPayNow" runat="server" CssClass="btn btn-outline-danger btn-sm"
                            Text="PayNow"
                            CommandArgument='<%# Eval("PurchaseID") %>'
                            OnClientClick='<%# "event.preventDefault(); var modal = new bootstrap.Modal(document.getElementById(\"payNowModal_" + Eval("PurchaseID") + "\")); modal.show();" %>' />

                        <asp:Button ID="btnCard" runat="server" CssClass="btn btn-outline-danger btn-sm"
                            Text="Card"
                            CommandArgument='<%# Eval("PurchaseID") %>'
                            OnClientClick='<%# "event.preventDefault(); var modal = new bootstrap.Modal(document.getElementById(\"cardModal_" + Eval("PurchaseID") + "\")); modal.show();" %>' />

                        <button type="button" class="btn btn-outline-primary btn-sm"
                            data-bs-toggle="modal"
                            data-bs-target='<%# "#giroModal_" + Eval("PurchaseID") %>'>
                            GIRO
                        </button>

                        <asp:Button ID="btnConfirmPayNow" runat="server" CssClass="btn btn-outline-success btn-sm"
                            Text="Confirm PayNow Payment"
                            CommandArgument='<%# Eval("PurchaseID") %>'
                            OnClientClick='<%# "event.preventDefault(); var modal = new bootstrap.Modal(document.getElementById(\"payNowConfirmModal_" + Eval("PurchaseID") + "\")); modal.show();" %>' />
                    </div>

                    <asp:PlaceHolder ID="phGiroActive" runat="server" Visible="false">
                        <div class="text-success fw-bold mt-3">
                            GIRO Payment Active - Auto deduction enabled
                        </div>
                        <asp:Button ID="btnCancelGiro" runat="server" Text="Cancel GIRO" CssClass="btn btn-outline-danger mt-2"
                            data-bs-toggle="modal" data-bs-target='<%# "#cancelGiroModal_" + Eval("PurchaseID") %>' UseSubmitBehavior="false" />
                    </asp:PlaceHolder>
                </div>

                <!-- Cancel GIRO Modal -->
                <div class="modal fade" id='<%# "cancelGiroModal_" + Eval("PurchaseID") %>' tabindex="-1" aria-labelledby="cancelGiroModalLabel" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content p-4">
                            <div class="modal-header">
                                <h5 class="modal-title">Confirm GIRO Cancellation</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <p>Are you sure you want to cancel GIRO for this plan? This will disable all future automatic deductions.</p>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnConfirmCancelGiro" runat="server" CssClass="btn btn-danger" Text="Yes, Cancel GIRO" OnClick="btnCancelGiro_Click" CommandArgument='<%# Eval("PurchaseID") %>' />
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">No, Keep GIRO</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- PayNow QR Modal -->
                <div class="modal fade" id='<%# "payNowModal_" + Eval("PurchaseID") %>' tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content text-center p-4">
                            <div class="modal-header">
                                <h5 class="modal-title">PayNow QR Code</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <img src="Images/UM_bank_OCBC_paynow_qr-code_480x480.jpg" alt="PayNow QR Code" style="max-width: 200px; margin: auto; display: block;" />
                                <p>Scan this QR code with your banking app to pay.</p>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Card Payment Modal -->
                <div class="modal fade" id='<%# "cardModal_" + Eval("PurchaseID") %>' tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content p-4">
                            <div class="modal-header">
                                <h5 class="modal-title">Card Payment</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <asp:Label ID="lblCardMessage" runat="server" CssClass="text-danger" />
                                <asp:TextBox ID="txtCardholderName" runat="server" CssClass="form-control mb-2" Placeholder="Cardholder Name" MaxLength="100" />
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
                                <asp:Button ID="btnSubmitCard" runat="server" CssClass="btn btn-danger" Text="Pay" OnClick="btnSubmitCard_Click" CommandArgument='<%# Eval("PurchaseID") %>' />
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- GIRO Modal -->
                <div class="modal fade" id='<%# "giroModal_" + Eval("PurchaseID") %>' tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content p-4">
                            <div class="modal-header">
                                <h5 class="modal-title">Upload GIRO Authorization Form (PDF)</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <asp:Label ID="lblGiroUploadMessage" runat="server" CssClass="text-danger" />
                                <asp:FileUpload ID="fuGiroForm" runat="server" CssClass="form-control" Accept=".pdf" />
                                <small class="form-text text-muted mt-2">Please upload a scanned GIRO authorization PDF.</small>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnUploadGiro" runat="server" CssClass="btn btn-primary" Text="Submit GIRO" OnClick="btnUploadGiro_Click" CommandArgument='<%# Eval("PurchaseID") %>' />
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- PayNow Confirmation Modal -->
                <div class="modal fade" id='<%# "payNowConfirmModal_" + Eval("PurchaseID") %>' tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content p-4">
                            <div class="modal-header">
                                <h5 class="modal-title">Confirm PayNow Payment</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <asp:Label ID="lblPayNowMessage" runat="server" CssClass="text-danger" />
                                <asp:TextBox ID="txtPayNowRef" runat="server" CssClass="form-control mb-3" Placeholder="Enter transaction reference" MaxLength="100" />
                                <asp:FileUpload ID="fuPayNowReceipt" runat="server" CssClass="form-control" Accept=".jpg,.png,.pdf" />
                                <small class="form-text text-muted mt-2">Upload payment receipt (optional)</small>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnSubmitPayNow" runat="server" CssClass="btn btn-danger" Text="Confirm Payment" OnClick="btnSubmitPayNow_Click" CommandArgument='<%# Eval("PurchaseID") %>' />
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            </div>
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
