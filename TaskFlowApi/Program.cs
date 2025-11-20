// 1. Define configuration constants
using TaskFlowApi.Data;

var corsConfig = "corsConfig";

// 2. Create the WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// 3. Add services to the DI container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configure database
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<TaskFlowDbContext>(connString);

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
app.UseAuthorization();
app.MapControllers();

// 8. Run database migrations
//await app.MigrateDbAsync();

// 9. Start the application
app.Run();
