using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.AgePriceTier
{
    public class AgePriceTierCreateRequestValidator : AbstractValidator<AgePriceTierCreateRequest>
    {
        public AgePriceTierCreateRequestValidator(RoomSercie roomSercie, AgePriceTierService agePriceTierService)
        {
            RuleFor(x => x.RoomId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("A szoba azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id, ct))
                .WithMessage("Nincs ilyen szoba.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Az ar nem lehet 0.");

            RuleFor(x => x.AgeRangeLow)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Az also eletkor nem lehet negativ.")
                .LessThanOrEqualTo(150)
                .WithMessage("Az also eletkor legfeljebb 150 lehet.");

            RuleFor(x => x.AgeRangeHigh)
                .GreaterThanOrEqualTo(x => x.AgeRangeLow)
                .WithMessage("A felso eletkor nem lehet kisebb mint az also eletkor.")
                .LessThanOrEqualTo(150)
                .WithMessage("A felso eletkor legfeljebb 150 lehet.");

            RuleFor(x => x)
                .MustAsync(async (request, ct) => await agePriceTierService.HasNoAgeRangeOverlapAsync(
                    request.RoomId,
                    request.AgeRangeLow,
                    request.AgeRangeHigh,
                    null,
                    ct))
                .WithMessage("Az eletkor sav atfed egy mar letezo savval.");
        }
    }
}
