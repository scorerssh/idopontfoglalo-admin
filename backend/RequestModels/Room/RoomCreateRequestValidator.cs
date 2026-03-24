using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Room
{
    public class RoomCreateRequestValidator:AbstractValidator<RoomCreateRequest>
    {
        public RoomCreateRequestValidator(ApartmanService apartmanService)
        {
            RuleFor(x=>x.Name)
                .NotEmpty()
                .WithMessage("A szoba neve megadasa kotelezo.")
                .MaximumLength(100)
                .WithMessage("A szoba neve legfeljebb 100 karakter lehet.");
            RuleFor(x=>x.MaxCapacity)
                .GreaterThan(0).WithMessage("A szoba kapacitasanak nagyobbnak kell lennie 0-nal.")
                .LessThan(100).WithMessage("A szoba kapacitasanak kevesebbnek kell lennie 100-nal.");
            RuleFor(x => x.ApartmanId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Az apartman azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await apartmanService.CheckApartmanExists(id, ct));
        }
    }
}
