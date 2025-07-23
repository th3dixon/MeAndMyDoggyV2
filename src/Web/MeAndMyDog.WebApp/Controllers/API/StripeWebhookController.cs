using System.IO;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MeAndMyDog.WebApp.Controllers.API
{
    /// <summary>
    /// Handles Stripe webhook events
    /// </summary>
    [ApiController]
    [Route("api/webhooks")]
    [AllowAnonymous] // Webhooks come from Stripe, not authenticated users
    public class StripeWebhookController : ControllerBase
    {
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public StripeWebhookController(
            ILogger<StripeWebhookController> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Handle Stripe webhook events
        /// </summary>
        [HttpPost("stripe")]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]
                );

                _logger.LogInformation("Received Stripe webhook event: {EventType}", stripeEvent.Type);

                // Handle the event
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        await HandlePaymentIntentSucceeded(paymentIntent);
                        break;

                    case "payment_intent.payment_failed":
                        var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                        await HandlePaymentIntentFailed(failedPayment);
                        break;

                    case "customer.subscription.created":
                    case "customer.subscription.updated":
                        var subscription = stripeEvent.Data.Object as Subscription;
                        await HandleSubscriptionChange(subscription);
                        break;

                    case "customer.subscription.deleted":
                        var deletedSubscription = stripeEvent.Data.Object as Subscription;
                        await HandleSubscriptionCancelled(deletedSubscription);
                        break;

                    case "invoice.payment_succeeded":
                        var invoice = stripeEvent.Data.Object as Invoice;
                        await HandleInvoicePaymentSucceeded(invoice);
                        break;

                    case "invoice.payment_failed":
                        var failedInvoice = stripeEvent.Data.Object as Invoice;
                        await HandleInvoicePaymentFailed(failedInvoice);
                        break;

                    case "payment_method.attached":
                        var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                        await HandlePaymentMethodAttached(paymentMethod);
                        break;

                    case "payment_method.detached":
                        var detachedMethod = stripeEvent.Data.Object as PaymentMethod;
                        await HandlePaymentMethodDetached(detachedMethod);
                        break;

                    default:
                        _logger.LogWarning("Unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe webhook error");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error processing webhook");
                return StatusCode(500);
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            if (paymentIntent == null) return;

            _logger.LogInformation("Payment succeeded for {PaymentIntentId} - Amount: {Amount}", 
                paymentIntent.Id, paymentIntent.Amount);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/payments/stripe-webhook/payment-succeeded", new
                {
                    PaymentIntentId = paymentIntent.Id,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency,
                    CustomerId = paymentIntent.CustomerId,
                    Metadata = paymentIntent.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding payment success to backend API");
            }
        }

        private async Task HandlePaymentIntentFailed(PaymentIntent paymentIntent)
        {
            if (paymentIntent == null) return;

            _logger.LogWarning("Payment failed for {PaymentIntentId}", paymentIntent.Id);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/payments/stripe-webhook/payment-failed", new
                {
                    PaymentIntentId = paymentIntent.Id,
                    CustomerId = paymentIntent.CustomerId,
                    FailureMessage = paymentIntent.LastPaymentError?.Message,
                    Metadata = paymentIntent.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding payment failure to backend API");
            }
        }

        private async Task HandleSubscriptionChange(Subscription subscription)
        {
            if (subscription == null) return;

            _logger.LogInformation("Subscription {SubscriptionId} updated - Status: {Status}", 
                subscription.Id, subscription.Status);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/subscriptions/stripe-webhook/subscription-changed", new
                {
                    SubscriptionId = subscription.Id,
                    CustomerId = subscription.CustomerId,
                    Status = subscription.Status,
                    Items = subscription.Items.Data.Select(i => new
                    {
                        PriceId = i.Price.Id,
                        ProductId = i.Price.ProductId,
                        Quantity = i.Quantity
                    }),
                    Metadata = subscription.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding subscription change to backend API");
            }
        }

        private async Task HandleSubscriptionCancelled(Subscription subscription)
        {
            if (subscription == null) return;

            _logger.LogWarning("Subscription {SubscriptionId} cancelled", subscription.Id);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/subscriptions/stripe-webhook/subscription-cancelled", new
                {
                    SubscriptionId = subscription.Id,
                    CustomerId = subscription.CustomerId,
                    CancelledAt = subscription.CanceledAt,
                    Metadata = subscription.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding subscription cancellation to backend API");
            }
        }

        private async Task HandleInvoicePaymentSucceeded(Invoice invoice)
        {
            if (invoice == null) return;

            _logger.LogInformation("Invoice {InvoiceId} paid - Amount: {Amount}", 
                invoice.Id, invoice.AmountPaid);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/invoices/stripe-webhook/invoice-paid", new
                {
                    InvoiceId = invoice.Id,
                    CustomerId = invoice.CustomerId,
                    // SubscriptionId might be null for one-time invoices
                    AmountPaid = invoice.AmountPaid,
                    Currency = invoice.Currency,
                    InvoicePdf = invoice.InvoicePdf,
                    HostedInvoiceUrl = invoice.HostedInvoiceUrl,
                    Metadata = invoice.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding invoice payment to backend API");
            }
        }

        private async Task HandleInvoicePaymentFailed(Invoice invoice)
        {
            if (invoice == null) return;

            _logger.LogWarning("Invoice {InvoiceId} payment failed", invoice.Id);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/invoices/stripe-webhook/invoice-failed", new
                {
                    InvoiceId = invoice.Id,
                    CustomerId = invoice.CustomerId,
                    // SubscriptionId might be null for one-time invoices
                    AttemptCount = invoice.AttemptCount,
                    Metadata = invoice.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding invoice failure to backend API");
            }
        }

        private async Task HandlePaymentMethodAttached(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null) return;

            _logger.LogInformation("Payment method {PaymentMethodId} attached to customer {CustomerId}", 
                paymentMethod.Id, paymentMethod.CustomerId);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/payment-methods/stripe-webhook/method-attached", new
                {
                    PaymentMethodId = paymentMethod.Id,
                    CustomerId = paymentMethod.CustomerId,
                    Type = paymentMethod.Type,
                    Card = paymentMethod.Card != null ? new
                    {
                        Brand = paymentMethod.Card.Brand,
                        Last4 = paymentMethod.Card.Last4,
                        ExpMonth = paymentMethod.Card.ExpMonth,
                        ExpYear = paymentMethod.Card.ExpYear
                    } : null,
                    Metadata = paymentMethod.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding payment method attachment to backend API");
            }
        }

        private async Task HandlePaymentMethodDetached(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null) return;

            _logger.LogInformation("Payment method {PaymentMethodId} detached", paymentMethod.Id);

            // Forward to backend API
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                await client.PostAsJsonAsync("api/v1/payment-methods/stripe-webhook/method-detached", new
                {
                    PaymentMethodId = paymentMethod.Id,
                    Metadata = paymentMethod.Metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding payment method detachment to backend API");
            }
        }
    }
}