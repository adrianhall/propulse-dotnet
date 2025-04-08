# 4. API Surface

The ProPulse platform will be built with an API-driven architecture, implementing a comprehensive REST-like API that serves as the foundation for all client interactions. This API will utilize modern web standards and practices including OData query capabilities, pagination, and RFC9110 conditional updates to ensure flexibility, performance, and compatibility.

## 4.1. API Design Principles

The API design will adhere to the following principles:

- **Resource-Oriented Architecture**: API endpoints represent resources rather than actions
- **Uniform Interface**: Consistent HTTP methods and response structures
- **Statelessness**: Each request contains all information necessary to process it
- **Cacheable**: Responses explicitly labeled as cacheable when appropriate
- **Layered System**: Client cannot tell whether it's connected directly to the end server
- **Versioned**: API versioning to manage changes and ensure backward compatibility

## 4.2. API Base Structure

### Base URL

```
https://api.propulse.com/api/v1
```

All API endpoints are under the `/api` path prefix (e.g., `/api/auth/register`, `/api/articles`, etc.). API versioning will be implemented through URL paths rather than headers to ensure maximum compatibility with all clients.

### Authentication

All API endpoints, except those explicitly marked as public, require authentication using JWT bearer tokens:

```
Authorization: Bearer {token}
```

Tokens will be obtained through the authentication endpoints and will include claims for user identity and roles.

### Content Negotiation

The API supports JSON as the primary data exchange format:

```
Accept: application/json
Content-Type: application/json
```

## 4.3. Common HTTP Status Codes

| Code | Description | Common Usage |
|------|-------------|-------------|
| 200 | OK | Successful GET, PUT, or PATCH |
| 201 | Created | Successful POST that created a resource |
| 204 | No Content | Successful operation with no response body (e.g., DELETE) |
| 400 | Bad Request | Invalid request format or validation error |
| 401 | Unauthorized | Missing authentication |
| 403 | Forbidden | Authentication provided but lacks permission |
| 404 | Not Found | Resource does not exist |
| 409 | Conflict | Resource state conflict (e.g., concurrent modification) |
| 412 | Precondition Failed | Conditional operation failed (ETag mismatch) |
| 422 | Unprocessable Entity | Well-formed request but semantic errors |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server-side error |

## 4.4. Common Response Format

All responses will follow a consistent format:

```json
{
  "data": {
    // Resource data or collection
  },
  "meta": {
    "pagination": {
      "page": 1,
      "pageSize": 25,
      "totalItems": 100,
      "totalPages": 4
    },
    "timestamp": "2025-04-08T14:32:15Z"
  },
  "links": {
    "self": "https://api.propulse.com/v1/articles?page=1&pageSize=25",
    "first": "https://api.propulse.com/v1/articles?page=1&pageSize=25",
    "prev": null,
    "next": "https://api.propulse.com/v1/articles?page=2&pageSize=25",
    "last": "https://api.propulse.com/v1/articles?page=4&pageSize=25"
  }
}
```

For error responses:

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The request contains invalid parameters",
    "details": [
      {
        "field": "title",
        "message": "Title must be between 5 and 200 characters"
      }
    ]
  },
  "meta": {
    "timestamp": "2025-04-08T14:32:15Z",
    "requestId": "7b44b0f2-95ce-4ec8-9a0a-26cbb397f0bf"
  }
}
```

## 4.5. OData Query Support

The API will implement OData query options to provide flexible querying capabilities:

### Filtering

```
GET /articles?$filter=status eq 'Published' and author eq 'John Doe'
```

### Sorting

```
GET /articles?$orderby=publishedAt desc
```

### Selecting Fields

```
GET /articles?$select=id,title,summary,publishedAt
```

### Expanding Related Entities

```
GET /articles?$expand=comments,author
```

## 4.6. Pagination

All collection endpoints support pagination:

```
GET /articles?$skip=50&$top=25
```

Or using page-based parameters:

```
GET /articles?page=3&pageSize=25
```

The response will include pagination metadata and HATEOAS links to navigate between pages.

## 4.7. Conditional Requests (RFC9110)

The API supports conditional requests using ETags and conditional headers:

### Entity Tags (ETags)

Every resource response includes an ETag header:

```
ETag: "33a64df551425fcc55e4d42a148795d9f25f89d4"
```

### Conditional GET

Clients can use If-None-Match for cache validation:

```
GET /articles/01H9A3K4ZWVR2MJY1FAAJN5MSP
If-None-Match: "33a64df551425fcc55e4d42a148795d9f25f89d4"
```

Server responds with 304 Not Modified if content hasn't changed.

### Conditional Updates

Clients can use If-Match to prevent lost updates:

```
PUT /articles/01H9A3K4ZWVR2MJY1FAAJN5MSP
If-Match: "33a64df551425fcc55e4d42a148795d9f25f89d4"
Content-Type: application/json

{
  "title": "Updated Article Title",
  "content": "Updated article content..."
}
```

Server responds with 412 Precondition Failed if ETag doesn't match (indicating someone else modified the resource).

## 4.8. Rate Limiting

Rate limiting is applied to prevent abuse:

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1714483935
```

When rate limit is exceeded:

```
HTTP/1.1 429 Too Many Requests
Retry-After: 60
```

## 4.9. API Endpoints Reference

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /auth/register | Register a new user account |
| POST | /auth/login | Obtain authentication token |
| POST | /auth/refresh | Refresh authentication token |
| POST | /auth/logout | Invalidate authentication token |
| POST | /auth/reset-password | Request password reset |
| POST | /auth/confirm-email | Confirm email address |

### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /users/me | Get current user profile |
| PUT | /users/me | Update current user profile |
| PATCH | /users/me | Partially update user profile |
| GET | /users/{id} | Get public user profile by ID |
| GET | /users | List users (admin only) |

### Articles

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /articles | List articles with filtering and pagination |
| POST | /articles | Create a new article |
| GET | /articles/{id} | Get article by ID |
| PUT | /articles/{id} | Update article by ID |
| PATCH | /articles/{id} | Partially update article by ID |
| DELETE | /articles/{id} | Delete article by ID |
| PUT | /articles/{id}/publish | Publish an article |
| PUT | /articles/{id}/unpublish | Unpublish an article |
| GET | /articles/{id}/analytics | Get article analytics |
| GET | /users/{userId}/articles | Get articles by user |

### Comments

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /articles/{articleId}/comments | List comments for an article |
| POST | /articles/{articleId}/comments | Add a comment to an article |
| GET | /comments/{id} | Get comment by ID |
| PUT | /comments/{id} | Update comment by ID (author only) |
| DELETE | /comments/{id} | Delete comment by ID |
| POST | /comments/{id}/replies | Reply to a comment |
| GET | /users/{userId}/comments | Get comments by user |

### Ratings

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /articles/{articleId}/ratings | Get ratings statistics for article |
| POST | /articles/{articleId}/ratings | Rate an article |
| PUT | /articles/{articleId}/ratings | Update user's rating for article |
| DELETE | /articles/{articleId}/ratings | Remove user's rating for article |

### Tags

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /tags | List all tags |
| POST | /tags | Create a new tag (admin only) |
| GET | /tags/{id} | Get tag by ID |
| PUT | /tags/{id} | Update tag by ID (admin only) |
| GET | /tags/{id}/articles | Get articles with specific tag |

### Social Media

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /social-accounts | List social media accounts |
| POST | /social-accounts | Connect a social media account |
| DELETE | /social-accounts/{id} | Disconnect social media account |
| GET | /social-posts | List scheduled social media posts |
| POST | /social-posts | Create a social media post |
| GET | /social-posts/{id} | Get social post details |
| PUT | /social-posts/{id}/approve | Approve a social media post |
| PUT | /social-posts/{id}/reject | Reject a social media post |
| DELETE | /social-posts/{id} | Delete a social media post |

## 4.10. Example API Interactions

### Creating an Article

**Request:**
```
POST /articles HTTP/1.1
Host: api.propulse.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "Understanding REST APIs",
  "summary": "A comprehensive guide to REST API design principles",
  "content": "<h1>Understanding REST APIs</h1><p>REST APIs are...</p>",
  "tags": ["api", "rest", "webdev"]
}
```

**Response:**
```
HTTP/1.1 201 Created
Content-Type: application/json
Location: https://api.propulse.com/v1/articles/01H9A3K4ZWVR2MJY1FAAJN5MSP
ETag: "67ab43f1c2d3a76e427766c383c64d36ff8a24997cbf71c"

{
  "data": {
    "id": "01H9A3K4ZWVR2MJY1FAAJN5MSP",
    "title": "Understanding REST APIs",
    "summary": "A comprehensive guide to REST API design principles",
    "content": "<h1>Understanding REST APIs</h1><p>REST APIs are...</p>",
    "slug": "understanding-rest-apis",
    "status": "Draft",
    "createdAt": "2025-04-08T14:30:15Z",
    "updatedAt": "2025-04-08T14:30:15Z",
    "publishedAt": null,
    "tags": [
      {"id": "01H8A2B4CWVR2MJY1FAAJN5MSA", "name": "api"},
      {"id": "01H8A2B4CWVR2MJY1FAAJN5MSB", "name": "rest"},
      {"id": "01H8A2B4CWVR2MJY1FAAJN5MSC", "name": "webdev"}
    ]
  },
  "meta": {
    "timestamp": "2025-04-08T14:30:15Z"
  },
  "links": {
    "self": "https://api.propulse.com/v1/articles/01H9A3K4ZWVR2MJY1FAAJN5MSP"
  }
}
```

### Retrieving Articles with Filtering and Pagination

**Request:**
```
GET /articles?$filter=status eq 'Published'&$orderby=publishedAt desc&$top=10&$select=id,title,summary,publishedAt,author&$expand=author($select=id,displayName) HTTP/1.1
Host: api.propulse.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
```
HTTP/1.1 200 OK
Content-Type: application/json
ETag: "b3e019250cd8fcd93566b10d64e3c04ab9bd7a215f16e621"

{
  "data": [
    {
      "id": "01H9A3K4ZWVR2MJY1FAAJN5MSP",
      "title": "Understanding REST APIs",
      "summary": "A comprehensive guide to REST API design principles",
      "publishedAt": "2025-04-08T15:30:00Z",
      "author": {
        "id": "01H8B3C4DWVR2MJY1FAAJN5MSD",
        "displayName": "Jane Smith"
      }
    },
    // Additional articles...
  ],
  "meta": {
    "pagination": {
      "page": 1,
      "pageSize": 10,
      "totalItems": 87,
      "totalPages": 9
    },
    "timestamp": "2025-04-08T14:35:22Z"
  },
  "links": {
    "self": "https://api.propulse.com/v1/articles?$filter=status eq 'Published'&$orderby=publishedAt desc&$top=10&$select=id,title,summary,publishedAt,author&$expand=author($select=id,displayName)",
    "first": "https://api.propulse.com/v1/articles?$filter=status eq 'Published'&$orderby=publishedAt desc&$top=10&$select=id,title,summary,publishedAt,author&$expand=author($select=id,displayName)&page=1",
    "prev": null,
    "next": "https://api.propulse.com/v1/articles?$filter=status eq 'Published'&$orderby=publishedAt desc&$top=10&$select=id,title,summary,publishedAt,author&$expand=author($select=id,displayName)&page=2",
    "last": "https://api.propulse.com/v1/articles?$filter=status eq 'Published'&$orderby=publishedAt desc&$top=10&$select=id,title,summary,publishedAt,author&$expand=author($select=id,displayName)&page=9"
  }
}
```

## 4.11. API Implementation

The API will be implemented using the following ASP.NET Core technologies:

- **Traditional API Controllers (MVC)**: For robust, feature-rich endpoints with full OData support
- **MediatR with CQRS Pattern**: For clean separation of commands and queries with cross-cutting concerns
- **OData Libraries**: Microsoft.AspNetCore.OData for rich query functionality
- **Data Validation**: FluentValidation for centralized data validation
- **Problem Details**: RFC 7807 compliant error responses
- **API Versioning**: Microsoft.AspNetCore.Mvc.Versioning
- **API Documentation**: Swagger/OpenAPI using Swashbuckle
- **Rate Limiting**: Built-in ASP.NET Core rate limiting middleware
- **Content Negotiation**: Support for JSON (and potential future formats)
- **EF Core**: For data access with query optimization techniques

The implementation will follow a modular structure with:

1. **Controllers**: Thin API controllers with responsibility for request/response handling
2. **Commands/Queries**: Separate command and query objects for CQRS implementation
3. **Handlers**: Business logic contained in command/query handlers
4. **Behaviors**: Cross-cutting concerns implemented as MediatR pipeline behaviors:
   - Validation
   - Caching (for queries)
   - Logging
   - Error handling
   - Transaction management
4. **Mappings**: AutoMapper profiles for clean entity-to-DTO conversions

This approach ensures compatibility with OData requirements, maintains clean separation of concerns, and provides a structured architecture that scales well for a complex enterprise application.
