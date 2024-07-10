using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto.ProductData>> GetProductsAsync(int? categoryId, int? authorId, string? searchTerm, int pageNumber, int pageSize);
        Task<ProductDto.ProductData?> GetProductDetailAsync(int id);
        Task<ProductDto.ProductData> CreateProductAsync(ProductDto.ProductCreate productCreateDto, int userId);
        Task<bool> UpdateProductAsync(int id, ProductDto.ProductUpdate productUpdateDto, int userUpdateId);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto.ProductData>> GetCheapestProductsAsync();
    }

    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public Task<IEnumerable<ProductDto.ProductData>> GetProductsAsync(int? categoryId, int? authorId, string? searchTerm, int pageNumber, int pageSize)
        {
            return productRepository.GetProductsAsync(categoryId, authorId, searchTerm, pageNumber, pageSize);
        }

        public async Task<ProductDto.ProductData?> GetProductDetailAsync(int id)
        {
            var products = await productRepository.GetProductsFullDataAsync();
            var product = products.FirstOrDefault(p => p.Id == id);
        
            return product ?? null;
        }
        
        public async Task<ProductDto.ProductData> CreateProductAsync(ProductDto.ProductCreate productCreateDto, int userId)
        {
            ArgumentNullException.ThrowIfNull(productCreateDto);
            var product = new Product()
            {
                Name = productCreateDto.Name,
                Price = productCreateDto.Price,
                Description = productCreateDto.Description,
                CategoryId = productCreateDto.CategoryId,
                AuthorId = productCreateDto.AuthorId,
                UserCreateId = userId,
                UserUpdateId = null
            };  

            await productRepository.CreateProductAsync(product);

            return new ProductDto.ProductData
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = new CategoryDto.CategoryData
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Author = new AuthorDto.AuthorData
                {
                    Id = product.Author.Id,
                    Name = product.Author.Name,
                    Biography = product.Author.Biography,
                    DateOfBirth = product.Author.DateOfBirth
                },
            };
        }
        
        public async Task<bool> UpdateProductAsync(int id, ProductDto.ProductUpdate productUpdateDto, int userUpdateId)
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
            product.UserUpdateId = userUpdateId;

            return await productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductDto.ProductData>> GetCheapestProductsAsync()
        {
            var cheapestProducts = await productRepository.GetProductsCheapestAsync();
            return cheapestProducts
                .Select(product => new ProductDto.ProductData
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    Category = new CategoryDto.CategoryData
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    },
                    Author = new AuthorDto.AuthorData
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