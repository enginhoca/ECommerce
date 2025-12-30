using System;

namespace ECommerce.Business.Exceptions;

public class NotFoundException : BusinessException
{
    public NotFoundException(string resourceName, object key) : base($"'{key}' id'li {resourceName} bulunamadı!", 404, "NOT_FOUND_ERROR")
    {

    }

    public NotFoundException(string message) : base(message, 404, "NOT_FOUND_ERROR")
    {

    }
}
