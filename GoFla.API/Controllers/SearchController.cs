using System;
using GoFla.API.DTOs.Search;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class SearchController (ISearchService _searchService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchRequestDto dto, CancellationToken cancellationToken)
    {
        var userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;
        var result = await _searchService.SearchAsync(dto, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("restaurants")]
    public async Task<IActionResult> SearchRestaurants([FromQuery] SearchRequestDto dto, CancellationToken cancellationToken)
    {
        var userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;
        var result = await _searchService.SearchRestaurantsAsync(dto, userId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("menu-items")]
    public async Task<IActionResult> SearchMenuItems([FromQuery] SearchRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _searchService.SearchMenuItemsAsync(dto, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularSearches(CancellationToken cancellationToken)
    {
        var result = await _searchService.GetPopularSearchesAsync(cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string query, CancellationToken cancellationToken)
    {
        var result = await _searchService.GetSuggestionsAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
