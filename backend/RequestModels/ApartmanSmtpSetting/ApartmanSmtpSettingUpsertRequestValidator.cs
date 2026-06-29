using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.ApartmanSmtpSetting
{
    public class ApartmanSmtpSettingUpsertRequestValidator : AbstractValidator<ApartmanSmtpSettingUpsertRequest>
    {
        public ApartmanSmtpSettingUpsertRequestValidator(ApartmanService apartmanService)
        {
            RuleFor(x => x.ApartmanId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Az apartman azonosito megadasa kotelezo.")
                .MustAsync(async (apartmanId, ct) => await apartmanService.CheckApartmanExists(apartmanId!.Value, ct))
                .WithMessage("Az apartman nem letezik.");

            RuleFor(x => x.Host)
                .NotEmpty().WithMessage("Az SMTP szerver megadasa kotelezo.")
                .MaximumLength(255).WithMessage("Az SMTP szerver legfeljebb 255 karakter lehet.");

            RuleFor(x => x.Port)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Az SMTP port megadasa kotelezo.")
                .InclusiveBetween(1, 65535).WithMessage("Az SMTP port 1 es 65535 kozott lehet.");

            RuleFor(x => x.UserName)
                .MaximumLength(255).WithMessage("A felhasznalonev legfeljebb 255 karakter lehet.");

            RuleFor(x => x.Password)
                .MaximumLength(1024).WithMessage("A jelszo legfeljebb 1024 karakter lehet.");

            RuleFor(x => x.SenderEmail)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A felado email cim megadasa kotelezo.")
                .EmailAddress().WithMessage("A felado email cim formatuma nem megfelelo.")
                .MaximumLength(255).WithMessage("A felado email cim legfeljebb 255 karakter lehet.");

            RuleFor(x => x.SenderName)
                .MaximumLength(100).WithMessage("A felado neve legfeljebb 100 karakter lehet.");

            RuleFor(x => x.GuestEmailIntroTemplate)
                .MaximumLength(1000).WithMessage("Az email bevezeto szoveg legfeljebb 1000 karakter lehet.");

            RuleFor(x => x.GuestSmsTemplate)
                .MaximumLength(1000).WithMessage("Az SMS sablon legfeljebb 1000 karakter lehet.");
        }
    }
}
