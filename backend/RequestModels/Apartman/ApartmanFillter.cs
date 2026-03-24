using ApartManBackend.Aditional.CoustumeFillter;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using FluentValidation;

namespace ApartManBackend.RequestModels.Apartman
{
    public class ApartmanFillter: PaginationFilterBase
    {
        [StringFilterOptions(AutoFilterer.Enums.StringFilterOption.Contains)]
        public string? Name { get; set; }

        [CompareTo(nameof(ApartManBackend.Models.DbModels.Models.Apartman.Users))]
        [NotInCollectionById]
        public int? NotInUserId { get; set; }
    }

    public class ApartmanfillterValidation:AbstractValidator<ApartmanFillter>
    {
        public ApartmanfillterValidation()
        {
            RuleFor(x => x.Page)
                .NotNull().WithMessage("A lap szám nem lehet nulla")
                .GreaterThan(0).WithMessage("A lap szám nem lehet nulla");
        }
    }
}
