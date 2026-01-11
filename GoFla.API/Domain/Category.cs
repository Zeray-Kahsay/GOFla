using System;

namespace GoFla.API.Domain;

public class Category
{
    public int  Id  { get; set; }
    public string  Name  { get; set; } = string.Empty;
    public int  SortOrder  { get; set; } = 0;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt  { get; set; } = DateTime.UtcNow;
    public int  RestaurantId  { get; set; } 
    public Restaurant Restaurant  { get; set; } = null!;

    public ICollection<MenuItem> MenuItems  { get; set; } = [];
}
