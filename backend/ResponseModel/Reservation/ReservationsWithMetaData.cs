namespace ApartManBackend.ResponseModel.Reservation
{
    public class ReservationsWithMetaData
    {
        public List<ReservationResponse>? Reservations { get; set; }

        public int Count { get; set; }

        public int ReservationsCreatedThisMonth { get; set; }

        public int ReservationsCreatedToday { get; set; }
    }
}
