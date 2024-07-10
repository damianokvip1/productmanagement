using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Models;

namespace ProductManagement.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductDto.ProductData>> GetProductsAsync(int? categoryId, int? authorId, string? searchTerm, int pageNumber, int pageSize);
    Task<IEnumerable<ProductDto.ProductData>> GetProductsFullDataAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<IEnumerable<ProductDto.ProductData>> GetProductsCheapestAsync();
}

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    private const int CheapestProducts = 5;

    public async Task<IEnumerable<ProductDto.ProductData>> GetProductsAsync(int? categoryId, int? authorId, string? searchTerm, int pageNumber, int pageSize)
    {
        var query = GetProductsDetailsQuery();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.Category.Id == categoryId.Value);
        }

        if (authorId.HasValue)
        {
            query = query.Where(p => p.Author.Id == authorId.Value);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Category.Name.Contains(searchTerm) ||
                p.Author.Name.Contains(searchTerm));
        }
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();

        await context.Entry(product).Reference(p => p.Category).LoadAsync();
        await context.Entry(product).Reference(p => p.Author).LoadAsync();
        
        return product;
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(product.Id))
            {
                return false;
            }

            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return false;

        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return true;
    }

    private bool ProductExists(int id) => context.Products.Any(e => e.Id == id);

    public async Task<IEnumerable<ProductDto.ProductData>> GetProductsFullDataAsync()
    {
        var productsWithDetails = await GetProductsDetailsQuery().ToListAsync();
        return productsWithDetails;
    }
    
    public async Task<IEnumerable<ProductDto.ProductData>> GetProductsCheapestAsync()
    {
        var cheapestProducts = await GetProductsDetailsQuery()
            .OrderBy(product => product.Price)
            .Take(CheapestProducts)
            .ToListAsync();

        return cheapestProducts;
    }
    
    private IQueryable<ProductDto.ProductData> GetProductsDetailsQuery()
    {
        return context.Products
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Include(p => p.UserCreate)
            .Include(p => p.UserUpdate)
            .Select(product => new ProductDto.ProductData
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = new CategoryDto.CategoryData
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Author = new AuthorDto.AuthorData
                {
                    Id = product.Author.Id,
                    Name = product.Author.Name,
                    Biography = product.Author.Biography,
                    DateOfBirth = product.Author.DateOfBirth
                },
                UserCreate = new UserDto.UserData
                {
                    Id = product.UserCreate.Id,
                    Email = product.UserCreate.Email,
                    UserName = product.UserCreate.UserName
                },
                UserUpdate = new UserDto.UserData
                {
                    Id = product.UserUpdate.Id,
                    Email = product.UserUpdate.Email,
                    UserName = product.UserUpdate.UserName
                }
            });
    }
}