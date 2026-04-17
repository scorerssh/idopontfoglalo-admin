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
                .NotNull().WithMessage("A foglalas azonositojanak megadasa kotelezo.")
                .GreaterThan(0).WithMessage("A foglalas azonositojanak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await reservationService.CheckReservationExistsAsync(id!.Value, ct))
                .WithMessage("Nincs ilyen foglalas.");

            When(x => x.StartTIme.HasValue, () =>
            {
                RuleFor(x => x.StartTIme)
                    .Cascade(CascadeMode.Stop)
                    .Must(startDate => startDate!.Value > today)
                    .WithMessage("Csak jovobeli idopontra lehet foglalni.");
            });

            When(x => x.EndTime.HasValue, () =>
            {
                RuleFor(x => x.EndTime)
                    .Cascade(CascadeMode.Stop)
                    .Must(endDate => endDate!.Value > today)
                    .WithMessage("Csak jovobeli idopontra lehet foglalni.");
            });

            When(x => x.RoomId.HasValue, () =>
            {
                RuleFor(x => x.RoomId)
                    .Cascade(CascadeMode.Stop)
                    .GreaterThan(0).WithMessage("A szoba azonositojanak nagyobbnak kell lennie 0-nal.")
                    .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id!.Value, ct))
                    .WithMessage("Nincs ilyen szoba.");
            });

            When(x => x.PearsonCount.HasValue, () =>
            {
                RuleFor(x => x.PearsonCount)
                    .GreaterThan(0).WithMessage("Legalabb 1 fo megadasa kotelezo.")
                    .Must((request, personCount) => request.Persons is not null && personCount == request.Persons.Count)
                    .WithMessage("A vendegszamnak egyeznie kell a megadott szemelyek szamaval.");
            });

            When(x => x.Persons is not null, () =>
            {
                RuleFor(x => x.Persons)
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
            });

            When(x => x.Email is not null, () =>
            {
                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Az email cim nem lehet ures.")
                    .EmailAddress().WithMessage("Az email cim formatuma nem megfelelo.");
            });

            When(x => x.Name is not null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("A nev nem lehet ures.");
            });

            When(x => x.StartTIme.HasValue || x.EndTime.HasValue, () =>
            {
                RuleFor(x => x)
                    .MustAsync(async (request, ct) => await reservationService.HasValidUpdatedDateRangeAsync(request, ct))
                    .WithMessage("A tavozasi idonek kesobbinek kell lennie, mint az erkezes.");
            });

            When(x => x.RoomId.HasValue || x.Persons is not null, () =>
            {
                RuleFor(x => x)
                    .MustAsync(async (request, ct) => await reservationService.IsUpdatedPersonCountWithinRoomCapacityAsync(request, ct))
                    .WithMessage("A megadott vendegszam nem fer bele a szoba minimum es maximum kapacitasaba.");
            });

            When(x => x.RoomId.HasValue || x.StartTIme.HasValue || x.EndTime.HasValue, () =>
            {
                RuleFor(x => x)
                    .MustAsync(async (request, ct) => await reservationService.IsUpdatedReservationRoomAvailableAsync(request, ct))
                    .WithMessage("A szoba a megadott idoszakban nem elerheto.");
            });
        }
    }
}
