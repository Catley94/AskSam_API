using AskSam.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AskSam_API.Data;

public class AskSamContext(DbContextOptions<AskSamContext> options) : DbContext(options)
{
    // public DbSet<QuestionDto> Questions => Set<QuestionDto>();
    public DbSet<QuestionDto> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<QuestionEntity>().HasData(
        //     new {Id}
        // )
    }
}

