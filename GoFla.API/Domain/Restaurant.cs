namespace GoFla.API.Domain;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
  
    public decimal DeliveryFee { get; set; }
    public int EstimatedDeliveryTime { get; set; } // in minutes
    public double  DeliveryRadiusKm  { get; set; } = 5;

    // Navigation properties
    public string?  OwnerId  { get; set; } = string.Empty; // ONLY FOR DEV purpose, not Nullable in prod
    public User Owner  { get; set; } = null!;

    public int  AddressId  { get; set; }
    public Address Address  { get; set; } = null!;

    public ICollection<MenuItem> MenuItems { get; set; } = [];
}
