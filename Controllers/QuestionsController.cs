using AskSam.Dtos;
using AskSam_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;


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
        _database = database;
    }

    [HttpGet("", Name = "GetQuestions")]
    public IResult Get() 
    {
        //Clients need to send their guid with the GET request.
        _logger.LogWarning("GetQuestions: GET request to /questions with no paramter received, this is incorrect behavior, user must GET using client guid, like so: /questions/7ac79c82-b01b-46de-af5c-3d7db4bfeeaf");
        return Results.Problem("No client guid found, please include this within GET request, like so: /questions/7ac79c82-b01b-46de-af5c-3d7db4bfeeaf");
    }

    [HttpGet("{guid}", Name = "GetAllClientIdQuestions")]
    public IResult GetAllClientIdQuestions(Guid guid)
    {
        _logger.LogInformation("GetAllClientIdQuestions: GET Request to /questions/{0}", guid);
        try {
            Task<List<QuestionDto>> questions = _database.FindAllByClientId(guid.ToString());
            _logger.LogInformation("Successfully retrieved all questions by client guid from DB");
            return Results.Ok(questions.Result);
        }
        catch (System.AggregateException ex)
        {
            _logger.LogError("Failed to query DB. {0}", ex);
            return Results.StatusCode(500);
        }
        
    }

    [HttpGet("allquestions", Name = "GetAllQuestions")]
    public ObjectResult GetAllQuestions()
    {
        _logger.LogInformation("GetAllQuestions: GET Request to /questions/allquestions");
        try {
            List<QuestionDto> questions = _database.FindAll().Result;
            _logger.LogInformation("Successfully retrieved all questions from DB");
            //return Results.Ok(questions);
            return StatusCode(200, questions);
        }
        catch (System.AggregateException ex)
        {
            _logger.LogError("Failed to query DB. {0}", ex);
            return StatusCode(500, ex);
        }
        // Create empty filter, which will return the full db list
        // var filter = Builders<QuestionDto>.Filter.Empty;
        // return publicDB.Mongo_DB_Question_Collection.Find(filter).SortBy(question => question.Id).ToList();
    }

    // GET: /questions/{id}
    [HttpGet("{guid}/{questionId}", Name = "GetQuestionsById")]
    public IResult GetQuestionByClientAndId(Guid guid, int questionId) 
    {
        _logger.LogInformation("GetQuestionsById: GET Request to /questions/{0}/{1}", guid, questionId);
        try {
            QuestionDto? question = _database.FindFirst(guid.ToString()).Result;

            if(question != null)
            {
                _logger.LogInformation("Successfully retrieved first question from DB using Client Guid: {0}/Question Guid: {1}.", guid, questionId);
                Results.Ok(question);
            }
            else
            {
                _logger.LogInformation("Succesfully queried DB but QuestionGuid: {0} could not be found.", questionId);
                Results.NotFound();
            }
        }
        catch (System.AggregateException ex)
        {
            _logger.LogError("Failed to query DB. {0}", ex);
            return Results.StatusCode(500);
        }
        _logger.LogError("A serious error occured if it has reached here.");
        return Results.StatusCode(500);


    }

    [HttpGet("getclientid", Name = "GetClientID")]
    public IResult GetClientId() 
    {
        _logger.LogInformation("GetClientID: GET request made to /questions/getclientid to generate new clientid for new users.");
        // Generate a random guid and send back to the client
        // Client should request this only if there isn't one already in cookies
        Guid? _guid = GenerateNewRandomGuid();
        if(_guid != null) 
        {
            _logger.LogInformation("Succesfully created guid: {0}", _guid);
            return Results.Ok(_guid);
        } 
        else 
        {
            _logger.LogError("Generating a new guid has failed, _guid == null");
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
        _logger.LogInformation("PostQuestions: POST request made to /questions to Insert new question into DB.");
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

        try {
            //Database API
            QuestionDto? retrievedQuestion = _database.Insert(_newQuestion).Result;

            if (retrievedQuestion != null)
            {
                // return Results.CreatedAtRoute("GetQuestions", new {id = _newQuestion.Id}, _newQuestion);
                //Database API
                _logger.LogInformation("Succesfully Inserted into DB and retrived question from DB.");
                return Results.CreatedAtRoute("GetQuestions", new { id = retrievedQuestion.Id }, retrievedQuestion);
            }
            else
            {
                _logger.LogError("An error occured finding the question Inserted into DB, unsure if question succesfully imported or just failed to find it after.");
                return Results.NoContent();
            }
        }
        catch (System.AggregateException ex)
        {
            _logger.LogError("Failed to query DB. {0}", ex);
            return Results.StatusCode(500);
        }

    }

    //PUT: /questions/{id}
    [HttpPut("{id}", Name = "UpdateQuestions")]
    public IResult Put(Guid? id, [FromBody] UpdateQuestionDto updatedQuestion)
    {

        // Creates a filter for all documents for a matching id
        // Filtering by Id is fine here, because it'll be the private frontend,
        // which will not have a client Id

        _logger.LogInformation("UpdateQuestions: PUT request made to /questions/{0} to Insert new question into DB.", id);


        try {
            var oldQuestion = _database.FindFirst(id.ToString()).Result;

            if (oldQuestion != null)
            {
                _logger.LogInformation("Succesfully queried DB and found question to be replaced.");
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
                _logger.LogInformation("Created new Question object to replace oldQuestion.");
                // publicDB.Mongo_DB_Question_Collection.ReplaceOne(filter, _updatedQuestion);
                _database.Replace(questionId, _updatedQuestion);

                _logger.LogInformation("Succesfully replaced Question in DB.");

                return Results.NoContent();
            }
            else
            {
                return Results.NotFound();
            }
        }
        catch (System.AggregateException ex)
        {
            _logger.LogError("Failed to query DB. {0}", ex);
            return Results.StatusCode(500);
        }

    }

    //DELETE: /questions/{id}
    [HttpDelete("{id}", Name = "DeleteQuestions")]
    public IResult Delete(Guid? id)
    {
        _logger.LogInformation("DeletQuestions: DELETE request to /questions/{0}", id);
        try
        {
            _database.DeleteOne(id.ToString());
            _logger.LogInformation("Successfully deleted Question from DB.");
            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Failed to delete Question from DB. {0}", ex);
            return Results.StatusCode(500);
        }
        catch (System.AggregateException ex)
        {
            _logger.LogError("Failed to query DB. {0}", ex);
            return Results.StatusCode(500);
        }
    }

    
}
