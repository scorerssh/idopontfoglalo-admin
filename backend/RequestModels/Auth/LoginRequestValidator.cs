using FluentValidation;

namespace ApartManBackend.RequestModels.Auth
{
    public class LoginRequestValidator:AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x=>x.Email)
                .NotEmpty()
                .WithMessage("Az email cim megadasa kotelezo.")
                .EmailAddress()
                .WithMessage("Az email cim formatuma nem megfelelo.")
                .MaximumLength(255)
                .WithMessage("Az email cim legfeljebb 255 karakter lehet.");
            RuleFor(x=>x.Password)
                .NotEmpty()
                .WithMessage("A jelszo megadasa kotelezo.")
                .MinimumLength(8)
                .WithMessage("A jelszonak legalabb 8 karakter hosszunak kell lennie.")
                .MaximumLength(128)
                .WithMessage("A jelszo legfeljebb 128 karakter lehet.");

        }
    }
}
