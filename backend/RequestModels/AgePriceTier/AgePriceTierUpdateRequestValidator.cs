using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.AgePriceTier
{
    public class AgePriceTierUpdateRequestValidator : AbstractValidator<AgePriceTierUpdateRequest>
    {
        public AgePriceTierUpdateRequestValidator(RoomSercie roomSercie, AgePriceTierService agePriceTierService)
        {
            RuleFor(x => x.AgePriceTierId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Az eletkor ar sav azonositonak megadasa kotelezo.")
                .GreaterThan(0).WithMessage("Az eletkor ar sav azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await agePriceTierService.CheckAgePriceTierExistsAsync(id!.Value, ct))
                .WithMessage("Nincs ilyen eletkor ar sav.");

            RuleFor(x => x)
                .Must(x => x.RoomId.HasValue || x.Price.HasValue || x.AgeRangeLow.HasValue || x.AgeRangeHigh.HasValue)
                .WithMessage("Legalabb egy mezot meg kell adni a frissiteshez.");

            When(x => x.RoomId.HasValue, () =>
            {
                RuleFor(x => x.RoomId)
                    .Cascade(CascadeMode.Stop)
                    .GreaterThan(0).WithMessage("A szoba azonositonak nagyobbnak kell lennie 0-nal.")
                    .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id!.Value, ct))
                    .WithMessage("Nincs ilyen szoba.");
            });

            When(x => x.Price.HasValue, () =>
            {
                RuleFor(x => x.Price)
                    .GreaterThan(0)
                    .WithMessage("Az ar nem lehet 0.");
            });

            When(x => x.AgeRangeLow.HasValue, () =>
            {
                RuleFor(x => x.AgeRangeLow)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Az also eletkor nem lehet negativ.")
                    .LessThanOrEqualTo(150)
                    .WithMessage("Az also eletkor legfeljebb 150 lehet.");
            });

            When(x => x.AgeRangeHigh.HasValue, () =>
            {
                RuleFor(x => x.AgeRangeHigh)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("A felso eletkor nem lehet negativ.")
                    .LessThanOrEqualTo(150)
                    .WithMessage("A felso eletkor legfeljebb 150 lehet.");
            });

            RuleFor(x => x)
                .MustAsync(async (request, ct) => await agePriceTierService.HasValidUpdatedAgeRangeAsync(request, ct))
                .WithMessage("A felso eletkor nem lehet kisebb mint az also eletkor.");

            RuleFor(x => x)
                .MustAsync(async (request, ct) => await agePriceTierService.HasNoAgeRangeOverlapAsync(request, ct))
                .WithMessage("Az eletkor sav atfed egy mar letezo savval.");
        }
    }
}
