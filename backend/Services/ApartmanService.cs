using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.Apartman;
using ApartManBackend.ResponseModel.Apartman;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class ApartmanService
    {
        private readonly ApartmanDbContext _db;
        private readonly ILogger<ApartmanService> _logger;
        private readonly IMapper _mapper;

        public ApartmanService(ApartmanDbContext dbContext, ILogger<ApartmanService> logger, IMapper mapper)
        {
            _db = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task CreateAync(ApartmanCreateRequest request, CancellationToken ct)
        {
            var apartman = _mapper.Map<Models.DbModels.Models.Apartman>(request);
            await _db.Apartmans.AddAsync(apartman, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(ApartmanUpdateRequest request,CancellationToken ct)
        {
            var apartman = await _db.Apartmans
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (apartman == null)
            {
                _logger.LogWarning("Apartman with id {Id} not found for update.", request.Id);
                return;
            }

            _mapper.Map(request, apartman);

            if(request.Users!=null)
                await HandelUsersUpdateForApartmanAsync(apartman,request.Users, ct);

            await _db.SaveChangesAsync(ct);
        }


        private async Task HandelUsersUpdateForApartmanAsync(Apartman apartman,UpdateApartmanUsersSubRequest request,CancellationToken ct)
        {
            await _db.Entry(apartman).Collection(x => x.Users).LoadAsync(ct);
            var userIdsToAdd = request.UserIdsToAdd;
            var userIdsToRemove = request.UserIdsToRemove;
            var currentUserIds = apartman.Users.Select(x => x.Id).ToHashSet();

            if (userIdsToAdd != null)
            {
                var userIdsToActuallyAdd = userIdsToAdd.Where(id => !currentUserIds.Contains(id)).ToList();
                if (userIdsToActuallyAdd.Count > 0)
                {
                    var usersToAdd = await _db.Users.Where(x => userIdsToActuallyAdd.Contains(x.Id)).ToListAsync(ct);
                    foreach (var user in usersToAdd)
                    {
                        apartman.Users.Add(user);
                    }
                }
            }
            if (userIdsToRemove != null)
            {
                var removeUserIds = userIdsToRemove.ToHashSet();
                var usersToRemove = apartman.Users
                    .Where(x => removeUserIds.Contains(x.Id))
                    .ToList();

                foreach (var user in usersToRemove)
                {
                    apartman.Users.Remove(user);
                }
            }
        }







        public Task<bool> CheckApartmanExists(int apartmanId, CancellationToken cancellationToken)
        {
            return _db.Apartmans.AnyAsync(x => x.Id == apartmanId, cancellationToken);
        }

        public async Task<bool> CheckApartmanNameExistAsync(string name)
        {
            return await _db.Apartmans.AnyAsync(x => x.Name == name);
        }


        public Task<bool> IsApartmanNameTakenByAnotherAsync(int apartmanId, string name, CancellationToken cancellationToken)
        {
            return _db.Apartmans.AnyAsync(x => x.Name == name && x.Id != apartmanId, cancellationToken);
        }

        public async Task<bool> CheckApartmansExistAsync(IEnumerable<int> apartmanIds, CancellationToken cancellationToken)
        {
            var distinctIds = apartmanIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (distinctIds.Count == 0)
            {
                return false;
            }

            var existingCount = await _db.Apartmans.AsNoTracking()
                .CountAsync(x => distinctIds.Contains(x.Id), cancellationToken);

            return existingCount == distinctIds.Count;
        }


        public async Task<ApartmanWithRoomsResponse?> GetWithRoomsAsync(int apartmanId, CancellationToken ct)
        {
            return await _db.Apartmans.Where(x => x.Id == apartmanId).ProjectTo<ApartmanWithRoomsResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(ct);
        }

        public async Task<List<ApartmanWithRoomsResponse>> GetAllWithRoomsAsync(ApartmanFillter fillter, CancellationToken ct)
        {
            fillter.PerPage = 10;
            fillter.Sort = nameof(Apartman.CreatedAt);
            fillter.SortBy = AutoFilterer.Enums.Sorting.Descending;

            return await _db.Apartmans.ApplyFilter(fillter).ProjectTo<ApartmanWithRoomsResponse>(_mapper.ConfigurationProvider).ToListAsync(ct);
        }

        public async Task<ApartmanResponse?> GetAsync(int apartmanId, CancellationToken ct)
        {
            return await _db.Apartmans.Where(x => x.Id == apartmanId).ProjectTo<ApartmanResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(ct);
        }

        public async Task<List<ApartmanResponse>> GetAllAsync(ApartmanFillter fillter, CancellationToken ct)
        {
            fillter.PerPage = 10;
            fillter.Sort = nameof(Apartman.CreatedAt);
            fillter.SortBy = AutoFilterer.Enums.Sorting.Descending;

            return await _db.Apartmans.ApplyFilter(fillter).ProjectTo<ApartmanResponse>(_mapper.ConfigurationProvider).ToListAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affectedRows = await _db.Apartmans.Where(x => x.Id == id).ExecuteDeleteAsync(ct);
            return affectedRows > 0;
        }
    }
}
