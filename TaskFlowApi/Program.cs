// 1. Define configuration constants
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskFlowApi.Data;
using TaskFlowApi.Extensions;
using TaskFlowApi.Helpers;
using TaskFlowApi.Services;

var corsConfig = "corsConfig";

// 2. Create the WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// Bind JWT settings
var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
if (!jwtSettingsSection.Exists())
{
    throw new InvalidOperationException("Jwt configuration section is missing.");
}

var jwtSettings = jwtSettingsSection.Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);

// 3. Add services to the DI container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configure database / Register Services
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<TaskFlowDbContext>(connString);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 5. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: corsConfig,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        }
    );
});

// Authentication
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // set true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

// 6. Build the application
var app = builder.Build();

// 7. Configure middleware pipeline (ORDER MATTERS!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsConfig);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 8. Run database migrations
await app.MigrateDbAsync(); // Ensure database migrations are applied

// 9. Start the application
app.Run();
