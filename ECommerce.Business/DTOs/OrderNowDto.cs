using System;

namespace ECommerce.Business.DTOs;

public class OrderNowDto
{
    public string? AppUserId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public ICollection<OrderItemCreateDto> OrderItems { get; set; } = [];
}
