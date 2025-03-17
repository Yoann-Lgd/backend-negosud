using backend_negosud.DTOs;
using FluentValidation;

namespace backend_negosud.Validation;

public class StockValidation : AbstractValidator<StockInputDto>
{
   public StockValidation()
   {
      RuleFor(stock => stock.ArticleId)
         .GreaterThan(0)
         .WithMessage("L'ID de l'article doit être supérieur à 0");

      RuleFor(stock => stock.Quantite)
         .ExclusiveBetween(0, Int32.MaxValue)
         .WithMessage("La quantité doit être supérieure à 0");

      RuleFor(stock => stock.RefLot)
         .NotEmpty()
         .NotNull()
         .WithMessage("La référence du lot est obligatoire");

      RuleFor(stock => stock.SeuilMinimum)
         .ExclusiveBetween(0, Int32.MaxValue)
         .WithMessage("Le seuil minimum doit être supérieur à 0");
   }
}
public class StockValidationUpdate : AbstractValidator<StockUpdateDto>
{
   public StockValidationUpdate()
   {
      RuleFor(stock => stock.nouvelleQuantite)
         .ExclusiveBetween(0, Int32.MaxValue);

   }
}
