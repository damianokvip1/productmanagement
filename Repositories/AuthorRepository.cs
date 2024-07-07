using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(Author author);
        Task<bool> DeleteAuthorAsync(int id);
    }

    public class AuthorRepository(ApplicationDbContext context) : IAuthorRepository
    {
        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            return await context.Authors.ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await context.Authors.FindAsync(id);
        }

        public async Task<Author> CreateAuthorAsync(Author author)
        {
            context.Authors.Add(author);
            await context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            context.Entry(author).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(author.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await context.Authors.FindAsync(id);
            if (author == null)
            {
                return false;
            }

            context.Authors.Remove(author);
            await context.SaveChangesAsync();
            return true;
        }

        private bool AuthorExists(int id)
        {
            return context.Authors.Any(e => e.Id == id);
        }
    }
}