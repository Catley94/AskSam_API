using AskSam.Dtos;
using MongoDB.Driver;

namespace AskSam_API;

public class Public_DB
{
    public IMongoCollection<QuestionDto> Mongo_DB_Question_Collection { get; set; }
}
