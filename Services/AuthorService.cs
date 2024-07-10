using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto.AuthorData>> GetAllAuthorsAsync();
        Task<AuthorDto.AuthorData?> GetAuthorByIdAsync(int id);
        Task<AuthorDto.AuthorData> CreateAuthorAsync(AuthorDto.AuthorCreateDTO authorCreateDto);
        Task<bool> UpdateAuthorAsync(int id, AuthorDto.AuthorUpdateDTO authorUpdateDto);
        Task<bool> DeleteAuthorAsync(int id);
    }

    public class AuthorService(IAuthorRepository authorRepository) : IAuthorService
    {
        public async Task<IEnumerable<AuthorDto.AuthorData>> GetAllAuthorsAsync()
        {
            var authors = await authorRepository.GetAuthorsAsync();
            return authors.Select(a => new AuthorDto.AuthorData
            {
                Id = a.Id,
                Name = a.Name,
                Biography = a.Biography,
                DateOfBirth = a.DateOfBirth
            });
        }

        public async Task<AuthorDto.AuthorData?> GetAuthorByIdAsync(int id)
        {
            var author = await authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return null;
            }

            return new AuthorDto.AuthorData
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                DateOfBirth = author.DateOfBirth
            };
        }

        public async Task<AuthorDto.AuthorData> CreateAuthorAsync(AuthorDto.AuthorCreateDTO authorCreateDto)
        {
            var author = new Author
            {
                Name = authorCreateDto.Name,
                Biography = authorCreateDto.Biography,
                DateOfBirth = authorCreateDto.DateOfBirth
            };

            await authorRepository.CreateAuthorAsync(author);

            return new AuthorDto.AuthorData
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                DateOfBirth = author.DateOfBirth
            };
        }

        public async Task<bool> UpdateAuthorAsync(int id, AuthorDto.AuthorUpdateDTO authorUpdateDto)
        {
            var author = await authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return false;
            }

            author.Name = authorUpdateDto.Name;
            author.Biography = authorUpdateDto.Biography;
            author.DateOfBirth = authorUpdateDto.DateOfBirth;

            return await authorRepository.UpdateAuthorAsync(author);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            return await authorRepository.DeleteAuthorAsync(id);
        }
    }
}