# Issue 2: Configure database access and migrations with DbUp

## Aim
Implement database schema migrations using DbUp and integrate with the existing PostgreSQL container configured in the .NET Aspire AppHost.

## Implementation Steps

1. **Set up database connection with the existing PostgreSQL container**:
   - Use the PostgreSQL container already configured in the .NET Aspire AppHost from step 1
   - Configure appropriate connection string variables in appsettings.json
   - Set up a dedicated database access service in the infrastructure layer

2. **Set up DbUp for database migrations**:
   - Add DbUp NuGet package to the appropriate project
   - Create a dedicated migrations project `ProPulse.Database`
   - Reference the Azure SQL DbUp sample (https://github.com/Azure-Samples/azure-sql-db-aspire/blob/main/hostedss%20-%20dbup/AspireApp1.AppHost/Program.cs) for AppHost integration
   - Configure the AppHost to run DbUp migrations before starting the application
   - Set up migration runner with proper error handling
   - Configure DbUp to track executed scripts in a journal table

3. **Create initial database schemas**:
   - Create SQL script for the `identity` schema
   - Create SQL script for the `propulse` schema
   - Add necessary PostgreSQL extensions (uuid-ossp, citext, pg_trgm, etc.)
   - Set up proper permissions between schemas

4. **Implement initial database migration scripts**:
   - Create scripts for User tables (leverage ASP.NET Identity schema)
   - Create scripts for Article tables
   - Create scripts for Category and Tag tables
   - Create scripts for Bookmark and ReadingHistory tables
   - Create scripts for Attachment tables
   - Add appropriate indexes and constraints

5. **Set up testing database configuration**:
   - Configure SQLite for unit testing
   - Set up TestContainers for integration testing with PostgreSQL

6. **Configure Entity Framework Core**:
   - Set up DbContext with proper configuration
   - Configure connection string management
   - Configure model mappings and relationships
   - Set up global query filters for soft delete

7. **Create utility scripts**:
   - Add script to reset development database
   - Create script to generate test data
   - Add database schema documentation generator

8. **Integrate with .NET Aspire**:
   - Configure migrations to run before application startup
   - Set up health checks for database connectivity
   - Add PostgreSQL dashboard for local development

## Definition of Done

- [ ] PostgreSQL container configured and running in .NET Aspire
- [ ] DbUp migrations successfully creating all database tables
- [ ] Database schemas created with proper permissions
- [ ] Entity Framework Core correctly configured for the application
- [ ] Migrations can be run as a separate step in the build process
- [ ] Test database configuration working with both SQLite and TestContainers
- [ ] Database schema documented with diagrams or schema visualization tool
- [ ] All tables from the data model in the technical specification created
- [ ] Database scripts include necessary indexes for performance
- [ ] Soft delete functionality configured and working correctly
