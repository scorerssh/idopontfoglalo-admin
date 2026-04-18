using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using ApartManBackend.StaticMambers.Extensions;
using Microsoft.EntityFrameworkCore;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services
{
    public class ResourceAuthService
    {
        private readonly ApartmanDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResourceAuthService(ApartmanDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckUserAccessForResourceAsync(
            int resourceId,
            ResourceObjectType resourceObjectType,
            CancellationToken ct = default)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (!principal.IsAuthenticated())
            {
                return false;
            }

            if (principal.GetUserRole() == UserRole.Admin)
            {
                return true;
            }

            var userId = principal.GetUserId();
            if (!userId.HasValue)
            {
                return false;
            }

            return resourceObjectType switch
            {
                ResourceObjectType.Apartman => await IsUserOwnApartmanAsync(resourceId, userId.Value, ct),
                ResourceObjectType.Room => await IsUserOwnRoomAsync(resourceId, userId.Value, ct),
                ResourceObjectType.RoomPriceTier => await IsUserOwnRoomPriceTierAsync(resourceId, userId.Value, ct),
                ResourceObjectType.RoomSpecialPricingRule => await IsUserOwnRoomSpecialPricingRuleAsync(resourceId, userId.Value, ct),
                ResourceObjectType.AgePriceTier => await IsUserOwnAgePriceTierAsync(resourceId, userId.Value, ct),
                ResourceObjectType.Reservation => await IsUserOwnReservationAsync(resourceId, userId.Value, ct),
                _ => false
            };
        }

        private Task<bool> IsUserOwnApartmanAsync(int apartmanId, int userId, CancellationToken ct)
        {
            return _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .AnyAsync(a => a.Id == apartmanId, ct);
        }

        private Task<bool> IsUserOwnRoomAsync(int roomId, int userId, CancellationToken ct)
        {
            return _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .AnyAsync(r => r.Id == roomId, ct);
        }

        private Task<bool> IsUserOwnRoomPriceTierAsync(int roomPriceTierId, int userId, CancellationToken ct)
        {
            return _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .SelectMany(r => r.RoomPriceTiers!)
                .AnyAsync(rpt => rpt.Id == roomPriceTierId, ct);
        }

        private Task<bool> IsUserOwnRoomSpecialPricingRuleAsync(int roomSpecialPricingRuleId, int userId, CancellationToken ct)
        {
            return _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .SelectMany(r => r.RoomSpecialPricingRules!)
                .AnyAsync(rspr => rspr.Id == roomSpecialPricingRuleId, ct);
        }

        private Task<bool> IsUserOwnAgePriceTierAsync(int agePriceTierId, int userId, CancellationToken ct)
        {
            return _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .SelectMany(r => r.AgePriceTiers!)
                .AnyAsync(apt => apt.Id == agePriceTierId, ct);
        }


        private Task<bool> IsUserOwnReservationAsync(int resId, int userId, CancellationToken ct)
        {
            return _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Apartmans)
                .SelectMany(a => a.Rooms)
                .SelectMany(r => r.Reservations)
                .AnyAsync(r => r.Id == resId, ct);
        }
    }
}
