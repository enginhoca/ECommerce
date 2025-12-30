using System;
using AutoMapper;
using ECommerce.Business.Abstract;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Data.Abstract;
using ECommerce.Entity.Concrete;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.Concrete;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Category> _categoryRepository;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _categoryRepository = _unitOfWork.GetRepository<Category>();
    }

    public async Task<ResponseDto<int>> CountAsync()
    {
        try
        {
            var count = await _categoryRepository.CountAsync();
            return ResponseDto<int>.Success(count, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto)
    {
        try
        {
            var isExists = await _categoryRepository.ExistsAsync(x => x.Name.ToLower() == categoryCreateDto.Name!.ToLower());
            if (isExists)
            {
                return ResponseDto<CategoryDto>.Fail($"{categoryCreateDto.Name} adında bir kategori, zaten mevcut!", StatusCodes.Status409Conflict);
            }
            var category = _mapper.Map<Category>(categoryCreateDto);
            await _categoryRepository.AddAsync(category);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<CategoryDto>.Fail("Veri tabanından kaynaklı bir hata oluştuğu için, kaydetme işlemi yapılamadı!", StatusCodes.Status500InternalServerError);
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ResponseDto<CategoryDto>.Success(categoryDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<CategoryDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (!categories.Any())
            {
                return ResponseDto<IEnumerable<CategoryDto>>.Fail("Hiç kategori bulunamadı!", StatusCodes.Status404NotFound);
            }
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return ResponseDto<IEnumerable<CategoryDto>>.Success(categoryDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<CategoryDto>>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<CategoryDto>> GetAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetAsync(id);
            if (category is null)
            {
                return ResponseDto<CategoryDto>.Fail($"{id} id'li kategori bilgisi bulunamadı!", StatusCodes.Status404NotFound);
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ResponseDto<CategoryDto>.Success(categoryDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<CategoryDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContent>> HardDeleteAsync(int id)
    {
        try
        {
            var deletedCategory = await _categoryRepository.GetAsync(
                predicate: x => x.Id == id,
                showIsDeleted: true);

            if (deletedCategory == null)
            {
                return ResponseDto<NoContent>.Fail($"{id} id'li kategori bulunamadığı için silme işlemi gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }

            _categoryRepository.Remove(deletedCategory);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Kategori silinmeye çalışılırken, veri tabanından kaynaklı bir sorun oluştu!", StatusCodes.Status500InternalServerError);
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
            var deletedCategory = await _categoryRepository.GetAsync(
                predicate: x => x.Id == id,
                showIsDeleted: true);

            if (deletedCategory == null)
            {
                return ResponseDto<NoContent>.Fail($"{id} id'li kategori bulunamadığı için silme işlemi gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }

            deletedCategory.IsDeleted = !deletedCategory.IsDeleted;
            deletedCategory.ModifiedAt = DateTime.UtcNow;

            _categoryRepository.Update(deletedCategory);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContent>.Fail("Kategori silinmeye çalışılırken, veri tabanından kaynaklı bir sorun oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContent>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContent>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContent>> UpdateAsync(int id, CategoryUpdateDto categoryUpdateDto)
    {
        try
        {
            if (id != categoryUpdateDto.Id)
            {
                return ResponseDto<NoContent>.Fail("Id bilgileri eşleşmiyor!", StatusCodes.Status400BadRequest);
            }
            var updatedCategory = await _categoryRepository.GetAsync(id);
            if (updatedCategory is null)
            {
                return ResponseDto<NoContent>.Fail($"{id} id'li kategori bilgisi, bulunamadıpğı için güncelleme yapılamadı", StatusCodes.Status404NotFound);
            }
            _mapper.Map(categoryUpdateDto, updatedCategory);
            updatedCategory.ModifiedAt = DateTime.UtcNow;
            _categoryRepository.Update(updatedCategory);
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
}
