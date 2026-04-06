using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationAvailabilityRequestValidation : AbstractValidator<ReservationAvailabilityRequest>
    {
        public ReservationAvailabilityRequestValidation(ReservationService reservationService)
        {
            RuleFor(x => x.RoomGUid)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A szoba azonosito megadasa kotelezo.")
                .MustAsync(async (guid, ct) => await reservationService.RoomExistsByPublicIdAsync(guid!.Value, ct))
                .WithMessage("A szoba nem letezik.");

            RuleFor(x => x.Month)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A honap megadasa kotelezo.")
                .Must(month => month!.Value.Day == 1)
                .WithMessage("A honap mezoben a honap elso napjat add meg 'yyyy-MM-dd' formatumban.");
        }
    }
}
