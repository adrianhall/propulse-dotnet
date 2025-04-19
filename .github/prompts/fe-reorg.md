I'd like to reorganize the FastEndpoints structure within the project.  Right now, the 
requirements for each endpoint are scattered across the project in multiple different
folders.

I'd like the project to have a single folder per endpoint - for example, `/Endpoints/CreateArticle`.  The folder will contain the request, response, command or query, and the handler.

So, for example, the `CreateArticle` endpoint will have the following files:

* `CreateArticleRequest.cs`
* `CreateArticleResponse.cs`
* `CreateArticleCommand.cs`
* `CreateArticleCommandHandler.cs`
* `CreateArticleEndpoint.cs`
* `CreateArticleRequestValidator.cs`

If there are shared components (for example, the mapper or a shared DTO), then these
should be placed in the `/Endpoints/Shared` folder.

Can you please reorganize the folder structure, paying particular attention to the namespace
translations?