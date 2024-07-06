using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; }
        [StringLength(2000, ErrorMessage = "Biography can't be longer than 2000 characters.")]
        public string Biography { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
