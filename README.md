# Entity Management API
### Documentation
Explore the API documentation [Live](https://entitymanagementapi.onrender.com/swagger/index.html).
## Overview

The Entity Management API is a RESTful web service built using .NET Core, designed to provide CRUD (Create, Read, Update, Delete) operations for managing entities. This project was developed as a technical test for the C# Developer role at KYC360.

## Features

- Create, read, update, and delete entities.
- Retrieve a list of entities with support for pagination and sorting.
- Search entities based on various fields.
- Filter entities based on gender, date range, and countries.
- Bonus challenge: Retry and backoff mechanism for database write operations.

## Technologies Used

- **.NET Core**: A cross-platform, open-source framework for building modern, cloud-based, and internet-connected applications.
- **C#**: A powerful, type-safe, and object-oriented programming language used in .NET development.
- **ASP.NET Core Web API**: A framework for building HTTP-based APIs using .NET Core.
- **Polly**: A .NET resilience and transient-fault-handling library for handling retries and circuit breakers.
- **xUnit**: A unit testing framework for .NET Core.

## Installation and Setup

1. Install the latest version of [.NET Core SDK](https://dotnet.microsoft.com/download) for your platform.
2. Clone this repository to your local machine.
3. Navigate to the project directory in the terminal.
4. Run `dotnet build` to build the project.
5. Run `dotnet run` to start the API server locally.

## Usage

- The API can be accessed via HTTP requests to the specified endpoints.
- Detailed documentation for each endpoint and its parameters can be found in the technical test document.

## Implementation Details

- The CRUD operations are implemented in the `EntitiesController.cs` file.
- Pagination and sorting functionality are added to the `GetAllEntities` endpoint.
- Searching and filtering capabilities are provided in accordance with the requirements.
- The retry and backoff mechanism is implemented for database write operations using Polly.

## Testing and Quality Assurance

- Unit tests are implemented using xUnit to ensure the correctness of API endpoints and functionalities.
- Integration tests can be added to test the interaction between different components.
- Continuous integration (CI) pipelines can be set up to automate the testing process.

## Future Improvements

- Enhance error handling and validation to provide informative responses to client requests.
- Implement integration tests to cover the interaction between various components.
- Optimize performance by fine-tuning database queries and caching mechanisms.
- Enhance security measures, such as implementing authentication and authorization.
- Monitor API performance and reliability in production environments.

## Conclusion

The Entity Management API provides a robust solution for managing entities with comprehensive functionality, including CRUD operations, pagination, sorting, searching, and filtering. The implementation adheres to industry best practices and demonstrates proficiency in .NET Core development. I am eager to contribute further to the project and leverage my skills to meet the evolving needs of KYC360.
