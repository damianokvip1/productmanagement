using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs;

public class UserDto
{
    public class UserData
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class UserCreate
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
    
    public class UserUpdate
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}