# ASP.NET API - .NET 8

Note: This project does include an appsettings.json file, purely as this is an example project. 
In production, this will be taken out.

## Endpoints
GET /questions => Results.Problem("No client guid found, please include this within GET request, like so: /questions/7ac79c82-b01b-46de-af5c-3d7db4bfeeaf");

GET /questions/\<client guid\> => Find all questions from client guid
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

GET /questions/allquestions => Return all questions from database.
Used for AskSam_Staff (https://github.com/Catley94/AskSam_Staff)

GET /questions/\<guid\>/\<questionId\> => Return question (if found) from guid and questionId
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

GET /questions/getclientid => Creates a new guid for client, only used for clients without a client id and user has accepted cookies.
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

POST /questions => Create question in database under client guid
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

PUT /questions/\<questionId\> => Update question to questionId, used for answering questions. This auto populates a "DateAnswered" with the date that it's updated.
Used for AskSam_Staff (https://github.com/Catley94/AskSam_Staff)

DELETE /questions/\<questionId\> => Used to delete questionIds, currently not being used by either Client or Staff.

## Databases
#### Mongodb - Is hosted on Azure.
#### SQL - Plans on using Azure for future.
(Adding more databases would be easy as I have abstracted this as a database layer.)


## Prerequisites for Development
- SQLite DB Locally
- MongoDB Locally
- Install
- dotnet ef installed (`dotnet tool install --global dotnet-ef`)
- Migrate DB InitialCreate
- Create appsettings.json (Below is a really basic example)
  - "MySettings": {
    "adminGuid": "02c45e2f-8299-44e2-aa24-0c99fc60c7bd"
    },
  - "ConnectionStrings": {
    "Local_AskSam_MongoDB": "mongodb://localhost:27017"
    }