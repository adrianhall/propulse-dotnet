# API Surface for ArticleServer

* Base Endpoint = "/api"
* No versioning at this time.  Long term plan is to use ?api-version or API-Version header.
* Request/Response is JSON formatted with a UTF-8 encoding and using standard casing (not camel-cased).
* The ETag is the base-64 encoding of the Version property.
* 400 Bad Request will always return a ProblemDetails.
* Use X-IncludeDeleted: true header to include deleted records in the output.

## Articles access

**GET /api/articles**

This is an OData query facility.  In the MVP, only `$skip` and `$top` will be supported to make
implementation easier.  However, future phases will include `$filter` (for structured filtering),
`$search` (for full text search), `$orderby` (for ordering), `$expand` (to include dependent
information like comments, tags, etc.), `$select` (to return a subset of information), and `$count` to return the count of items in the dataset.

By default, `$skip=0&$top=10` is assumed. 

Limits:
- `$skip` >= 0 &&
- `$top` > 0 && < AppConstants.MaxTop
- `$count` - true or false

Responses:
- 200 OK - the response is a JSON object representing a page, with the following properties:
  - `List<Article> Items` - an array of items in the data set.
  - `long? Count` - the count of items in the dataset (with filtering but without paging)
  - `long? TotalCount` - the count of items in the dataset (without filtering or paging)
  - `NextLink` - the path+querystring to load the next page.
  - `PrevLink` - the path+querystring to load the first page.
- 400 Bad Request - the request is invalid (e.g. bad query parameters)

**GET /api/articles/{articleId}**

Retrieves a specific article.  Supports RFC 9110 conditional requests (If-None-Match or If-Modified-Since).  Eventually, will support `$expand` (to include dependent information
like comments, tags, etc.)

Accepts headers `If-Match`, `If-None-Match`, `If-Unmodified-Since`, `If-Modified-Since`.
Returns headers `ETag`, `Last-Modified`, and `Location`.

Responses:
- 200 OK - the response is the JSON object representing the content.
- 304 Not Modified
- 400 Bad Request (probably $expand is wrong or the headers are invalid).
- 404 Not Found (including if soft-deleted)
- 412 Precondition Failed


**POST /api/articles**

Accepts a JSON object `{ "Title": "xxx", "Content": "xxx" }`.  This may be expanded in the future.  The content must be valid Markdown to be accepted. The title must not contain 
embedded HTML except for entities. Use Markdig to validate the content; sanitize to ensure no embedded HTML except entities.

Only Title and Content can be specified.  All other properties are ignored, even if available
in the view model.

Returns the same as the GET response on success.

Responses:
- 201 Created - the response is the same as the GET response
- 400 Bad Request - probably the JSON content is bad.

**PUT /api/articles/{articleId}**

Accepts the output of the GET response as input (i.e. a complete record).  Only editable fields will be transferred to the updated record (in this case, IsDeleted, PublishedAt, Title, Content).
Other updates are ignored (but do not cause an error).

Supports RFC 9110 conditional requests.

Accepts headers `If-Match`, `If-None-Match`, `If-Unmodified-Since`, `If-Modified-Since`.
Returns headers `ETag`, `Last-Modified`, and `Location`.

Responses:
- 200 OK - the response is the updated record as returned by the GET response.
- 400 Bad Request - probably the JSON content is bad.
- 404 Not Found - no record exists with that ID.
- 409 Conflict - the version submitted does not match the version in the server.
- 410 Gone - the entity is soft-deleted (IsDeleted = true)
- 412 Precondition Failed - used a conditional header and the condition was not met.

**DELETE /api/articles/{articleId}**

Soft-deletes an article.

Supports RFC 9110 conditional requests.  This method REQUIRES a conditional request; failure
to provide a conditional request will result in a 400 Bad Re

Accepts headers `If-Match`, `If-None-Match`, `If-Unmodified-Since`, `If-Modified-Since`.

Responses:
- 204 No Content - the article is soft-deleted or does not exist.
- 400 Bad Request - likely, the conditional request failed.
- 412 Precondition Failed - the condition was not met.
- 428 Precondition Required - the request did not contain a If-Match request.
