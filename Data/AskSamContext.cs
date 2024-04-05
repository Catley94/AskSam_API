using Microsoft.EntityFrameworkCore;

namespace AskSam_API.Data;

public class AskSamContext(DbContextOptions<AskSamContext> options) : DbContext(options)
{
    public DbSet<QuestionEntity> Questions => Set<QuestionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<QuestionEntity>().HasData(
        //     new {Id}
        // )
    }
}

