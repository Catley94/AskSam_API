using AskSam.Dtos;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;


namespace AskSam_API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{

    /*
        TODO: Create additional database to hold all guids,
            check new guids against all documents in DB so not to create a duplicate.
        TODO-BUG: Currently we create the IDs of questions with the count of the documents.
            However when I delete a question, and create another one,
            the new ID it will create is the count of the db, which will already have been taken
            by the previous question. 
            Generate random guid for each Question
    */
    private readonly Public_DB publicDB;

    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(ILogger<QuestionsController> logger, Public_DB mongoDBs)
    {
        _logger = logger;
        publicDB = mongoDBs;
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
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByClientId(guid); 
        List<QuestionDto> questions = publicDB.Mongo_DB_Question_Collection.Find(filter).SortBy(question => question.QuestionId).ToList();
        return Results.Ok(questions);
    }

    [HttpGet("allquestions", Name = "GetAllQuestions")]
    public IEnumerable<QuestionDto> GetAllQuestions()
    {
        // Create empty filter, which will return the full db list
        var filter = Builders<QuestionDto>.Filter.Empty;
        return publicDB.Mongo_DB_Question_Collection.Find(filter).SortBy(question => question.QuestionId).ToList();
    }

    // GET: /questions/{id}
    [HttpGet("{guid}/{questionId}", Name = "GetQuestionsById")]
    public IResult GetQuestionByClientAndId(Guid guid, int questionId) 
    {
        //TODO: Should filter out only questions matching guid and question id
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByClientId(guid); 
        // Retrieves the first document that matches the filter
        var question = publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefault();

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

        // Get a count of all the documents in DB
        long count = publicDB.Mongo_DB_Question_Collection.EstimatedDocumentCount();
        // Get new random guid
        Guid? guid = GenerateNewRandomGuid();
        

        QuestionDto _newQuestion = new QuestionDto(
            guid,
            newQuestion.ClientGuid,
            newQuestion.Answered,
            newQuestion.Question,
            newQuestion.Answer,
            newQuestion.Type,
            DateOnly.FromDateTime(DateTime.Now),
            null
        );

        //Insert into DB
        publicDB.Mongo_DB_Question_Collection.InsertOne(_newQuestion);
        
        //Find in DB to check it has ben inserted correctly
        QuestionDto retrievedQuestion = GetData(_newQuestion);
        
        if(retrievedQuestion != null) 
        {
            return Results.CreatedAtRoute("GetQuestions", new {id = _newQuestion.QuestionId}, _newQuestion);
        }
        else 
        {
            return Results.NoContent();
        }

    }

    //PUT: /questions/{id}
    [HttpPut("{id}", Name = "UpdateQuestions")]
    public IResult Put(Guid? id, UpdateQuestionDto updatedQuestion)
    {

        // Creates a filter for all documents for a matching id
        // Filtering by Id is fine here, because it'll be the private frontend,
        // which will not have a client Id
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(id);
        
        var oldQuestion = publicDB.Mongo_DB_Question_Collection.Find(filter).First();

        if(oldQuestion != null) 
        {
            DateOnly dateCreated = oldQuestion.DateCreated;
            Guid? questionGuid = oldQuestion.QuestionId;
            Guid clientGuid = oldQuestion.ClientGuid;

            QuestionDto _updatedQuestion = new QuestionDto(
                questionGuid,
                clientGuid,
                updatedQuestion.Answered,
                updatedQuestion.Question,
                updatedQuestion.Answer,
                updatedQuestion.Type,
                dateCreated,
                DateOnly.FromDateTime(DateTime.Now)
            );

            publicDB.Mongo_DB_Question_Collection.ReplaceOne(filter, _updatedQuestion);
            
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
    
        // Creates a filter for all documents for a matching id
        // This is also OK to filter by ID, since it won't be the public front end deleting questions
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(id);

        // Deletes the first document that matches the filter
        publicDB.Mongo_DB_Question_Collection.DeleteOne(filter);
        return Results.NoContent();
        
        
    }

    private QuestionDto GetData(QuestionDto _newQuestion)
    {        
        var filter = CreateQuestionDTOFilterByQuestionId(_newQuestion.QuestionId);

        return publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefault();
    }

    private FilterDefinition<QuestionDto> CreateQuestionDTOFilterByQuestionId(Guid? id) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.QuestionId, id);
    }

    private FilterDefinition<QuestionDto> CreateQuestionDTOFilterByClientId(Guid guid) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.ClientGuid, guid);
    }
}
