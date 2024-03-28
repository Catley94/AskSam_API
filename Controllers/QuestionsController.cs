using EmptyDotNetWebAPI2.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AskSam_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{
    private static readonly List<QuestionDto> questions = 
    [
        new (
            0,
            true,
            "What is my question?",
            "No idea!",
            "General",
            new DateOnly(2024,03,27),
            null
        ),
        new (
            1,
            false,
            "Is this empty?",
            "",
            "General",
            new DateOnly(2024,03,27),
            null
        )
    ];

    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(ILogger<QuestionsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetQuestions")]
    public IEnumerable<QuestionDto> Get()
    {
        return questions.ToArray();
    }

    // GET: /Questions/{id}
    [HttpGet("{id}", Name = "GetQuestionById")]
    public IResult GetQuestionById(int id) 
    {
        QuestionDto? question = questions.Find(question => question.Id == id);

            return question is null ? Results.NotFound() : Results.Ok(question);
    }

    [HttpPost(Name = "PostQuestions")]
    public IResult Post(CreateQuestionDto newQuestion)
    {
        QuestionDto _newQuestion = new QuestionDto(
                questions.Count(),
                newQuestion.Answered,
                newQuestion.Question,
                newQuestion.Answer,
                newQuestion.Type,
                DateOnly.FromDateTime(DateTime.Now),
                null
            );

            questions.Add(_newQuestion);

            int foundIndex = questions.FindIndex(question => question == _newQuestion);

            if(foundIndex > -1) 
            {
                return Results.CreatedAtRoute("GetQuestions", new {id = _newQuestion.Id}, _newQuestion);
            } 
            else
            {
                //Check this is the right code
                return Results.NoContent();
            } 
    }

    [HttpPut("{id}", Name = "UpdateQuestions")]
    public IResult Put(int id, CreateQuestionDto updatedQuestion)
    {
        var index = questions.FindIndex((question) => question.Id == id);

            if(index == -1) 
            {
                return Results.NotFound();
            }

            DateOnly dateCreated = questions[index].DateCreated;
            questions[index] = new QuestionDto(
                index,
                updatedQuestion.Answered,
                updatedQuestion.Question,
                updatedQuestion.Answer,
                updatedQuestion.Type,
                dateCreated,
                DateOnly.FromDateTime(DateTime.Now)
            );

            return Results.NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteQuestions")]
    public IResult Delete(int id)
    {
        questions.RemoveAll(question => question.Id == id);

        return Results.NoContent();
    }
}
