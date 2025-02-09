using backend_negosud.DTOs.Article.ArticleInputDto;
using FluentValidation;

namespace backend_negosud.Validation;

public class ArticleInputCreateDtoValidator : AbstractValidator<ArticleInputCreateDto>
{
    public ArticleInputCreateDtoValidator()
    {
        RuleFor(article => article.Libelle)
            .NotEmpty().WithMessage("Le libellé est requis.")
            .MaximumLength(100).WithMessage("Le libellé ne doit pas dépasser 100 caractères.");

        RuleFor(article => article.Reference)
            .NotEmpty().WithMessage("La référence est requise.")
            .MaximumLength(50).WithMessage("La référence ne doit pas dépasser 50 caractères.");

        RuleFor(article => article.Prix)
            .GreaterThan(0).WithMessage("Le prix doit être supérieur à zéro.");

        RuleFor(article => article.FamilleId)
            .NotEmpty().WithMessage("L'identifiant de la famille est requis.");

        RuleFor(article => article.FournisseurId)
            .NotEmpty().WithMessage("L'identifiant du fournisseur est requis.");

        RuleFor(article => article.TvaId)
            .NotEmpty().WithMessage("L'identifiant de la TVA est requis.");
    }
}