## Article Management - Core

**Objective:** Implement basic article creation and management.

**Steps:**

1.  **Define Article Commands and Queries:**
    *   In the `ProPulse.Web` project, create the following commands and queries in the appropriate folders:
        *   `CreateArticleCommand`
        *   `UpdateArticleCommand`
        *   `DeleteArticleCommand`
        *   `GetArticleQuery`
        *   `ListArticlesQuery`
2.  **Implement Article Handlers:**
    *   Create handler classes for each command and query that implement the `IRequestHandler<TRequest, TResponse>` interface.
    *   Implement the business logic for each handler, including:
        *   Creating a new article in the database.
        *   Updating an existing article in the database.
        *   Deleting an article from the database.
        *   Retrieving an article from the database by ID.
        *   Listing articles with filtering and pagination.
3.  **Implement Article Controller:**
    *   Create an `ArticlesController` class that inherits from `BaseApiController`.
    *   Define API endpoints for CRUD operations using appropriate HTTP methods (GET, POST, PUT, DELETE).
    *   Use MediatR to dispatch commands and queries from the API endpoints to the corresponding handlers.
4.  **Implement Validation:**
    *   Create validation classes for `CreateArticleCommand` and `UpdateArticleCommand` using FluentValidation.
    *   Validate the following properties:
        *   `Title` (not empty, minimum length, maximum length)
        *   `Summary` (not empty, maximum length)
        *   `Content` (not empty)
    *   Implement a MediatR pipeline behavior for validation.
5.  **Add Integration Tests:**
    *   In the `ProPulse.Web.Tests` project, create integration tests for the article management endpoints.
    *   Test CRUD operations for articles.
    *   Test validation and exception handling.

**Projects Affected:**

*   `ProPulse.Web`

**Class Diagram:**

```mermaid
classDiagram
    class ArticlesController {
        GET /articles
        POST /articles
        GET /articles/{id}
        PUT /articles/{id}
        DELETE /articles/{id}
    }
    class CreateArticleCommand {
        string Title
        string Summary
        string Content
    }
    class UpdateArticleCommand {
        string Id
        string Title
        string Summary
        string Content
    }
    class DeleteArticleCommand {
        string Id
    }
    class GetArticleQuery {
        string Id
    }
    class ListArticlesQuery {
        string Filter
        string Sort
        int Page
        int PageSize
    }
    ArticlesController --> CreateArticleCommand
    ArticlesController --> UpdateArticleCommand
    ArticlesController --> DeleteArticleCommand
    ArticlesController --> GetArticleQuery
    ArticlesController --> ListArticlesQuery
```

**Design Patterns & Best Practices:**

*   Use MediatR for decoupling controllers from business logic.
*   Implement the CQRS pattern to separate read and write operations.
*   Use FluentValidation for model validation.
*   Implement a global exception handler for consistent error handling.
*   Use a consistent API response format.
*   Follow RESTful API design principles.

**Definition of Done:**

*   \[x] Article commands and queries are defined in the `ProPulse.Web` project.
*   \[x] Article handlers are implemented for each command and query.
*   \[x] `ArticlesController` class is created with API endpoints for CRUD operations.
*   \[x] Validation is implemented for `CreateArticleCommand` and `UpdateArticleCommand` using FluentValidation.
*   \[x] Integration tests are created for article management endpoints.
*   \[x] All tests pass successfully.
*   \[x] Initial commit with article management core implementation is created.
