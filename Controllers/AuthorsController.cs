using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController(IAuthorService authorService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto.AuthorData>>> GetAuthors()
        {
            var authors = await authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDto.AuthorData>> GetAuthor(int id)
        {
            var author = await authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto.AuthorData>> PostAuthor(AuthorDto.AuthorCreateDTO authorCreateDto)
        {
            var author = await authorService.CreateAuthorAsync(authorCreateDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorDto.AuthorUpdateDTO authorUpdateDto)
        {
            if (!await authorService.UpdateAuthorAsync(id, authorUpdateDto))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (!await authorService.DeleteAuthorAsync(id))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
