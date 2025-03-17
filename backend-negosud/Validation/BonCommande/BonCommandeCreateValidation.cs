using backend_negosud.DTOs.Commande_fournisseur.Inputs;
using FluentValidation;

namespace backend_negosud.Validation.BonCommande;

public class BonCommandeCreateValidation : AbstractValidator<BonCommandeCreateInputDto>
{
    public BonCommandeCreateValidation()
    {
        RuleFor(bonCommande => bonCommande.FournisseurID)
            .NotEmpty().WithMessage("L'identifiant du fournisseur est requis.");
        RuleForEach(commande => commande.LigneCommandes)
            .ChildRules(ligneCommande =>
            {
                ligneCommande.RuleFor(x => x.ArticleId)
                    .NotEmpty().WithMessage("L'identifiant de l'article est requis.");  
                ligneCommande.RuleFor(x => x.Quantite)
                    .NotEmpty().WithMessage("La quantité est requise.")
                    .InclusiveBetween(1, 100).WithMessage("La quantité doit être comprise entre 1 et 100.");
                ligneCommande.RuleFor(ligneCommande => ligneCommande.PrixUnitaire)
                    .NotEmpty().WithMessage("La prix unitaire est requis.");
            });
    }
}