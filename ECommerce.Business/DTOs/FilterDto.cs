using System;
using System.Linq.Expressions;

namespace ECommerce.Business.DTOs;

public class FilterDto
{
    public bool? IncludeCategories { get; set; } = true;
    public int? CategoryId { get; set; } = null;
    public bool? IsDeleted { get; set; } = null;

}
