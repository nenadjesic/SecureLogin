Implement a webservice for managing a repository of users and their passwords. Data should be stored in sql database. Service should be secured with API key, client should submit valid API key with each request. Each client should get its own API key.
 
Service must support:
•	Adding a new user
•	Updating user’s data 
•	Deleting user
•	Retrieving user’s data
•	Validating user’s password
 
Service should store at least the following information for each user
•	Id (Immutable, generated automatically)
•	UserName
•	FullName 
•	E-mail 
•	Mobile number
•	Language
•	Culture 
•	Password 
 
Each API call should be logged to application log file. A new log file should be created for each day. Log messages should contain following information:
•	Log level (Info or Error)
•	Time
•	Client IP
•	Client name
•	Name of host on which service is running
•	Name of API method
•	Request parameters
•	Message
 
Solution must be implemented in c#. Request and response messages of all APIs should be in json format. Solution must include instructions for running it in our environment and any scripts required to set up its database. 



#Solution#

1. Running with Docker
To spin up the entire environment (including the API and required infrastructure) using containers, execute the following command in the project's root directory:

bash
docker compose up --build

This command automatically builds the necessary images and starts the defined services.

2. Local Development (Visual Studio)
   
To run the application directly from your IDE:
Open the solution file (.sln) in Visual Studio.
Press CTRL + F5 to launch the application without the debugger.
The API documentation and testing interface (Swagger) will be available at:
👉 [http://localhost:5175/swagger/index.html]

3. System Testing
To ensure all components are working together correctly, run the integration tests:
Open Test Explorer in Visual Studio.
Select Run All Tests (or right-click the test project and select Run Tests).
