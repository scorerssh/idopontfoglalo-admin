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
                .NotNull().WithMessage("A szoba azonosito megadasa kotelezo.")
                .MustAsync(async (guid, ct) => await reservationService.RoomExistsByPublicIdAsync(guid!.Value, ct))
                .WithMessage("A szoba nem letezik.");

            RuleFor(x => x.StartTIme)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A kezdesi ido megadasa kotelezo.")
                .Must(startDate => startDate!.Value > today)
                .WithMessage("Csak jovobeli idopontra lehet foglalni.");

            RuleFor(x => x.EndTime)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A tavozasi ido megadasa kotelezo.")
                .Must((model, endDate) => model.StartTIme is not null && endDate!.Value > model.StartTIme.Value)
                .WithMessage("A tavozasi idonek kesobbinek kell lennie, mint az erkezes.");

            RuleFor(x => x.Persons)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A vendegek eletkoranak megadasa kotelezo.")
                .NotEmpty().WithMessage("Legalabb 1 fo megadasa kotelezo.");

            RuleForEach(x => x.Persons)
                .ChildRules(person =>
                {
                    person.RuleFor(x => x.Age)
                        .Cascade(CascadeMode.Stop)
                        .NotNull().WithMessage("A vendeg eletkoranak megadasa kotelezo.")
                        .GreaterThanOrEqualTo(0).WithMessage("A vendeg eletkora nem lehet negativ.")
                        .LessThanOrEqualTo(150).WithMessage("A vendeg eletkora legfeljebb 150 lehet.");
                });

            When(x => x.PearsonCount.HasValue, () =>
            {
                RuleFor(x => x.PearsonCount)
                    .GreaterThan(0).WithMessage("Legalabb 1 fo megadasa kotelezo.")
                    .Must((request, personCount) => request.Persons is null || personCount == request.Persons.Count)
                    .WithMessage("A vendegszamnak egyeznie kell a megadott szemelyek szamaval.");
            });

            RuleFor(x => x)
                .MustAsync(async (request, ct) =>
                {
                    if (request.RoomGUid is null || request.Persons is null)
                    {
                        return true;
                    }

                    return await reservationService.IsPersonCountWithinRoomCapacityAsync(
                        request.RoomGUid.Value,
                        request.Persons.Count,
                        ct);
                })
                .WithMessage("A megadott vendegszam nem fer bele a szoba minimum es maximum kapacitasaba.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Az email cim megadasa kotelezo.")
                .EmailAddress().WithMessage("Az email cim formatuma nem megfelelo.");

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
                .WithMessage("A szoba a megadott idoszakban nem elerheto.");
        }
    }
}
