using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Apartman
{
    public class ApartmanCreateRequestValidation:AbstractValidator<ApartmanCreateRequest>
    {
        public ApartmanCreateRequestValidation(ApartmanService apartmanService)
        {
            RuleFor(x=>x.Name)
                .NotEmpty()
                .WithMessage("Az apartman nev megadasa kotelezo.")
                .MaximumLength(100)
                .WithMessage("Az apartman nev legfeljebb 100 karakter lehet.")
                .MustAsync(async (name, ct) => !await apartmanService.CheckApartmanNameExistAsync(name))
                .WithMessage("Már létezik ilyen nevű apartman.");
        }
    }
}
