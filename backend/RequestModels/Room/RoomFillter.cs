using ApartManBackend.RequestModels.Apartman;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using FluentValidation;

namespace ApartManBackend.RequestModels.Room
{
    public class RoomFillter: PaginationFilterBase
    {
        [StringFilterOptions(AutoFilterer.Enums.StringFilterOption.Contains)]
        public string? Name { get; set; }

        [OperatorComparison(OperatorType.GreaterThanOrEqual)]
        public int? MinCapacity { get; set; }

        [OperatorComparison(OperatorType.LessThanOrEqual)]
        public int? MaxCapacity { get; set; }

        public int? ApartmanId { get; set; }

        [CompareTo(nameof(ApartmanId))]
        [OperatorComparison(OperatorType.NotEqual)]
        public int? NotInApartmanId { get; set; }

        public ApartmanFillter? Apartman { get; set; }

    }

    public class RoomFillterValidation:AbstractValidator<RoomFillter>
    {
        public RoomFillterValidation()
        {
            RuleFor(x => x.Page)
                .NotNull().WithMessage("A lap szám nem lehet nulla")
                .GreaterThan(0).WithMessage("A lap szám nem lehet nulla");
        }
    }
}
