using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Apartman
{
    public class UpdateApartmanRequestValidator:AbstractValidator<ApartmanUpdateRequest>
    {
        public UpdateApartmanRequestValidator(ApartmanService apartmanService,UserService userService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Az apartman azonositonak megadasa kotelezo.")
                .GreaterThan(0).WithMessage("Az apartman azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await apartmanService.CheckApartmanExists(id!.Value, ct))
                .WithMessage("Nincs ilyen apartman.");




            When(x => !string.IsNullOrEmpty(x.Name), () =>
            {
                RuleFor(x => x.Name)
                    .MaximumLength(100)
                    .WithMessage("Az apartman nev legfeljebb 100 karakter lehet.")
                    .MustAsync(async (request,name, ct) => !await apartmanService.IsApartmanNameTakenByAnotherAsync(request.Id!.Value,name,ct))
                    .WithMessage("Már létezik ilyen nevű apartman.");
            });

            When(x => x.Users != null, () =>
            {
                RuleFor(x => x.Users)
                    .SetValidator(new UpdateApartmanUsersSubRequestValidator(userService)!);
            });
        }
    }
}
