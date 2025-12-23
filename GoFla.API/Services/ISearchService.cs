using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Search;

namespace GoFla.API.Services;

public interface ISearchService
{
    Task<Result<SearchResultDto>> SearchAsync(SearchRequestDto dto, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<RestaurantSearchResultDto>>> SearchRestaurantsAsync(SearchRequestDto dto, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<MenuItemSearchResultDto>>> SearchMenuItemsAsync(SearchRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetPopularSearchesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetSuggestionsAsync(string query, CancellationToken cancellationToken = default);
}
