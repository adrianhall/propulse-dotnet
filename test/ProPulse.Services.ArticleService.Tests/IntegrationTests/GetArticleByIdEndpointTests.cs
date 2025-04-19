using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Extensions;
using ProPulse.Core;
using ProPulse.Core.Entities;
using ProPulse.Services.ArticleService.ViewModels;
using ProPulseTests.Common;

namespace ProPulse.Services.ArticleService.Tests.IntegrationTests;

/// <summary>
/// Integration tests for the GetArticleById endpoint.
/// </summary>
[Collection("TestContainerCollection")]
public sealed class GetArticleByIdEndpointTests : IAsyncLifetime
{
    private ApiServiceFixture<Program> _fixture = null!;

    #region IAsyncLifetime Members
    /// <summary>
    /// per-test async initialization
    /// </summary>
    public async Task InitializeAsync()
    {
        _fixture = new ApiServiceFixture<Program>();
        await _fixture.InitializeAsync();
    }

    /// <summary>
    /// per-test async cleanup
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_fixture is not null)
        {
            await _fixture.DisposeAsync();
        }
    }
    #endregion

    /// <summary>
    /// Should return 404 when the article does not exist.
    /// </summary>
    [Fact]
    public async Task GetArticleById_ReturnsNotFound_WhenArticleDoesNotExist()
    {
        // Arrange
        string nonExistentId = Guid.NewGuid().ToString();
        string url = $"/api/articles/{nonExistentId}";

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Should return 404 Not Found with X-Include-Deleted header for a deleted article.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetArticleById_ReturnsNotFound_ForDeletedArticle(bool addHeader)
    {
        // Arrange
        Article article = _fixture.GetRandomArticle();
        await _fixture.SoftDeleteArticleAsync(article);
        string url = $"/api/articles/{article.Id}";
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        if (addHeader)
        {
            request.Headers.TryAddWithoutValidation("X-Include-Deleted", "false");
        }

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Should return 400 Bad Request when the ArticleId is not a GUID.
    /// </summary>
    [Fact]
    public async Task GetArticleById_ReturnsBadRequest_WhenArticleIdIsNotGuid()
    {
        // Arrange
        string url = "/api/articles/not-a-guid";

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Should return 400 Bad Request when Last-Modified header is not a valid value.
    /// </summary>
    [Theory]
    [InlineData("If-Modified-Since", "not-a-date")]
    [InlineData("If-Unmodified-Since", "not-a-date")]
    [InlineData("X-Include-Deleted", "not-a-bool")]
    [InlineData("If-None-Match", "not-a-etag")]
    [InlineData("If-Match", "not-a-etag")]
    public async Task GetArticleById_ReturnsBadRequest_WhenHeaderIsInvalid(string headerName, string headerValue)
    {
        // Arrange
        string articleId = _fixture.GetRandomArticle().Id.ToString();
        using HttpRequestMessage request = new(HttpMethod.Get, $"/api/articles/{articleId}");
        request.Headers.TryAddWithoutValidation(headerName, headerValue);

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // TODO:: Check that the response body is a problem details object which mentions the header name.
    }

    /// <summary>
    /// Should succeed in returning an article with If-None-Match header.
    /// </summary>
    [Theory]
    [InlineData("If-None-Match")]
    [InlineData("If-Modified-Since")]
    public async Task GetArticleById_ReturnsNotModified_WithConditionalRequest(string headerName)
    {
        // Arrange
        Article article = _fixture.GetRandomArticle();
        string url = $"/api/articles/{article.Id}";
        using HttpRequestMessage request = new(HttpMethod.Get, url);

        if (headerName == "If-None-Match")
        {
            request.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{Convert.ToBase64String(article.Version)}\""));
        }
        else if (headerName == "If-Modified-Since")
        {
            request.Headers.IfModifiedSince = article.UpdatedAt.AddSeconds(5);
        }

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotModified);
    }

    /// <summary>
    /// Should succeed in returning an article with no headers.
    /// </summary>
    [Fact]
    public async Task GetArticleById_Succeeds_WithNoHeaders()
    {
        // Arrange
        Article article = _fixture.GetRandomArticle();
        string url = $"/api/articles/{article.Id}";

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.GetAsync(url);

        // Assert
        response.Should().Be200Ok();
        response.Should().HaveHeader("Content-Type").And.Match("application/json; charset=utf-8");
        response.Should().HaveHeader("ETag").And.Match($"\"{Convert.ToBase64String(article.Version)}\"");
        response.Should().HaveDateHeader("Last-Modified", "R")
            .And.BeCloseTo(article.UpdatedAt, TimeSpan.FromSeconds(1));
        response.Should().HaveDateHeader("Last-Modified-Timestamp", AppConstants.TimestampFormat)
            .And.BeCloseTo(article.UpdatedAt, TimeSpan.FromSeconds(1));

        ArticleResponse? received = await response.Content.ReadFromJsonAsync<ArticleResponse>();
        received.Should().NotBeNull().And.BeEquivalentTo(article, options => options.ComparingByMembers<Article>());
    }

    /// <summary>
    /// Should succeed in returning an article with X-Include-Deleted header.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetArticleById_ReturnsArticle_WithXIncludeDeleted(bool articleIsDeleted)
    {
        // Arrange
        Article article = _fixture.GetRandomArticle();
        if (articleIsDeleted)
        {
            article = await _fixture.SoftDeleteArticleAsync(article);
        }

        string url = $"/api/articles/{article.Id}";
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("X-Include-Deleted", "true");

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.SendAsync(request);

        // Assert
        response.Should().Be200Ok();
        response.Should().HaveHeader("Content-Type").And.Match("application/json; charset=utf-8");
        response.Should().HaveHeader("ETag").And.Match($"\"{Convert.ToBase64String(article.Version)}\"");
        response.Should().HaveDateHeader("Last-Modified", "R")
            .And.BeCloseTo(article.UpdatedAt, TimeSpan.FromSeconds(1));
        response.Should().HaveDateHeader("Last-Modified-Timestamp", AppConstants.TimestampFormat)
            .And.BeCloseTo(article.UpdatedAt, TimeSpan.FromSeconds(1));

        ArticleResponse? received = await response.Content.ReadFromJsonAsync<ArticleResponse>();
        received.Should().NotBeNull().And.BeEquivalentTo(article, options => options.ComparingByMembers<Article>());
    }

    /// <summary>
    /// Should succeed in returning an article with If-None-Match header.
    /// </summary>
    [Theory]
    [InlineData("If-None-Match")]
    [InlineData("If-Modified-Since")]
    public async Task GetArticleById_Succeeds_WithConditionalRequest(string headerName)
    {
        // Arrange
        Article article = _fixture.GetRandomArticle();
        string url = $"/api/articles/{article.Id}";
        using HttpRequestMessage request = new(HttpMethod.Get, url);

        if (headerName == "If-None-Match")
        {
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue($"\"{Guid.NewGuid():N}\""));
        }
        else if (headerName == "If-Modified-Since")
        {
            request.Headers.IfModifiedSince = article.UpdatedAt.AddDays(-1);
        }

        // Act
        HttpResponseMessage response = await _fixture.ServiceClient.SendAsync(request);

        // Assert: Status code should be 200 OK
        response.Should().Be200Ok();
        response.Should().HaveHeader("Content-Type").And.Match("application/json; charset=utf-8");
        response.Should().HaveHeader("ETag").And.Match($"\"{Convert.ToBase64String(article.Version)}\"");
        response.Should().HaveDateHeader("Last-Modified", "R")
            .And.BeCloseTo(article.UpdatedAt, TimeSpan.FromSeconds(1));
        response.Should().HaveDateHeader("Last-Modified-Timestamp", AppConstants.TimestampFormat)
            .And.BeCloseTo(article.UpdatedAt, TimeSpan.FromSeconds(1));

        ArticleResponse? received = await response.Content.ReadFromJsonAsync<ArticleResponse>();
        received.Should().NotBeNull().And.BeEquivalentTo(article, options => options.ComparingByMembers<Article>());
    }
}
