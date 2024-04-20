using AskSam.Dtos;
using AskSam_API.Interfaces;
using MongoDB.Driver;

namespace AskSam_API.Database_APIs;

public class MongoDB_API : IDatabase
{

    private Public_DB? publicDB;

    string askSamQuestionCollectionName = "askSamQuestionCollection";
    string askSamQuestionDBName = "AS_Question_DB";

    public MongoDB_API(string? connectionString)
    {
        if (connectionString != string.Empty || connectionString != null)
        {
            var client = new MongoClient(connectionString);

            //If it cannot find the DB, it will create it
            //Then create the collection
            var questionDB = client.GetDatabase(askSamQuestionDBName);



            IMongoCollection<QuestionDto> questionCollection = questionDB.GetCollection<QuestionDto>(askSamQuestionCollectionName);

            publicDB = new Public_DB
            {
                Mongo_DB_Question_Collection = questionCollection
            };

        }
        else
        {
            Console.WriteLine("Warning: Connection string is empty, there is no connection to a DB.");
        }
    }

    public async Task DeleteOne(string? questionId)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(questionId);
        await publicDB.Mongo_DB_Question_Collection.DeleteOneAsync(filter);
    }

    public async Task<List<QuestionDto>> FindAll()
    {
        FilterDefinition<QuestionDto> filter = Builders<QuestionDto>.Filter.Empty;
        return await publicDB.Mongo_DB_Question_Collection.Find(filter).ToListAsync();
    }

    public async Task<List<QuestionDto>> FindAllByClientId(string? clientId)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByClientId(clientId);
        return await publicDB.Mongo_DB_Question_Collection.Find(filter).ToListAsync();
    }

    public async Task<QuestionDto?> FindFirst(string? questionId)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(questionId);
        return await publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<QuestionDto?> Insert(QuestionDto question)
    {
        await publicDB.Mongo_DB_Question_Collection.InsertOneAsync(question);
        return await GetData(question);
    }

    public async Task<QuestionDto?> Replace(string? questionId, QuestionDto updatedQuestion)
    {
        FilterDefinition<QuestionDto> filter = CreateQuestionDTOFilterByQuestionId(questionId);
        await publicDB.Mongo_DB_Question_Collection.ReplaceOneAsync(filter, updatedQuestion);
        return await GetData(updatedQuestion);
    }

    public async Task<long> TotalCount()
    {
        return await publicDB.Mongo_DB_Question_Collection.EstimatedDocumentCountAsync();
    }

    protected async Task<QuestionDto> GetData(QuestionDto _question)
    {
        var filter = CreateQuestionDTOFilterByQuestionId(_question.Id);

        return await publicDB.Mongo_DB_Question_Collection.Find(filter).FirstOrDefaultAsync();
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
