using AskSam.Dtos;

namespace AskSam_API.Interfaces;

public interface IDatabase
{
    Task<QuestionDto?> Insert(QuestionDto question);
    Task DeleteOne(string? questionId);
    Task<long> TotalCount();
    Task<List<QuestionDto>> FindAll();
    Task<List<QuestionDto>> FindAllByClientId(string? clientId);
    Task<QuestionDto?> FindFirst(string? questionId);
    Task<QuestionDto?> Replace(string? questionId, QuestionDto updatedQuestion);
}
