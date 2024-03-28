namespace EmptyDotNetWebAPI2.Dtos;

public record class QuestionDto(
    int Id,
    bool Answered,
    string Question,
    string Answer,
    string Type,
    DateOnly DateCreated,
    DateOnly? DateAnswered
);
