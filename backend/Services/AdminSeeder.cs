using ApartManBackend.Models.DbModels.Models;
using ApartManBackend.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.Services
{
    public class AdminSeeder
    {
        private readonly IServiceProvider _sp;

        public AdminSeeder(IServiceProvider sp)
        {
            _sp = sp;
        }

        public async Task SeedAsync()
        {
            using var scope = _sp.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApartmanDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var adminSection = config.GetSection("AdminSeed");

            var email = adminSection["Email"];
            var password = adminSection["Password"];
            var fullName = adminSection["FullName"];
            if(string.IsNullOrEmpty(email)||string.IsNullOrEmpty(password)||string.IsNullOrEmpty(fullName))
                {
                throw new Exception("AdminSeed configuration is missing or incomplete.");
            }

            var existing = await db.Users.FirstOrDefaultAsync(x => x.UserEmail == email);
            if (existing != null) return;

            var user = new User
            {
                UserEmail = email,
                UserName = fullName,
                Role = UserRole.Admin
            };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
    }
}
