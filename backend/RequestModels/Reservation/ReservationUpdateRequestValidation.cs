using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationUpdateRequestValidation : AbstractValidator<ReservationUpdateRequest>
    {
        public ReservationUpdateRequestValidation(ReservationService reservationService, RoomSercie roomSercie)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            RuleFor(x => x.ReservationId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A foglalás azonosítójának megadása kötelező.")
                .GreaterThan(0).WithMessage("A foglalás azonosítójának nagyobbnak kell lennie 0-nál.")
                .MustAsync(async (id, ct) => await reservationService.CheckReservationExistsAsync(id!.Value, ct))
                .WithMessage("Nincs ilyen foglalás.");

            When(x => x.StartTIme.HasValue, () =>
            {
                RuleFor(x => x.StartTIme)
                    .Cascade(CascadeMode.Stop)
                    .Must(startDate => startDate!.Value > today)
                    .WithMessage("Csak jövőbeli időpontra lehet foglalni.");
            });

            When(x => x.EndTime.HasValue, () =>
            {
                RuleFor(x => x.EndTime)
                    .Cascade(CascadeMode.Stop)
                    .Must(endDate => endDate!.Value > today)
                    .WithMessage("Csak jövőbeli időpontra lehet foglalni.");
            });

            When(x => x.RoomId.HasValue, () =>
            {
                RuleFor(x => x.RoomId)
                    .Cascade(CascadeMode.Stop)
                    .GreaterThan(0).WithMessage("A szoba azonosítójának nagyobbnak kell lennie 0-nál.")
                    .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id!.Value, ct))
                    .WithMessage("Nincs ilyen szoba.");
            });

            When(x => x.PearsonCount.HasValue, () =>
            {
                RuleFor(x => x.PearsonCount)
                    .GreaterThan(0).WithMessage("Legalább 1 fő megadása kötelező.");
            });

            When(x => x.Email is not null, () =>
            {
                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Az email cím nem lehet üres.")
                    .EmailAddress().WithMessage("Az email cím formátuma nem megfelelő.");
            });

            When(x => x.Name is not null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("A név nem lehet üres.");
            });

            When(x => x.StartTIme.HasValue || x.EndTime.HasValue, () =>
            {
                RuleFor(x => x)
                    .MustAsync(async (request, ct) => await reservationService.HasValidUpdatedDateRangeAsync(request, ct))
                    .WithMessage("A távozási időnek későbbinek kell lennie, mint az érkezés.");
            });

            When(x => x.RoomId.HasValue || x.StartTIme.HasValue || x.EndTime.HasValue, () =>
            {
                RuleFor(x => x)
                    .MustAsync(async (request, ct) => await reservationService.IsUpdatedReservationRoomAvailableAsync(request, ct))
                    .WithMessage("A szoba a megadott időszakban nem elérhető.");
            });
        }
    }
}
