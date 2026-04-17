namespace ApartManBackend.RequestModels.Reservation
{
    public class ReservationCreateRequest
    {
        public DateOnly? StartTIme { get; set; }
        public DateOnly? EndTime { get; set; }
        public int? PearsonCount { get; set; }
        public Guid? RoomGUid { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? Email { get; set; } 
        public string? Description { get; set; }
        public List<ReservationPersonRequest>? Persons { get; set; }


    }
}
