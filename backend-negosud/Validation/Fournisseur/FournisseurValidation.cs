using System.Runtime.Serialization;
using backend_negosud.DTOs.Fournisseur.FournisseurInputDto;
using FluentValidation;

namespace backend_negosud.Validation.Fournisseur;
public class FournisseurValidation : AbstractValidator<FournisseurInputMinimal>
{
    public FournisseurValidation()
    {
        RuleFor(f => f.FournisseurId)
            .NotNull().WithMessage("L'identifiant du fournisseur est requis.");

        RuleFor(f => f.Nom)
            .MaximumLength(100).WithMessage("Le nom ne doit pas dépasser 100 caractères.")
            .When(f => !string.IsNullOrEmpty(f.Nom));

        RuleFor(f => f.Email)
            .EmailAddress().WithMessage("Saisir une adresse mail valide.")
            .When(f => !string.IsNullOrEmpty(f.Email));

        RuleFor(f => f.Tel)
            .MaximumLength(25).WithMessage("Le numéro de téléphone ne doit pas dépasser 25 caractères.")
            .When(f => !string.IsNullOrEmpty(f.Tel));

        RuleFor(f => f.RaisonSociale)
            .MaximumLength(150).WithMessage("La raison sociale ne doit pas dépasser 150 caractères.")
            .When(f => !string.IsNullOrEmpty(f.RaisonSociale));
    }
}
