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
* ASP.NET Core for web applications.
* dotnet Aspire for orchestration during development.
* PostgreSQL for remote databases; SQLite for unit test databases.
* Entity Framework Core for an ORM.
* xUnit, NSubstitute, FluentAssertions for unit and integration testing.
* Azure for cloud hosting.

# Project Layout

* /project is used for project related documentation such as product proposals, technical specifications and implementation plans.
* /docs is used for project documentation
* /src is used for source code for the deployable code.
* /test is used for unit, UI, and integration test code.

# Other instructions

* Create a history file for this chat session within the .github/history file using Markdown.  The filename should be of the form `YYYYMMDD-ID-history.md`.
* Write each prompt I send you and the text content for the response to the history file as you go along.  Do not write file edits to the history file.
