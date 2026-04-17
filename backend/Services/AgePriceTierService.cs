using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.AgePriceTier;
using ApartManBackend.ResponseModel.AgePriceTier;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class AgePriceTierService
    {
        private readonly ApartmanDbContext _db;
        private readonly IMapper _mapper;

        public AgePriceTierService(ApartmanDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public Task<bool> CheckAgePriceTierExistsAsync(int agePriceTierId, CancellationToken ct)
        {
            return _db.AgePriceTiers.AnyAsync(x => x.Id == agePriceTierId, ct);
        }

        public async Task<AgePriceTierResponse> CreateAsync(AgePriceTierCreateRequest request, CancellationToken ct)
        {
            var agePriceTier = _mapper.Map<AgePriceTier>(request);

            await _db.AgePriceTiers.AddAsync(agePriceTier, ct);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<AgePriceTierResponse>(agePriceTier);
        }

        public async Task<AgePriceTierResponse?> UpdateAsync(AgePriceTierUpdateRequest request, CancellationToken ct)
        {
            var agePriceTier = await _db.AgePriceTiers
                .FirstOrDefaultAsync(x => x.Id == request.AgePriceTierId, ct);

            if (agePriceTier is null)
            {
                return null;
            }

            _mapper.Map(request, agePriceTier);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<AgePriceTierResponse>(agePriceTier);
        }

        public async Task<bool> DeleteAsync(int agePriceTierId, CancellationToken ct)
        {
            var affectedRows = await _db.AgePriceTiers
                .Where(x => x.Id == agePriceTierId)
                .ExecuteDeleteAsync(ct);

            return affectedRows > 0;
        }

        public Task<AgePriceTierResponse?> GetAsync(int agePriceTierId, CancellationToken ct)
        {
            return _db.AgePriceTiers
                .AsNoTracking()
                .Where(x => x.Id == agePriceTierId)
                .ProjectTo<AgePriceTierResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<AgePriceTierResponse>> GetAllByRoomAsync(int roomId, CancellationToken ct)
        {
            return await _db.AgePriceTiers
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.AgeRangeLow)
                .ThenBy(x => x.AgeRangeHigh)
                .ProjectTo<AgePriceTierResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<bool> HasNoAgeRangeOverlapAsync(
            int roomId,
            int ageRangeLow,
            int ageRangeHigh,
            int? excludedAgePriceTierId,
            CancellationToken ct)
        {
            return !await _db.AgePriceTiers
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .Where(x => !excludedAgePriceTierId.HasValue || x.Id != excludedAgePriceTierId.Value)
                .AnyAsync(x => x.AgeRangeLow <= ageRangeHigh && x.AgeRangeHigh >= ageRangeLow, ct);
        }

        public async Task<bool> HasValidUpdatedAgeRangeAsync(AgePriceTierUpdateRequest request, CancellationToken ct)
        {
            var effectiveRange = await GetEffectiveUpdatedRangeAsync(request, ct);
            if (effectiveRange is null)
            {
                return false;
            }

            return effectiveRange.Value.AgeRangeHigh >= effectiveRange.Value.AgeRangeLow;
        }

        public async Task<bool> HasNoAgeRangeOverlapAsync(AgePriceTierUpdateRequest request, CancellationToken ct)
        {
            var effectiveRange = await GetEffectiveUpdatedRangeAsync(request, ct);
            if (effectiveRange is null)
            {
                return false;
            }

            return await HasNoAgeRangeOverlapAsync(
                effectiveRange.Value.RoomId,
                effectiveRange.Value.AgeRangeLow,
                effectiveRange.Value.AgeRangeHigh,
                request.AgePriceTierId,
                ct);
        }

        private async Task<(int RoomId, int AgeRangeLow, int AgeRangeHigh)?> GetEffectiveUpdatedRangeAsync(
            AgePriceTierUpdateRequest request,
            CancellationToken ct)
        {
            var agePriceTier = await _db.AgePriceTiers
                .AsNoTracking()
                .Where(x => x.Id == request.AgePriceTierId)
                .Select(x => new { x.RoomId, x.AgeRangeLow, x.AgeRangeHigh })
                .FirstOrDefaultAsync(ct);

            if (agePriceTier is null)
            {
                return null;
            }

            return (
                request.RoomId ?? agePriceTier.RoomId,
                request.AgeRangeLow ?? agePriceTier.AgeRangeLow,
                request.AgeRangeHigh ?? agePriceTier.AgeRangeHigh);
        }
    }
}
