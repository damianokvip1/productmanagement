using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs
{
    public class CustomResponseDto
    {
        public int StatusCode { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }

    public class ProductCreateDTO
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

    public class ProductUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
    }

    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public CategoryDTO Category { get; set; }
        public AuthorDTO Author { get; set; }
    }

    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class CategoryCreateDTO
    {
        public string Name { get; set; }
    }
    
    public class CategoryUpdateDTO
    {
        public string Name { get; set; }
    }

    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    
    public class AuthorCreateDTO
    {
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    
    public class AuthorUpdateDTO
    {
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
    
    public class UserUpdateDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
