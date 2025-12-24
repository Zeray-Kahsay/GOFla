using System;
using GoFla.API.Commons;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

[Authorize]
public class FavoritesController (IFavoriteService favoriteService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyFavorites(
       [FromQuery] PaginationParams paginationParams,
       CancellationToken cancellationToken)
    {
        var result = await favoriteService.GetUserFavoritesAsync(GetUserId(), paginationParams, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("check/{restaurantId}")]
    public async Task<IActionResult> CheckIsFavorite(int restaurantId, CancellationToken cancellationToken)
    {
        var result = await favoriteService.IsFavoriteAsync(GetUserId(), restaurantId, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{restaurantId}")]
    public async Task<IActionResult> AddFavorite(int restaurantId, CancellationToken cancellationToken)
    {
        var result = await favoriteService.AddFavoriteAsync(GetUserId(), restaurantId, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{restaurantId}")]
    public async Task<IActionResult> RemoveFavorite(int restaurantId, CancellationToken cancellationToken)
    {
        var result = await favoriteService.RemoveFavoriteAsync(GetUserId(), restaurantId, cancellationToken);
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("restaurant/{restaurantId}/count")]
    public async Task<IActionResult> GetFavoriteCount(int restaurantId, CancellationToken cancellationToken)
    {
        var result = await favoriteService.GetFavoriteCountAsync(restaurantId, cancellationToken);
        return HandleResult(result);
    }
}
