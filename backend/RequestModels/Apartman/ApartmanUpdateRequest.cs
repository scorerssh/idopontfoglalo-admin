namespace ApartManBackend.RequestModels.Apartman
{
    public class ApartmanUpdateRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public UpdateApartmanUsersSubRequest? Users { get; set; }
    }
}
