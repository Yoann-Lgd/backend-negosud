using backend_negosud.DTOs.Commande_client;
using backend_negosud.Services;
using backend_negosud.Validation;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace backend_negosud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly StripeService _stripeService;
        private readonly ILogger<StripeController> _logger;
        private readonly string _webhookSecret;

        public StripeController(StripeService stripeService, ILogger<StripeController> logger, IConfiguration configuration)
        {
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
            string json;
            try
            {
                using var reader = new StreamReader(HttpContext.Request.Body);
                json = await reader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Erreur lors de la lecture du corps de la requête webhook : {e.Message}");
                return BadRequest();
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webhookSecret);

                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        var session = stripeEvent.Data.Object as Session;
                        _logger.LogInformation($"✅ Paiement réussi pour la session : {session?.Id}");

                        // TODO: Mettre à jour la commande comme "Payée" et enregistrer le règlement
                        if (session != null)
                        {
                            var orderId = session.Metadata?["order_id"];
                            if (orderId != null)
                            {
                                // TODO : logique pour mettre à jour la commande avec l'ID de commande récupéré depuis les metadata
                                // var result = await _commandeService.UpdateOrderStatusToPaidAsync(orderId);
                                // if (result)
                                // {
                                //     _logger.LogInformation($"Commande {orderId} mise à jour avec succès comme payée.");
                                // }
                                // else
                                // {
                                //     _logger.LogError($"Erreur lors de la mise à jour de la commande {orderId}.");
                                // }
                            }
                        }
                        break;

                    case Events.PaymentIntentPaymentFailed:
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogWarning($"❌ Échec du paiement pour {paymentIntent?.Id}. Raison : {paymentIntent?.LastPaymentError?.Message}");
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
