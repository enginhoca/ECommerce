using System;
using ECommerce.Business.DTOs;
using FluentValidation;

namespace ECommerce.Business.Validators;

public class ProductCreateDtoValidator:AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
    {
        RuleFor(x=>x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur!")
            .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır!")
            .MaximumLength(100).WithMessage("Ürün adı en fazla 100 karakter olabilir!")
            .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ:\s]+$").WithMessage("Ürün adı sadece harf, rakam ve boşluk içerebilir!");
        
        RuleFor(x=>x.Properties)
            .NotEmpty().WithMessage("Ürün açıklaması zorunludur!")
            .MinimumLength(10).WithMessage("Ürün açıklaması en az 10 karakter olmalıdır!");

        RuleFor(x=>x.Price)
            .NotEmpty().WithMessage("Fiyat zorunludur!")
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır!")
            .LessThanOrEqualTo(1000000).WithMessage("Fiyat en fazla 1.000.000 TL olabilir!");

        RuleFor(x=>x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz!")
            .LessThanOrEqualTo(10000).WithMessage("Stok miktarı en fazla 10.000 adet olabilir!");

        RuleFor(x=>x.CategoryIds)
            .NotEmpty().WithMessage("En az bir kategori seçilmelidir!")
            .Must(categories=> categories!=null && categories.Count>0).WithMessage("En az bir kategori seçilmelidir!");

    }
}
