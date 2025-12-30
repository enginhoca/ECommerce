using System;
using ECommerce.Entity.Concrete;

namespace ECommerce.Business.DTOs;

public class OrderFilterDto
{
    public OrderStatus? OrderStatus { get; set; } = null;
    public string? AppUserId { get; set; } = null;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public byte? Month { get; set; } = null;
}
