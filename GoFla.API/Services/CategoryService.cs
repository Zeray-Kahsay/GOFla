using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Category;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IRestaurantRepository restaurantRepository

    ) : ICategoryService
{
    public async Task<Result<CategoryDto>> CreateAsync(int restaurantId, string ownerId, CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);

        if (restaurant == null )
            return Result<CategoryDto>.Failure("Restaurant not found.", "Not_Found");
        

        if (restaurant.OwnerId != ownerId)
            return Result<CategoryDto>.Failure("Unauthorized access to the restaurant.", "UNAUTHORIZED");
        

        var category = new Category
        {
            Name = dto.Name,
            RestaurantId = restaurantId,
            SortOrder = dto.SortOrder
        };

        await categoryRepository.AddAsync(category, cancellationToken);

        return Result<CategoryDto>.Success(category.ToCategoryDto());

    }

    public async Task<Result<List<CategoryDto>>> GetByRestaurantAsync(int restaurantId, string ownerId, CancellationToken cancellationToken)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
            return Result<List<CategoryDto>>.Failure("Restaurant not found.", "Not_Found");
        
        if (restaurant.OwnerId != ownerId)
          return Result<List<CategoryDto>>.Failure("Unauthorized access to the restaurant.", "UNAUTHORIZED");
        
        var categories = await categoryRepository.GetByRestaurantIdAsync(restaurantId, cancellationToken);

        return Result<List<CategoryDto>>.Success(categories.Select(c => c.ToCategoryDto()).ToList());
    }


    public async Task<Result<CategoryDto>> UpdateAsync(int categoryId, string ownerId, UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            return Result<CategoryDto>.Failure("Category not found.", "Not_Found");
        
        if (category.Restaurant!.OwnerId != ownerId)
            return Result<CategoryDto>.Failure("Unauthorized access to the category.", "UNAUTHORIZED");
        
        category.Name = dto.Name;
        category.SortOrder = dto.SortOrder;
        category.UpdatedAt = DateTime.UtcNow;

        await categoryRepository.UpdateAsync(category, cancellationToken);

        return Result<CategoryDto>.Success(category.ToCategoryDto());   
    }
    

    public async Task<Result<bool>> DeleteAsync(int categoryId, string ownerId, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            return Result<bool>.Failure("Category not found.", "Not_Found");
        
        if (category.Restaurant!.OwnerId != ownerId)
            return Result<bool>.Failure("Unauthorized access to the category.", "UNAUTHORIZED");
        
        await categoryRepository.DeleteAsync(category, cancellationToken);
        return Result<bool>.Success(true);
    }


}
