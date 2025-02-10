using backend_negosud.DTOs.Pays.PaysInputDto;
using FluentValidation;

namespace backend_negosud.Validation.Pays;

public class PaysCreateValidation : AbstractValidator<PaysInputDto>
{
    public PaysCreateValidation()
    {
        RuleFor(pays => pays.Nom)
            .NotEmpty().WithMessage("Le nom est requis.")
            .MaximumLength(150).WithMessage("Le nom ne doit pas dépasser 150 caractères.");
        }   
}