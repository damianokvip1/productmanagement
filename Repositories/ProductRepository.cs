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
        return from product in context.Products
            join category in context.Categories on product.CategoryId equals category.Id into categoryGroup
            from category in categoryGroup.DefaultIfEmpty()
            join author in context.Authors on product.AuthorId equals author.Id into authorGroup
            from author in authorGroup.DefaultIfEmpty()
            join userCreate in context.Users on product.UserCreateId equals userCreate.Id into userCreateGroup
            from userCreate in userCreateGroup.DefaultIfEmpty()
            join userUpdate in context.Users on product.UserUpdateId equals userUpdate.Id into userUpdateGroup
            from userUpdate in userUpdateGroup.DefaultIfEmpty()
            select new ProductDto.ProductData
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = category == null ? null : new CategoryDto.CategoryData
                {
                    Id = category.Id,
                    Name = category.Name
                },
                Author = author == null ? null : new AuthorDto.AuthorData
                {
                    Id = author.Id,
                    Name = author.Name,
                    Biography = author.Biography,
                    DateOfBirth = author.DateOfBirth
                },
                UserCreate = userCreate == null ? null : new UserDto.UserData
                {
                    Id = userCreate.Id,
                    Email = userCreate.Email,
                    UserName = userCreate.UserName
                },
                UserUpdate = userUpdate == null ? null : new UserDto.UserData
                {
                    Id = userUpdate.Id,
                    Email = userUpdate.Email,
                    UserName = userUpdate.UserName
                }
            };
    }
}