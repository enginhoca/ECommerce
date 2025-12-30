using System;
using ECommerce.Entity.Abstract;

namespace ECommerce.Entity.Concrete;

public class Category : BaseClass
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; } = [];
}
