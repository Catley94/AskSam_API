using EmptyDotNetWebAPI2.Dtos;
using MongoDB.Driver;

namespace AskSam_API;

public class Public_DB
{
    public IMongoCollection<QuestionDto> Mongo_DB_Collection { get; set; }
}
