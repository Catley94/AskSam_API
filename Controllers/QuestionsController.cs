using EmptyDotNetWebAPI2.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZstdSharp.Unsafe;


namespace AskSam_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{

    private readonly Public_DB _options;

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

    private bool useMongoDB = true;

    public QuestionsController(ILogger<QuestionsController> logger, Public_DB options)
    {
        _logger = logger;
        _options = options;
    }

    [HttpGet(Name = "GetQuestions")]
    public IEnumerable<QuestionDto> Get()
    {
        if(useMongoDB) 
        {
            var filter = Builders<QuestionDto>.Filter.Empty;
            return _options.Mongo_DB_Collection.Find(filter)
                                        .ToList();
        } 
        else
        {
            return questions.ToArray();
        }
    }

    // GET: /Questions/{id}
    [HttpGet("{id}", Name = "GetQuestionById")]
    public IResult GetQuestionById(int id) 
    {
        if(useMongoDB) 
        {
           // Creates a filter for all documents that have a "name" value of "Bagels N Buns"
            var filter = Builders<QuestionDto>.Filter
                .Eq(question => question.Id, id);
            // Retrieves the first document that matches the filter
            var question = _options.Mongo_DB_Collection.Find(filter).FirstOrDefault();

            return question is null ? Results.NotFound() : Results.Ok(question);

        }
        else 
        {

            QuestionDto? question = questions.Find(question => question.Id == id);

            return question is null ? Results.NotFound() : Results.Ok(question);
        }
    }

    [HttpPost(Name = "PostQuestions")]
    public IResult Post(CreateQuestionDto newQuestion)
    {
        
        
        

        if(useMongoDB)
        {

            //TEMP!! TODO: Do not find by question, find by ID.
            // var filter = Builders<QuestionDto>.Filter
            //                                 .Eq(question => question.Question, _newQuestion.Question);
            // var count = _options.Mongo_DB_Collection.Find(filter).CountDocuments();
            
            // Creates a filter for all documents that have a "cuisine" value of "Pizza"
            long count = _options.Mongo_DB_Collection.EstimatedDocumentCount();
            QuestionDto _newQuestion = new QuestionDto(
                count,
                newQuestion.Answered,
                newQuestion.Question,
                newQuestion.Answer,
                newQuestion.Type,
                DateOnly.FromDateTime(DateTime.Now),
                null
            );

            //Insert into DB
            _options.Mongo_DB_Collection.InsertOne(_newQuestion);
            
            //Find in DB to check it has ben inserted correctly
            QuestionDto retrievedQuestion = GetData(_newQuestion);
            

            Console.WriteLine(retrievedQuestion);


            return Results.CreatedAtRoute("GetQuestions", new {id = _newQuestion.Id}, _newQuestion);

        }
        else 
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
    }

    [HttpPut("{id}", Name = "UpdateQuestions")]
    public IResult Put(int id, CreateQuestionDto updatedQuestion)
    {

        if(useMongoDB)
        {

            // Creates a filter for all documents with a "name" of "Bagels N Buns"
            var filter = Builders<QuestionDto>.Filter
                .Eq(question => question.Id, id);
            

            var oldQuestion = _options.Mongo_DB_Collection.Find(filter).First();

            if(oldQuestion != null) 
            {
                DateOnly dateCreated = oldQuestion.DateCreated;
                QuestionDto _updatedQuestion = new QuestionDto(
                    id,
                    updatedQuestion.Answered,
                    updatedQuestion.Question,
                    updatedQuestion.Answer,
                    updatedQuestion.Type,
                    dateCreated,
                    DateOnly.FromDateTime(DateTime.Now)
                );

                _options.Mongo_DB_Collection.ReplaceOne(filter, _updatedQuestion);
                
                return Results.NoContent();
            } 
            else 
            {
                return Results.NotFound();
            }
        } 
        else
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
    }

    [HttpDelete("{id}", Name = "DeleteQuestions")]
    public IResult Delete(int id)
    {
        if(useMongoDB)
        {
            //Placeholder
            questions.RemoveAll(question => question.Id == id);

            return Results.NoContent();
        }
        else 
        {
            questions.RemoveAll(question => question.Id == id);

            return Results.NoContent();
        }
    }

    private QuestionDto GetData(QuestionDto _newQuestion)
    {        
        var filter = Builders<QuestionDto>.Filter
                                        .Eq(question => question.Id, _newQuestion.Id);

        return _options.Mongo_DB_Collection.Find(filter).FirstOrDefault();
    }
}
