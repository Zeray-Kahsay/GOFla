using System;

namespace GoFla.API.DTOs.MenuItems;

public record CreateMenuItemDto 
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public string CategoryName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
  
   
}
