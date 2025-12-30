using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs;

public class ProductCreateDto
{
    public string? Name { get; set; }
    public string? Properties { get; set; }
    public decimal? Price { get; set; }
    public int StockQuantity { get; set; }
    public ICollection<int>? CategoryIds { get; set; }
}
