using AskSam.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

    [HttpGet(Name = "GetQuestions")]
    public IEnumerable<QuestionDto> Get([FromQuery] int[] ids)
    {
        if(ids.Count() == 0)
        {
            // Create empty filter, which will return the full db list
            var filter = Builders<QuestionDto>.Filter.Empty;
            return publicDB.Mongo_DB_Collection.Find(filter).SortBy(question => question.Id).ToList();
        } 
        else 
        {
            List<QuestionDto> collectedQuestionsFromIds = new List<QuestionDto>();
            foreach (int id in ids)
            {
                FilterDefinition<QuestionDto> filter = CreateFilterBy(id);
                List<QuestionDto> foundQuestionList = publicDB.Mongo_DB_Collection.Find(filter).ToList();

                if(foundQuestionList.Count > 0)
                {
                    foreach(QuestionDto question in foundQuestionList)
                    {
                        collectedQuestionsFromIds.Add(question);
                    }
                }
            }
            return collectedQuestionsFromIds.ToList();
        }
    }

    // GET: /questions/{id}
    [HttpGet("{id}", Name = "GetQuestionsById")]
    public IResult GetQuestionById(int id) 
    {
        FilterDefinition<QuestionDto> filter = CreateFilterBy(id);
        // Retrieves the first document that matches the filter
        var question = publicDB.Mongo_DB_Collection.Find(filter).FirstOrDefault();

        return question is null ? Results.NotFound() : Results.Ok(question);        
    }

    //POST: /questions
    [HttpPost(Name = "PostQuestions")]
    public IResult Post(CreateQuestionDto newQuestion)
    {

        // Get a count of all the documents in DB
        long count = publicDB.Mongo_DB_Collection.EstimatedDocumentCount();

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
    public IResult Put(int id, CreateQuestionDto updatedQuestion)
    {

        // Creates a filter for all documents for a matching id
        FilterDefinition<QuestionDto> filter = CreateFilterBy(id);
        
        var oldQuestion = publicDB.Mongo_DB_Collection.Find(filter).First();

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
        FilterDefinition<QuestionDto> filter = CreateFilterBy(id);

        // Deletes the first document that matches the filter
        publicDB.Mongo_DB_Collection.DeleteOne(filter);
        return Results.NoContent();
        
        
    }

    private QuestionDto GetData(QuestionDto _newQuestion)
    {        
        var filter = CreateFilterBy(_newQuestion.Id);

        return publicDB.Mongo_DB_Collection.Find(filter).FirstOrDefault();
    }

    private FilterDefinition<QuestionDto> CreateFilterBy(long id) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.Id, id);
    }
}
