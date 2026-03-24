using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.User
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateRequestValidator(ApartmanService apartmanService)
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("A felhasznalonev megadasa kotelezo.")
                .MaximumLength(100)
                .WithMessage("A felhasznalonev legfeljebb 100 karakter lehet.");

            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("Az email cim megadasa kotelezo.")
                .EmailAddress()
                .WithMessage("Az email cim formatuma nem megfelelo.")
                .MaximumLength(255)
                .WithMessage("Az email cim legfeljebb 255 karakter lehet.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("A jelszo megadasa kotelezo.")
                .MinimumLength(8)
                .WithMessage("A jelszonak legalabb 8 karakter hosszunak kell lennie.")
                .MaximumLength(128)
                .WithMessage("A jelszo legfeljebb 128 karakter lehet.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Az adott szerepkor nem ervenyes.");

            

        }
    }
}
