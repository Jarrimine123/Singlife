using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Stripe;
using Stripe.Checkout;

public class StripeWebhookHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var json = new StreamReader(context.Request.InputStream).ReadToEnd();
        var webhookSecret = ConfigurationManager.AppSettings["StripeWebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                context.Request.Headers["Stripe-Signature"],
                webhookSecret);

            // Use string literal for event type here
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                // Fetch full session details with line items and payment intent info
                var service = new SessionService();
                session = service.Get(session.Id, new SessionGetOptions
                {
                    Expand = new List<string> { "line_items", "payment_intent" }
                });

                SavePurchaseToDatabase(session);
                SendConfirmationEmail(session);
            }

            context.Response.StatusCode = 200;
            context.Response.Write("Webhook received");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Webhook Error: " + ex.Message);
        }
    }

    private void SavePurchaseToDatabase(Session session)
    {
        var connStr = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;

        string accountIdStr = session.Metadata.ContainsKey("AccountID") ? session.Metadata["AccountID"] : "0";
        string fullName = session.Metadata.ContainsKey("FullName") ? session.Metadata["FullName"] : "";
        string email = session.Metadata.ContainsKey("Email") ? session.Metadata["Email"] : "";
        string phone = session.Metadata.ContainsKey("Phone") ? session.Metadata["Phone"] : "";
        string address = session.Metadata.ContainsKey("Address") ? session.Metadata["Address"] : "";

        int accountId = int.TryParse(accountIdStr, out int accId) ? accId : 0;

        Guid purchaseGroupId = Guid.NewGuid();
        DateTime purchaseDate = DateTime.Now;
        string paymentMethod = "Stripe Card";
        string cardLast4 = "";

        if (session.PaymentIntent != null)
        {
            var chargeService = new ChargeService();
            var charges = chargeService.List(new ChargeListOptions
            {
                PaymentIntent = session.PaymentIntent.Id,
                Limit = 1,
            });

            var charge = charges.Data.Count > 0 ? charges.Data[0] : null;
            if (charge != null && charge.PaymentMethodDetails?.Card != null)
            {
                cardLast4 = charge.PaymentMethodDetails.Card.Last4;
            }
        }


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();

            foreach (var item in session.LineItems.Data)
            {
                string productName = item.Description ?? "Unknown Product";
                string planName = "-";
                decimal coverageAmount = 0m;
                decimal annualPremium = 0m;
                decimal monthlyPremium = 0m;
                string paymentFrequency = "-";
                DateTime nextBillingDate = purchaseDate.AddYears(1);

                if (item.Price?.Product != null)
                {
                    var productService = new ProductService();
                    var product = productService.Get(item.Price.ProductId);
                    if (product.Metadata.ContainsKey("PlanName"))
                        planName = product.Metadata["PlanName"];
                    if (product.Metadata.ContainsKey("CoverageAmount"))
                        decimal.TryParse(product.Metadata["CoverageAmount"], out coverageAmount);
                    if (product.Metadata.ContainsKey("PaymentFrequency"))
                        paymentFrequency = product.Metadata["PaymentFrequency"];
                }

                decimal priceAmount = item.Price.UnitAmount.HasValue ? item.Price.UnitAmount.Value / 100m : 0m;
                if (paymentFrequency.ToLower() == "monthly")
                {
                    monthlyPremium = priceAmount;
                    annualPremium = monthlyPremium * 12;
                    nextBillingDate = purchaseDate.AddMonths(1);
                }
                else
                {
                    annualPremium = priceAmount;
                    monthlyPremium = annualPremium / 12;
                    nextBillingDate = purchaseDate.AddYears(1);
                }

                string insertQuery = @"
                    INSERT INTO Purchases
                    (PurchaseGroupID, AccountID, FullName, Email, Phone, Address,
                     ProductName, PlanName, CoverageAmount, AnnualPremium, MonthlyPremium,
                     PurchaseDate, PaymentMethod, CardLast4, PaymentFrequency, NextBillingDate)
                    VALUES
                    (@PurchaseGroupID, @AccountID, @FullName, @Email, @Phone, @Address,
                     @ProductName, @PlanName, @CoverageAmount, @AnnualPremium, @MonthlyPremium,
                     @PurchaseDate, @PaymentMethod, @CardLast4, @PaymentFrequency, @NextBillingDate)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@PurchaseGroupID", purchaseGroupId);
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@PlanName", planName);
                    cmd.Parameters.AddWithValue("@CoverageAmount", coverageAmount);
                    cmd.Parameters.AddWithValue("@AnnualPremium", annualPremium);
                    cmd.Parameters.AddWithValue("@MonthlyPremium", monthlyPremium);
                    cmd.Parameters.AddWithValue("@PurchaseDate", purchaseDate);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@CardLast4", cardLast4);
                    cmd.Parameters.AddWithValue("@PaymentFrequency", paymentFrequency);
                    cmd.Parameters.AddWithValue("@NextBillingDate", nextBillingDate);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    private void SendConfirmationEmail(Session session)
    {
        try
        {
            string email = session.Metadata.ContainsKey("Email") ? session.Metadata["Email"] : null;
            if (string.IsNullOrEmpty(email)) return;

            var sb = new StringBuilder();
            sb.AppendLine("Thank you for your purchase! Here are your purchased plan(s):\n");

            foreach (var item in session.LineItems.Data)
            {
                string product = item.Description ?? "Unknown Product";
                decimal price = item.Price.UnitAmount.HasValue ? item.Price.UnitAmount.Value / 100m : 0m;
                string currency = item.Price.Currency?.ToUpper() ?? "SGD";

                sb.AppendLine($"- {product}: {currency} {price:F2}");
            }

            sb.AppendLine("\nYour policy will be processed and activated shortly.");
            sb.AppendLine("\nRegards,");
            sb.AppendLine("Singlife Team");

            MailMessage message = new MailMessage();
            message.To.Add(email);
            message.From = new MailAddress(
                ConfigurationManager.AppSettings["EmailSenderAddress"],
                "Singlife Team");
            message.Subject = "Singlife Insurance Purchase Confirmation";
            message.Body = sb.ToString();
            message.IsBodyHtml = false;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["EmailSenderAddress"],
                ConfigurationManager.AppSettings["EmailSenderPassword"]);
            client.EnableSsl = true;

            client.Send(message);
        }
        catch (Exception)
        {
            // Optionally log error here
        }
    }

    public bool IsReusable => false;
}
