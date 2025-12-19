using System;
using Microsoft.AspNetCore.Identity;


namespace GoFla.API.Domain;

public class AppUserRole : IdentityUserRole<string>
{
    public User User { get; set; } = null!;
    public AppRole Role   { get; set; } = null!;
}
