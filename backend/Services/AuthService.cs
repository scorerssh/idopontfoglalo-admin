using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.RequestModels.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace ApartManBackend.Services
{
    public class AuthService
    {
        private readonly ApartmanDbContext _db;
        private readonly ILogger<AuthService> _logger;
        private readonly IMemoryCache _cache;

        public AuthService(ApartmanDbContext db, ILogger<AuthService> logger, IMemoryCache cache)
        {
            _db = db;
            _logger = logger;
            _cache = cache;
        }

  


        public async Task<(bool Susscess,User? User)> ValidateUserCredentialsAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserEmail == request.Email, ct);
            if (user == null)
            {
                return (false,null);
            }
            var resault = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (resault)
            {
             
                return (true,user);
            }
            return (false,null);
           
          
        }

        public async Task<ClaimsPrincipal> GenerateClaimsForTheUserAsync(User user, CancellationToken ct)
        {
         
           
            user.AuthGuid = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("AuthGuid", user.AuthGuid.ToString()) 
            };
            var identity = new ClaimsIdentity(claims, "Custom");
            CacheUserIdAndAuthGuid(user.Id, user.AuthGuid);
            await _db.SaveChangesAsync(ct);
            return new ClaimsPrincipal(identity);
        }

        public async Task<bool> ValidateAuthGuidForUserIdAsync(int userId, Guid authGuid, CancellationToken ct)
        {
            var cachedAuthGuid = GetAuthGuidForUserId(userId);
            if (cachedAuthGuid.HasValue && cachedAuthGuid.Value == authGuid)
            {
                return true;
            }
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return false;
            }
            if (user.AuthGuid == authGuid)
            {
                CacheUserIdAndAuthGuid(userId, authGuid);
                return true;
            }
            return false;
        }

        private void CacheUserIdAndAuthGuid(int userId, Guid authGuid)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            _cache.Set(userId, authGuid, cacheEntryOptions);

        }

        public Guid? GetAuthGuidForUserId(int userId)
        {
            if (_cache.TryGetValue(userId, out Guid authGuid))
            {
                return authGuid;
            }
            return null;
        }

        public void RemoveAuthGuidForUserId(int userId)
        {
            _cache.Remove(userId);
        }

        public async Task<bool> InvalidateUserSessionAsync(int userId, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return false;
            }

            user.AuthGuid = Guid.NewGuid();
            RemoveAuthGuidForUserId(userId);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
