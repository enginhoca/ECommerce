using System;

namespace ECommerce.Business.Exceptions;

public class BusinessException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public BusinessException(string message, int statusCode = 400, string errorCode = "BUSINESS_ERROR") : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
    public BusinessException(string message, Exception innerException, int statusCode = 400, string errorCode = "BUSINESS_ERROR") : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
