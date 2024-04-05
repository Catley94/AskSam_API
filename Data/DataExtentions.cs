using Microsoft.EntityFrameworkCore;

namespace AskSam_API.Data;

public static class DataExtentions
{
    public static void MigrateDb(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AskSamContext dbContext = scope.ServiceProvider.GetRequiredService<AskSamContext>();
        dbContext.Database.Migrate();
    }
}
