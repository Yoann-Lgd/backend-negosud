using backend_negosud.DTOs.Article.ArticleInputDto;
using FluentValidation;

namespace backend_negosud.Validation;

public class ArticleUpdateValidator : AbstractValidator<ArticleUpdateInputDto>
{
    public ArticleUpdateValidator()
    {
        RuleFor(article => article.ArticleId)
            .NotEmpty().WithMessage("L'identifiant de l'article est requis.");

        RuleFor(article => article.Libelle)
            .NotEmpty().When(article => !string.IsNullOrEmpty(article.Libelle))
            .MaximumLength(100).When(article => !string.IsNullOrEmpty(article.Libelle))
            .WithMessage("Le libellé ne doit pas dépasser 100 caractères.");

        RuleFor(article => article.Reference)
            .NotEmpty().When(article => !string.IsNullOrEmpty(article.Reference))
            .MaximumLength(50).When(article => !string.IsNullOrEmpty(article.Reference))
            .WithMessage("La référence ne doit pas dépasser 50 caractères.");

        RuleFor(article => article.Prix)
            .GreaterThan(0).When(article => article.Prix > 0)
            .WithMessage("Le prix doit être supérieur à zéro.");

        RuleFor(article => article.FamilleId)
            .NotEmpty().When(article => article.FamilleId != 0)
            .WithMessage("L'identifiant de la famille est requis.");

        RuleFor(article => article.FournisseurId)
            .NotEmpty().When(article => article.FournisseurId != 0)
            .WithMessage("L'identifiant du fournisseur est requis.");

        RuleFor(article => article.TvaId)
            .NotEmpty().When(article => article.TvaId != 0)
            .WithMessage("L'identifiant de la TVA est requis.");
    }
}