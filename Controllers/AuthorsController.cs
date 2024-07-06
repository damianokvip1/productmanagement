using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            var authors = await _context.Authors
                .Select(a => new AuthorDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Biography = a.Biography,
                    DateOfBirth = a.DateOfBirth
                })
                .ToListAsync();

            return authors;
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Select(a => new AuthorDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Biography = a.Biography,
                    DateOfBirth = a.DateOfBirth
                })
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<AuthorDTO>> PostAuthor(AuthorCreateDTO authorCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = new Author
            {
                Name = authorCreateDto.Name,
                Biography = authorCreateDto.Biography,
                DateOfBirth = authorCreateDto.DateOfBirth
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            var authorReturn = new AuthorDTO
            {
                Id = author.Id,
                Name = authorCreateDto.Name,
                Biography = authorCreateDto.Biography,
                DateOfBirth = authorCreateDto.DateOfBirth
            };

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id },  authorReturn);
        }

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorDTO authorDTO)
        {
            if (id != authorDTO.Id)
            {
                return BadRequest(new { message = "Id mismatch" });
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new { message = "Author not found" });
            }

            author.Name = authorDTO.Name;
            author.Biography = authorDTO.Biography;
            author.DateOfBirth = authorDTO.DateOfBirth;

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Sửa thành công",
                    author = authorDTO
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound(new { message = "Author not found" });
                }
                throw;
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
