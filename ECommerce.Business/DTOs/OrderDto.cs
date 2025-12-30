using System;
using ECommerce.Entity.Concrete;

namespace ECommerce.Business.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? AppUserId { get; set; }
    public AppUserDto? AppUser { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = [];
    public decimal OrderTotalPrice => OrderItems.Sum(oi => oi.ItemTotalPrice);
}








// public decimal OrderTotalPrice()
// {
//     // return OrderItems.Sum(oi=>oi.Quantity*oi.UnitPrice);
//     return OrderItems.Sum(oi=>oi.ItemTotalPrice);
// }