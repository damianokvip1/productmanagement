using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO categoryCreateDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryUpdateDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
    
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await categoryRepository.GetCategoriesAsync();
            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO categoryCreateDto)
        {
            var category = new Category
            {
                Name = categoryCreateDto.Name
            };

            await categoryRepository.CreateCategoryAsync(category);

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryUpdateDto)
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
