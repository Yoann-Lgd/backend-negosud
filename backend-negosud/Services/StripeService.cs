using backend_negosud.DTOs.Commande_client;
using backend_negosud.Repository;
using backend_negosud.Services;
using backend_negosud.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Stripe;
using Stripe.Checkout;
public class StripeService
{
    private readonly ICommandeRepository _commandeRepository;
    private readonly ICommandeService _commandeService;
    
    public StripeService(ICommandeRepository commandeRepository, ICommandeService commandeService)
    {
        _commandeRepository = commandeRepository;
        _commandeService = commandeService;
    }
    public async Task<string> CreateCheckoutSessionAsync(CheckoutStripeInputDto checkoutStripeInputDto)
    {
        var validation = new StripeValidation();
        var validationResult = validation.Validate(checkoutStripeInputDto);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException("Validation échouée: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        if (string.IsNullOrEmpty(checkoutStripeInputDto.Currency) || checkoutStripeInputDto.Currency.Equals("string", StringComparison.OrdinalIgnoreCase))
        {
            checkoutStripeInputDto.Currency = "eur";
        }
        if (string.IsNullOrEmpty(checkoutStripeInputDto.SuccessUrl) || checkoutStripeInputDto.SuccessUrl.Equals("string", StringComparison.OrdinalIgnoreCase))
        {
            checkoutStripeInputDto.SuccessUrl = "http://aremplacer.fr/success?session_id={CHECKOUT_SESSION_ID}";
        }                
        if (string.IsNullOrEmpty(checkoutStripeInputDto.CancelUrl) || checkoutStripeInputDto.CancelUrl.Equals("string", StringComparison.OrdinalIgnoreCase))
        {
            checkoutStripeInputDto.CancelUrl = "http://aremplacer.fr/cancel";
        }
        
        var commande = await _commandeService.GetCommandeById(checkoutStripeInputDto.CommandeId);
        if (commande == null || commande.LigneCommandes.Count == 0)
        {
            throw new Exception("La commande n'existe pas ou n'a pas de ligne de commande");
        }

        Console.WriteLine(checkoutStripeInputDto.Currency);
        
        var lineItems = new List<SessionLineItemOptions>();
        foreach (var ligne in commande.LigneCommandes)
        {
            var article = ligne.Article;
            
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = checkoutStripeInputDto.Currency,
                    UnitAmount = (long)(article.Prix * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = article.Libelle 
                    }
                },
                Quantity = ligne.Quantite
            });
        }

        Console.WriteLine(checkoutStripeInputDto.SuccessUrl);
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = checkoutStripeInputDto.SuccessUrl,
            CancelUrl = checkoutStripeInputDto.CancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "commande id", checkoutStripeInputDto.CommandeId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }
}