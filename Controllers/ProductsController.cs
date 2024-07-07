using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private const int PageNumber = 1;
        private const int PageSize = 10;

        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
            [FromQuery] int? categoryId,
            [FromQuery] int? authorId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNumber = PageNumber,
            [FromQuery] int pageSize = PageSize)
        {
            var products = await _productService.GetProductsAsync(categoryId, authorId, searchTerm, pageNumber, pageSize);
            return Ok(products);
        }
        
        [HttpGet("get-cheapest-products")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetCheapestProducts()
        {
            var products = await _productService.GetCheapestProductsAsync();
        
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productService.GetProductDetailAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductCreateDTO productCreateDto)
        {
            var product = await _productService.CreateProductAsync(productCreateDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateDTO productUpdateDto)
        {
            if (!await _productService.UpdateProductAsync(id, productUpdateDto))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await _productService.DeleteProductAsync(id))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
