using AskSam.Dtos;
using AskSam_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;


namespace AskSam_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{

    // private readonly Public_DB publicDB;
    private readonly IDatabase _database;

    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(ILogger<QuestionsController> logger, IDatabase database)
    {
        _logger = logger;
        // publicDB = mongoDBs;
        _database = database;
    }

    [HttpGet("", Name = "GetQuestions")]
    public IResult Get() 
    {
        //Clients need to send their guid with the GET request.
        return Results.Problem("No client guid found, please include this within GET request, like so: /questions/7ac79c82-b01b-46de-af5c-3d7db4bfeeaf");
    }

    [HttpGet("{guid}", Name = "GetAllClientIdQuestions")]
    public IResult GetAllClientIdQuestions(Guid guid)
    {
        // FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByClientId(guid); 
        // List<QuestionDto> questions = publicDB.Mongo_DB_Question_Collection.Find(filter).SortBy(question => question.Id).ToList();
        List<QuestionDto> questions = _database.FindAllByClientId(guid.ToString());
        return Results.Ok(questions);
    }

    [HttpGet("allquestions", Name = "GetAllQuestions")]
    public IEnumerable<QuestionDto> GetAllQuestions()
    {
        return _database.FindAll();
        // Create empty filter, which will return the full db list
        // var filter = Builders<QuestionDto>.Filter.Empty;
        // return publicDB.Mongo_DB_Question_Collection.Find(filter).SortBy(question => question.Id).ToList();
    }

    // GET: /questions/{id}
    [HttpGet("{guid}/{questionId}", Name = "GetQuestionsById")]
    public IResult GetQuestionByClientAndId(Guid guid, int questionId) 
    {
    
        // FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByClientId(guid); 
       
        // Retrieves the first document that matches the filter
        // var question = publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefault();
        
        QuestionDto? question = _database.FindFirst(guid.ToString());

        

        return question is null ? Results.NotFound() : Results.Ok(question);        
    }

    [HttpGet("getclientid", Name = "GetClientID")]
    public IResult GetClientId() 
    {
        // Generate a random guid and send back to the client
        // Client should request this only if there isn't one already in cookies
        Guid? _guid = GenerateNewRandomGuid();
        if(_guid != null) 
        {
            return Results.Ok(_guid);
        } 
        else 
        {
            return Results.Problem("Generating a new guid has failed, please try again");
        }
    }

    private Guid? GenerateNewRandomGuid() 
    {
        //Create new guid
        IRandomizerGuid randomizerGuid = RandomizerFactory.GetRandomizer(new FieldOptionsGuid());
        return randomizerGuid.Generate();
    }



    //POST: /questions
    [HttpPost(Name = "PostQuestions")]
    public IResult Post(CreateQuestionDto newQuestion)
    {

        // Get new random guid
        Guid? guid = GenerateNewRandomGuid();
        

        QuestionDto _newQuestion = new QuestionDto(
            guid.ToString(),
            newQuestion.ClientGuid,
            newQuestion.Answered,
            newQuestion.Question,
            newQuestion.Answer,
            newQuestion.Type,
            DateOnly.FromDateTime(DateTime.Now),
            null
        );

        //Insert into DB
        // publicDB.Mongo_DB_Question_Collection.InsertOne(_newQuestion);
        
        //Find in DB to check it has ben inserted correctly
        // QuestionDto retrievedQuestion = GetData(_newQuestion);

        //Database API
        QuestionDto retrievedQuestion = _database.Insert(_newQuestion);
        
        if(retrievedQuestion != null) 
        {
            // return Results.CreatedAtRoute("GetQuestions", new {id = _newQuestion.Id}, _newQuestion);
            //Database API
            return Results.CreatedAtRoute("GetQuestions", new {id = retrievedQuestion.Id}, retrievedQuestion);
        }
        else 
        {
            return Results.NoContent();
        }

    }

    //PUT: /questions/{id}
    [HttpPut("{id}", Name = "UpdateQuestions")]
    public IResult Put(Guid? id, [FromBody] UpdateQuestionDto updatedQuestion)
    {

        // Creates a filter for all documents for a matching id
        // Filtering by Id is fine here, because it'll be the private frontend,
        // which will not have a client Id
        
        var oldQuestion = _database.FindFirst(id.ToString());

        if(oldQuestion != null) 
        {
            DateOnly dateCreated = oldQuestion.DateCreated;
            string? questionId = oldQuestion?.Id?.ToString();
            string? clientGuid = oldQuestion?.ClientGuid?.ToString();

            QuestionDto _updatedQuestion = new QuestionDto(
                questionId,
                clientGuid,
                updatedQuestion.Answered,
                updatedQuestion.Question,
                updatedQuestion.Answer,
                updatedQuestion.Type,
                dateCreated,
                DateOnly.FromDateTime(DateTime.Now)
            );

            // publicDB.Mongo_DB_Question_Collection.ReplaceOne(filter, _updatedQuestion);
            _database.Replace(questionId, _updatedQuestion);

            return Results.NoContent();
        } 
        else 
        {
            return Results.NotFound();
        }
        
    }

    //DELETE: /questions/{id}
    [HttpDelete("{id}", Name = "DeleteQuestions")]
    public IResult Delete(Guid? id)
    {
        // publicDB.Mongo_DB_Question_Collection.DeleteOne(filter);
        _database.DeleteOne(id.ToString());
        return Results.NoContent();
    }

    
}
