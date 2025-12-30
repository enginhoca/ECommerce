using System;

namespace ECommerce.Business.DTOs;

public class TokenDto
{
    public string? AccessToken { get; set; }
    public DateTime AccessTokenExpirationDate { get; set; }
}
