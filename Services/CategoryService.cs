using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto.CategoryData>> GetAllCategoriesAsync();
        Task<CategoryDto.CategoryData?> GetCategoryByIdAsync(int id);
        Task<CategoryDto.CategoryData> CreateCategoryAsync(CategoryDto.CategoryCreate categoryCreateDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryDto.CategoryUpdate categoryUpdateDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
    
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDto.CategoryData>> GetAllCategoriesAsync()
        {
            var categories = await categoryRepository.GetCategoriesAsync();
            return categories.Select(c => new CategoryDto.CategoryData
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<CategoryDto.CategoryData?> GetCategoryByIdAsync(int id)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            return new CategoryDto.CategoryData
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto.CategoryData> CreateCategoryAsync(CategoryDto.CategoryCreate categoryCreateDto)
        {
            var category = new Category
            {
                Name = categoryCreateDto.Name
            };

            await categoryRepository.CreateCategoryAsync(category);

            return new CategoryDto.CategoryData
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryDto.CategoryUpdate categoryUpdateDto)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            category.Name = categoryUpdateDto.Name;

            return await categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await categoryRepository.DeleteCategoryAsync(id);
        }
    }
}
