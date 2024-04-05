namespace AskSam_API;

public class QuestionEntity
{
    public required string Id { get; set; }
    public required string ClientGuid { get; set; }
    public required bool Answered { get; set; }
    public required string Question { get; set; }
    public string Answer { get; set; }
    public string Type { get; set; }
    public required DateOnly DateCreated { get; set; }
    public DateOnly DateAnswered { get; set; }
}
