using System;
using ECommerce.Business.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.API.Filters;

public class ValidationFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
            throw new ValidationException(errors);
        }
        await next();
    }
}
