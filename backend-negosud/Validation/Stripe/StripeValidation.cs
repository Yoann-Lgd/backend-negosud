using backend_negosud.DTOs.Commande_client;
using FluentValidation;

namespace backend_negosud.Validation;

public class StripeValidation: AbstractValidator<CheckoutStripeInputDto>
{
    public StripeValidation()
    {
        RuleFor(stripe => stripe.Amount).ExclusiveBetween(0, Int32.MaxValue).NotEmpty().NotNull();
        RuleFor(stripe => stripe.Currency).NotEmpty();
        RuleFor(stripe => stripe.SuccessUrl).NotEmpty();
        RuleFor(stripe => stripe.CancelUrl).NotEmpty();
        RuleFor(stripe => stripe.CommandeId).NotNull().NotNull().ExclusiveBetween(0, Int32.MaxValue);
    }
}