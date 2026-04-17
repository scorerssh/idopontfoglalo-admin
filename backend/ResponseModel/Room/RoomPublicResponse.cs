using ApartManBackend.ResponseModel.AgePriceTier;
using ApartManBackend.ResponseModel.RoomPriceTier;

namespace ApartManBackend.ResponseModel.Room
{
    public class RoomPublicResponse
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public List<RoomPriceTierResponse>? RoomPriceTiers { get; set; }
        public List<AgePriceTierResponse>? AgePriceTiers { get; set; }
    }

}
