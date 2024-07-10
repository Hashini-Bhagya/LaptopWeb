using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LaptopWeb.Models
{
    public class Desktop
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = " ";
        [MaxLength(100)]
        public string Brand { get; set; } = " ";
        [MaxLength(100)]
        public string Category { get; set; } = " ";
        [Precision(16, 2)]
        public double Price { get; set; }

        public string Description { get; set; } = " ";
        [MaxLength(100)]
        public string ImageFileName { get; set; } = " ";
        public DateTime CreatedAt { get; set; }
    }
}
