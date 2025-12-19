using System;
using Microsoft.AspNetCore.Identity;

namespace GoFla.API.Domain;

public class AppRole : IdentityRole
{
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}
