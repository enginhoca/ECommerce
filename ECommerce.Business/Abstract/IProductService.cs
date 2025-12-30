using System;
using System.Linq.Expressions;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Entity.Concrete;

namespace ECommerce.Business.Abstract;

public interface IProductService
{
    Task<ResponseDto<ProductDto>> GetAsync(int id);
    Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync(
        Expression<Func<Product, bool>>? predicate,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy,
        bool? includeCategories = null,
        int? categoryId = null,
        bool? isDeleted = null);


    Task<ResponseDto<PagedResultDto<ProductDto>>> GetAllPagedAsync(
        PaginationQueryDto paginationQueryDto,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        bool? includeCategories = false,
        int? categoryId = null,
        bool? isDeleted = null);

    Task<ResponseDto<IEnumerable<ProductDto>>> GetLowStockAsync(int threshold);

    Task<ResponseDto<int>> CountAsync();

    Task<ResponseDto<ProductDto>> CreateAsync(ProductCreateDto productCreateDto);
    Task<ResponseDto<NoContent>> UpdateAsync(int id, ProductUpdateDto productUpdateDto);
    Task<ResponseDto<NoContent>> SoftDeleteAsync(int id);
    Task<ResponseDto<NoContent>> HardDeleteAsync(int id);

}
