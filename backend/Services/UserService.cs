using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.User;
using ApartManBackend.ResponseModel.User;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApartManBackend.Services
{
    public class UserService
    {
        private readonly ApartmanDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly AuthService _authService;

        public UserService(ApartmanDbContext db, IMapper mapper, ILogger<UserService> logger, AuthService authService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _authService = authService;
        }


        public async Task CreateAsync(UserCreateRequest request, CancellationToken ct)
        {
            var user = _mapper.Map<User>(request);
            await _db.Users.AddAsync(user,ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> UpdateAsync(UserUpdateRequest request, CancellationToken ct)
        {
            var user = await _db.Users.FindAsync(request.Id);
              

            if (user == null)
            {
                return false;
            }

            _mapper.Map(request, user);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<UserResponse?> GetAsync(int userId, CancellationToken ct)
        {
            return await _db.Users
                .Where(x => x.Id == userId)
                .ProjectTo<UserResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<UserResponse>> GetAllAsync(UserFilter filter, CancellationToken ct)
        {
            filter.PerPage = 10;
            filter.Sort = nameof(User.CreatedAt);
            filter.SortBy = AutoFilterer.Enums.Sorting.Descending;

            return await _db.Users
                .ApplyFilter(filter)
                .ProjectTo<UserResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id,CancellationToken ct)
        {
            var affectedRows = await _db.Users.Where(x => x.Id == id).ExecuteDeleteAsync(ct);
            if(affectedRows>0)
                _authService.RemoveAuthGuidForUserId(id);
            return affectedRows > 0;

        }


        public async Task<bool> CheckUsersExistAsync(IEnumerable<int> userids,CancellationToken ct)
        {
            var distinctIds = userids
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            if (distinctIds.Count == 0)
            {
                return false;
            }
            var existingUserIds = await _db.Users
                .Where(u => distinctIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync(ct);
            return existingUserIds.Count == distinctIds.Count;
        }
    }
}
