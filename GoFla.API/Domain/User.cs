using Microsoft.AspNetCore.Identity;

namespace GoFla.API.Domain;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<AppUserRole> UserRoles { get; set; } = [];

    public Cart? Cart { get; set; }
}

