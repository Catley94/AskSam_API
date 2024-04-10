# ASP.NET API

## Endpoints
GET /questions => Results.Problem("No client guid found, please include this within GET request, like so: /questions/7ac79c82-b01b-46de-af5c-3d7db4bfeeaf");

GET /questions/\<client guid\> => Find all questions from client guid
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

GET /allquestions => Return all questions from database.
Used for AskSam_Staff (https://github.com/Catley94/AskSam_Staff)

GET /\<guid\>/\<questionId\> => Return question (if found) from guid and questionId
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

GET /getclientid => Creates a new guid for client, only used for clients without a client id and user has accepted cookies.
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

POST /questions => Create question in database under client guid
Used for AskSam_Client (https://github.com/Catley94/AskSam_Client)

PUT /\<questionId\> => Update question to questionId, used for answering questions. This auto populates a "DateAnswered" with the date that it's updated.
Used for AskSam_Staff (https://github.com/Catley94/AskSam_Staff)

DELTE /\<questionId\> => Used to delete questionIds, currently not being used by either Client or Staff.

## Databases
#### Mongodb - Is hosted on Azure.
#### SQL - Plans on using Azure for future.
(Adding more databases would be easy as I have abstracted this as a database layer.)

