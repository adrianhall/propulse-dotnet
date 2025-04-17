# Coding style

* Use spaces for indentation with four spaces per level.  If this is a csproj or props file, use two-spaces.
* All public and internal members require documentation comments
* Prefer type declarations over var when the type isn't obvious
* Use file-scoped namespaces.
* Use simplified collection initializers.
* Use the latest preview C# language version.
* Use dotnet 9.

# Preferred technology stack

I prefer to use the following technologies:

* C#, dotnet 9.
* ASP.NET Core for web APIs.
    * FastEndpoints, CQRS, FluentValidation for APIs
* dotnet Aspire for orchestration during development.
* PostgreSQL for remote databases; SQLite for unit test databases.
* Entity Framework Core for an ORM.
* Serilog for structured logging.
* xUnit, NSubstitute, FluentAssertions, TestContainers for unit and integration testing.
* Azure for cloud hosting.

For frontend work:

* React apps using TypeScript, scaffolded via `vite`.
* State management via Redux Toolkit.
* Microsoft Fluent UI components
* Enforce code style in the React app with eslint, prettier.

# Project Layout

* /project is used for project related documentation such as product proposals, technical specifications and implementation plans.
* /src is used for source code for the deployable code.
* /test is used for unit, UI, and integration test code.

# Maintaining, building and testing the project.

* Use `dotnet` commands to alter the solution and add packages to the projects.
    * Prefer the latest GA version compatible with net9.0 of any NuGet package.
    * Avoid pre-release packages where possible.
* Use `npm run build` to build the application.
* Use `npm run test` to run all tests.
