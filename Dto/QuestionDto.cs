namespace AskSam.Dtos;

public record class QuestionDto(
    Guid? QuestionId,
    Guid ClientGuid,
    bool Answered,
    string Question,
    string Answer,
    string Type,
    DateOnly DateCreated,
    DateOnly? DateAnswered
);
