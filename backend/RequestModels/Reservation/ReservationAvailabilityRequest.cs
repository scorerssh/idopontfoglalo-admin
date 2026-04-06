namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationAvailabilityRequest
    {
        public Guid? RoomGUid { get; set; }
        public DateOnly? Month { get; set; }
    }
}
