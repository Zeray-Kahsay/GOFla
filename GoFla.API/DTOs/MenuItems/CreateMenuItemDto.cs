using System;

namespace GoFla.API.DTOs.MenuItems;

public record CreateMenuItemDto 
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    //public int  RestaurantId { get; set; }

    public bool IsAvailable { get; set; } = true;
    public IFormFile Image  { get; set; } = null!;
}
