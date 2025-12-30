using System;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Entity.Concrete;

namespace ECommerce.Business.Abstract;

public interface IOrderService
{
    Task<ResponseDto<OrderDto>> OrderNowAsync(OrderNowDto orderNowDto);
    Task<ResponseDto<NoContent>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto);
    Task<ResponseDto<NoContent>> CancelAsync(int id);
    Task<ResponseDto<OrderDto>> GetAsync(int id);
    Task<ResponseDto<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto? orderFilterDto = null);
}
