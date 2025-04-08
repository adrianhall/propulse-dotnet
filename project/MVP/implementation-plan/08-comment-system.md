## Comment System

**Objective:** Implement comment functionality including nested comments.

**Steps:**

1.  **Define Comment Commands and Queries:**
    *   In the `ProPulse.Web` project, create the following commands and queries in the appropriate folders:
        *   `CreateCommentCommand`
        *   `UpdateCommentCommand`
        *   `DeleteCommentCommand`
        *   `GetCommentQuery`
        *   `ListCommentsQuery`
2.  **Implement Comment Handlers:**
    *   Create handler classes for each command and query that implement the `IRequestHandler<TRequest, TResponse>` interface.
    *   Implement the business logic for each handler, including:
        *   Creating a new comment in the database.
        *   Updating an existing comment in the database.
        *   Deleting a comment from the database.
        *   Retrieving a comment from the database by ID.
        *   Listing comments for an article with nested comments.
3.  **Implement Comment Controller:**
    *   Create a `CommentsController` class that inherits from `BaseApiController`.
    *   Define API endpoints for CRUD operations using appropriate HTTP methods (GET, POST, PUT, DELETE).
    *   Use MediatR to dispatch commands and queries from the API endpoints to the corresponding handlers.
4.  **Implement Validation:**
    *   Create validation classes for `CreateCommentCommand` and `UpdateCommentCommand` using FluentValidation.
    *   Validate the following properties:
        *   `Content` (not empty, maximum length)
    *   Implement a MediatR pipeline behavior for validation.
5.  **Implement Nested Comments:**
    *   Update the `Comment` model to include a `ParentCommentId` property.
    *   Implement logic to retrieve comments with nested comments.
6.  **Add Integration Tests:**
    *   In the `ProPulse.Web.Tests` project, create integration tests for the comment system endpoints.
    *   Test CRUD operations for comments.
    *   Test validation and exception handling.
    *   Test nested comments functionality.

**Projects Affected:**

*   `ProPulse.Web`
*   `ProPulse.Core`

**Class Diagram:**

```mermaid
classDiagram
    class CommentsController {
        GET /articles/{articleId}/comments
        POST /articles/{articleId}/comments
        GET /comments/{id}
        PUT /comments/{id}
        DELETE /comments/{id}
    }
    class CreateCommentCommand {
        string ArticleId
        string Content
        string ParentCommentId
    }
    class UpdateCommentCommand {
        string Id
        string Content
    }
    class DeleteCommentCommand {
        string Id
    }
    class GetCommentQuery {
        string Id
    }
    class ListCommentsQuery {
        string ArticleId
    }
    CommentsController --> CreateCommentCommand
    CommentsController --> UpdateCommentCommand
    CommentsController --> DeleteCommentCommand
    CommentsController --> GetCommentQuery
    CommentsController --> ListCommentsQuery
```

**Design Patterns & Best Practices:**

*   Use MediatR for decoupling controllers from business logic.
*   Implement the CQRS pattern to separate read and write operations.
*   Use FluentValidation for model validation.
*   Implement a global exception handler for consistent error handling.
*   Use a consistent API response format.
*   Follow RESTful API design principles.
*   Use recursive queries or CTEs for retrieving nested comments efficiently.

**Definition of Done:**

*   \[x] Comment commands and queries are defined in the `ProPulse.Web` project.
*   \[x] Comment handlers are implemented for each command and query.
*   \[x] `CommentsController` class is created with API endpoints for CRUD operations.
*   \[x] Validation is implemented for `CreateCommentCommand` and `UpdateCommentCommand` using FluentValidation.
*   \[x] Nested comments functionality is implemented.
*   \[x] Integration tests are created for comment system endpoints.
*   \[x] All tests pass successfully.
*   \[x] Initial commit with comment system implementation is created.
