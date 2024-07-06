using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorDTO?> GetAuthorByIdAsync(int id);
        Task<AuthorDTO> CreateAuthorAsync(AuthorCreateDTO authorCreateDto);
        Task<bool> UpdateAuthorAsync(int id, AuthorUpdateDTO authorUpdateDto);
        Task<bool> DeleteAuthorAsync(int id);
    }

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAuthorsAsync();
            return authors.Select(a => new AuthorDTO
            {
                Id = a.Id,
                Name = a.Name,
                Biography = a.Biography,
                DateOfBirth = a.DateOfBirth
            });
        }

        public async Task<AuthorDTO?> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return null;
            }

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                DateOfBirth = author.DateOfBirth
            };
        }

        public async Task<AuthorDTO> CreateAuthorAsync(AuthorCreateDTO authorCreateDto)
        {
            var author = new Author
            {
                Name = authorCreateDto.Name,
                Biography = authorCreateDto.Biography,
                DateOfBirth = authorCreateDto.DateOfBirth
            };

            await _authorRepository.CreateAuthorAsync(author);

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                DateOfBirth = author.DateOfBirth
            };
        }

        public async Task<bool> UpdateAuthorAsync(int id, AuthorUpdateDTO authorUpdateDto)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return false;
            }

            author.Name = authorUpdateDto.Name;
            author.Biography = authorUpdateDto.Biography;
            author.DateOfBirth = authorUpdateDto.DateOfBirth;

            return await _authorRepository.UpdateAuthorAsync(author);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            return await _authorRepository.DeleteAuthorAsync(id);
        }
    }
}