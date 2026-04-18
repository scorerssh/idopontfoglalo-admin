using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.RoomSpecialPricingRule
{
    public class RoomSpecialPricingRuleCreateRequestValidator : AbstractValidator<RoomSpecialPricingRuleCreateRequest>
    {
        public RoomSpecialPricingRuleCreateRequestValidator(RoomSercie roomSercie)
        {
            RuleFor(x => x.RoomId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("A szoba azonositonak nagyobbnak kell lennie 0-nal.")
                .MustAsync(async (id, ct) => await roomSercie.CheckRoomExistsAsync(id, ct))
                .WithMessage("Nincs ilyen szoba.");

            RuleFor(x => x.RuleType)
                .IsInEnum()
                .WithMessage("A specialis arazasi szabaly tipusa ervenytelen.");

            RuleFor(x => x.Priority)
                .GreaterThanOrEqualTo(0)
                .WithMessage("A prioritas nem lehet negativ.");
        }
    }
}
