using System;

namespace ECommerce.Business.Exceptions;

public class ValidationException : BusinessException
{
    public Dictionary<string, string[]> Errors { get; }
    public ValidationException(Dictionary<string, string[]> errors) : base("Doğrulama hatası!", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string message) : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }
}
