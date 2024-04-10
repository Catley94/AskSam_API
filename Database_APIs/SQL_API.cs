using AskSam.Dtos;
using AskSam_API.Interfaces;

namespace AskSam_API.Database_APIs;

public class SQL_API : IDatabase
{

    private Public_DB? publicDB;
    private string? v;

    public SQL_API(string? connectionString)
    {
        if (connectionString != string.Empty || connectionString != null)
        {
           
        }
        else
        {
            Console.WriteLine("Warning: Connection string is empty, there is no connection to a DB.");
        }
    }

    public void DeleteOne(string? questionId)
    {
        throw new NotImplementedException();
    }

    public List<QuestionDto> FindAll()
    {
        throw new NotImplementedException();
    }

    public List<QuestionDto> FindAllByClientId(string? clientId)
    {
        throw new NotImplementedException();
    }

    public QuestionDto? FindFirst(string? questionId)
    {
        throw new NotImplementedException();
    }

    public QuestionDto Insert(QuestionDto question)
    {
        throw new NotImplementedException();
    }

    public void Replace(string? questionId, QuestionDto updatedQuestion)
    {
        throw new NotImplementedException();
    }

    public long TotalCount()
    {
        throw new NotImplementedException();
    }
}
