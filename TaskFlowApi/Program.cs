// 1. Define configuration constants
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter 'Bearer [space] and then your token in the text input below.\n\nExample: 'Bearer abc123'",
        }
    );
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// 4. Configure database / Register Services
var connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddNpgsql<TaskFlowDbContext>(connString);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IBoardMemberService, BoardMemberService>();
builder.Services.AddScoped<IColumnService, ColumnService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();

// 5. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: corsConfig,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200", "https://taskflow-1-5p62.onrender.com")
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
