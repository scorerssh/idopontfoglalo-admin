using ApartManBackend;
using ApartManBackend.Models.Mappers;
using ApartManBackend.Repository;
using ApartManBackend.Services;
using ApartManBackend.Services.RoomSpecialPricingRules;
using FluentValidation;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddHttpClient(nameof(RoomCalendarService));
builder.Services.AddScoped<ApartmanService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ResourceAuthService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<RoomSercie>();
builder.Services.AddScoped<RoomPriceTierService>();
builder.Services.AddScoped<RoomSpecialPricingRuleService>();
builder.Services.AddScoped<RoomSpecialPricingRuleSolver>();
builder.Services.AddScoped<IRoomSpecialPricingRuleCalculator, OneNightSurchargeRuleCalculator>();
builder.Services.AddScoped<IRoomSpecialPricingRuleCalculator, WeekendSurchargeRuleCalculator>();
builder.Services.AddScoped<IRoomSpecialPricingRuleCalculator, HolidaySurchargeRuleCalculator>();
builder.Services.AddScoped<IRoomSpecialPricingRuleCalculator, LongStayDiscountRuleCalculator>();
builder.Services.AddScoped<AgePriceTierService>();
builder.Services.AddScoped<RoomCalendarService>();
builder.Services.AddScoped<RoomCalendarRefreshJob>();
builder.Services.AddScoped<UserService>();
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());
builder.Services.AddHangfireServer();


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

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new
            {
                field = NormalizeFieldName(entry.Key),
                error = TranslateModelBindingError(entry.Key, error.ErrorMessage)
            }))
            .ToList();

        return new BadRequestObjectResult(errors);
    };
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
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAdminAuthorizationFilter()]
});

app.MapControllers();

RecurringJob.AddOrUpdate<RoomCalendarRefreshJob>(
    "refresh-room-calendars",
    job => job.RefreshAllCalendarsAsync(),
    "*/10 * * * *");

app.Run();

static string NormalizeFieldName(string fieldName)
{
    if (string.Equals(fieldName, "request", StringComparison.OrdinalIgnoreCase))
    {
        return "request";
    }

    if (fieldName.StartsWith("$.", StringComparison.Ordinal))
    {
        return fieldName[2..];
    }

    return fieldName;
}

static string TranslateModelBindingError(string fieldName, string errorMessage)
{
    var normalizedFieldName = NormalizeFieldName(fieldName);

    if (string.Equals(normalizedFieldName, "request", StringComparison.OrdinalIgnoreCase))
    {
        return "A kérés törzse hiányzik vagy nem feldolgozható.";
    }

    if (errorMessage.Contains("could not be converted to System.Nullable`1[System.DateOnly]", StringComparison.OrdinalIgnoreCase) ||
        errorMessage.Contains("could not be converted to System.DateOnly", StringComparison.OrdinalIgnoreCase))
    {
        return $"A(z) '{normalizedFieldName}' mező érvénytelen dátum. Használd a 'yyyy-MM-dd' formátumot.";
    }

    return errorMessage;
}
