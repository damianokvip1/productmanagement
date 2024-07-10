using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs;

public class ProductDto
{
    public class ProductCreate
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; }
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00.")]
        public double Price { get; set; }
        [StringLength(2000, ErrorMessage = "Description can't be longer than 2000 characters.")]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int AuthorId { get; set; }
    }

    public class ProductUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
    }

    public class ProductData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public UserDto.UserData UserCreate { get; set; }
        public UserDto.UserData UserUpdate { get; set; }
        public CategoryDto.CategoryData Category { get; set; }
        public AuthorDto.AuthorData Author { get; set; }
    }
}