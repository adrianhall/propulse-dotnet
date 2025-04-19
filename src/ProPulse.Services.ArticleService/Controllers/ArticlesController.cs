using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProPulse.Core.Entities;
using ProPulse.DataModel;
using ProPulse.Services.ArticleService.ViewModels;

namespace ProPulse.Services.ArticleService.Controllers;

[Route("api/articles")]
[ApiController]
public class ArticlesController(AppDbContext context, IMapper mapper) : ApiServiceController
{
    /// <summary>
    /// Retrieves an article based on its ID.
    /// </summary>
    /// <param name="articleId">The article ID as a string.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The article if found and permitted, or an error response.</returns>
    [HttpGet("{articleId}")]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [Produces("application/json")]
    public async Task<IActionResult> GetArticleByIdAsync(string articleId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(articleId, out Guid articleGuid))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid article ID",
                Detail = $"The provided article ID '{articleId}' is not a valid GUID.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        Article? article = await context.Articles
            .IncludingDeletedEntities(IncludeDeletedEntities)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == articleGuid, cancellationToken);

        if (article is null)
        {
            return NotFound();
        }

        if (IsConditionalRequest && !EntityIsModified(article))
        {
            return StatusCode(304);
        }

        // Map to response DTO
        ArticleResponse response = mapper.Map<ArticleResponse>(article);

        // Set response headers
        return Ok(response);
    }


}