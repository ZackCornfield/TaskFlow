using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;

namespace TaskFlowApi.Extensions;

public static class MigrationExtensions
{
    public static async Task MigrateDbAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
