using System.Text;
using System.Threading.RateLimiting;
using LMS.Api.Constants;
using LMS.Api.Data;
using LMS.Api.Data.Seed;
<<<<<<< HEAD
using LMS.Api.Models;
using LMS.Api.Services.Implementations.Auth;
using LMS.Api.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
=======
using LMS.Api.Repositories.Implementations;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
>>>>>>> 64b8332 (Added course controller, service and repo with GET endpoint. Added Scalar.)

var builder = WebApplication.CreateBuilder(args);

string jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
string jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");
string frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:5173";

builder.Services.AddDbContext<LMSDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.MaxDepth = 128;
});

builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<LMSDbContext>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthCookieService, AuthCookieService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(RateLimitConstants.LoginPolicy, context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey:
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = RateLimitConstants.LoginPermitLimit,
                Window = TimeSpan.FromMinutes(RateLimitConstants.LoginWindowMinutes),
                QueueLimit = RateLimitConstants.LoginQueueLimit,
                AutoReplenishment = true
            }
        )
    );
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<ICourserepository, CourseRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LMSDbContext>();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    await context.Database.MigrateAsync();

    await DatabaseSeeder.SeedAsync(
        context,
        userManager,
        roleManager
    );
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

<<<<<<< HEAD
app.UseRouting();

app.UseCors("Frontend");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

=======
>>>>>>> 64b8332 (Added course controller, service and repo with GET endpoint. Added Scalar.)
app.MapControllers();

app.Run();
