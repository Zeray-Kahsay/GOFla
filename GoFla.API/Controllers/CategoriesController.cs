using GoFla.API.DTOs.Category;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;
// TODO: CATEGORIES CONTROLLER IMPLEMENTATION WILL BE UPDATED LATER
public class CategoriesController(
    ICategoryService categoryService
) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        int restaurantId,
        [FromBody] CreateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        string ownerId = GetUserId();

        return HandleResult(await categoryService.CreateAsync(
            restaurantId,
            ownerId,
            dto,
            cancellationToken));

    }

    [HttpGet("restaurants/{restaurantId}/categories")]
    public async Task<IActionResult> GetAllCategories(
        int restaurantId,
        CancellationToken cancellationToken)
    {
        var ownerId = GetUserId();

        return HandleResult(await categoryService.GetByRestaurantAsync(
            restaurantId,
            ownerId,
            cancellationToken));

    }

    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory(
        int categoryId,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var ownerId = GetUserId();

        return HandleResult(await categoryService.UpdateAsync(
            categoryId,
            ownerId,
            dto,
            cancellationToken));
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(
        int categoryId,
        CancellationToken cancellationToken)
    {
        var ownerId = GetUserId();

        return HandleResult(await categoryService.DeleteAsync(
            categoryId,
            ownerId,
            cancellationToken));

    }
}
