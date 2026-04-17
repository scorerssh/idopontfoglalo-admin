using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.RoomPriceTier;
using ApartManBackend.ResponseModel.RoomPriceTier;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class RoomPriceTierService
    {
        private readonly ApartmanDbContext _db;
        private readonly IMapper _mapper;

        public RoomPriceTierService(ApartmanDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public Task<bool> CheckRoomPriceTierExistsAsync(int roomPriceTierId, CancellationToken ct)
        {
            return _db.RoomPriceTiers.AnyAsync(x => x.Id == roomPriceTierId, ct);
        }

        public async Task<RoomPriceTierResponse?> UpdateAsync(RoomPriceTierUpdateRequest request, CancellationToken ct)
        {
            var roomPriceTier = await _db.RoomPriceTiers
                .FirstOrDefaultAsync(x => x.Id == request.RoomPriceTierId, ct);

            if (roomPriceTier is null)
            {
                return null;
            }

            _mapper.Map(request, roomPriceTier);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<RoomPriceTierResponse>(roomPriceTier);
        }
    }
}
