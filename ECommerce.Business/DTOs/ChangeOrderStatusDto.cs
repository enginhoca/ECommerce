using System;
using ECommerce.Entity.Concrete;

namespace ECommerce.Business.DTOs;

public class ChangeOrderStatusDto
{
    public int OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
}
