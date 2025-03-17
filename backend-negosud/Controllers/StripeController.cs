using backend_negosud.DTOs.Commande_client;
using backend_negosud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace backend_negosud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly ICommandeService _commandeService;
        private readonly StripeService _stripeService;
        private readonly ILogger<StripeController> _logger;
        private readonly string _webhookSecret;

        public StripeController(StripeService stripeService, ILogger<StripeController> logger,
            IConfiguration configuration, ICommandeService commandeService)
        {
            _commandeService = commandeService;
            _stripeService = stripeService;
            _logger = logger;
            _webhookSecret = configuration["Stripe:WebhookSecret"];
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutStripeInputDto checkoutStripeInputDto)
        {
            try
            {
                var sessionUrl = await _stripeService.CreateCheckoutSessionAsync(checkoutStripeInputDto);
                return Ok(new { url = sessionUrl });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Erreur de validation : {ex.Message}");
                return BadRequest(new { error = "Erreur de validation : " + ex.Message });
            }
            catch (StripeException e)
            {
                _logger.LogError($"Erreur Stripe lors de la création de la session : {e.Message}");
                return BadRequest(new { error = $"Erreur lors de la création de la session Stripe : {e.Message}" });
            }
            catch (Exception e)
            {
                _logger.LogError($"Erreur lors de la création de la session : {e.Message}");
                return BadRequest(new { error = "Erreur lors de la création de la session." });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            _logger.LogInformation("Webhook Stripe reçu");

            string json;
            try
            {
                using var reader = new StreamReader(HttpContext.Request.Body);
                json = await reader.ReadToEndAsync();
                _logger.LogInformation("Corps du webhook reçu: {Json}", json);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erreur lors de la lecture du corps de la requête webhook : {e.Message}");
                return BadRequest();
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _webhookSecret,
                    throwOnApiVersionMismatch: false
                );

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Session;
                        _logger.LogInformation($"✅ Paiement réussi pour la session : {session?.Id}");

                        if (session != null)
                        {
                            var orderId = session.Metadata?["commande id"];
                            if (orderId != null)
                            {
                                // récupération du montant depuis la session
                                decimal? stripeAmount = session.AmountTotal;

                                var result = await _commandeService.UpdateOrderStatusToPaidAsync(orderId, stripeAmount);
                                if (result.Success)
                                {
                                    _logger.LogInformation($"commande {orderId} mise à jour avec succès comme payée.");
                                }
                                else
                                {
                                    _logger.LogError(
                                        $"Erreur lors de la mise à jour de la commande {orderId}: {result.Message}");
                                }
                            }
                        }

                        break;

                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogInformation($"🔍 Paiement réussi pour l'intention : {paymentIntent?.Id}");

                        // rçupération de l'id de la commande à partir des métadonnées du paiement
                        if (paymentIntent?.Metadata?.TryGetValue("commande id", out string paymentOrderId) == true)
                        {
                            decimal? amount = paymentIntent.Amount;
                            var result = await _commandeService.UpdateOrderStatusToPaidAsync(paymentOrderId, amount);
                            if (result.Success)
                            {
                                _logger.LogInformation(
                                    $"commande {paymentOrderId} mise à jour avec succès comme payée.");
                            }
                            else
                            {
                                _logger.LogError(
                                    $"Erreur lors de la mise à jour de la commande {paymentOrderId}: {result.Message}");
                            }
                        }

                        break;

                    case "payment_intent.payment_failed":
                        var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogWarning(
                            $"❌ Échec du paiement pour {failedPaymentIntent?.Id}. Raison : {failedPaymentIntent?.LastPaymentError?.Message}");
                        break;

                    default:
                        _logger.LogInformation($"🔍 Événement Stripe reçu : {stripeEvent.Type}");
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError($"❌ Erreur Webhook Stripe : {e.Message}");
                return BadRequest();
            }
        }
    }
}