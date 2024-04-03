using MongoDB.Driver;
using MongoDB.Bson;
using AskSam_API;
using AskSam.Dtos;



var builder = WebApplication.CreateBuilder(args);


string? connectionString = builder.Configuration.GetConnectionString("AskSam_MongoDB");
string askSamQuestionCollectionName = "askSamQuestionCollection";
string askSamQuestionDBName = "AS_Question_DB";


if(connectionString != String.Empty)
{
    var client = new MongoClient(connectionString);
    
    //If it cannot find the DB, it will create it
    //Then create the collection
    var questionDB = client.GetDatabase(askSamQuestionDBName);
    questionDB.CreateCollection(askSamQuestionCollectionName);

    IMongoCollection<QuestionDto> questionCollection = questionDB.GetCollection<QuestionDto>(askSamQuestionCollectionName);

    builder.Services.AddSingleton(new Public_DB {
        Mongo_DB_Question_Collection = questionCollection
    });

} else {
    Console.WriteLine("Warning: Connection string is empty, there is no connection to a DB.");
}



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; 


builder.Services.AddCors(options =>  
{  
    // options.AddPolicy(name: MyAllowSpecificOrigins,  
    //                   policy  =>  
    //                   {  
    //                       policy.WithOrigins("http://localhost:5173",  
    //                                           "http://www.contoso.com"); // add the allowed origins  
    //                   });  
    options.AddDefaultPolicy(  
                      policy  =>  
                      {  
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });  
});  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
