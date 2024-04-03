using MongoDB.Driver;
using MongoDB.Bson;
using AskSam_API;
using AskSam.Dtos;



var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("AskSam_MongoDB");
Database database = new MongoDB_API(connectionString);


builder.Services.AddSingleton(database);

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
