using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductDetailAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDto);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDTO>> GetCheapestProductsAsync();
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetProductsFullDataAsync();
            return products;
        }

        public async Task<ProductDTO?> GetProductDetailAsync(int id)
        {
            var products = await GetAllProductsAsync();
            var product = products.FirstOrDefault(p => p.Id == id);
        
            if (product != null)
            {
                return product;
            }
            else
            {
                return null;
            }
        }
        
        public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDto)
        {
            if (productCreateDto == null)
            {
                throw new ArgumentNullException(nameof(productCreateDto));
            }
            
            var product = new Product()
            {
                Name = productCreateDto.Name,
                Price = productCreateDto.Price,
                Description = productCreateDto.Description,
                CategoryId = productCreateDto.CategoryId,
                AuthorId = productCreateDto.AuthorId
            };  

            await _productRepository.CreateProductAsync(product);

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
            var product = await _productRepository.GetProductByIdAsync(id);
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

            return await _productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductDTO>> GetCheapestProductsAsync()
        {
            var products = await _productRepository.GetProductsFullDataAsync();
            var cheapestProducts = products
                .OrderBy(product => product.Price)
                .Take(2)
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

            return cheapestProducts;
        }
    }
}