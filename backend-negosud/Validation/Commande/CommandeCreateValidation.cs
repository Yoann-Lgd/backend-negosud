using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using FluentValidation;
using Stripe.Radar;

namespace backend_negosud.Validation.Commande;

public class CommandeCreateValidation : AbstractValidator<CommandeInputDto>
{
    public CommandeCreateValidation()
    {
        RuleFor(commande => commande.ClientId)
            .NotEmpty().WithMessage("L'identifiant du client est requis.");
        RuleForEach(commande => commande.LigneCommandes)
            .ChildRules(ligneCommande =>
            {
                ligneCommande.RuleFor(x => x.ArticleId)
                    .NotEmpty().WithMessage("L'identifiant de l'article est requis.");  
                ligneCommande.RuleFor(x => x.Quantite)
                    .NotEmpty().WithMessage("La quantité est requise.")
                    .InclusiveBetween(1, 100).WithMessage("La quantité doit être comprise entre 1 et 100.");
            });
        RuleFor(commande => commande.Livraison.Livree).Equal(false);
        RuleFor(commande => commande.Livraison.DateEstimee).NotEmpty();
    }
    
}