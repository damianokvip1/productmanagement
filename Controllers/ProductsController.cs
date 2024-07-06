using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Category = new CategoryDTO
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name
                    },
                    Author = new AuthorDTO
                    {
                        Id = p.Author.Id,
                        Name = p.Author.Name,
                        Biography = p.Author.Biography,
                        DateOfBirth = p.Author.DateOfBirth
                    }
                })
                .ToListAsync();

            return products;
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = new CategoryDTO
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Author = new AuthorDTO
                {
                    Id = product.Author.Id,
                    Name = product.Author.Name,
                    Biography = product.Author.Biography,
                    DateOfBirth = product.Author.DateOfBirth
                }
            };

            return productDto;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductCreateDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Description = productDTO.Description,
                CategoryId = productDTO.CategoryId,
                AuthorId = productDTO.AuthorId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Refresh product with Category information
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.Author).LoadAsync();

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = new CategoryDTO
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Author = new AuthorDTO
                {
                    Id = product.Author.Id,
                    Name = product.Author.Name,
                    Biography = product.Author.Biography,
                    DateOfBirth = product.Author.DateOfBirth
                }
            };

            // 201 stt & productDto
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateDTO productDTO)
        {
            if (id != productDTO.Id)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Id = productDTO.Id;
            product.Name = productDTO.Name;
            product.Price = productDTO.Price;
            product.Description = productDTO.Description;
            product.CategoryId = productDTO.CategoryId;
            product.AuthorId = productDTO.AuthorId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
