using System;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;
using AutoMapper;
using ECommerce.Business.Abstract;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Data.Abstract;
using ECommerce.Entity.Concrete;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business.Concrete;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Order> _orderRepository;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productRepository = unitOfWork.GetRepository<Product>();
        _orderRepository = unitOfWork.GetRepository<Order>();
    }

    public async Task<ResponseDto<NoContent>> CancelAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetAsync(id);
            if (order is null)
            {
                return ResponseDto<NoContent>.Fail("İlgili sipariş bulunamadığı için iptal işlemi gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            order.OrderStatus = OrderStatus.Cancelled;
            order.ModifiedAt = DateTime.UtcNow;
            _orderRepository.Update(order);
            await _orderRepository.AddAsync(order);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Sipariş edilirken bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContent>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContent>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto)
    {
        try
        {
            var order = await _orderRepository.GetAsync(changeOrderStatusDto.OrderId);
            if (order is null)
            {
                return ResponseDto<NoContent>.Fail("İlgili sipariş bulunamadığı için iptal işlemi gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            order.OrderStatus = changeOrderStatusDto.OrderStatus;
            order.ModifiedAt = DateTime.UtcNow;
            _orderRepository.Update(order);
            await _orderRepository.AddAsync(order);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Sipariş edilirken bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContent>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto? orderFilterDto = null!)
    {
        try
        {
            Expression<Func<Order, bool>> pred = x => true;
            if (orderFilterDto is not null)
            {
                if (!string.IsNullOrEmpty(orderFilterDto.AppUserId))
                {
                    pred = pred.And(x => x.AppUserId == orderFilterDto.AppUserId);
                }
                if (orderFilterDto.OrderStatus.HasValue)
                {
                    pred = pred.And(x => x.OrderStatus == orderFilterDto.OrderStatus);
                }
                if (orderFilterDto.StartDate.HasValue && orderFilterDto.EndDate.HasValue)
                {
                    pred = pred.And(x => x.CreatedAt >= orderFilterDto.StartDate && x.CreatedAt <= orderFilterDto.EndDate);
                }
                if (orderFilterDto.Month.HasValue)
                {
                    // Month=5
                    var today = DateTime.UtcNow; // 2025-12-16
                    var date = today.AddMonths(-(orderFilterDto.Month.Value - 1)); // 2025-8-16
                    var newDate = new DateTime(date.Year, date.Month, 1); // 2025-8-1
                                                                          //var date = today.AddMonths(-(byte)orderFilterDto.Month);
                    pred = pred.And(x => x.CreatedAt >= newDate);
                }
            }

            var orders = await _orderRepository.GetAllAsync(
                predicate: pred,
                orderBy: x => x.OrderByDescending(o => o.CreatedAt),
                includes: query => query
                    .Include(x => x.AppUser)
                    .Include(x => x.OrderItems)
                    .ThenInclude(oi => oi.Product)
            );
            if (orders is null || !orders.Any())
            {
                return ResponseDto<IEnumerable<OrderDto>>.Fail("Herhangi bir sipariş bilgisi bulunamadı!", StatusCodes.Status404NotFound);
            }
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ResponseDto<IEnumerable<OrderDto>>.Success(orderDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<OrderDto>>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<OrderDto>> GetAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetAsync(
                predicate: x => x.Id == id,
                includes: q => q
                        .Include(x => x.AppUser)
                        .Include(x => x.OrderItems)
                        .ThenInclude(y => y.Product)
            );
            if (order is null)
            {
                return ResponseDto<OrderDto>.Fail("İlgili sipariş bulunamadı.", StatusCodes.Status404NotFound);
            }
            var orderDto = _mapper.Map<OrderDto>(order);
            return ResponseDto<OrderDto>.Success(orderDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<OrderDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<OrderDto>> OrderNowAsync(OrderNowDto orderNowDto)
    {
        try
        {
            // Sipariş edilen ürünler veri tabanında var mı yok mu kontrolü yapıyoruz
            foreach (var orderItem in orderNowDto.OrderItems)
            {
                var isExists = await _productRepository.ExistsAsync(p => p.Id == orderItem.ProductId);
                if (!isExists)
                {
                    return ResponseDto<OrderDto>.Fail($"{orderItem.ProductId} id'li ürün bulunamadığı için sipariş işlemi tamamlanamamıştır. Lütfen daha sonra yeniden deneyiniz!", StatusCodes.Status404NotFound);
                }
            }
            var order = _mapper.Map<Order>(orderNowDto);
            // Normalde tam bu aşamada ödeme operasyonu yapılmalı, ödeme başarılıysa order veri tabanına kaydedilmeli. Değilse order kaydedilmemeli.
            await _orderRepository.AddAsync(order);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<OrderDto>.Fail("Sipariş veri tabanına kaydedilirken bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }
            // Normalde tam bu aşamada sepet boşaltılmalı.
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product = await _productRepository.GetAsync(orderItem.ProductId);
            }
            var orderDto = _mapper.Map<OrderDto>(order);
            return ResponseDto<OrderDto>.Success(orderDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<OrderDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }
}
