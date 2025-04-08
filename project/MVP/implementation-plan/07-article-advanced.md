## Article Management - Advanced

**Objective:** Add ratings, attachments, and publishing features for articles.

**Steps:**

1.  **Implement Rating Functionality:**
    *   Define commands and queries for:
        *   Creating a rating (`CreateRatingCommand`)
        *   Updating a rating (`UpdateRatingCommand`)
        *   Deleting a rating (`DeleteRatingCommand`)
        *   Getting article ratings (`GetArticleRatingsQuery`)
    *   Implement handler classes for each command and query.
    *   Add endpoints to the `ArticlesController` for rating operations.
    *   Implement validation for rating values (1-5).
2.  **Implement Attachment Functionality:**
    *   Define commands and queries for:
        *   Adding an attachment (`AddAttachmentCommand`)
        *   Deleting an attachment (`DeleteAttachmentCommand`)
        *   Getting article attachments (`GetArticleAttachmentsQuery`)
    *   Implement handler classes for each command and query.
    *   Add endpoints to the `ArticlesController` for attachment operations.
    *   Implement validation for attachment file types and sizes.
    *   Integrate with Azure Blob Storage for storing attachments.
3.  **Implement Publishing Functionality:**
    *   Define commands for:
        *   Publishing an article (`PublishArticleCommand`)
        *   Unpublishing an article (`UnpublishArticleCommand`)
    *   Implement handler classes for each command.
    *   Add endpoints to the `ArticlesController` for publishing operations.
    *   Update the `Article` model to include `PublishedAt` and `PublishedUntil` properties.
    *   Implement logic to set the `PublishedAt` property when an article is published and clear it when unpublished.
4.  **Add Integration Tests:**
    *   In the `ProPulse.Web.Tests` project, create integration tests for the new functionality.
    *   Test rating, attachment, and publishing operations.

**Projects Affected:**

*   `ProPulse.Web`
*   `ProPulse.Core`

**Class Diagram:**

```mermaid
classDiagram
    class ArticlesController {
        GET /articles/{id}/ratings
        POST /articles/{id}/ratings
        PUT /articles/{id}/ratings
        DELETE /articles/{id}/ratings
        POST /articles/{id}/attachments
        GET /articles/{id}/attachments
        DELETE /articles/{id}/attachments/{attachmentId}
        PUT /articles/{id}/publish
        PUT /articles/{id}/unpublish
    }
    class CreateRatingCommand {
        string ArticleId
        int Score
    }
    class UpdateRatingCommand {
        string ArticleId
        int Score
    }
    class DeleteRatingCommand {
        string ArticleId
    }
    class GetArticleRatingsQuery {
        string ArticleId
    }
    class AddAttachmentCommand {
        string ArticleId
        IFormFile File
    }
    class DeleteAttachmentCommand {
        string ArticleId
        string AttachmentId
    }
    class GetArticleAttachmentsQuery {
        string ArticleId
    }
    class PublishArticleCommand {
        string ArticleId
    }
    class UnpublishArticleCommand {
        string ArticleId
    }
    ArticlesController --> CreateRatingCommand
    ArticlesController --> UpdateRatingCommand
    ArticlesController --> DeleteRatingCommand
    ArticlesController --> GetArticleRatingsQuery
    ArticlesController --> AddAttachmentCommand
    ArticlesController --> DeleteAttachmentCommand
    ArticlesController --> GetArticleAttachmentsQuery
    ArticlesController --> PublishArticleCommand
    ArticlesController --> UnpublishArticleCommand
```

**Design Patterns & Best Practices:**

*   Use MediatR for decoupling controllers from business logic.
*   Implement the CQRS pattern to separate read and write operations.
*   Use FluentValidation for model validation.
*   Implement a global exception handler for consistent error handling.
*   Use a consistent API response format.
*   Follow RESTful API design principles.
*   Use Azure Blob Storage for storing attachments.
*   Implement proper security measures for file uploads.

**Definition of Done:**

*   \[x] Rating functionality is implemented with commands, queries, handlers, and API endpoints.
*   \[x] Attachment functionality is implemented with commands, queries, handlers, API endpoints, and Azure Blob Storage integration.
*   \[x] Publishing functionality is implemented with commands, handlers, API endpoints, and updates to the `Article` model.
*   \[x] Integration tests are created for rating, attachment, and publishing operations.
*   \[x] All tests pass successfully.
*   \[x] Initial commit with article management advanced implementation is created.
