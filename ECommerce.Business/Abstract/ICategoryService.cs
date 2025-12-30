using System;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;

namespace ECommerce.Business.Abstract;

public interface ICategoryService
{
    Task<ResponseDto<CategoryDto>> GetAsync(int id);
    Task<ResponseDto<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<ResponseDto<int>> CountAsync();
    Task<ResponseDto<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto);
    Task<ResponseDto<NoContent>> UpdateAsync(int id, CategoryUpdateDto categoryUpdateDto);
    Task<ResponseDto<NoContent>> SoftDeleteAsync(int id);
    Task<ResponseDto<NoContent>> HardDeleteAsync(int id);
}
