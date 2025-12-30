using System;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Entity.Concrete;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
