using System;
using AutoMapper;
using ECommerce.Business.DTOs;
using ECommerce.Entity.Concrete;

namespace ECommerce.Business.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region AppUser
        CreateMap<AppUser, AppUserDto>();
        #endregion

        #region Category
        CreateMap<Category, CategoryDto>()
            .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt.UtcDateTime)
            )
            .ForMember(
                dest => dest.ModifiedAt,
                opt => opt.MapFrom(src => src.ModifiedAt.UtcDateTime)
            );
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();
        #endregion

        #region Product
        CreateMap<Product, ProductDto>()
            .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt.UtcDateTime)
            )
            .ForMember(
                dest => dest.ModifiedAt,
                opt => opt.MapFrom(src => src.ModifiedAt.UtcDateTime)
            )
            .ForMember(
                dest => dest.Categories,
                opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.Category)));
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
        #endregion

        #region Order/OrderItem
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(
                dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product!.Name))
            .ForMember(
                dest => dest.ProductImageUrl,
                opt => opt.MapFrom(src => src.Product!.ImageUrl));
        CreateMap<OrderItemDto, OrderDto>();
        CreateMap<OrderItemCreateDto, OrderItem>();

        CreateMap<Order, OrderDto>()
            .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt.UtcDateTime)
            )
            .ForMember(
                dest => dest.ModifiedAt,
                opt => opt.MapFrom(src => src.ModifiedAt.UtcDateTime)
            )
            .ForMember(
                dest => dest.AppUser,
                opt => opt.MapFrom(src => src.AppUser))
            .ForMember(
                dest => dest.OrderItems,
                opt => opt.MapFrom(src => src.OrderItems));
        CreateMap<IEnumerable<Order>, IEnumerable<OrderDto>>()
            .ConvertUsing((src, dest, context) => src.Select(o => context.Mapper.Map<OrderDto>(o)).ToList());
        CreateMap<OrderNowDto, Order>().ReverseMap();

        #endregion
    }
}
