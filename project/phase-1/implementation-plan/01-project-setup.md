# Issue 1: Set up initial project structure with .NET Aspire

## Aim
Create the foundational project structure for ProPulse using .NET 9 and set up .NET Aspire for local development orchestration.

## Implementation Steps

1. **Create solution structure**:
   - Create a new .NET 9 solution `ProPulse.sln`
   - Organize solution folders to match the desired structure (`src`, `test`, `docs`)

2. **Set up basic projects**:
   - Create the main web application project `ProPulse.Web` (ASP.NET Core MVC with Razor Pages)
   - Create the API project `ProPulse.Api` (ASP.NET Core Web API)
   - Create the core application project `ProPulse.Application`
   - Create the domain project `ProPulse.Domain`
   - Create the infrastructure project `ProPulse.Infrastructure`

3. **Configure .NET Aspire for orchestration**:
   - Add a .NET Aspire app host project `ProPulse.AppHost`
   - Add a .NET Aspire service defaults project `ProPulse.ServiceDefaults`
   - Configure the AppHost to reference all application projects
   - Set up service discovery and telemetry

4. **Configure containers in .NET Aspire AppHost**:
   - Add PostgreSQL container configuration to AppHost
   - Configure Azurite (Azure Storage Emulator) container
   - Set up container networking and persistent volumes
   - Configure connection strings and environment variables
   - Ensure health checks for all containers

5. **Set up common infrastructure**:
   - Configure build properties in `Directory.Build.props` using 4-space indentation
   - Set up `.editorconfig` for consistent coding style
   - Add `.gitignore` for .NET projects
   - Initialize `README.md` with basic project information

6. **Configure dependency injection**:
   - Set up dependency injection containers in each project
   - Configure service registration extensions
   - Create service registration extensions for AppHost project

7. **Configure initial application settings**:
   - Create environment-specific `appsettings.json` files
   - Set up User Secrets for local development
   - Configure logging with Serilog
   - Set up configuration providers for different environments

8. **Set up cross-cutting components**:
   - Add structured logging with Serilog
   - Configure exception handling middleware
   - Add initial health check endpoints
   - Set up Aspire dashboard for local development

## Definition of Done

- [ ] Solution structure created with all necessary projects
- [ ] .NET Aspire configured and working for local development
- [ ] Application builds successfully with no errors
- [ ] Docker setup complete with running containers
- [ ] Basic health check endpoint returns successful response
- [ ] Project follows the specified coding style (4-space indentation)
- [ ] README updated with setup instructions
- [ ] Push changes to the main branch with a descriptive commit message
