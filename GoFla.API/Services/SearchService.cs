using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.DTOs.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoFla.API.Services;

public class SearchService(AppDbContext _context, ILogger<SearchService> _logger) : ISearchService
{
    public async Task<Result<SearchResultDto>> SearchAsync(
     SearchRequestDto dto,
     string? userId = null,
     CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SearchAsync called with Query='{Query}', UserId='{UserId}'", dto?.Query, userId);
        if (string.IsNullOrWhiteSpace(dto?.Query))
        {
            _logger.LogInformation("SearchAsync invalid query: '{Query}'", dto?.Query);
            return Result<SearchResultDto>.Failure("Search query is required", "INVALID_QUERY");
        }

        var query = dto.Query.ToLower().Trim();

        // Search restaurants
        var restaurantsQuery = _context.Restaurants
            .Where(r => r.IsActive && (
                r.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                r.Description.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                r.Address.Contains(query, StringComparison.CurrentCultureIgnoreCase)
            ));

        var restaurants = await restaurantsQuery
            .Take(10)
            .Select(r => new RestaurantSearchResultDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                Address = r.Address,
                DeliveryFee = r.DeliveryFee,
                EstimatedDeliveryTime = r.EstimatedDeliveryTime,
                AverageRating = _context.Reviews
                    .Where(rev => rev.RestaurantId == r.Id && rev.IsApproved && !rev.IsFlagged)
                    .Average(rev => (double?)rev.Rating) ?? 0,
                ReviewCount = _context.Reviews
                    .Count(rev => rev.RestaurantId == r.Id && rev.IsApproved && !rev.IsFlagged),
                IsFavorite = userId != null && _context.Favorites
                    .Any(f => f.UserId == userId && f.RestaurantId == r.Id)
            })
            .ToListAsync(cancellationToken);

        // Search menu items
        var menuItemsQuery = _context.MenuItems
            .Include(m => m.Restaurant)
            .Where(m => m.IsAvailable && m.Restaurant.IsActive && (
                m.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                m.Description.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                m.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase)
            ));

        if (dto.Category != null)
        {
            menuItemsQuery = menuItemsQuery.Where(m => m.Category == dto.Category);
        }

        if (dto.MinPrice.HasValue)
        {
            menuItemsQuery = menuItemsQuery.Where(m => m.Price >= dto.MinPrice.Value);
        }

        if (dto.MaxPrice.HasValue)
        {
            menuItemsQuery = menuItemsQuery.Where(m => m.Price <= dto.MaxPrice.Value);
        }

        var menuItems = await menuItemsQuery
            .Take(20)
            .Select(m => new MenuItemSearchResultDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ImageUrl = m.ImageUrl,
                Price = m.Price,
                Category = m.Category,
                RestaurantId = m.RestaurantId,
                RestaurantName = m.Restaurant.Name
            })
            .ToListAsync(cancellationToken);

        return Result<SearchResultDto>.Success(new SearchResultDto
        {
            Restaurants = restaurants,
            MenuItems = menuItems,
            TotalResults = restaurants.Count + menuItems.Count
        });
    }

    public async Task<Result<PagedResult<RestaurantSearchResultDto>>> SearchRestaurantsAsync(
        SearchRequestDto dto,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SearchRestaurantsAsync called with Query='{Query}', PageSize={PageSize}, UserId='{UserId}'", dto?.Query, dto?.PageSize, userId);
        if (string.IsNullOrWhiteSpace(dto?.Query))
        {
            _logger.LogInformation("SearchRestaurantsAsync invalid query: '{Query}'", dto?.Query);
            return Result<PagedResult<RestaurantSearchResultDto>>.Failure("Search query is required", "INVALID_QUERY");
        }

        var query = dto.Query.ToLower().Trim();

        var restaurantsQuery = _context.Restaurants
            .Where(r => r.IsActive && (
                r.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                r.Description.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                r.Address.Contains(query, StringComparison.CurrentCultureIgnoreCase)
            ));

        // Apply filters
        if (dto.MinRating.HasValue)
        {
            restaurantsQuery = restaurantsQuery.Where(r =>
                _context.Reviews
                    .Where(rev => rev.RestaurantId == r.Id && rev.IsApproved && !rev.IsFlagged)
                    .Average(rev => (double?)rev.Rating) >= dto.MinRating.Value
            );
        }

        // Sorting
        restaurantsQuery = dto.SortBy?.ToLower() switch
        {
            "rating" => restaurantsQuery.OrderByDescending(r =>
                _context.Reviews
                    .Where(rev => rev.RestaurantId == r.Id && rev.IsApproved && !rev.IsFlagged)
                    .Average(rev => (double?)rev.Rating) ?? 0),
            "deliveryfee" => restaurantsQuery.OrderBy(r => r.DeliveryFee),
            _ => restaurantsQuery.OrderBy(r => r.Name)
        };

        // Pagination
        var totalCount = await restaurantsQuery.CountAsync(cancellationToken);

        var items = await restaurantsQuery
            .Skip(0) // For cursor-based, implement similar to Repository
            .Take(dto.PageSize)
            .Select(r => new RestaurantSearchResultDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                Address = r.Address,
                DeliveryFee = r.DeliveryFee,
                EstimatedDeliveryTime = r.EstimatedDeliveryTime,
                AverageRating = _context.Reviews
                    .Where(rev => rev.RestaurantId == r.Id && rev.IsApproved && !rev.IsFlagged)
                    .Average(rev => (double?)rev.Rating) ?? 0,
                ReviewCount = _context.Reviews
                    .Count(rev => rev.RestaurantId == r.Id && rev.IsApproved && !rev.IsFlagged),
                IsFavorite = userId != null && _context.Favorites
                    .Any(f => f.UserId == userId && f.RestaurantId == r.Id)
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<RestaurantSearchResultDto>>.Success(new PagedResult<RestaurantSearchResultDto>
        {
            Items = items,
            TotalCount = totalCount,
            NextCursor = null, // Implement cursor logic if needed
            HasMore = totalCount > dto.PageSize
        });
    }

    public async Task<Result<PagedResult<MenuItemSearchResultDto>>> SearchMenuItemsAsync(
        SearchRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SearchMenuItemsAsync called with Query='{Query}', PageSize={PageSize}, Category='{Category}'", dto?.Query, dto?.PageSize, dto?.Category);
        if (string.IsNullOrWhiteSpace(dto?.Query))
        {
            _logger.LogInformation("SearchMenuItemsAsync invalid query: '{Query}'", dto?.Query);
            return Result<PagedResult<MenuItemSearchResultDto>>.Failure("Search query is required", "INVALID_QUERY");
        }

        var query = dto.Query.ToLower().Trim();

        var menuItemsQuery = _context.MenuItems
            .Include(m => m.Restaurant)
            .Where(m => m.IsAvailable && m.Restaurant.IsActive && (
                m.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                m.Description.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                m.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase)
            ));

        // Apply filters
        if (dto.Category != null)
        {
            menuItemsQuery = menuItemsQuery.Where(m => m.Category == dto.Category);
        }

        if (dto.MinPrice.HasValue)
        {
            menuItemsQuery = menuItemsQuery.Where(m => m.Price >= dto.MinPrice.Value);
        }

        if (dto.MaxPrice.HasValue)
        {
            menuItemsQuery = menuItemsQuery.Where(m => m.Price <= dto.MaxPrice.Value);
        }

        // Sorting
        menuItemsQuery = dto.SortBy?.ToLower() switch
        {
            "price" => menuItemsQuery.OrderBy(m => m.Price),
            "price_desc" => menuItemsQuery.OrderByDescending(m => m.Price),
            _ => menuItemsQuery.OrderBy(m => m.Name)
        };

        var totalCount = await menuItemsQuery.CountAsync(cancellationToken);

        var items = await menuItemsQuery
            .Skip(0)
            .Take(dto.PageSize)
            .Select(m => new MenuItemSearchResultDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ImageUrl = m.ImageUrl,
                Price = m.Price,
                Category = m.Category,
                RestaurantId = m.RestaurantId,
                RestaurantName = m.Restaurant.Name
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<MenuItemSearchResultDto>>.Success(new PagedResult<MenuItemSearchResultDto>
        {
            Items = items,
            TotalCount = totalCount,
            NextCursor = null,
            HasMore = totalCount > dto.PageSize
        });
    }

    public async Task<Result<List<string>>> GetPopularSearchesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetPopularSearchesAsync called");
        // This would typically come from a search analytics table
        // For now, return popular restaurant names
        var popularSearches = await _context.Restaurants
            .Where(r => r.IsActive)
            .OrderByDescending(r => _context.Orders.Count(o => o.RestaurantId == r.Id))
            .Take(10)
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        return Result<List<string>>.Success(popularSearches);
    }

    public async Task<Result<List<string>>> GetSuggestionsAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetSuggestionsAsync called with Query='{Query}'", query);
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            _logger.LogInformation("GetSuggestionsAsync returning empty suggestions for Query='{Query}'", query);
            return Result<List<string>>.Success(new List<string>());
        }

        var searchQuery = query.ToLower().Trim();

        var restaurantNames = await _context.Restaurants
            .Where(r => r.IsActive && r.Name.ToLower().Contains(searchQuery))
            .Select(r => r.Name)
            .Take(5)
            .ToListAsync(cancellationToken);

        var categories = await _context.MenuItems
            .Where(m => m.IsAvailable && m.Category.ToLower().Contains(searchQuery))
            .Select(m => m.Category)
            .Distinct()
            .Take(5)
            .ToListAsync(cancellationToken);

        var menuItems = await _context.MenuItems
            .Where(m => m.IsAvailable && m.Name.ToLower().Contains(searchQuery))
            .Select(m => m.Name)
            .Take(5)
            .ToListAsync(cancellationToken);

        var suggestions = restaurantNames
            .Concat(categories)
            .Concat(menuItems)
            .Distinct()
            .Take(10)
            .ToList();

        return Result<List<string>>.Success(suggestions);
    }
}
