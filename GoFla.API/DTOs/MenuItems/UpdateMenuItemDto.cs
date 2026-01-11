using System;

namespace GoFla.API.DTOs.MenuItems;

public class UpdateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public bool IsAvailable { get; set; } = true;
    public string?  ImageUrl { get; set; }
}
