# ProPulse

ProPulse is a modern content management system built with .NET 9, designed for publishing and managing articles, blog posts, and digital content. It provides a seamless authoring experience, efficient content organization, and powerful distribution capabilities.

## Technology Stack

- **Backend**: .NET 9 with C# and ASP.NET Core
- **Frontend**: ASP.NET Core MVC with Razor Pages
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Local Development**: .NET Aspire for orchestration
- **Cloud Hosting**: Azure

## Project Structure

- `/src` - Source code for all application projects
- `/test` - Test projects (unit, integration, and UI tests)
- `/docs` - Documentation for the project
- `/project` - Product proposal, technical specifications, and implementation plans

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker Desktop (for containerized dependencies)
- Visual Studio 2025 or later / VS Code with C# extensions

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application with .NET Aspire:

```bash
cd src/ProPulse.AppHost
dotnet run
```

This will start the Aspire dashboard and launch all project components, including:
- Web application (MVC)
- API endpoints
- PostgreSQL database
- Azurite (Azure Storage Emulator)

## Development

ProPulse follows clean architecture principles with the following key projects:

- **ProPulse.Domain**: Core domain entities and business logic
- **ProPulse.Application**: Application services and use cases
- **ProPulse.Infrastructure**: External services and data access
- **ProPulse.Web**: Web UI for content management and display
- **ProPulse.Api**: RESTful API endpoints
- **ProPulse.AppHost**: .NET Aspire host for local development orchestration
- **ProPulse.ServiceDefaults**: Common service configuration

## License

This project is licensed under the MIT License - see the LICENSE file for details.
