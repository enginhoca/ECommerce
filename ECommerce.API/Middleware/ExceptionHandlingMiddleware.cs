using System;
using System.Net;
using System.Text.Json;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Business.Exceptions;

namespace ECommerce.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;
        ResponseDto<object> errorResponse;

        switch (exception)
        {
            case NotFoundException notFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = ResponseDto<object>.Fail(notFoundException.Message, (int)HttpStatusCode.NotFound);
                _logger.LogWarning(exception, "Kaynak bulunamadı:{Mesaj}", notFoundException.Message);
                break;

            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;

                var validationMessage =
                    validationException.Errors.Any()
                        ? string.Join("; ", validationException.Errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")))
                        : validationException.Message;

                errorResponse = ResponseDto<object>.Fail(validationMessage, (int)HttpStatusCode.BadRequest);
                _logger.LogWarning(exception, "BACKEND 13 Doğrulama hatası:{Mesaj}", validationException.Message);
                break;

            case UnauthorizedException unauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = ResponseDto<object>.Fail(unauthorizedException.Message, (int)HttpStatusCode.Unauthorized);
                _logger.LogWarning(exception, "Yetki hatası:{Mesaj}", unauthorizedException.Message);
                break;

            case BusinessException businessException:
                response.StatusCode = businessException.StatusCode;
                var businessErrorMessage = _environment.IsDevelopment()
                    ? $"[{businessException.ErrorCode}] {businessException.Message}"
                    : businessException.Message;
                errorResponse = ResponseDto<object>.Fail(businessErrorMessage, businessException.StatusCode);
                _logger.LogWarning(exception, "Servis hatası:{Hata Kodu} - {Mesaj}", businessException.ErrorCode, businessException.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorMessage = _environment.IsDevelopment()
                    ? exception.Message
                    : "Bir hata oluştu.";
                errorResponse = ResponseDto<object>.Fail(errorMessage, (int)HttpStatusCode.InternalServerError);
                _logger.LogWarning(exception, "Beklenmedik hata:{Mesaj}", exception.Message);
                break;
        }
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(jsonResponse);
    }
}
