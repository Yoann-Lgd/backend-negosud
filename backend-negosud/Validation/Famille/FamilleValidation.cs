using backend_negosud.DTOs.Famille;
using FluentValidation;

namespace backend_negosud.Validation;

public class FamilleValidation : AbstractValidator<FamilleCreateInputDto>
{
    public FamilleValidation()
    {
        RuleFor(famille =>  famille.Nom)
            .NotEmpty()
            .Length(0, 100)
            .WithMessage("Saisissez un nom");
    }
}