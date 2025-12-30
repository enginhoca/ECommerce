using ECommerce.Business.DTOs.ResponseDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected static IActionResult CreateResult<T>(ResponseDto<T> responseDto)
        {
            return new ObjectResult(responseDto) { StatusCode = responseDto.StatusCode };
        }

        protected string GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            return userId;
        }
    }
}
