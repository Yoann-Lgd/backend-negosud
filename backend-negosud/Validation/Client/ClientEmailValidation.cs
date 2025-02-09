using backend_negosud.DTOs;
using FluentValidation;

namespace backend_negosud.Validation;

public class ClientEmailValidation : AbstractValidator<ClientEmailInputDto>
{
    public ClientEmailValidation()
    {
        RuleFor(emailInput => emailInput.Email)
            .NotEmpty().WithMessage("L'email ne peut pas être vide.")
            .EmailAddress().WithMessage("Saisir une adresse mail valide.");
    }
}