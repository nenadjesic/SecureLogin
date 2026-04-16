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
