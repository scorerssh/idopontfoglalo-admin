namespace ApartManBackend.RequestModels.Apartman
{
    public class UpdateApartmanUsersSubRequest
    {
        public List<int>? UserIdsToAdd { get; set; }
        public List<int>? UserIdsToRemove { get; set; }
    }
}
