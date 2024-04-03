using AskSam.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using ZstdSharp.Unsafe;


namespace AskSam_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{

    private readonly Public_DB publicDB;

    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(ILogger<QuestionsController> logger, Public_DB options)
    {
        _logger = logger;
        publicDB = options;
    }

    [HttpGet("", Name = "GetQuestions")]
    public IResult Get() 
    {
        //Clients need to send their guid with the GET request.
        return Results.Problem("No client guid found, please include this within GET request, like so: /questions/7ac79c82-b01b-46de-af5c-3d7db4bfeeaf");
    }

    [HttpGet("allquestions", Name = "GetAllQuestions")]
    public IEnumerable<QuestionDto> GetAllQuestions()
    {
        // Create empty filter, which will return the full db list
        var filter = Builders<QuestionDto>.Filter.Empty;
        return publicDB.Mongo_DB_Collection.Find(filter).SortBy(question => question.Id).ToList();
    }

    [HttpGet("{guid}", Name = "GetAllClientIdQuestions")]
    public IResult GetAllClientIdQuestions(Guid guid)
    {
        FilterDefinition<QuestionDto> filter = CreateFilterByClientId(guid); 
        List<QuestionDto> questions = publicDB.Mongo_DB_Collection.Find(filter).SortBy(question => question.Id).ToList();
        return Results.Ok(questions);
    }

    // GET: /questions/{id}
    [HttpGet("{clientId}/{questionId}", Name = "GetQuestionsById")]
    public IResult GetQuestionByClientAndId(Guid guid, int questionId) 
    {
        //TODO: Should filter out only questions matching guid and question id
        FilterDefinition<QuestionDto> filter = CreateFilterByClientId(guid); 
        // Retrieves the first document that matches the filter
        var question = publicDB.Mongo_DB_Collection.Find(filter).FirstOrDefault();

        return question is null ? Results.NotFound() : Results.Ok(question);        
    }

    [HttpGet("getclientid", Name = "GetClientID")]
    public IResult GetClientId() {
        // Generate a random guid and send back to the client
        // Client should request this only if there isn't one already in cookies
        IRandomizerGuid randomizerGuid = RandomizerFactory.GetRandomizer(new FieldOptionsGuid());
        Guid? _guid = randomizerGuid.Generate();
        return Results.Ok(_guid);
    }



    //POST: /questions
    [HttpPost(Name = "PostQuestions")]
    public IResult Post(CreateQuestionDto newQuestion)
    {

        // Get a count of all the documents in DB
        long count = publicDB.Mongo_DB_Collection.EstimatedDocumentCount();

        QuestionDto _newQuestion = new QuestionDto(
            count,
            newQuestion.Guid,
            newQuestion.Answered,
            newQuestion.Question,
            newQuestion.Answer,
            newQuestion.Type,
            DateOnly.FromDateTime(DateTime.Now),
            null
        );

        //Insert into DB
        publicDB.Mongo_DB_Collection.InsertOne(_newQuestion);
        
        //Find in DB to check it has ben inserted correctly
        QuestionDto retrievedQuestion = GetData(_newQuestion);
        
        if(retrievedQuestion != null) 
        {
            return Results.CreatedAtRoute("GetQuestions", new {id = _newQuestion.Id}, _newQuestion);
        }
        else 
        {
            return Results.NoContent();
        }

    }

    //PUT: /questions/{id}
    [HttpPut("{id}", Name = "UpdateQuestions")]
    public IResult Put(int id, UpdateQuestionDto updatedQuestion)
    {

        // Creates a filter for all documents for a matching id
        // Filtering by Id is fine here, because it'll be the private frontend,
        // which will not have a client Id
        FilterDefinition<QuestionDto> filter = CreateFilterByQuestionId(id);
        
        var oldQuestion = publicDB.Mongo_DB_Collection.Find(filter).First();

        if(oldQuestion != null) 
        {
            DateOnly dateCreated = oldQuestion.DateCreated;
            Guid guid = oldQuestion.Guid;

            QuestionDto _updatedQuestion = new QuestionDto(
                id,
                guid,
                updatedQuestion.Answered,
                updatedQuestion.Question,
                updatedQuestion.Answer,
                updatedQuestion.Type,
                dateCreated,
                DateOnly.FromDateTime(DateTime.Now)
            );

            publicDB.Mongo_DB_Collection.ReplaceOne(filter, _updatedQuestion);
            
            return Results.NoContent();
        } 
        else 
        {
            return Results.NotFound();
        }
        
    }

    //DELETE: /questions/{id}
    [HttpDelete("{id}", Name = "DeleteQuestions")]
    public IResult Delete(int id)
    {
    
        // Creates a filter for all documents for a matching id
        // This is also OK to filter by ID, since it won't be the public front end deleting questions
        FilterDefinition<QuestionDto> filter = CreateFilterByQuestionId(id);

        // Deletes the first document that matches the filter
        publicDB.Mongo_DB_Collection.DeleteOne(filter);
        return Results.NoContent();
        
        
    }

    private QuestionDto GetData(QuestionDto _newQuestion)
    {        
        var filter = CreateFilterByQuestionId(_newQuestion.Id);

        return publicDB.Mongo_DB_Collection.Find(filter).FirstOrDefault();
    }

    private FilterDefinition<QuestionDto> CreateFilterByQuestionId(long id) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.Id, id);
    }

    private FilterDefinition<QuestionDto> CreateFilterByClientId(Guid guid) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.Guid, guid);
    }
}
