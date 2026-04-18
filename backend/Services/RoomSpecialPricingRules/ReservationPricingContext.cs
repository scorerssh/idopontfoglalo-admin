using ApartManBackend.Models.DbModels.Models;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public class ReservationPricingContext
    {
        public required Room Room { get; init; }
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        public required IReadOnlyList<ReservationPerson> Persons { get; init; }
        public required decimal BaseTotalPrice { get; init; }

        public int Nights => EndDate.DayNumber - StartDate.DayNumber;
        public decimal BasePricePerNight => Nights > 0 ? BaseTotalPrice / Nights : 0;

        public IEnumerable<DateOnly> NightsInStay
        {
            get
            {
                for (var date = StartDate; date < EndDate; date = date.AddDays(1))
                {
                    yield return date;
                }
            }
        }
    }
}
