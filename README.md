# A Clean Architecture + CQRS implementation in .NET

## Overview
This is a simple ordering application that I built with the help of DeepSeek. The conversation and prompting started with wanting to build 
a sample C# project that demonstrated - in working code - the ideas behind Clean Architecture, some of Domain Driven Design (DDD) and 
Command Query Responsibility Segragation (CQRS) pattern. 

Minimal yet structured approach to Clean Architecture with:

✅ CQRS (via MediatR)
✅ Repository + Unit of Work patterns
✅ Vertical Slicing by feature
✅ Minimal APIs for endpoints


| Layer          | Components                          | NuGet Packages Used       |
|----------------|-------------------------------------|---------------------------|
| **Web**        | Minimal APIs, DTOs                  | `Microsoft.AspNetCore`    |
| **Application**| Commands, Queries, Validators       | `MediatR`, `FluentValidation` |
| **Infrastructure** | EF Core, Repositories           | `Microsoft.EntityFrameworkCore` |
| **Domain** | Domain entities and business logic  | `` |

## Direction of dependencies

Web --> Application
Application --> Domain
Application --> Infrastructure
Infrastructure --> Domain

### CQRS
A version of the CQRS pattern has been implemented in the Application project using MediatR and also separating update and query logic in the
solution. This is a popular approach in .NET prjects and it's implemention supported by the MediatR package. Overall this provides a nice
separation of concerns where Commands validate and update state and return minimal data, Queries fetch data and enrich it for use in the 
presentation layer.

### Generic Repository and Unit of Work
Though not always necessary in simple projects the repository and unit of work patterns help to abstract the data layer away from the projects 
that use it. 
- The key benifit of a generic repository is avoidsing repetitive code across entity-specific repositories while providing common CRUD operations 
(GetById, Add, Update, Delete) for any entity. 
- The Unit of Work groups multiple database related operations (e.g., updating an Order and deducting inventory) into a single transaction. Which 
ensures all changes succeed or fail together. 
- It can also reduce the chattiness of the application with regards to its interactions with the
database.
- This also means that there is no direct exposure of DbContext to the other projects (keeps layers clean).
The interfaces for these are defined in the Domain project but implemented in the Infrascture project which deals with persistence.


### DDD 
This has been used at a basic level and in not the full implementation - in most cases the complexity of a complete DDD solution is too much 
and makes the code base diffictult to work in for developers.
What has been retained from DDD is the idea of a central and dependency free Domain project that represents the real world objects that the 
app is working with. The entities in the Domain project encapsulate as much of the domain business logic as possible. 
There are no dependency in the Domain project to the Application, Insfrastructure or Web projects.

Domain project:

✅ No EF Core, MediatR, Newtonsoft.Json, etc.

✅ No references to Application, Infrastructure, or Web projects.

✅ Only pure C# and .NET Standard/Core base libraries (e.g., System).


