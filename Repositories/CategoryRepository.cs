using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category author);
    Task<bool> UpdateCategoryAsync(Category author);
    Task<bool> DeleteCategoryAsync(int id);
}

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await context.Categories.ToListAsync();
    }
    
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await context.Categories.FindAsync(id);
    }
    
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }
    
    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        context.Entry(category).State = EntityState.Modified;
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategotyExists(category.Id))
                return false;

            throw;
        }
    }
    
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }
    
    private bool CategotyExists(int id) => context.Authors.Any(e => e.Id == id);
}

