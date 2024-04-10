using AskSam.Dtos;

namespace AskSam_API.Interfaces;

public interface IDatabase
{
    QuestionDto Insert(QuestionDto question);
    void DeleteOne(string? questionId);
    long TotalCount();
    List<QuestionDto> FindAll();
    List<QuestionDto> FindAllByClientId(string? clientId);
    QuestionDto? FindFirst(string? questionId);
    void Replace(string? questionId, QuestionDto updatedQuestion);
}
