namespace AskSam.Dtos;

public record class QuestionDto(
    string? Id,
    string? ClientGuid,
    bool Answered,
    string Question,
    string Answer,
    string Type,
    DateOnly DateCreated,
    DateOnly? DateAnswered
);
