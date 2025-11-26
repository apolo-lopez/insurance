Hi...

Thanks for passing by on this readme, my name is Apolo and I'm a veteran on IT and development.

This project is composed of two parts:

API:
  - It has the backend logic and services, it was created using Clean Architecture as follows:
    ⦁	Domain: Contains the domain models and business rules.
    ⦁	Application: Contains the application logic and services.
    ⦁	Infrastructure: Contains the data access and external service integrations.
  ⦁	API: Contains the ASP.NET Core Web API code.

Client:
  - Created in angular, I tried to use the best practices by dividing the project in components, services, interceptor and so on
  - It has a small authentication module with a JWT interceptor that catches every request and checks if the user is logged in

To run the API and front I have packed everything on docker so the only thing you need to have is docker installed on your rig.

Then you need to execute: docker compose up --build -d

Everything will be created and even a migration is available. Once deployed you can use the following credentials:

user: admin@system.com

Pass... Ask me if you want to access.

Happy coding!
