namespace AskSam.Dtos;

public record class QuestionDto(
    long Id,
    Guid Guid,
    bool Answered,
    string Question,
    string Answer,
    string Type,
    DateOnly DateCreated,
    DateOnly? DateAnswered
);
