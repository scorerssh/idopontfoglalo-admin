using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.RoomSpecialPricingRule
{
    public class RoomSpecialPricingRuleUpdateRequestValidator : AbstractValidator<RoomSpecialPricingRuleUpdateRequest>
    {
        public RoomSpecialPricingRuleUpdateRequestValidator(
            RoomSercie roomSercie,
            RoomSpecialPricingRuleService roomSpecialPricingRuleService)
        {
            RuleFor(x => x.RoomSpecialPricingRuleId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A specialis arazasi szabaly azonositonak megadasa kotelezo.")
                .GreaterThan(0).WithMessage("A specialis arazasi szabaly azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await roomSpecialPricingRuleService.CheckRoomSpecialPricingRuleExistsAsync(id!.Value, ct))
                .WithMessage("Nincs ilyen specialis arazasi szabaly.");

            RuleFor(x => x)
                .Must(x => x.RoomId.HasValue || x.RuleType.HasValue || x.Priority.HasValue || x.Active.HasValue)
                .WithMessage("Legalabb egy mezot meg kell adni a frissiteshez.");

            When(x => x.RoomId.HasValue, () =>
            {
                RuleFor(x => x.RoomId)
                    .Cascade(CascadeMode.Stop)
                    .GreaterThan(0).WithMessage("A szoba azonositonak nagyobbnak kell lennie 0-nal.")
                    .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id!.Value, ct))
                    .WithMessage("Nincs ilyen szoba.");
            });

            When(x => x.RuleType.HasValue, () =>
            {
                RuleFor(x => x.RuleType)
                    .IsInEnum()
                    .WithMessage("A specialis arazasi szabaly tipusa ervenytelen.");
            });

            When(x => x.Priority.HasValue, () =>
            {
                RuleFor(x => x.Priority)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("A prioritas nem lehet negativ.");
            });
        }
    }
}
