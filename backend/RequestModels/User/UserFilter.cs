using ApartManBackend.Aditional.CoustumeFillter;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using FluentValidation;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.RequestModels.User
{
    public class UserFilter : PaginationFilterBase
    {
        public int? Id { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string? UserName { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string? UserEmail { get; set; }

        public UserRole? Role { get; set; }

        [CompareTo(nameof(ApartManBackend.Models.DbModels.Models.User.Apartmans))]
        [NotInCollectionById]
        public int? NotInApartmanId { get; set; }
    }

    public class UserFileterValidation:AbstractValidator<UserFilter>
    {
        public UserFileterValidation()
        {
            RuleFor(x => x.Page)
                .NotNull().WithMessage("A lap szám nem lehet nulla")
                .GreaterThan(0).WithMessage("A lap szám nem lehet nulla");
        }
    }
}
