﻿using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; }
        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00.")]
        public double Price { get; set; }
        [StringLength(2000, ErrorMessage = "Description can't be longer than 2000 characters.")]
        public string Description { get; set; }
        public int? UserCreateId { get; set; }
        public User UserCreate { get; set; }
        public int? UserUpdateId { get; set; }
        public User UserUpdate { get; set; }
        [Required]
        public int CategoryId { get; set; } // Foreign key (optional)
        public Category Category { get; set; } // Navigation property
        [Required]
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
