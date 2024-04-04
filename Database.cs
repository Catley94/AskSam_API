using AskSam.Dtos;
using MongoDB.Driver;

namespace AskSam_API;

public abstract class Database
{


    protected Public_DB publicDB;

    

    public Database(string? connectionString)
    {
        // publicDB = mongoDBs;
        

    }

    public abstract QuestionDto Insert(QuestionDto question); 
    public abstract void DeleteOne(string? questionId);
    public abstract long TotalCount();
    public abstract List<QuestionDto> FindAll();
    public abstract List<QuestionDto> FindAllByClientId(string? clientId);
    public abstract QuestionDto? FindFirst(string? questionId);
    public abstract ReplaceOneResult Replace(string? questionId, QuestionDto updatedQuestion);

    

}
