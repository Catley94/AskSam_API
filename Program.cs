using MongoDB.Driver;
using MongoDB.Bson;
using AskSam_API;
using AskSam.Dtos;
using AskSam_API.Data;

const string MongoDbService = "MongoDB";
const string SqliteService = "sqlite";
const string SqlService = "sql";

const bool localDB = false;

var builder = WebApplication.CreateBuilder(args);

string dbService = MongoDbService;

string? localMongoDbConnectionString = builder.Configuration.GetConnectionString("Local_AskSam_MongoDB");
string? azureMongoDbConnectionString = builder.Configuration.GetConnectionString("Azure_AskSam_MongoDB");
string? sqliteConnectionString = builder.Configuration.GetConnectionString("Local_AskSam_Sqlite");
string? localSqlConnectionString = builder.Configuration.GetConnectionString("Local_AskSam_Sql");
string? azureSqlConnectionString = builder.Configuration.GetConnectionString("Azure_AskSam_Sql");
Database? database = null;

switch(dbService) 
{
    case MongoDbService:
        database = new MongoDB_API(localDB ? localMongoDbConnectionString : azureMongoDbConnectionString);
    break;
    case SqliteService:
        database = new SqliteDB_API(sqliteConnectionString);
        builder.Services.AddSqlite<AskSamContext>(sqliteConnectionString);
        
    break;
    case SqlService:
        database = new SQL_API(localDB ? localSqlConnectionString : azureSqlConnectionString);
        builder.Services.AddSqlServer<AskSamContext>(sqliteConnectionString);
    break;
    default:
        database = null;
        Console.WriteLine("Error, unable to determine what database service to use.");
    break;
}

if(database != null) builder.Services.AddSingleton(database);


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

if(dbService == SqliteService) app.MigrateDb();

app.Run();
