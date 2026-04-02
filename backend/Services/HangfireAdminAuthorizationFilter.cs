using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace ApartManBackend.Services
{
    public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var user = httpContext.User;

            return user.Identity?.IsAuthenticated == true && user.IsInRole("Admin");
        }
    }
}
