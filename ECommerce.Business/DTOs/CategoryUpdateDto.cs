using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs;

public class CategoryUpdateDto
{
    [Required(ErrorMessage = "Id zorunludur!")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur!")]
    [MinLength(3, ErrorMessage = "Kategori adı en az 3 karakter olmalıdır!")]
    public string? Name { get; set; }

    public string? Description { get; set; }
}
