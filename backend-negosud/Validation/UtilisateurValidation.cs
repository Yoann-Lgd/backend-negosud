using backend_negosud.DTOs;
using backend_negosud.DTOs.Utilisateur.Input;
using backend_negosud.Entities;
using FluentValidation;

namespace backend_negosud.Validation;

public class UtilisateurValidation : AbstractValidator<UtilisateurInputDto>
{
    public UtilisateurValidation()
    {
        RuleFor(utilisateur => utilisateur.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Saisir une adresse mail valide");

        RuleFor(utilisateur => utilisateur.MotDePasse)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage("Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial, et faire au moins 8 caractères.");
    }    
}