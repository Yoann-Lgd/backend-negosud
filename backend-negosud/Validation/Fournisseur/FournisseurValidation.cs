using System.Runtime.Serialization;
using backend_negosud.DTOs.Fournisseur.FournisseurInputDto;
using FluentValidation;

namespace backend_negosud.Validation.Fournisseur;

public class FournisseurValidation : AbstractValidator<FournisseurInputMinimal>
{
    public FournisseurValidation()
    {
        RuleFor(f => f.FournisseurId)
            .NotEmpty().WithMessage("L'identifiant du fournisseur est requis.");
        RuleFor(f =>  f.Nom)
            .NotEmpty()
            .Length(0, 100)
            .WithMessage("Saisissez un nom");
        RuleFor(f => f.Email).NotEmpty().EmailAddress().WithMessage("Saisir une adresse mail valide.");
        RuleFor(f => f.Tel).NotEmpty();
        RuleFor(f => f.RaisonSociale).NotEmpty();
    }
}