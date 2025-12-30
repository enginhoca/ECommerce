using ECommerce.Business.Abstract;
using ECommerce.Business.DTOs;
using ECommerce.Entity.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CustomControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetAllProductsPaged(
            [FromQuery] PaginationQueryDto paginationQueryDto,
            [FromQuery] int? categoryId = null
        )
        {
            var response = await _productService.GetAllPagedAsync(
                paginationQueryDto: paginationQueryDto,
                orderBy: null,
                includeCategories: true,
                categoryId: categoryId
            );
            return CreateResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllAsync(
                predicate: x => true,
                orderBy: x => x.OrderByDescending(y => y.CreatedAt),
                includeCategories: true
            );
            return CreateResult(response);
        }

        [HttpGet("getallnew")]
        public async Task<IActionResult> GetAllProductsNew()
        {
            var response = await _productService.GetAllAsync(
                predicate: x => true,
                orderBy: x => x.OrderByDescending(y => y.CreatedAt),
                includeCategories: true
            );
            return CreateResult(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productCreateDto)
        {
            var response = await _productService.CreateAsync(productCreateDto);
            return CreateResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetAsync(id);
            return CreateResult(response);
        }


        [HttpGet("low")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int thresold)
        {
            var response = await _productService.GetLowStockAsync(thresold);
            return CreateResult(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteProduct(int id)
        {
            var response = await _productService.SoftDeleteAsync(id);
            return CreateResult(response);
        }

        // [HttpDelete("{id}/hard-delete")]
        // public async Task<IActionResult> HardDeleteProduct(int id)
        [HttpDelete]
        public async Task<IActionResult> HardDeleteProduct([FromQuery] int id)
        {
            var response = await _productService.HardDeleteAsync(id);
            return CreateResult(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productUpdateDto)
        {
            var response = await _productService.UpdateAsync(id, productUpdateDto);
            return CreateResult(response);
        }

    }
}
