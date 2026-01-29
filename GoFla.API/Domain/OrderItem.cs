using System;

namespace GoFla.API.Domain;

public class OrderItem
{
    public int  MenuItemId  { get; set; }
    public string  Name  { get; set; } = string.Empty;
    public decimal  UnitPrice  { get; set; }
    public int  Quantity  { get; set; }
    public decimal  TotalPrice { get; set; }
    public string  SpecialInstructions  { get; set; } = string.Empty;
}
