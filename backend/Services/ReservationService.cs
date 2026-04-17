using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.Reservation;
using ApartManBackend.ResponseModel.Reservation;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using static ApartManBackend.StaticMambers.Enums;

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
            var room = await GetRoomWithPricingByPublicIdAsync(request.RoomGUid!.Value,ct);
            if (room == null) { return; }
            var reservation = _mapper.Map<Reservation>(request);

            // Set the FK explicitly so the insert never depends on relationship fixup alone.
            reservation.RoomId = room.Id;
            reservation.Room = room;
            reservation.Source = ReservationSource.Website;
            ApplyPersonsAndPrice(
                reservation,
                room,
                request.Persons!,
                request.StartTIme!.Value,
                request.EndTime!.Value);

            await _db.Reservations.AddAsync(reservation, ct);
            await _db.SaveChangesAsync(ct);

        }

        public async Task<ReservationResponse?> GetAsnyc(int id,CancellationToken ct)
        {
            return await  _db.Reservations.Where(x=>x.Id== id).ProjectTo<ReservationResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(ct);
        }

        public Task<bool> CheckReservationExistsAsync(int reservationId, CancellationToken ct)
        {
            return _db.Reservations.AnyAsync(x => x.Id == reservationId, ct);
        }

        public async Task<bool> UpdateAsync(ReservationUpdateRequest request, CancellationToken ct)
        {
            var reservation = await _db.Reservations
                .Include(x => x.Persons)
                .FirstOrDefaultAsync(x => x.Id == request.ReservationId, ct);

            if (reservation == null)
            {
                return false;
            }

            _mapper.Map(request, reservation);

            if (request.RoomId.HasValue)
            {
                var roomExists = await _db.Rooms.AnyAsync(x => x.Id == request.RoomId.Value, ct);
                if (!roomExists)
                {
                    return false;
                }

                reservation.RoomId = request.RoomId.Value;
            }

            if (request.Persons is not null || request.RoomId.HasValue || request.StartTIme.HasValue || request.EndTime.HasValue)
            {
                var room = await _db.Rooms
                    .Include(x => x.RoomPriceTiers)
                    .Include(x => x.AgePriceTiers)
                    .FirstOrDefaultAsync(x => x.Id == reservation.RoomId, ct);

                if (room is null)
                {
                    return false;
                }

                var effectivePersons = request.Persons ?? reservation.Persons
                    .Select(x => new ReservationPersonRequest { Age = x.Age })
                    .ToList();

                _db.ReservationPersons.RemoveRange(reservation.Persons);

                ApplyPersonsAndPrice(
                    reservation,
                    room,
                    effectivePersons,
                    reservation.StartTIme,
                    reservation.EndTime);
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int reservationId, CancellationToken ct)
        {
            var affectedRows = await _db.Reservations
                .Where(x => x.Id == reservationId)
                .ExecuteDeleteAsync(ct);

            return affectedRows > 0;
        }


        public async Task<ReservationsWithMetaData> GetAllAsync(ReservationFillter fillter, CancellationToken ct)
        {
            fillter.PerPage = 10;
            fillter.Sort = nameof(Reservation.CreatedAt);
            fillter.SortBy = AutoFilterer.Enums.Sorting.Descending;

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var nextMonthStart = monthStart.AddMonths(1);
            var todayStart = now.Date;
            var nextDayStart = todayStart.AddDays(1);

            var filteredQuery = _db.Reservations.ApplyFilterWithoutPagination(fillter);
            var pagedQuery = _db.Reservations.ApplyFilter(fillter);

            var reservations = await pagedQuery
                .ProjectTo<ReservationResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            var count = await filteredQuery.CountAsync(ct);
            var reservationsCreatedThisMonth = await filteredQuery.CountAsync(x =>
                x.CreatedAt >= monthStart &&
                x.CreatedAt < nextMonthStart, ct);
            var reservationsCreatedToday = await filteredQuery.CountAsync(x =>
                x.CreatedAt >= todayStart &&
                x.CreatedAt < nextDayStart, ct);

            return new ReservationsWithMetaData
            {
                Count = count,
                Reservations = reservations,
                ReservationsCreatedThisMonth = reservationsCreatedThisMonth,
                ReservationsCreatedToday = reservationsCreatedToday
            };
        }

        public async Task<ReservationAvailabilityResponse?> GetAvailabilityAsync(ReservationAvailabilityRequest request, CancellationToken ct)
        {
            var room = await _db.Rooms
                .AsNoTracking()
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.GuidId == request.RoomGUid, ct);

            if (room is null || !request.Month.HasValue || !request.RoomGUid.HasValue)
            {
                return null;
            }

            var monthStart = new DateOnly(request.Month.Value.Year, request.Month.Value.Month, 1);
            var monthEndExclusive = monthStart.AddMonths(1);
            var availableDates = new List<DateOnly>();

            for (var currentDay = monthStart; currentDay < monthEndExclusive; currentDay = currentDay.AddDays(1))
            {
                var nextDay = currentDay.AddDays(1);
                var hasOverlappingReservation = room.Reservations
                    .Any(reservation => reservation.StartTIme < nextDay && reservation.EndTime > currentDay);

                if (!hasOverlappingReservation)
                {
                    availableDates.Add(currentDay);
                }
            }

            return new ReservationAvailabilityResponse
            {
                RoomGUid = request.RoomGUid.Value,
                Month = monthStart,
                AvailableDates = availableDates
            };
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

        public Task<bool> IsPersonCountWithinRoomCapacityAsync(int roomId, int personCount, CancellationToken ct)
        {
            return _db.Rooms.AnyAsync(x =>
                x.Id == roomId &&
                personCount >= x.MinCapacity &&
                personCount <= x.MaxCapacity,
                ct);
        }

        public async Task<bool> IsUpdatedPersonCountWithinRoomCapacityAsync(ReservationUpdateRequest request, CancellationToken ct)
        {
            var reservation = await _db.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ReservationId, ct);

            if (reservation is null)
            {
                return false;
            }

            var effectiveRoomId = request.RoomId ?? reservation.RoomId;
            var effectivePersonCount = request.Persons?.Count ?? reservation.PearsonCount;

            return await IsPersonCountWithinRoomCapacityAsync(effectiveRoomId, effectivePersonCount, ct);
        }

        public async Task<bool> HasValidUpdatedDateRangeAsync(ReservationUpdateRequest request, CancellationToken ct)
        {
            var reservation = await _db.Reservations
                .FirstOrDefaultAsync(x => x.Id == request.ReservationId, ct);

            if (reservation is null)
            {
                return false;
            }

            var effectiveStart = request.StartTIme ?? reservation.StartTIme;
            var effectiveEnd = request.EndTime ?? reservation.EndTime;
            var today = DateOnly.FromDateTime(DateTime.Now);

            return effectiveStart > today && effectiveEnd > effectiveStart;
        }

        public async Task<bool> IsUpdatedReservationRoomAvailableAsync(ReservationUpdateRequest request, CancellationToken ct)
        {
            var reservation = await _db.Reservations
                .FirstOrDefaultAsync(x => x.Id == request.ReservationId, ct);

            if (reservation is null)
            {
                return false;
            }

            var effectiveRoomId = request.RoomId ?? reservation.RoomId;
            var effectiveStart = request.StartTIme ?? reservation.StartTIme;
            var effectiveEnd = request.EndTime ?? reservation.EndTime;

            var hasOverlappingReservation = await _db.Reservations
                .AnyAsync(x =>
                    x.Id != reservation.Id &&
                    x.RoomId == effectiveRoomId &&
                    x.StartTIme < effectiveEnd &&
                    x.EndTime > effectiveStart,
                    ct);

            return !hasOverlappingReservation;
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

        private Task<Room?> GetRoomWithPricingByPublicIdAsync(Guid roomPublicId, CancellationToken ct)
        {
            return _db.Rooms
                .Include(x => x.Reservations)
                .Include(x => x.RoomPriceTiers)
                .Include(x => x.AgePriceTiers)
                .FirstOrDefaultAsync(x => x.GuidId == roomPublicId, ct);
        }

        private static void ApplyPersonsAndPrice(
            Reservation reservation,
            Room room,
            List<ReservationPersonRequest> personRequests,
            DateOnly startTime,
            DateOnly endTime)
        {
            var nights = endTime.DayNumber - startTime.DayNumber;
            var fallbackPricePerNight = GetFallbackPricePerNight(room, personRequests.Count);
            var persons = personRequests.Select(personRequest =>
            {
                var age = personRequest.Age!.Value;
                var ageTier = room.AgePriceTiers?
                    .FirstOrDefault(x => x.AgeRangeLow <= age && x.AgeRangeHigh >= age);

                return new ReservationPerson
                {
                    Age = age,
                    PricePerNight = ageTier?.Price ?? fallbackPricePerNight
                };
            }).ToList();

            reservation.PearsonCount = persons.Count;
            reservation.Persons = persons;
            reservation.TotalPrice = persons.Sum(x => x.PricePerNight) * nights;
        }

        private static decimal GetFallbackPricePerNight(Room room, int personCount)
        {
            var roomPriceTier = room.RoomPriceTiers?
                .FirstOrDefault(x => x.GuestCount == personCount);

            return roomPriceTier?.Price ?? room.Price;
        }


    }
}
