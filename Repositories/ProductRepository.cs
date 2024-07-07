using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Models;

namespace ProductManagement.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<ProductDTO>> GetProductsFullDataAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<IEnumerable<ProductDTO>> GetProductsCheapestAsync();
}

public class ProductRepository : IProductRepository
{
    const int CHEAPEST_PRODUCTS = 5;
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        await _context.Entry(product).Reference(p => p.Category).LoadAsync();
        await _context.Entry(product).Reference(p => p.Author).LoadAsync();
        
        return product;
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(product.Id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
    
    public async Task<IEnumerable<ProductDTO>> GetProductsFullDataAsync()
    {
        var productsWithDetails = await GetProductsDetailsQuery().ToListAsync();
        return productsWithDetails;
    }
    
    public async Task<IEnumerable<ProductDTO>> GetProductsCheapestAsync()
    {
        var cheapestProducts = await GetProductsDetailsQuery()
            .OrderBy(product => product.Price)
            .Take(CHEAPEST_PRODUCTS)
            .ToListAsync();

        return cheapestProducts;
    }
    
    private IQueryable<ProductDTO> GetProductsDetailsQuery()
    {
        return _context.Products
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Select(product => new ProductDTO
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
            });
    }
}