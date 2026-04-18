using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.RoomSpecialPricingRule;
using ApartManBackend.ResponseModel.RoomSpecialPricingRule;
using ApartManBackend.StaticMambers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services
{
    public class RoomSpecialPricingRuleService
    {
        private readonly ApartmanDbContext _db;
        private readonly IMapper _mapper;

        public RoomSpecialPricingRuleService(ApartmanDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public Task<bool> CheckRoomSpecialPricingRuleExistsAsync(int roomSpecialPricingRuleId, CancellationToken ct)
        {
            return _db.RoomSpecialPricingRules.AnyAsync(x => x.Id == roomSpecialPricingRuleId, ct);
        }

        public List<RoomSpecialPricingRuleTypeResponse> GetRuleTypes()
        {
            return Enum.GetValues<SpecialPricingRuleType>()
                .Select(ruleType => new RoomSpecialPricingRuleTypeResponse
                {
                    RuleType = ruleType,
                    Description = SpecialPricingRuleDescriptions.GetDescription(ruleType)
                })
                .ToList();
        }

        public async Task<RoomSpecialPricingRuleResponse> CreateAsync(RoomSpecialPricingRuleCreateRequest request, CancellationToken ct)
        {
            var rule = _mapper.Map<RoomSpecialPricingRule>(request);

            await _db.RoomSpecialPricingRules.AddAsync(rule, ct);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<RoomSpecialPricingRuleResponse>(rule);
        }

        public async Task<RoomSpecialPricingRuleResponse?> UpdateAsync(RoomSpecialPricingRuleUpdateRequest request, CancellationToken ct)
        {
            var rule = await _db.RoomSpecialPricingRules
                .FirstOrDefaultAsync(x => x.Id == request.RoomSpecialPricingRuleId, ct);

            if (rule is null)
            {
                return null;
            }

            _mapper.Map(request, rule);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<RoomSpecialPricingRuleResponse>(rule);
        }

        public async Task<bool> DeleteAsync(int roomSpecialPricingRuleId, CancellationToken ct)
        {
            var affectedRows = await _db.RoomSpecialPricingRules
                .Where(x => x.Id == roomSpecialPricingRuleId)
                .ExecuteDeleteAsync(ct);

            return affectedRows > 0;
        }

        public Task<RoomSpecialPricingRuleResponse?> GetAsync(int roomSpecialPricingRuleId, CancellationToken ct)
        {
            return _db.RoomSpecialPricingRules
                .AsNoTracking()
                .Where(x => x.Id == roomSpecialPricingRuleId)
                .ProjectTo<RoomSpecialPricingRuleResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public Task<List<RoomSpecialPricingRuleResponse>> GetAllByRoomAsync(int roomId, CancellationToken ct)
        {
            return _db.RoomSpecialPricingRules
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Id)
                .ProjectTo<RoomSpecialPricingRuleResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }
    }
}
