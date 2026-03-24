using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Room
{
    public class UpdaetRoomValidation:AbstractValidator<RoomUpdateRequest>
    {
        public UpdaetRoomValidation(ApartmanService apartmanService, RoomSercie roomSercie)
        {

            RuleFor(x => x.RoomId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A szoba azonositonak megadasa kotelezo.")
                .GreaterThan(0).WithMessage("A szoba azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id!.Value, ct))
                .WithMessage("Nincs ilyen szoba.");



            When(x => x.MaxCapacity.HasValue, () =>
            {

                RuleFor(x => x.MaxCapacity)
             .GreaterThan(0).WithMessage("A szoba kapacitasanak nagyobbnak kell lennie 0-nal.")
             .LessThan(100).WithMessage("A szoba kapacitasanak kevesebbnek kell lennie 100-nal.");
            });

            When(x => x.Name is not null, () =>
            {

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("A szoba neve megadasa kotelezo.")
                    .MaximumLength(100)
                    .WithMessage("A szoba neve legfeljebb 100 karakter lehet.");
            });

            When(x => x.ApartmanId.HasValue, () =>
            {
                RuleFor(x => x.ApartmanId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Az apartman azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await apartmanService.CheckApartmanExists(id!.Value, ct));
            });


        }
    }
}
