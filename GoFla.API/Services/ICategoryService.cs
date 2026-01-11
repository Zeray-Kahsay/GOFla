using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Category;

namespace GoFla.API.Services;

public interface ICategoryService
{
    Task<Result<CategoryDto>> CreateAsync(
        int restaurantId,
        string ownerId,
        CreateCategoryDto dto,
        CancellationToken cancellationToken);

    Task<Result<List<CategoryDto>>> GetByRestaurantAsync(
        int restaurantId,
        string ownerId,
        CancellationToken cancellationToken);

    Task<Result<CategoryDto>> UpdateAsync(
        int categoryId,
        string ownerId,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken);

    Task<Result<bool>> DeleteAsync(
        int categoryId,
        string ownerId,
        CancellationToken cancellationToken);
}

