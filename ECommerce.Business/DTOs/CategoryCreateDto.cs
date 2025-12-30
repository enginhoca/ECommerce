using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs;

public class CategoryCreateDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur!")]
    [MinLength(3, ErrorMessage = "Kategori adı en az 3 karakter olmalıdır!")]
    public string? Name { get; set; }

    public string? Description { get; set; }
}
