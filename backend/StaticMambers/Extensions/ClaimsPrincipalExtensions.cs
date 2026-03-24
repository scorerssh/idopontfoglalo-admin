using System.Security.Claims;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.StaticMambers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal? principal)
        {
            var rawUserId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(rawUserId, out var userId) ? userId : null;
        }

        public static UserRole? GetUserRole(this ClaimsPrincipal? principal)
        {
            var rawRole = principal?.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(rawRole, out var role) ? role : null;
        }

        public static bool IsAuthenticated(this ClaimsPrincipal? principal)
        {
            return principal?.Identity?.IsAuthenticated == true;
        }
    }
}
