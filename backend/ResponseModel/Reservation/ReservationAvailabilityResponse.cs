namespace ApartManBackend.ResponseModel.Reservation
{
    public class ReservationAvailabilityResponse
    {
        public Guid RoomGUid { get; set; }
        public DateOnly Month { get; set; }
        public List<DateOnly> AvailableDates { get; set; } = [];
    }
}
