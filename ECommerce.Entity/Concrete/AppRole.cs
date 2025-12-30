using System;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Entity.Concrete;

public class AppRole:IdentityRole
{
    public string? Description { get; set; }
}
