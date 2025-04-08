# 2. Technology Choices

## 2.1. Core Technologies

- **Framework**: ASP.NET Core 9
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL for production, SQLite for testing
- **Caching**: Azure Cache for Redis
- **Authentication**: ASP.NET Identity with social providers
- **Media Storage**: Azure Blob Storage
- **Hosting**: Azure App Service
- **Development Orchestration**: dotnet Aspire
- **Containerization**: Docker for local development

## 2.2. Environment Differences

### Development Environment
- **Infrastructure**: Local with Docker Desktop and dotnet Aspire
- **Database**: Local PostgreSQL container
- **Authentication**: Mock social providers or development accounts
- **Caching**: Local Redis container
- **Media Storage**: Azurite container for local blob storage emulation
- **Logging**: Console and file-based logging
- **Configuration**: Local user secrets and appsettings.Development.json

### Test Environment
- **Infrastructure**: Automated testing on CI/CD pipelines
- **Database**: SQLite for unit tests, PostgreSQL container for integration tests
- **Authentication**: Test mocks
- **Media Storage**: Mocked blob storage services
- **Testing Framework**: xUnit, NSubstitute, FluentAssertions

### Staging Environment
- **Infrastructure**: Azure App Service (smaller instance sizes)
- **Database**: Azure Database for PostgreSQL (basic tier)
- **Authentication**: Real social providers with test accounts
- **Caching**: Azure Cache for Redis (basic tier)
- **Media Storage**: Azure Blob Storage (standard tier)
- **Logging**: Application Insights
- **Configuration**: Azure Key Vault and Azure App Configuration

### Production Environment
- **Infrastructure**: Azure App Service
- **Database**: Azure Database for PostgreSQL (production tier)
- **Authentication**: Real social providers
- **Caching**: Azure Cache for Redis (standard tier)
- **Media Storage**: Azure Blob Storage (premium tier)
- **Logging**: Application Insights with alerting
- **Configuration**: Azure Key Vault and Azure App Configuration
- **CDN**: Azure CDN for static assets and media content

## 2.3. API Implementation Approaches

When implementing a REST API in ASP.NET Core, several approaches are available, each with its own advantages and trade-offs. This section compares these approaches to determine the most suitable one for the ProPulse platform.

### Minimal APIs

**Pros:**
- Simplified, low-ceremony code with less boilerplate
- Improved performance due to reduced overhead
- Co-location of endpoints, handlers, and models
- Great for microservices or small APIs with limited complexity

**Cons:**
- Limited support for complex scenarios such as model validation, authorization, and filters
- Not compatible with Microsoft.AspNetCore.OData, which is critical for the complex querying needs of ProPulse
- Less structured approach can lead to maintainability issues in larger applications
- Reduced discoverability and organization as the API grows

### Traditional API Controllers (MVC)

**Pros:**
- Full compatibility with Microsoft.AspNetCore.OData for rich querying
- Well-established patterns and practices for organizing complex APIs
- Rich feature set including filters, model binding, validation, and authorization attributes
- Better separation of concerns with dedicated controller classes

**Cons:**
- More boilerplate code compared to Minimal APIs
- Slightly higher performance overhead due to the MVC pipeline
- Often leads to larger controller classes that handle multiple responsibilities

### FastEndpoints

**Pros:**
- Vertical slice architecture promoting high cohesion
- One file per endpoint for better organization
- Strong CQRS support built-in
- Better performance than traditional controllers
- Feature-rich with validation, mapping, and other capabilities

**Cons:**
- Third-party library that may introduce dependencies
- Less integrated with some Microsoft libraries like OData
- Learning curve for developers not familiar with the library
- May require custom integration work for certain Microsoft libraries

### Controller Method Execution Approaches

**Plain Controller Actions:**
- Direct execution within controller actions
- Simple and straightforward for smaller applications
- Can become unwieldy as complexity increases

**CQRS via MediatR:**
- Separation of command and query responsibilities
- Decouples controllers from business logic
- Enables cross-cutting concerns through behaviors (logging, validation, caching)
- Facilitates testability and promotes single responsibility principle
- Added complexity for simple CRUD operations

## Implementation Recommendation

Based on the requirements of ProPulse, particularly the need for OData support, complex API interactions, and enterprise-grade features, the recommended implementation approach is:

**Primary Approach: Traditional API Controllers (MVC) with CQRS via MediatR**

This combination provides:
1. Full compatibility with Microsoft.AspNetCore.OData for the rich filtering, sorting, and querying capabilities required
2. Clean separation of concerns with CQRS pattern implementation via MediatR
2. Well-structured organization of API endpoints through controller classes
4. Strong support for cross-cutting concerns through MediatR behaviors
5. Improved testability and maintainability for a complex enterprise application

While Minimal APIs offer performance benefits, the lack of OData support and limited features for complex scenarios make them unsuitable for ProPulse. FastEndpoints is an excellent alternative, but given the project's requirements for OData integration and the enterprise nature of the application, the traditional controllers with MediatR provide the most robust and well-supported solution.

The CQRS approach via MediatR will help maintain clean domain models, separate read and write operations, and provide a structured way to handle the complex business logic required by the platform.

## 2.4. Frontend Technology Approaches

When implementing the frontend for the ProPulse platform, several approaches are available within the ASP.NET Core ecosystem. This section compares these approaches to determine the most suitable one for the platform.

### Razor Pages

**Pros:**
- Simplified page-based programming model ideal for content-heavy sites
- Page-focused architecture that naturally maps to content publishing workflows
- Minimizes the need for complex client-side JavaScript frameworks
- Page and handler code can be kept together for better organization
- Strong integration with ASP.NET Core identity and other framework features
- High performance through server rendering with minimal client overhead

**Cons:**
- Less suited for highly interactive single-page applications
- May require more round trips to the server for certain interactions
- Limited component reusability compared to Blazor

### Server-Side Blazor

**Pros:**
- Component-based architecture with high reusability
- C# code across the stack, reducing context switching
- Real-time updates through SignalR integration
- Interactive experience without extensive JavaScript
- Seamless integration with ASP.NET Core services

**Cons:**
- Increased server resource utilization due to maintaining circuit per user
- Potential latency issues with SignalR connection
- More complex deployment and scaling considerations
- Learning curve for component lifecycle and state management

### MVC with Razor Views

**Pros:**
- Traditional controller-based architecture familiar to many developers
- Strong separation of concerns with Model-View-Controller pattern
- Well-established patterns and extensive documentation
- Suitable for a wide range of application types

**Cons:**
- More boilerplate code compared to Razor Pages for simple content sites
- Less intuitive routing compared to page-based architectures
- Often requires more files and folders for the same functionality as Razor Pages

### PWA with SPA Frameworks (React, Vue, Angular)

**Pros:**
- Rich, highly interactive user experiences
- Sophisticated state management options
- Extensive ecosystem of libraries and tools
- Better offline capabilities through service workers
- Potential for native-like mobile experiences

**Cons:**
- Separate frontend and backend codebases requiring different skill sets
- Additional complexity in development, testing, and deployment pipelines
- May require more robust API design upfront
- Potential SEO challenges without server-side rendering
- Higher initial loading time for JavaScript bundles

## Implementation Recommendation

Based on the requirements of ProPulse, particularly the content-focused nature of the application with its emphasis on article publishing, consumption, and social media integration, the recommended frontend implementation approach is:

**Primary Approach: Razor Pages with API endpoints for dynamic content**

This combination provides:
1. Simplified development model ideal for content-heavy sites like ProPulse
2. Natural mapping to the article publishing and reading workflows
2. Strong performance through server rendering for core content
4. SEO-friendly approach critical for content discovery
5. Ability to enhance with JavaScript where needed for interactive features
6. Streamlined development experience with a unified technology stack
7. Clean separation between page rendering and API endpoints

The architecture will use:
- Razor Pages for the primary user interface
- API controllers for dynamic content updates and interactions
- JavaScript enhancements for interactive elements (article editor, rating system)
- SignalR for real-time features (comment notifications, analytics updates)

This approach balances development simplicity, performance, and user experience considerations while aligning with the static-content-with-dynamic-elements nature of the ProPulse platform.

## 2.5. Database Migration Approaches

When implementing database schema migrations and versioning for the ProPulse platform, several approaches are available within the .NET ecosystem. This section compares these approaches to determine the most suitable one for the platform.

### DbUp

**Pros:**
- Script-based migration with precise control over SQL
- Simple, lightweight library with minimal dependencies
- Excellent integration with dotnet Aspire through AppHost configuration
- Clean separation of database code from application code
- Journal table tracks executed scripts to prevent re-running
- Can be packaged as a separate console application or service
- Works well with all major databases including PostgreSQL
- Scripts can be embedded resources or loaded from filesystem
- Easy integration with CI/CD pipelines

**Cons:**
- No automatic schema difference detection
- Limited built-in rollback support (requires manual rollback scripts)
- Scripts must be managed carefully to ensure proper execution order
- Requires more manual SQL writing compared to ORM-based migrations

### Entity Framework Core Migrations

**Pros:**
- Deep integration with the ORM used by the application
- Code-first approach allows defining schema in C# classes
- Automatic generation of migration scripts based on model changes
- Built-in command-line tools (dotnet ef migrations)
- Supports scaffolding from existing databases
- Can generate idempotent SQL scripts for production deployment
- Migration history tracked in __EFMigrationsHistory table

**Cons:**
- Tight coupling between application code and database schema
- Generated SQL may not be optimized for complex scenarios
- Limited control over the exact SQL executed
- More complex migrations may require manual customization
- Can be challenging to integrate with existing databases
- May add unnecessary complexity for simple schema changes

### DACPAC (SQL Server Data-Tier Application Package)

**Pros:**
- Comprehensive schema management with state-based approach
- Strong tooling support in Visual Studio and Azure Data Studio
- Detailed comparison and deployment reports
- Supports pre/post deployment scripts
- Can include reference data
- Good for SQL Server projects

**Cons:**
- Limited support for PostgreSQL (primarily a SQL Server technology)
- Requires SSDT (SQL Server Data Tools) or equivalent
- State-based approach may cause issues with data preservation
- Less flexible than script-based approaches for complex changes
- More challenging to integrate with continuous integration

### Other Solutions

#### Flyway

**Pros:**
- Cross-platform with strong PostgreSQL support
- Versioned migration scripts with clear naming conventions
- Command-line tools and Java API
- Supports baseline for existing databases
- Community and commercial editions available

**Cons:**
- Java-based, requiring additional runtime dependencies
- Less native integration with .NET ecosystem
- Limited free features in the community edition

#### Evolve

**Pros:**
- .NET migration tool inspired by Flyway
- Native .NET integration
- Support for multiple databases including PostgreSQL
- Simple, focused API
- Transaction support

**Cons:**
- Smaller community compared to other options
- Less mature than alternatives
- Fewer integrations with development tools

## Implementation Recommendation

Based on the requirements of ProPulse, particularly the need for clear separation of concerns, integration with Aspire, and support for PostgreSQL, the recommended database migration approach is:

**Primary Approach: DbUp in a dedicated migration project**

This combination provides:
1. Clean separation of database migration code from application code
2. Excellent integration with dotnet Aspire through direct AppHost configuration
3. Fine-grained control over SQL scripts for PostgreSQL-specific features
4. Simple versioning of database schema changes
5. Easy integration with CI/CD pipelines
6. Support for all target environments including development, test, staging, and production

The architecture will use:
- A dedicated DbUp project within the solution
- Script naming convention with sequential numbering (e.g., Script001_CreateBaseSchema.sql)
- Embedded SQL scripts as resources
- Separate folders for schema, data, and reference scripts
- Integration with Aspire AppHost for local development

This approach maintains a clear separation between the database schema evolution and application code, while providing the necessary flexibility to handle complex database changes and PostgreSQL-specific features needed for ProPulse.
