## Social Media Post Management

**Objective:** Implement social media post creation and scheduling.

**Steps:**

1.  **Define Social Media Post Commands and Queries:**
    *   In the `ProPulse.Web` project, create the following commands and queries in the appropriate folders:
        *   `CreateSocialMediaPostCommand`
        *   `UpdateSocialMediaPostCommand`
        *   `DeleteSocialMediaPostCommand`
        *   `GetSocialMediaPostQuery`
        *   `ListSocialMediaPostsQuery`
        *   `ApproveSocialMediaPostCommand`
        *   `RejectSocialMediaPostCommand`
2.  **Implement Social Media Post Handlers:**
    *   Create handler classes for each command and query that implement the `IRequestHandler<TRequest, TResponse>` interface.
    *   Implement the business logic for each handler, including:
        *   Creating a new social media post in the database.
        *   Updating an existing social media post in the database.
        *   Deleting a social media post from the database.
        *   Retrieving a social media post from the database by ID.
        *   Listing social media posts for a user or social media account.
        *   Approving a social media post.
        *   Rejecting a social media post.
3.  **Implement Social Media Post Controller:**
    *   Create a `SocialMediaPostsController` class that inherits from `BaseApiController`.
    *   Define API endpoints for CRUD operations using appropriate HTTP methods (GET, POST, PUT, DELETE).
    *   Define API endpoints for approving and rejecting social media posts.
    *   Use MediatR to dispatch commands and queries from the API endpoints to the corresponding handlers.
4.  **Implement Scheduling Logic:**
    *   Use a background task scheduler (e.g., Hangfire) to schedule social media posts for publishing.
    *   Implement logic to publish social media posts to the appropriate social media platforms.
5.  **Implement Approval Workflow:**
    *   Implement an approval workflow for social media posts.
    *   Only approved posts should be scheduled for publishing.
6.  **Implement Validation:**
    *   Create validation classes for `CreateSocialMediaPostCommand` and `UpdateSocialMediaPostCommand` using FluentValidation.
    *   Validate the following properties:
        *   `Content` (not empty, maximum length)
        *   `SocialMediaAccountId` (required)
        *   `ScheduledDate` (required, in the future)
    *   Implement a MediatR pipeline behavior for validation.
7.  **Add Integration Tests:**
    *   In the `ProPulse.Web.Tests` project, create integration tests for the social media post management endpoints.
    *   Test CRUD operations for social media posts.
    *   Test validation and exception handling.
    *   Test scheduling logic.
    *   Test approval workflow.

**Projects Affected:**

*   `ProPulse.Web`
*   `ProPulse.Core`

**Class Diagram:**

```mermaid
classDiagram
    class SocialMediaPostsController {
        GET /social-posts
        POST /social-posts
        GET /social-posts/{id}
        PUT /social-posts/{id}
        DELETE /social-posts/{id}
        PUT /social-posts/{id}/approve
        PUT /social-posts/{id}/reject
    }
    class CreateSocialMediaPostCommand {
        string Content
        string ArticleId
        string SocialMediaAccountId
        DateTimeOffset ScheduledDate
    }
    class UpdateSocialMediaPostCommand {
        string Id
        string Content
        string ArticleId
        string SocialMediaAccountId
        DateTimeOffset ScheduledDate
    }
    class DeleteSocialMediaPostCommand {
        string Id
    }
    class GetSocialMediaPostQuery {
        string Id
    }
    class ListSocialMediaPostsQuery {
        string UserId
        string SocialMediaAccountId
    }
     class ApproveSocialMediaPostCommand {
        string Id
    }
    class RejectSocialMediaPostCommand {
        string Id
    }
    SocialMediaPostsController --> CreateSocialMediaPostCommand
    SocialMediaPostsController --> UpdateSocialMediaPostCommand
    SocialMediaPostsController --> DeleteSocialMediaPostCommand
    SocialMediaPostsController --> GetSocialMediaPostQuery
    SocialMediaPostsController --> ListSocialMediaPostsQuery
    SocialMediaPostsController --> ApproveSocialMediaPostCommand
    SocialMediaPostsController --> RejectSocialMediaPostCommand
```

**Design Patterns & Best Practices:**

*   Use MediatR for decoupling controllers from business logic.
*   Implement the CQRS pattern to separate read and write operations.
*   Use FluentValidation for model validation.
*   Implement a global exception handler for consistent error handling.
*   Use a consistent API response format.
*   Follow RESTful API design principles.
*   Use a background task scheduler for scheduling social media posts.
*   Implement proper error handling for publishing to social media platforms.

**Definition of Done:**

*   \[x] Social media post commands and queries are defined in the `ProPulse.Web` project.
*   \[x] Social media post handlers are implemented for each command and query.
*   \[x] `SocialMediaPostsController` class is created with API endpoints for CRUD operations.
*   \[x] Scheduling logic is implemented using a background task scheduler.
*   \[x] Approval workflow is implemented.
*   \[x] Validation is implemented for `CreateSocialMediaPostCommand` and `UpdateSocialMediaPostCommand` using FluentValidation.
*   \[x] Integration tests are created for social media post management endpoints.
*   \[x] All tests pass successfully.
*   \[x] Initial commit with social media post management implementation is created.
