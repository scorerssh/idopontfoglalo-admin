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
            await _db.Rooms.AddAsync(room, ct);
            await _db.SaveChangesAsync(ct);
        }

        public Task<bool> CheckRoomExistsAsync(int roomId, CancellationToken ct)
        {
            return _db.Rooms.AnyAsync(x => x.Id == roomId, ct);
        }

        public async Task<bool> UpdateAsync(RoomUpdateRequest request, CancellationToken ct)
        {
            var id = request.RoomId;
            var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (room == null)
                return false;
            _mapper.Map(request, room);
            await _db.SaveChangesAsync(ct);
            return true;
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
