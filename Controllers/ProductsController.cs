using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private const int PageNumber = 1;
        private const int PageSize = 10;

        [HttpGet]
        public async Task<CustomResponseDto> GetProducts(
            [FromQuery] int? categoryId,
            [FromQuery] int? authorId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNumber = PageNumber,
            [FromQuery] int pageSize = PageSize)
        {
            var products = await productService.GetProductsAsync(categoryId, authorId, searchTerm, pageNumber, pageSize);
            
            return !products.Any()
                ? new CustomResponseDto { StatusCode = 404, IsError = true, Message = "No products found matching the criteria.", Data = null }
                : new CustomResponseDto { StatusCode = 200, IsError = false, Message = "Success!", Data = products };
        }
        
        [HttpGet("get-cheapest-products")]
        public async Task<CustomResponseDto> GetCheapestProducts()
        {
            var products = await productService.GetCheapestProductsAsync();
            return !products.Any()
                ? new CustomResponseDto { StatusCode = 404, IsError = true, Message = "No products found matching the criteria.", Data = null }
                : new CustomResponseDto { StatusCode = 200, IsError = false, Message = "Success!", Data = products };
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomResponseDto>> GetProduct(int id)
        {
            var product = await productService.GetProductDetailAsync(id);
            if (product == null)
            {
                return new CustomResponseDto
                {
                    StatusCode = 404, IsError = true, Message = "Not found product", Data = null
                };
            }

            return new CustomResponseDto { StatusCode = 200, IsError = false, Message = "Success!", Data = product };
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductCreateDTO productCreateDto)
        {
            var userCreateId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await productService.CreateProductAsync(productCreateDto, int.Parse(userCreateId));
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateDTO productUpdateDto)
        {
            var userUpdateId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await productService.UpdateProductAsync(id, productUpdateDto, int.Parse(userUpdateId)))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await productService.DeleteProductAsync(id))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
