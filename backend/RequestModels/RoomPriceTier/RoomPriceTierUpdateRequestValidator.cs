using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.RoomPriceTier
{
    public class RoomPriceTierUpdateRequestValidator : AbstractValidator<RoomPriceTierUpdateRequest>
    {
        public RoomPriceTierUpdateRequestValidator(RoomPriceTierService roomPriceTierService)
        {
            RuleFor(x => x.RoomPriceTierId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Az ar sav azonositonak megadasa kotelezo.")
                .GreaterThan(0).WithMessage("Az ar sav azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await roomPriceTierService.CheckRoomPriceTierExistsAsync(id!.Value, ct))
                .WithMessage("Nincs ilyen ar sav.");

            RuleFor(x => x.Price)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Az ar megadasa kotelezo.")
                .GreaterThan(0).WithMessage("Az ar nem lehet 0.");
        }
    }
}
