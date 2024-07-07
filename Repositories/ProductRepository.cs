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
}

public class ProductRepository : IProductRepository
{
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
        var productsWithCategoriesAndAuthors = await (
                from product in _context.Products
                join category in _context.Categories on product.CategoryId equals category.Id
                join author in _context.Authors on product.AuthorId equals author.Id
                select new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    Category = new CategoryDTO
                    {
                        Id = category.Id,
                        Name = category.Name
                    },
                    Author = new AuthorDTO()
                    {
                        Id = author.Id,
                        Name = author.Name,
                        Biography = author.Biography,
                        DateOfBirth = author.DateOfBirth,
                    }
                })
            .ToListAsync();
        
        var productsWithCategoriesAndAuthors2 = await _context.Products
            .Join(_context.Categories, product => product.CategoryId, category => category.Id, (product, category) => new { product, category })
            .Join(_context.Authors, pc => pc.product.AuthorId, author => author.Id, (pc, author) => new ProductDTO
            {
                Id = pc.product.Id,
                Name = pc.product.Name,
                Price = pc.product.Price,
                Description = pc.product.Description,
                Category = new CategoryDTO
                {
                    Id = pc.category.Id,
                    Name = pc.category.Name
                },
                Author = new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name,
                    Biography = author.Biography,
                    DateOfBirth = author.DateOfBirth
                }
            })
            .ToListAsync();

        return productsWithCategoriesAndAuthors;
    }
}