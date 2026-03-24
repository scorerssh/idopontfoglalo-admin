using ApartManBackend;
using ApartManBackend.Models.Mappers;
using ApartManBackend.Repository;
using ApartManBackend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;

const string CookieAuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["Database:ConnectionString"];
var autoMapperLicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
var configuredCorsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>()?
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray() ?? [];
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Add services to the container.
builder.Services.AddDbContextPool<ApartmanDbContext>(options =>
{
    options
        .UseMySQL(connectionString)
        .EnableSensitiveDataLogging(false)
        .EnableDetailedErrors(false)
        .LogTo(_ => { }, LogLevel.None);
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = autoMapperLicenseKey;
}, typeof(UserMapper).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApartmanService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ResourceAuthService>();
builder.Services.AddScoped<RoomSercie>();
builder.Services.AddScoped<UserService>();


builder.Services.AddAuthentication(CookieAuthScheme)
    .AddCookie(CookieAuthScheme, options =>
    {
        options.Cookie.Name = "ApartManAuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;

        options.Events.OnValidatePrincipal = async context =>
        {
            var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
            var claimsPrincipal = context.Principal;
            var authGuidClaim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == "AuthGuid");
            var userIdClaim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (authGuidClaim == null || userIdClaim == null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthScheme);
                return;
            }
            if (!Guid.TryParse(authGuidClaim.Value, out var authGuid) || !int.TryParse(userIdClaim.Value, out var userId))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthScheme);
                return;
            }
            if (!await authService.ValidateAuthGuidForUserIdAsync(userId, authGuid,context.HttpContext.RequestAborted))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthScheme);
                return;
            }
        };


    });

builder.Services.AddAuthorization();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
              {
                  if (string.IsNullOrWhiteSpace(origin) ||
                      !Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                  {
                      return false;
                  }

                  var normalizedOrigin = $"{uri.Scheme}://{uri.Authority}";
                  if (configuredCorsOrigins.Contains(normalizedOrigin, StringComparer.OrdinalIgnoreCase))
                  {
                      return true;
                  }

                  return uri.Scheme == Uri.UriSchemeHttps &&
                         (uri.Host.EndsWith(".trycloudflare.com", StringComparison.OrdinalIgnoreCase) ||
                          uri.Host.EndsWith(".cfargotunnel.com", StringComparison.OrdinalIgnoreCase));
              })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationActionFilter>();
})
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApartmanDbContext>();
    await db.Database.MigrateAsync();

    var seeder = new AdminSeeder(scope.ServiceProvider);
    await seeder.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
