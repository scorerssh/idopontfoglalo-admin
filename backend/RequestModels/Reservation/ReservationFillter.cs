using ApartManBackend.Aditional.CoustumeFillter;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using FluentValidation;

namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationFillter : PaginationFilterBase
    {
        [StringFilterOptions(StringFilterOption.Contains)]
        public string? Name { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string? Email { get; set; }

        public int? RoomId { get; set; }

        [CompareTo(nameof(RoomId))]
        [OperatorComparison(OperatorType.NotEqual)]
        public int? NotInRoomId { get; set; }

        [CompareTo(nameof(ApartManBackend.Models.DbModels.Models.Reservation.Room))]
        [ReservationInApartmanById]
        public int? ApartmanId { get; set; }

        [CompareTo(nameof(ApartManBackend.Models.DbModels.Models.Reservation.Room))]
        [ReservationInUserById]
        public int? UserId { get; set; }

        [OperatorComparison(OperatorType.GreaterThanOrEqual)]
        public DateOnly? StartTIme { get; set; }

        [OperatorComparison(OperatorType.LessThanOrEqual)]
        public DateOnly? EndTime { get; set; }
    }

    public class ReservationFillterValidation : AbstractValidator<ReservationFillter>
    {
        public ReservationFillterValidation()
        {
            RuleFor(x => x.Page)
                .NotNull().WithMessage("A lap szĂˇm nem lehet nulla")
                .GreaterThan(0).WithMessage("A lap szĂˇm nem lehet nulla");
        }
    }
}
