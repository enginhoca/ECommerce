using System;
using System.Linq.Expressions;
using AutoMapper;
using ECommerce.Business.Abstract;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Business.Exceptions;
using ECommerce.Data.Abstract;
using ECommerce.Entity.Concrete;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business.Concrete;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = _unitOfWork.GetRepository<Product>();
        _categoryRepository = _unitOfWork.GetRepository<Category>();
        _mapper = mapper;
    }

    public async Task<ResponseDto<ProductDto>> CreateAsync(ProductCreateDto productCreateDto)
    {
        try
        {
            // En az bir kategori id'si gönderilmiş mi?
            if (productCreateDto.CategoryIds is null || productCreateDto.CategoryIds.Count == 0)
            {
                return ResponseDto<ProductDto>.Fail("En az bir kategori seçilmelidiri!", StatusCodes.Status400BadRequest);
            }

            List<ProductCategory> productCategories = [];
            // Gönderilen kategori id'leri, gerçekten db'de var mı?
            foreach (var categoryId in productCreateDto.CategoryIds)
            {
                var category = await _categoryRepository.GetAsync(categoryId);
                if (category is null)
                {
                    return ResponseDto<ProductDto>.Fail($"{categoryId} id'li kategori veri tabanında bulunamadığı için kayıt gerçekleştirilemedi!", StatusCodes.Status400BadRequest);
                }
                productCategories.Add(new ProductCategory { Category = category, CategoryId = categoryId });
            }

            var product = _mapper.Map<Product>(productCreateDto);

            product.ProductCategories = productCategories;

            await _productRepository.AddAsync(product);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<ProductDto>.Fail("Veri tabanından kaynaklı bir hata oluştuğu için, kaydetme işlemi yapılamadı!", StatusCodes.Status500InternalServerError);
            }

            var productDto = _mapper.Map<ProductDto>(product);

            return ResponseDto<ProductDto>.Success(productDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContent>> HardDeleteAsync(int id)
    {
        try
        {
            var deletedProduct = await _productRepository.GetAsync(
                predicate: x => x.Id == id,
                showIsDeleted: true);

            if (deletedProduct == null)
            {
                return ResponseDto<NoContent>.Fail($"{id} id'li ürün bulunamadığı için silme işlemi gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }

            _productRepository.Remove(deletedProduct);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Ürün silinmeye çalışılırken, veri tabanından kaynaklı bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContent>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }


    public async Task<ResponseDto<NoContent>> SoftDeleteAsync(int id)
    {
        try
        {
            var deletedProduct = await _productRepository.GetAsync(
                predicate: x => x.Id == id,
                showIsDeleted: true);

            if (deletedProduct == null)
            {
                return ResponseDto<NoContent>.Fail($"{id} id'li ürün bulunamadığı için silme işlemi gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }

            deletedProduct.IsDeleted = !deletedProduct.IsDeleted;
            deletedProduct.ModifiedAt = DateTime.UtcNow;

            _productRepository.Update(deletedProduct);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Ürün silinmeye çalışılırken, veri tabanından kaynaklı bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContent>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync(
        Expression<Func<Product, bool>>? predicate,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy,
        bool? includeCategories = false,
        int? categoryId = null,
        bool? isDeleted = null)
    {
        try
        {
            if (predicate is null)
            {
                predicate = PredicateBuilder.New<Product>(true);
            }
            // predicate = x=>x.CategoryId==45 && x.IsDeleted==filterDto.IsDeleted ;
            if (isDeleted.HasValue)
            {
                predicate = predicate.And(x => x.IsDeleted == isDeleted);
            }

            if (categoryId.HasValue)
            {
                predicate = predicate.And(x => x.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));
            }

            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>();
            if (includeCategories.HasValue)
            {
                includeList.Add(q => q.Include(x => x.ProductCategories).ThenInclude(y => y.Category));
            }

            var products = await _productRepository.GetAllAsync(
                predicate: predicate,
                showIsDeleted: isDeleted ?? false,
                orderBy: orderBy!,
                asExpanded: true,
                includes: includeList.ToArray()
            );
            if (!products.Any())
            {
                return ResponseDto<IEnumerable<ProductDto>>.Fail("Hiç ürün bulunamadı!", StatusCodes.Status404NotFound);
            }

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }

    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetLowStockAsync(int threshold)
    {
        try
        {
            var products = await _productRepository.GetAllAsync(
                predicate: x => x.StockQuantity < threshold, orderBy: x => x.OrderByDescending(y => y.StockQuantity), asExpanded: true, includes: q => q.Include(x => x.ProductCategories).ThenInclude(pc => pc.Category)
            );
            if (!products.Any())
            {
                return ResponseDto<IEnumerable<ProductDto>>.Fail("Hiç ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }

    }

    public async Task<ResponseDto<ProductDto>> GetAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == id,
                asExpanded: true,
                includes: query => query.Include(x => x.ProductCategories).ThenInclude(y => y.Category)
            );
            if (product == null)
            {
                return ResponseDto<ProductDto>.Fail($"{id} id'li ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return ResponseDto<ProductDto>.Success(productDto, 200);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContent>> UpdateAsync(int id, ProductUpdateDto productUpdateDto)
    {
        try
        {
            if (id != productUpdateDto.Id)
            {
                return ResponseDto<NoContent>.Fail("Id bilgileri eşleşmiyor!", StatusCodes.Status400BadRequest);
            }

            var updatedProduct = await _productRepository.GetAsync(
                predicate: x => x.Id == id,
                includes: q => q.Include(x => x.ProductCategories)
            );

            if (updatedProduct == null)
            {
                return ResponseDto<NoContent>.Fail($"{id} id'li ürün bulunamadığı için, güncelleme işlemi yapılamadı.", StatusCodes.Status404NotFound);
            }

            // En az bir kategori id'si gönderilmiş mi?
            if (productUpdateDto.CategoryIds is null || productUpdateDto.CategoryIds.Count == 0)
            {
                return ResponseDto<NoContent>.Fail("En az bir kategori seçilmelidir!", StatusCodes.Status400BadRequest);
            }

            // Gönderilen kategori id'leri, gerçekten db'de var mı?
            foreach (var categoryId in productUpdateDto.CategoryIds)
            {
                bool categoryExists = await _categoryRepository.ExistsAsync(x => x.Id == categoryId);
                if (!categoryExists)
                {
                    return ResponseDto<NoContent>.Fail($"{categoryId} id'li kategori veri tabanında bulunamadığı için kayıt güncellenemedi!", StatusCodes.Status400BadRequest);
                }
            }

            _mapper.Map(productUpdateDto, updatedProduct);
            updatedProduct.ProductCategories.Clear();
            updatedProduct.ProductCategories =
                productUpdateDto
                    .CategoryIds
                    .Select(categoryId => new ProductCategory
                    {
                        ProductId = updatedProduct.Id,
                        CategoryId = categoryId
                    }).ToList();
            updatedProduct.ModifiedAt = DateTime.UtcNow;

            _productRepository.Update(updatedProduct);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Ürün güncellenirken, veri tabanından kaynaklı bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContent>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<int>> CountAsync()
    {
        try
        {
            var count = await _productRepository.CountAsync();
            return ResponseDto<int>.Success(count, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<PagedResultDto<ProductDto>>> GetAllPagedAsync(PaginationQueryDto paginationQueryDto, Expression<Func<Product, bool>>? predicate = null, Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null, bool? includeCategories = false, int? categoryId = null, bool? isDeleted = null)
    {
        try
        {
            if (predicate is null)
            {
                predicate = PredicateBuilder.New<Product>(true);// x=>true
            }
            if (isDeleted.HasValue)
            {
                predicate = predicate.And(x => x.IsDeleted == isDeleted);
            }
            if (categoryId.HasValue)
            {
                predicate = predicate.And(x => x.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));
            }
            if (orderBy is null)
            {
                orderBy = x => x.OrderByDescending(y => y.CreatedAt);
            }
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>();
            if (includeCategories.HasValue && includeCategories.Value)
            {
                includeList.Add(q => q.Include(x => x.ProductCategories).ThenInclude(y => y.Category));
            }
            var (products, totalCount) = await _productRepository.GetPagedAsync(
                predicate: predicate,
                orderBy: orderBy,
                skip: paginationQueryDto.Skip,
                take: paginationQueryDto.Take,
                showIsDeleted: isDeleted ?? false,
                asExpanded: true,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            var pagedResultDto = PagedResultDto<ProductDto>.Create(productDtos, paginationQueryDto.PageNumber, paginationQueryDto.PageSize, totalCount);
            return ResponseDto<PagedResultDto<ProductDto>>.Success(pagedResultDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            throw new BusinessException(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }
}

