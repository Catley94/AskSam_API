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
    public abstract void DeleteOne(Guid? questionId);
    public abstract long TotalCount();
    public abstract List<QuestionDto> FindAll();
    public abstract List<QuestionDto> FindAllByClientId(Guid? clientId);
    public abstract QuestionDto? FindFirst(Guid? questionId);
    public abstract ReplaceOneResult Replace(Guid? questionId, QuestionDto updatedQuestion);

    

}
