using AskSam.Dtos;

namespace AskSam_API;

public class SQL_API : Database
{
    public SQL_API(string? connectionString) : base(connectionString)
    {
        
    }

    public override void DeleteOne(string? questionId)
    {
        throw new NotImplementedException();
    }

    public override List<QuestionDto> FindAll()
    {
        throw new NotImplementedException();
    }

    public override List<QuestionDto> FindAllByClientId(string? clientId)
    {
        throw new NotImplementedException();
    }

    public override QuestionDto? FindFirst(string? questionId)
    {
        throw new NotImplementedException();
    }

    public override QuestionDto Insert(QuestionDto question)
    {
        throw new NotImplementedException();
    }

    public override void Replace(string? questionId, QuestionDto updatedQuestion)
    {
        throw new NotImplementedException();
    }

    public override long TotalCount()
    {
        throw new NotImplementedException();
    }
}
