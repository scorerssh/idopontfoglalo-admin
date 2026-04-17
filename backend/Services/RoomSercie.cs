using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.Room;
using ApartManBackend.ResponseModel.Room;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class RoomSercie
    {
        private readonly IMapper _mapper;
        private readonly ApartmanDbContext _db;

        public RoomSercie(IMapper mapper, ApartmanDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task CreateAsync(RoomCreateRequest request, CancellationToken ct)
        {
            var room = _mapper.Map<Room>(request);
            room.RoomPriceTiers = new List<RoomPriceTier>();

            decimal basePrice = room.Price;
            decimal extraGuestPrice = 4000;

            for (var i = room.MinCapacity; i <= room.MaxCapacity; i++)
            {
                decimal price = basePrice + ((i - room.MinCapacity) * extraGuestPrice);

                room.RoomPriceTiers.Add(new RoomPriceTier
                {
                    GuestCount = i,
                    Price = price
                });
            }

            await _db.Rooms.AddAsync(room, ct);
            await _db.SaveChangesAsync(ct);
        }
        public Task<bool> CheckRoomExistsAsync(int roomId, CancellationToken ct)
        {
            return _db.Rooms.AnyAsync(x => x.Id == roomId, ct);
        }

        public async Task<bool> HasValidUpdatedCapacityAsync(RoomUpdateRequest request, CancellationToken ct)
        {
            if (!request.MinCapacity.HasValue && !request.MaxCapacity.HasValue)
            {
                return true;
            }

            var room = await _db.Rooms
                .AsNoTracking()
                .Where(x => x.Id == request.RoomId)
                .Select(x => new { x.MinCapacity, x.MaxCapacity })
                .FirstOrDefaultAsync(ct);

            if (room is null)
            {
                return false;
            }

            var newMinCapacity = request.MinCapacity ?? room.MinCapacity;
            var newMaxCapacity = request.MaxCapacity ?? room.MaxCapacity;

            return newMinCapacity > 0 && newMaxCapacity >= newMinCapacity;
        }

        public async Task<bool> UpdateAsync(RoomUpdateRequest request, CancellationToken ct)
        {
            var id = request.RoomId;
            var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (room == null)
                return false;
            if(request.MaxCapacity.HasValue||request.MinCapacity.HasValue)
            {
                await ReCalculatePriceTires(request, room, ct);
            }

            _mapper.Map(request, room);
            await _db.SaveChangesAsync(ct);
            return true;
        }


        private async Task ReCalculatePriceTires(RoomUpdateRequest request, Room room, CancellationToken ct)
        {
            await _db.Entry(room)
                .Collection(x => x.RoomPriceTiers!)
                .LoadAsync(ct);

            var newMinCapacity = request.MinCapacity ?? room.MinCapacity;
            var newMaxCapacity = request.MaxCapacity ?? room.MaxCapacity;

            if (newMinCapacity <= 0)
                throw new Exception("A minimum kapacitásnak nagyobbnak kell lennie mint 0.");

            if (newMaxCapacity < newMinCapacity)
                throw new Exception("A maximum kapacitás nem lehet kisebb mint a minimum kapacitás.");

            var existingTiers = room.RoomPriceTiers!.ToList();

            var tiersToRemove = existingTiers
                .Where(x => x.GuestCount < newMinCapacity || x.GuestCount > newMaxCapacity)
                .ToList();

            if (tiersToRemove.Count > 0)
            {
                _db.RoomPriceTiers.RemoveRange(tiersToRemove);
            }

            decimal GetDefaultPrice(int guestCount)
            {
                if (existingTiers.Count == 0)
                    return 10000;

                var nearestLower = existingTiers
                    .Where(x => x.GuestCount < guestCount)
                    .OrderByDescending(x => x.GuestCount)
                    .FirstOrDefault();

                if (nearestLower != null)
                    return nearestLower.Price;

                var nearestHigher = existingTiers
                    .Where(x => x.GuestCount > guestCount)
                    .OrderBy(x => x.GuestCount)
                    .FirstOrDefault();

                if (nearestHigher != null)
                    return nearestHigher.Price;

                return 10000;
            }

            for (int guestCount = newMinCapacity; guestCount <= newMaxCapacity; guestCount++)
            {
                var exists = existingTiers.Any(x => x.GuestCount == guestCount);
                if (exists)
                    continue;

                room.RoomPriceTiers!.Add(new RoomPriceTier
                {
                    GuestCount = guestCount,
                    Price = GetDefaultPrice(guestCount)
                });
            }
        }


        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affectedRows = await _db.Rooms.Where(x => x.Id == id).ExecuteDeleteAsync(ct);
            return affectedRows > 0;
        }


        public async Task<RoomResponse?> GetAsync(int roomid, CancellationToken ct)
        {
            return await _db.Rooms.Where(x => x.Id == roomid).ProjectTo<RoomResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(ct);
        }

        public async Task<RoomPublicResponse?> GetByGuidIdAsync(Guid guidId, CancellationToken ct)
        {
            return await _db.Rooms.Where(x => x.GuidId == guidId).ProjectTo<RoomPublicResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(ct);
        }

        public async Task<RoomsWithMetaData> GetAllAsync(RoomFillter fillter, CancellationToken ct)
        {
            fillter.PerPage = 10;
            fillter.Sort = nameof(Room.CreatedAt);
            fillter.SortBy = AutoFilterer.Enums.Sorting.Descending;

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var filteredQuery = _db.Rooms.ApplyFilterWithoutPagination(fillter);
            var pagedQuery = _db.Rooms.ApplyFilter(fillter);

            var rooms = await pagedQuery
                .ProjectTo<RoomResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            var count = await filteredQuery.CountAsync(ct);
            var activeCount = await filteredQuery.CountAsync(x => x.Active, ct);
            var inActiveCount = await filteredQuery.CountAsync(x => !x.Active, ct);
            var inUseCount = await filteredQuery.CountAsync(x =>
                x.Reservations.Any(r =>
                    r.EndTime >= currentDate &&
                    r.StartTIme <= currentDate), ct);

            var res = new RoomsWithMetaData
            {
                Count = count,
                ActiveCount = activeCount,
                InActiveCount = inActiveCount,
                InUseCount = inUseCount,
                Rooms = rooms
            };

            return res;
        }

    }
}
