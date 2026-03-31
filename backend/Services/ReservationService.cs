using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.Reservation;
using ApartManBackend.ResponseModel.Reservation;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class ReservationService
    {
        private readonly Dictionary<Guid, Room> _roomCache = [];
        private readonly ApartmanDbContext _db;
        private readonly IMapper _mapper;

        public ReservationService(ApartmanDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        public async Task CreateAsync(ReservationCreateRequest request,CancellationToken ct)
        {
            var room = await GetRoomWithReservationsByPublicIdAsync(request.RoomGUid!.Value,ct);
            if (room == null) { return; }
            var reservation=_mapper.Map<Reservation>(request);
            room.Reservations.Add(reservation);
            await _db.SaveChangesAsync(ct);

        }

        public async Task<ReservationResponse?> GetAsnyc(int id,CancellationToken ct)
        {
            return await  _db.Reservations.Where(x=>x.Id== id).ProjectTo<ReservationResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(ct);
        }


        public async Task<List<ReservationResponse>> GetAllAsync(ReservationFillter fillter, CancellationToken ct)
        {
            fillter.PerPage = 10;
            fillter.Sort = nameof(Reservation.CreatedAt);
            fillter.SortBy = AutoFilterer.Enums.Sorting.Descending;
            return await _db.Reservations
                .ApplyFilter(fillter)
                .ProjectTo<ReservationResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<bool> RoomExistsByPublicIdAsync(Guid roomPublicId, CancellationToken ct)
        {
            var room = await GetRoomWithReservationsByPublicIdAsync(roomPublicId, ct);
            return room is not null;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomPublicId, DateOnly startDate, DateOnly endDate, CancellationToken ct)
        {
            var room = await GetRoomWithReservationsByPublicIdAsync(roomPublicId, ct);
            if (room is null)
            {
                return false;
            }

            var hasOverlappingReservation = room.Reservations
                .Any(reservation => reservation.StartTIme < endDate && reservation.EndTime > startDate);

            return !hasOverlappingReservation;
        }

        public async Task<bool> IsPersonCountWithinRoomCapacityAsync(Guid roomPublicId, int personCount, CancellationToken ct)
        {
            var room = await GetRoomWithReservationsByPublicIdAsync(roomPublicId, ct);
            if (room is null)
            {
                return false;
            }

            return personCount >= room.MinCapacity && personCount <= room.MaxCapacity;
        }

        private async Task<Room?> GetRoomWithReservationsByPublicIdAsync(Guid roomPublicId, CancellationToken ct)
        {
            if (_roomCache.TryGetValue(roomPublicId, out var cachedRoom))
            {
                return cachedRoom;
            }

            var room = await _db.Rooms
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.GuidId == roomPublicId, ct);

            if (room is not null)
            {
                _roomCache[roomPublicId] = room;
            }

            return room;
        }


    }
}
