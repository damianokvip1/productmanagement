using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync(int? categoryId, int? authorId, string? searchTerm, int pageNumber, int pageSize);
        Task<ProductDTO?> GetProductDetailAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDto);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDTO>> GetCheapestProductsAsync();
    }

    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public Task<IEnumerable<ProductDTO>> GetProductsAsync(int? categoryId, int? authorId, string? searchTerm, int pageNumber, int pageSize)
        {
            return productRepository.GetProductsAsync(categoryId, authorId, searchTerm, pageNumber, pageSize);
        }

        public async Task<ProductDTO?> GetProductDetailAsync(int id)
        {
            var products = await productRepository.GetProductsFullDataAsync();
            var product = products.FirstOrDefault(p => p.Id == id);
        
            return product ?? null;
        }
        
        public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDto)
        {
            ArgumentNullException.ThrowIfNull(productCreateDto);

            var product = new Product()
            {
                Name = productCreateDto.Name,
                Price = productCreateDto.Price,
                Description = productCreateDto.Description,
                CategoryId = productCreateDto.CategoryId,
                AuthorId = productCreateDto.AuthorId
            };  

            await productRepository.CreateProductAsync(product);

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = new CategoryDTO
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Author = new AuthorDTO
                {
                    Id = product.Author.Id,
                    Name = product.Author.Name,
                    Biography = product.Author.Biography,
                    DateOfBirth = product.Author.DateOfBirth
                }
            };
        }
        
        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDto)
        {
            var product = await productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            product.Id = productUpdateDto.Id;
            product.Name = productUpdateDto.Name;
            product.Price = productUpdateDto.Price;
            product.Description = productUpdateDto.Description;
            product.CategoryId = productUpdateDto.CategoryId;
            product.AuthorId = productUpdateDto.AuthorId;

            return await productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductDTO>> GetCheapestProductsAsync()
        {
            var cheapestProducts = await productRepository.GetProductsCheapestAsync();
            return cheapestProducts
                .Select(product => new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    Category = new CategoryDTO
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    },
                    Author = new AuthorDTO
                    {
                        Id = product.Author.Id,
                        Name = product.Author.Name,
                        Biography = product.Author.Biography,
                        DateOfBirth = product.Author.DateOfBirth
                    }
                });
        }
    }
}