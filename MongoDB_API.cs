using AskSam.Dtos;
using MongoDB.Driver;

namespace AskSam_API;

public class MongoDB_API : Database
{

    string askSamQuestionCollectionName = "askSamQuestionCollection";
    string askSamQuestionDBName = "AS_Question_DB";

    public MongoDB_API(string? connectionString) : base(connectionString) 
    {
        if(connectionString != String.Empty || connectionString != null)
        {
            var client = new MongoClient(connectionString);
            
            //If it cannot find the DB, it will create it
            //Then create the collection
            var questionDB = client.GetDatabase(askSamQuestionDBName);
            questionDB.CreateCollection(askSamQuestionCollectionName);

            IMongoCollection<QuestionDto> questionCollection = questionDB.GetCollection<QuestionDto>(askSamQuestionCollectionName);

            publicDB = new Public_DB {
                Mongo_DB_Question_Collection = questionCollection
            };

        } else {
            Console.WriteLine("Warning: Connection string is empty, there is no connection to a DB.");
        }   
    }

    public override void DeleteOne(string? questionId)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(questionId);
        publicDB.Mongo_DB_Question_Collection.DeleteOne(filter);
    }

    public override List<QuestionDto> FindAll()
    {
        FilterDefinition<QuestionDto> filter = Builders<QuestionDto>.Filter.Empty;
        return publicDB.Mongo_DB_Question_Collection.Find(filter).ToList();
    }

    public override List<QuestionDto> FindAllByClientId(string? clientId)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByClientId(clientId); 
        return publicDB.Mongo_DB_Question_Collection.Find(filter).ToList();
    }

    public override QuestionDto? FindFirst(string? questionId)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(questionId);
        
        return publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefault();
    }

    public override QuestionDto Insert(QuestionDto question)
    {
        //Insert into DB
        publicDB.Mongo_DB_Question_Collection.InsertOne(question);

        //Find in DB to check it has ben inserted correctly
        return GetData(question);
    }

    public override void Replace(string? questionId, QuestionDto updatedQuestion)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(questionId);
        ReplaceOneResult ReplacedResult = publicDB.Mongo_DB_Question_Collection.ReplaceOne(filter, updatedQuestion);
    }

    public override long TotalCount()
    {
        return publicDB.Mongo_DB_Question_Collection.EstimatedDocumentCount();
    }

    protected QuestionDto GetData(QuestionDto _newQuestion)
    {        
        var filter = CreateQuestionDTOFilterByQuestionId(_newQuestion.Id);

        return publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefault();
    }

    protected FilterDefinition<QuestionDto> CreateQuestionDTOFilterByQuestionId(string? id) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.Id, id);
    }

    private FilterDefinition<QuestionDto> CreateQuestionDTOFilterByClientId(string? guid) 
    {
        return Builders<QuestionDto>.Filter
                    .Eq(question => question.ClientGuid, guid);
    }

    
}
