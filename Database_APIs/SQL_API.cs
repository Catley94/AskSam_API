using AskSam.Dtos;
using AskSam_API.Data;
using AskSam_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AskSam_API.Database_APIs;

public class SQL_API : IDatabase
{
    private string _connectionString;

    public SQL_API(string? connectionString)
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            _connectionString = connectionString;
        }
        else
        {
            Console.WriteLine("Warning: Connection string is empty, there is no connection to a DB.");
        }
    }

    private AskSamContext? CreateContext()
    {
        if(!string.IsNullOrEmpty(_connectionString))
        {
            var optionsBuilder = new DbContextOptionsBuilder<AskSamContext>();
            optionsBuilder.UseSqlServer(_connectionString, options => options.EnableRetryOnFailure());
            return new AskSamContext(optionsBuilder.Options);
        } 
        else
        {
            return null;
        }
    }


    public async Task DeleteOne(string? questionId)
    {
        using (var context = CreateContext())
        {
            if (context != null && questionId != null)
            {
                // Find the question entity with the provided questionId
                var questionToDelete = context.Questions.Find(questionId);

                if (questionToDelete != null)
                {
                    // Remove the question entity
                    context.Questions.Remove(questionToDelete);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // Handle the case when the question with the specified ID is not found
                    throw new InvalidOperationException("Question not found.");
                }
            }
            else
            {
                // Handle the case when context is null or questionId is null
                throw new InvalidOperationException("Database context is not initialized or questionId is null.");
            }
        }
    }

    public async Task<List<QuestionDto>> FindAll()
    {
        using (var context = CreateContext())
        {
            if(context != null)
            {
                return await context.Questions.ToListAsync();
            }
            else
            {
                return new List<QuestionDto>();
            }
        }

    }

    public async Task<List<QuestionDto>> FindAllByClientId(string? clientId)
    {
        using (var context = CreateContext())
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                if(context != null)
                {
                    return await context.Questions
                        .Where(q => q.ClientGuid == clientId)
                        .ToListAsync();
                }
                else
                {
                    return new List<QuestionDto>();
                }
            }
            else
            {
                // Handle the case when clientId is empty
                return new List<QuestionDto>();
            }
        }
    }

    public async Task<QuestionDto?> FindFirst(string? questionId)
    {
        using (var context = CreateContext())
        {
            if (!string.IsNullOrEmpty(questionId))
            {
                return await context.Questions
                    .FirstOrDefaultAsync(q => q.Id == questionId);
            }
            else
            {
                // Handle the case when questionId is empty
                return null;
            }
        }
    }

    public async Task<QuestionDto?> Insert(QuestionDto question)
    {
        using (var context = CreateContext())
        {
            if (context != null)
            {
                context.Questions.Add(question);
                await context.SaveChangesAsync();
                return await context.Questions.FindAsync(question);
                
            }
            else
            {
                // Handle the case when context is null
                throw new InvalidOperationException("Database context is not initialized.");
            }
        }

    }

    public async Task<QuestionDto?> Replace(string? questionId, QuestionDto updatedQuestion)
    {
        using (var context = CreateContext())
        {
            if (context != null)
            {
                // Find the existing question by its ID
                var existingQuestion = context.Questions.FirstOrDefault(q => q.Id == questionId);

                if (existingQuestion != null)
                {
                    context.Questions.Entry(existingQuestion).CurrentValues.SetValues(updatedQuestion);

                    // Save changes to the database
                    await context.SaveChangesAsync();
                    return await context.Questions.FindAsync(updatedQuestion);
                }
                else
                {
                    // Handle the case when the question with the specified ID is not found
                    throw new InvalidOperationException($"Question with ID {questionId} not found.");
                }
            }
            else
            {
                // Handle the case when context is null
                throw new InvalidOperationException("Database context is not initialized.");
            }
        }
    }

    public async Task<long> TotalCount()
    {
        using (var context = CreateContext())
        {
            if (context != null)
            {
                // Count the total number of records in the Questions table
                return await context.Questions.LongCountAsync();
            }
            else
            {
                // Handle the case when context is null
                throw new InvalidOperationException("Database context is not initialized.");
            }
        }
    }
}
