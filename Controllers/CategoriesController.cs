using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> PostCategory(CategoryCreateDTO categoryCreateDto)
        {
            var category = await categoryService.CreateCategoryAsync(categoryCreateDto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCategory(int id, CategoryUpdateDTO categoryUpdateDto)
        {
            if (!await categoryService.UpdateCategoryAsync(id, categoryUpdateDto))
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!await categoryService.DeleteCategoryAsync(id))
                return NotFound();

            return NoContent();
        }
    }
}
