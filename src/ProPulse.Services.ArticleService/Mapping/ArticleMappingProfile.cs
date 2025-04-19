using AutoMapper;
using ProPulse.Core.Entities;
using ProPulse.Services.ArticleService.ViewModels;

namespace ProPulse.Services.ArticleService.Mapping;

/// <summary>
/// Provides AutoMapper configuration for mapping between <see cref="Article"/> and <see cref="ArticleResponse"/>.
/// </summary>
public sealed class ArticleMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleMappingProfile"/> class.
    /// </summary>
    public ArticleMappingProfile()
    {
        CreateMap<Article, ArticleResponse>().ReverseMap();
    }
}
