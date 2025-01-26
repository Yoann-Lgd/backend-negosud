using backend_negosud.DTOs;
using FluentValidation;

namespace backend_negosud.Validation;

public class ClientValidation : AbstractValidator<ClientInputDto>
{
    public ClientValidation()
    {
        RuleFor(client => client.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Saisir une adresse mail valide");

        RuleFor(client => client.MotDePasse)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage("Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial, et faire au moins 8 caractères.");
    }
}