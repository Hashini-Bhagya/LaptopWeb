using System.ComponentModel.DataAnnotations;

namespace LaptopWeb.Models
{
    public class AccessoriesDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a value greater than 0")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }

    }
}
