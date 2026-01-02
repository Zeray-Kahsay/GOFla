using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.MenuItems;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class MenuItemService(
    IRepository<MenuItem> menuRepository,
     IRestaurantRepository restaurantRepository,
     IUserContext userContext) : IMenuItemService
{
    public async Task<Result<MenuItemDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var menuItem = await menuRepository.GetByIdAsync(id, cancellationToken);
        if (menuItem is null)
        {
            return Result<MenuItemDto>.Failure("Menu item not found", "NOT_FOUND");
        }

        return Result<MenuItemDto>.Success(menuItem.ToMenuItemDto());
    }


    public async Task<Result<PagedResult<MenuItemDto>>> GetByRestaurantAsync(
         int restaurantId,
         PaginationParams paginationParams,
         CancellationToken cancellationToken = default)
    {
        var pagedResult = await menuRepository.GetPagedAsync(
            predicate: mi => mi.RestaurantId == restaurantId && mi.IsAvailable,
            orderBy: mi => mi.Id,
            cursor: paginationParams.Cursor,
            pageSize: paginationParams.PageSize,
            cancellationToken: cancellationToken);

        var dtoList = pagedResult.Items.Select(mi => mi.ToMenuItemDto()).ToList();

        return Result<PagedResult<MenuItemDto>>.Success(new PagedResult<MenuItemDto>
        {
            Items = dtoList,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }


    public async Task<Result<PagedResult<MenuItemDto>>> GetByCategoryAsync(
        int restaurantId, 
        string category, 
        PaginationParams paginationParams, 
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await menuRepository.GetPagedAsync(
            predicate: mi => mi.RestaurantId == restaurantId && mi.Category == category && mi.IsAvailable,
            orderBy: mi => mi.Id,
            cursor: paginationParams.Cursor,
            pageSize: paginationParams.PageSize,
            cancellationToken: cancellationToken);
        
        var dtoList = pagedResult.Items.Select(mi => mi.ToMenuItemDto()).ToList();

        return Result<PagedResult<MenuItemDto>>.Success(new PagedResult<MenuItemDto>
        {
            Items = dtoList,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }



    public async Task<Result<MenuItemDto>> CreateAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(dto.RestaurantId, cancellationToken);
        if (restaurant is null)
        {
            return Result<MenuItemDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        // Ownership check
        if (restaurant.OwnerId != userContext.UserId)
             return Result<MenuItemDto>.Failure("Access Denied", "FORBIDDEN");


        var menuItem = new MenuItem
        {
            RestaurantId = dto.RestaurantId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Category = dto.Category,
            ImageUrl = string.Empty, // Will be updated via separate endpoint
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await menuRepository.AddAsync(menuItem, cancellationToken);
        
        return Result<MenuItemDto>.Success(created.ToMenuItemDto());
    }


    public async Task<Result<MenuItemDto>> UpdateAsync(int id, UpdateMenuItemDto dto, CancellationToken cancellationToken = default)
    {
        var menuItem = await menuRepository.GetByIdAsync(id, cancellationToken);
        if (menuItem is null)
        {
            return Result<MenuItemDto>.Failure("Menu item not found", "NOT_FOUND");
        }

        menuItem.Name = dto.Name;
        menuItem.Description = dto.Description;
        menuItem.Price = dto.Price;
        menuItem.Category = dto.Category;
        menuItem.UpdatedAt = DateTime.UtcNow;

         await menuRepository.UpdateAsync(menuItem, cancellationToken);
        
        return Result<MenuItemDto>.Success(menuItem.ToMenuItemDto());
    }


    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var menuItem = await menuRepository.GetByIdAsync(id, cancellationToken);
        if (menuItem is null)
        {
            return Result<bool>.Failure("Menu item not found", "NOT_FOUND");
        }

        await menuRepository.DeleteAsync(menuItem, cancellationToken);
        return Result<bool>.Success(true);
    }




    public async Task<Result<bool>> ToggleAvailabilityAsync(int id, CancellationToken cancellationToken = default)
    {
        var menuItem = await menuRepository.GetByIdAsync(id, cancellationToken);
        if (menuItem is null)
        {
            return Result<bool>.Failure("Menu item not found", "NOT_FOUND");
        }

        menuItem.IsAvailable = !menuItem.IsAvailable;
        menuItem.UpdatedAt = DateTime.UtcNow;

        await menuRepository.UpdateAsync(menuItem, cancellationToken);
        return Result<bool>.Success(true);
    }

}
