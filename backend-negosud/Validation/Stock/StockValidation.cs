using backend_negosud.DTOs;
using FluentValidation;

namespace backend_negosud.Validation;

public class StockValidation : AbstractValidator<StockInputDto>
{
   public StockValidation()
   {
      RuleFor(stock => stock.Quantite)
         .ExclusiveBetween(0, Int32.MaxValue);
      RuleFor(stock => stock.RefLot)
         .NotEmpty()
         .NotNull();
      RuleFor(stock => stock.SeuilMinimum)
         .ExclusiveBetween(0, Int32.MaxValue);
   }
}
public class StockValidationUpdate : AbstractValidator<StockUpdateDto>
{
   public StockValidationUpdate()
   {
      RuleFor(stock => stock.Quantite)
         .ExclusiveBetween(0, Int32.MaxValue);
      RuleFor(stock => stock.RefLot)
         .NotEmpty()
         .NotNull();
      RuleFor(stock => stock.SeuilMinimum)
         .ExclusiveBetween(0, Int32.MaxValue);
   }
}
