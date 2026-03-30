using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationCreateRequestValidation : AbstractValidator<ReservationCreateRequest>
    {
        public ReservationCreateRequestValidation(ReservationService reservationService)
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            var today = DateOnly.FromDateTime(DateTime.Now);

            RuleFor(x => x.RoomGUid)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A szoba azonosító megadása kötelező.")
                .MustAsync(async (guid, ct) => await reservationService.RoomExistsByPublicIdAsync(guid!.Value, ct))
                .WithMessage("A szoba nem létezik.");

            RuleFor(x => x.StartTIme)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A kezdési idő megadása kötelező.")
                .Must(startDate => startDate!.Value > today)
                .WithMessage("Csak jövőbeli időpontra lehet foglalni.");

            RuleFor(x => x.EndTime)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A távozási idő megadása kötelező.")
                .Must((model, endDate) => model.StartTIme is not null && endDate!.Value > model.StartTIme.Value)
                .WithMessage("A távozási időnek későbbinek kell lennie, mint az érkezés.");

            RuleFor(x => x.PearsonCount)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A vendégek számának megadása kötelező.")
                .GreaterThan(0).WithMessage("Legalább 1 fő megadása kötelező.")
                .MustAsync(async (request, personCount, ct) =>
                {
                    if (request.RoomGUid is null || personCount is null)
                    {
                        return true;
                    }

                    return await reservationService.IsPersonCountWithinRoomCapacityAsync(
                        request.RoomGUid.Value,
                        personCount.Value,
                        ct);
                })
                .WithMessage("A megadott vendégszám nem fér bele a szoba minimum és maximum kapacitásába.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Az email cím megadása kötelező.")
                .EmailAddress().WithMessage("Az email cím formátuma nem megfelelő.");

            RuleFor(x => x)
                .MustAsync(async (request, ct) =>
                {
                    if (request.RoomGUid is null || request.StartTIme is null || request.EndTime is null)
                    {
                        return true;
                    }

                    return await reservationService.IsRoomAvailableAsync(
                        request.RoomGUid.Value,
                        request.StartTIme.Value,
                        request.EndTime.Value,
                        ct);
                })
                .WithMessage("A szoba a megadott időszakban nem elérhető.");
        }
    }
}
