using LaptopWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LaptopWeb.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Product> Products { get; set; }    
        public DbSet<Accessories> Accessories { get; set; }
        public DbSet<Cameras> Cameras { get; set; }
        public DbSet<Desktop>   Desktops { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Other> Other { get; set; }
        public DbSet<PrintersScanners> PrintersScanners { get; set; }


    }

    
    
}
