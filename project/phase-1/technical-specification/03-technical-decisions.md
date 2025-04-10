# 3. Technical Decisions

## 3.1. Database Layer

### ID Format

Choosing the right ID format is critical for ensuring scalability, uniqueness, and ease of use. Below are the options considered:

#### 1. UUIDv4
- **Description**: Randomly generated 128-bit identifier.
- **Comparison**:
  - **Pros**: Universally unique, widely supported, and easy to generate.
  - **Cons**: Lack of temporal ordering can lead to inefficient indexing.
- **Use Case**: Suitable for distributed systems where uniqueness is the primary concern.

#### 2. UUIDv7
- **Description**: Time-ordered UUID based on the upcoming UUIDv7 standard.
- **Comparison**:
  - **Pros**: Combines temporal ordering with uniqueness, improving indexing performance.
  - **Cons**: Slightly less mature and supported compared to UUIDv4.
- **Use Case**: Ideal for systems requiring both uniqueness and efficient indexing.

#### 3. ULID (Universally Unique Lexicographically Sortable Identifier)
- **Description**: 128-bit identifier with a timestamp component.
- **Comparison**:
  - **Pros**: Human-readable, lexicographically sortable, and efficient for indexing.
  - **Cons**: Slightly less widely supported than UUIDs.
- **Use Case**: Suitable for systems requiring human readability and temporal ordering.

#### 4. Snowflake ID
- **Description**: 64-bit identifier with a timestamp, machine ID, and sequence number.
- **Comparison**:
  - **Pros**: Compact, highly efficient, and sortable.
  - **Cons**: Requires a central coordination service or careful configuration to avoid collisions.
- **Use Case**: Commonly used in distributed systems like social media platforms.

#### 5. NanoID
- **Description**: Customizable, URL-friendly identifier.
- **Comparison**:
  - **Pros**: Compact, customizable, and URL-safe.
  - **Cons**: Lack of temporal ordering and less standardization.
- **Use Case**: Suitable for systems requiring short, URL-friendly IDs.

#### Decision
We have chosen **UUIDv7** as the ID format for this project. It provides a good balance between uniqueness, temporal ordering, and indexing efficiency. While UUIDv4 is more widely supported, the temporal ordering of UUIDv7 makes it a better fit for our use case, especially for database indexing and query performance.

### PostgreSQL Extensions

#### 1. `pg_trgm` (Trigram Matching)
- **Use Case**: Improves text search capabilities, such as fuzzy matching for search functionality.
- **Comparison**:
  - **Pros**: Enhances user experience by allowing partial or misspelled search terms to return relevant results.
  - **Cons**: Adds slight overhead to indexing and storage.
- **Decision**: Include `pg_trgm` to improve search functionality for articles and tags.

#### 2. `PostGIS`
- **Use Case**: Adds support for geographic objects, enabling location-based features.
- **Comparison**:
  - **Pros**: Essential for geolocation features.
  - **Cons**: Unnecessary if no geolocation features are planned.
- **Decision**: Exclude `PostGIS` for now, as geolocation is not part of the MVP scope.

#### 3. `uuid-ossp`
- **Use Case**: Generates universally unique identifiers (UUIDs).
- **Comparison**:
  - **Pros**: Ensures unique primary keys across distributed systems.
  - **Cons**: Slightly larger storage size compared to integers.
- **Decision**: Include `uuid-ossp` for generating UUIDs for primary keys.

#### 4. `pg_stat_statements`
- **Use Case**: Tracks execution statistics of SQL queries.
- **Comparison**:
  - **Pros**: Helps in performance tuning by identifying slow queries.
  - **Cons**: Adds slight overhead to query execution.
- **Decision**: Include `pg_stat_statements` for performance monitoring.

#### 5. `hstore`
- **Use Case**: Stores key-value pairs within a single column.
- **Comparison**:
  - **Pros**: Useful for semi-structured data.
  - **Cons**: Limited querying capabilities compared to JSONB.
- **Decision**: Exclude `hstore` in favor of JSONB for greater flexibility.

#### 6. `citext`
- **Use Case**: Provides case-insensitive text columns.
- **Comparison**:
  - **Pros**: Simplifies case-insensitive comparisons.
  - **Cons**: Slightly less efficient than regular text columns.
- **Decision**: Include `citext` for case-insensitive fields like usernames and tags.

#### 7. `pgcrypto`
- **Use Case**: Provides cryptographic functions.
- **Comparison**:
  - **Pros**: Useful for encrypting sensitive data.
  - **Cons**: Adds complexity to database operations.
- **Decision**: Include `pgcrypto` for encrypting sensitive data like API keys.

### PostgreSQL Features

#### 1. Triggers
- **Use Case**: Automates actions based on database events.
- **Comparison**:
  - **Pros**: Reduces application logic by handling repetitive tasks at the database level.
  - **Cons**: Can make debugging more complex.
- **Decision**: Use triggers for tasks like updating `last_modified` timestamps and enforcing business rules.

#### 2. Enums
- **Use Case**: Defines a fixed set of values for a column.
- **Comparison**:
  - **Pros**: Ensures data integrity and simplifies validation.
  - **Cons**: Requires schema changes to update values.
- **Decision**: Use enums for fields like article status (`DRAFT`, `REVIEW`, `PUBLISHED`) and user roles (`AUTHOR`, `ADMIN`, `READER`).

#### 3. Partitioning
- **Use Case**: Splits large tables into smaller, more manageable pieces.
- **Comparison**:
  - **Pros**: Improves query performance and simplifies data management.
  - **Cons**: Adds complexity to table design and maintenance.
- **Decision**: Exclude partitioning for now, as the expected data volume does not justify the added complexity.

#### 4. JSON/JSONB
- **Use Case**: Stores semi-structured data in a column.
- **Comparison**:
  - **Pros**: Provides flexibility for evolving data models.
  - **Cons**: Slightly less efficient than structured columns for querying.
- **Decision**: Use JSONB for flexible metadata storage, such as article settings and user preferences.

#### 5. Full-Text Search
- **Use Case**: Enables advanced search capabilities.
- **Comparison**:
  - **Pros**: Improves search performance and relevance.
  - **Cons**: Requires additional indexing and configuration.
- **Decision**: Use PostgreSQL's built-in full-text search for article content and tags.

#### 6. Foreign Data Wrappers (FDW)
- **Use Case**: Access external data sources as if they were local tables.
- **Comparison**:
  - **Pros**: Simplifies data integration and migration.
  - **Cons**: Adds complexity to query execution.
- **Decision**: Exclude FDW for now, as no external data sources are planned for the MVP.

### Deletion Strategy

#### Purpose of Deletion Strategy
Managing deletions in a heavily normalized database schema is critical to maintaining data integrity and avoiding orphaned records. Articles, attachments, tags, ratings, and comments, as well as user-generated content, must be handled carefully to ensure consistency.

#### Options for Deletion Handling
1. **Cascade Delete**
   - **Description**: Automatically deletes related records when a parent record is deleted.
   - **Comparison**:
     - **Pros**: Simple to implement, ensures data integrity by preventing orphaned records.
     - **Cons**: Can lead to unintended data loss if not carefully configured.
   - **Use Case**: Suitable for relationships where dependent data should always be removed with the parent (e.g., comments and attachments tied to an article).

2. **Soft Delete with Cleanup Service**
   - **Description**: Marks records as deleted (e.g., with a `DeletedAt` timestamp) and uses a background service to clean up data.
   - **Comparison**:
     - **Pros**: Allows for recovery of deleted data, provides an audit trail.
     - **Cons**: Requires additional infrastructure and logic for cleanup.
   - **Use Case**: Suitable for critical data where recovery or auditing is required (e.g., user accounts).

#### Decision
We will use a combination of **Cascade Delete** and a **Backend Cleanup Service**:
- **Cascade Delete**: For relationships where dependent data should always be removed with the parent, such as:
  - Comments, tags, and attachments tied to an article.
  - Ratings tied to a user or article.
- **Backend Cleanup Service**: For soft-deleted records that require delayed or conditional cleanup, such as:
  - User accounts marked for deletion.
  - Articles flagged for archival instead of immediate removal.

This hybrid approach ensures data integrity while providing flexibility for recovery and auditing when needed.

## 3.2. Content Storage

### Article Content Format
- **Decision**: Articles will be stored in **Markdown** format.
- **Reasoning**:
  - **Human-Readable**: Markdown is easy to read and write for authors.
  - **Flexibility**: Supports rich text formatting while remaining lightweight.
  - **Compatibility**: Can be easily converted to HTML for rendering in the web frontend.

### Media Attachments
- **Decision**: Authors will be able to attach images and videos to their articles.
- **Implementation Details**:
  - **Logical Name**: Media files will be referenced within the Markdown document using logical names (e.g., `![Image Description](image-name.jpg)`).
  - **Physical Storage**: Media files will be stored in a physical location relative to the selected media storage solution (e.g., Azure Blob Storage or Azurite during development).
  - **Storage Structure**: Media files will be organized by article ID to ensure efficient retrieval and avoid naming conflicts.

### Media Storage Options
- **Azure Blob Storage**:
  - **Pros**: Scalable, secure, and integrates well with the Azure ecosystem.
  - **Cons**: Requires additional configuration and costs for storage and bandwidth.
- **Azurite - an Azure Storage emulator**:
  - **Pros**: Simple to set up and use during development; Containerized for easy deployment.
  - **Cons**: Not suitable for production due to scalability and reliability concerns.
- **Decision**: Use Azurite for development and Azure Blob Storage for staging and production environments.

## API Layer

### API Technology Choices

#### 1. REST (Representational State Transfer)
- **Description**: A widely adopted architectural style using HTTP methods.
- **Comparison**:
  - **Pros**: Simple, widely supported, and mature tooling.
  - **Cons**: Can lead to over-fetching or under-fetching of data.
- **Use Case**: General-purpose APIs with predictable data requirements.

#### 2. OData (Open Data Protocol)
- **Description**: A standardized protocol for querying and updating data.
- **Comparison**:
  - **Pros**: Metadata-driven, supports advanced querying.
  - **Cons**: Higher complexity and less flexibility compared to REST.
- **Use Case**: Data-centric APIs requiring standardized querying.

#### 3. GraphQL
- **Description**: A query language for APIs that allows clients to request specific data.
- **Comparison**:
  - **Pros**: Single endpoint, precise data fetching, introspection.
  - **Cons**: Higher complexity and potential for overloading the server with complex queries.
- **Use Case**: APIs with complex and dynamic data requirements.

#### 4. gRPC (gRPC Remote Procedure Calls)
- **Description**: A high-performance RPC framework.
- **Comparison**:
  - **Pros**: Efficient binary protocol, excellent for internal services.
  - **Cons**: Requires interface definition and is less human-readable.
- **Use Case**: High-performance, internal services.

#### Decision
We have chosen **REST** for the API layer due to its simplicity, wide adoption, and mature tooling. REST aligns well with the project's requirements and allows for future evolution to GraphQL if needed.

### API Implementation Style

#### 1. Minimal APIs
- **Description**: Lightweight, function-based approach to building APIs.
- **Comparison**:
  - **Pros**: Simple, fast to set up, and minimal boilerplate.
  - **Cons**: Less structured, can become difficult to manage in larger projects.
- **Use Case**: Small, simple APIs or microservices.

#### 2. Controllers
- **Description**: Traditional, structured approach using MVC-style controllers.
- **Comparison**:
  - **Pros**: Well-organized, supports complex routing and middleware.
  - **Cons**: More boilerplate compared to Minimal APIs.
- **Use Case**: Larger projects requiring clear organization.

#### 3. FastEndpoints
- **Description**: A library for building REST APIs with a focus on vertical slice architecture.
- **Comparison**:
  - **Pros**: Aligns with vertical slice architecture, reduces boilerplate, and improves maintainability.
  - **Cons**: Requires learning a new library.
- **Use Case**: Projects using vertical slice architecture.

#### Decision
We have chosen **FastEndpoints** for the API implementation style. It aligns with the vertical slice architecture and simplifies the development of self-contained features.

### Data Access Pattern

#### 1. Standard Access
- **Description**: Directly interacts with the database through an ORM or raw queries.
- **Comparison**:
  - **Pros**: Simple and straightforward.
  - **Cons**: Can lead to tightly coupled code.
- **Use Case**: Small projects or simple data access requirements.

#### 2. CQRS (Command Query Responsibility Segregation)
- **Description**: Separates read and write operations into distinct models.
- **Comparison**:
  - **Pros**: Improves scalability and performance, aligns with vertical slice architecture.
  - **Cons**: Adds complexity and requires more code.
- **Use Case**: Projects with complex business logic or high scalability requirements.

#### Decision
We have chosen **CQRS** for the data access pattern. It aligns with the vertical slice architecture and supports scalability and maintainability.

### Caching

#### Purpose of Caching
Caching is essential for improving API performance and reducing load on the database by storing frequently accessed data in memory. It can also enhance user experience by reducing response times.

#### Caching Strategies
1. **In-Memory Caching**
   - **Description**: Stores data in the memory of the application server.
   - **Comparison**:
     - **Pros**: Simple to implement, fast access.
     - **Cons**: Limited scalability, data is lost on server restarts.
   - **Use Case**: Suitable for small-scale applications or caching ephemeral data.

2. **Distributed Caching**
   - **Description**: Stores data in a distributed cache, such as Redis.
   - **Comparison**:
     - **Pros**: Scalable, shared across multiple application instances, persistent.
     - **Cons**: Requires additional infrastructure and configuration.
   - **Use Case**: Ideal for large-scale applications with multiple servers.

#### Cache Invalidation Strategies
- **Time-Based Expiry**: Automatically removes cached data after a specified duration.
- **Event-Based Invalidation**: Removes or updates cached data when specific events occur (e.g., database updates).
- **Manual Invalidation**: Allows developers to explicitly clear the cache when needed.

#### Decision
We have chosen to use **Redis** for distributed caching in staging and production environments due to its scalability and performance. For development, we will use **in-memory caching** to simplify setup. Cache invalidation will primarily use a combination of time-based expiry and event-based invalidation to ensure data consistency.

## Web Layer

### Web Technology Choices

#### 1. Razor Pages
- **Description**: A lightweight framework for building server-side rendered (SSR) web applications.
- **Comparison**:
  - **Pros**: Simple to use, tightly integrated with ASP.NET Core, and good for SEO.
  - **Cons**: Limited interactivity compared to modern client-side frameworks.
- **Use Case**: Suitable for simple, server-rendered applications.

#### 2. MVC (Model-View-Controller)
- **Description**: A traditional framework for building server-side web applications with a clear separation of concerns.
- **Comparison**:
  - **Pros**: Well-structured, supports complex applications, and good for SEO.
  - **Cons**: More boilerplate compared to Razor Pages.
- **Use Case**: Suitable for large, server-rendered applications with complex requirements.

#### 3. Blazor
- **Description**: A framework for building interactive web UIs using C# instead of JavaScript.
- **Comparison**:
  - **Pros**: Allows full-stack development in C#, supports both server-side and WebAssembly (WASM) hosting models.
  - **Cons**: Larger payload size for WASM, less mature ecosystem compared to JavaScript frameworks.
- **Use Case**: Suitable for applications where C# expertise is preferred and interactivity is required.

#### 4. Progressive Web Applications (PWAs)
- **Description**: Client-side frameworks like React, Vue, and Angular for building highly interactive web applications.
- **Comparison**:
  - **React**:
    - **Pros**: Large ecosystem, excellent for building scalable and interactive UIs.
    - **Cons**: Requires additional libraries for state management and routing.
  - **Vue**:
    - **Pros**: Simple to learn, good for small to medium-sized applications.
    - **Cons**: Smaller ecosystem compared to React.
  - **Angular**:
    - **Pros**: Full-featured framework with built-in state management and routing.
    - **Cons**: Steeper learning curve and heavier framework.
- **Use Case**: Suitable for highly interactive, client-rendered applications.

#### Decision
We have chosen a hybrid approach:
- **Reader Section**: Implemented as **Razor Pages** to ensure scalability and interactivity for a large number of readers.
- **Authoring Environment**: Implemented using **Razor Pages** with **JavaScript** to provide some complex interactivity using **jQuery** modules.

### CSS vs. SCSS
- **CSS**:
  - **Pros**: Simple to use, widely supported.
  - **Cons**: Limited features for complex styling.
- **SCSS**:
  - **Pros**: Supports variables, nesting, and other advanced features.
  - **Cons**: Requires a build step.
- **Decision**: Use **SCSS** for advanced styling components.
  - Potential libraries include AspNetCore.SassCompiler and LigerShark.Sass

## Build and Deployment Process

- Use .NET Aspire to build and run the development environment.
  - Additional containers such as DbGate can be used to generate developer aids and provide repeatable deployments.
  - The Web application will automatically build and cache the SCSS components into CSS for serving.
- Use Bicep and Azure Developer CLI to automate production and staging deployments.

ProPulse will use GitHub Actions to automatically build and test the application when feature check-ins are made.

## Testing Strategy

### Overview
The testing strategy for ProPulse will ensure the reliability, performance, and security of the system. It will include multiple layers of testing to cover different aspects of the application, from individual components to the entire system.

### Testing Types

#### 1. Unit Testing
- **Purpose**: Validate the functionality of individual components or methods in isolation.
- **Tools**: 
  - **xUnit**: For writing and running unit tests.
  - **NSubstitute**: For mocking dependencies.
  - **AwesomeAssertions**: For expressive and readable assertions.
- **Scope**:
  - Business logic in the API layer.
  - Utility functions and helper methods.
  - Database interactions using in-memory SQLite.

#### 2. Integration Testing
- **Purpose**: Test the interaction between multiple components to ensure they work together as expected.
- **Tools**:
  - **xUnit**: For writing and running integration tests.
  - **TestServer**: For hosting the API in-memory during tests.
  - **SQLite**: For simulating the database in a controlled environment.
- **Scope**:
  - API endpoints and their interaction with the database.
  - Authentication and authorization flows.
  - Data consistency and integrity.

#### 3. End-to-End (E2E) Testing
- **Purpose**: Validate the entire system from the user's perspective.
- **Tools**:
  - **Playwright**: For simulating user interactions with the web frontend.
  - **Azure Test Plans**: For managing and tracking test cases.
- **Scope**:
  - User registration and login.
  - Article creation, editing, and publishing workflows.
  - Reader interactions, such as searching and viewing articles.

#### 4. Performance Testing
- **Purpose**: Ensure the system meets performance requirements under expected and peak loads.
- **Tools**:
  - **Apache JMeter**: For simulating concurrent users and measuring response times.
  - **Azure Monitor**: For tracking performance metrics in staging and production.
- **Scope**:
  - API response times.
  - Database query performance.
  - Scalability of the web frontend.

#### 5. Security Testing
- **Purpose**: Identify and mitigate security vulnerabilities.
- **Tools**:
  - **OWASP ZAP**: For automated security scanning.
  - **Azure Security Center**: For monitoring and managing security risks.
- **Scope**:
  - SQL injection and XSS vulnerabilities.
  - Authentication and authorization mechanisms.
  - Secure storage and transmission of sensitive data.

### CI/CD Integration
- **Automated Testing**: All unit and integration tests will run automatically as part of the CI pipeline.
- **Code Coverage**: Use tools like **Coverlet** to measure and enforce code coverage thresholds.
- **Test Environments**:
  - Development: Local testing with in-memory databases.
  - Test: Automated tests against a SQLite database.
  - Staging: Manual and automated tests in a production-like environment.

### Reporting and Monitoring
- **Test Reports**: Generate detailed test reports using tools like **ReportGenerator**.
- **Error Tracking**: Use **Azure Application Insights** to monitor and log errors in staging and production.

### Decision
This multi-layered testing strategy ensures that the system is robust, secure, and performs well under various conditions. By leveraging modern tools and frameworks, we can maintain high-quality standards throughout the development lifecycle.

