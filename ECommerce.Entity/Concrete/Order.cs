using System;
using ECommerce.Entity.Abstract;

namespace ECommerce.Entity.Concrete;

public class Order : BaseClass
{
    public string AppUserId { get; set; }=string.Empty;
    public AppUser? AppUser { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}

public enum OrderStatus
{
    Pending = 0,
    Proccesing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}