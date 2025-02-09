using backend_negosud.DTOs.Utilisateur.Input;
using FluentValidation;

namespace backend_negosud.Validation;

public class UtilisateurEmailInputDtoValidator : AbstractValidator<UtilisateurEmailInputDto>
{
    public UtilisateurEmailInputDtoValidator()
    {
        RuleFor(emailInput => emailInput.Email)
            .NotEmpty().WithMessage("L'email ne peut pas être vide.")
            .EmailAddress().WithMessage("Saisir une adresse mail valide.");
    }
}