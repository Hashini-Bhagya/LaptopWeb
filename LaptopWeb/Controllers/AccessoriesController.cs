using LaptopWeb.Models;
using LaptopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaptopWeb.Controllers
{
    public class AccessoriesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public AccessoriesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var accessories = context.Accessories.OrderByDescending(a => a.Id).ToList();
            return View(accessories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AccessoriesDto accessoriesDto)
        {
            if (accessoriesDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(accessoriesDto);
            }

            // Save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(accessoriesDto.ImageFile!.FileName);
            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                accessoriesDto.ImageFile.CopyTo(stream);
            }

            // Save the accessories data
            Accessories accessories = new Accessories()
            {
                Name = accessoriesDto.Name,
                Brand = accessoriesDto.Brand,
                Category = accessoriesDto.Category,
                Price = accessoriesDto.Price,
                Description = accessoriesDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Accessories.Add(accessories);
            context.SaveChanges();

            return RedirectToAction("Index", "Accessories");
        }

        public IActionResult Edit(int id)
        {
            var accessories = context.Accessories.Find(id);
            if (accessories == null)
            {
                return RedirectToAction("Index", "Accessories");
            }

            var accessoriesDto = new AccessoriesDto()
            {
                Name = accessories.Name,
                Brand = accessories.Brand,
                Category = accessories.Category,
                Price = accessories.Price,
                Description = accessories.Description,
            };

            ViewData["AccessoriesId"] = accessories.Id;
            ViewData["ImageFileName"] = accessories.ImageFileName;
            ViewData["CreatedAt"] = accessories.CreatedAt.ToString("MM/dd/yyyy");

            return View(accessoriesDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, AccessoriesDto accessoriesDto)
        {
            var accessories = context.Accessories.Find(id);
            if (accessories == null)
            {
                return RedirectToAction("Index", "Accessories");
            }

            if (!ModelState.IsValid)
            {
                ViewData["AccessoriesId"] = accessories.Id;
                ViewData["ImageFileName"] = accessories.ImageFileName;
                ViewData["CreatedAt"] = accessories.CreatedAt.ToString("MM/dd/yyyy");
                return View(accessoriesDto);
            }

            string newFileName = accessories.ImageFileName;
            if (accessoriesDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(accessoriesDto.ImageFile.FileName);
                string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    accessoriesDto.ImageFile.CopyTo(stream);
                }

                // Delete old image
                string oldImageFullPath = Path.Combine(environment.WebRootPath, "Images", accessories.ImageFileName);
                System.IO.File.Delete(oldImageFullPath);
            }

            // Update accessories data
            accessories.Name = accessoriesDto.Name;
            accessories.Brand = accessoriesDto.Brand;
            accessories.Category = accessoriesDto.Category;
            accessories.Price = accessoriesDto.Price;
            accessories.Description = accessoriesDto.Description;
            accessories.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Accessories");
        }

        public IActionResult Delete(int id)
        {
            var accessories = context.Accessories.Find(id);
            if (accessories == null)
            {
                return RedirectToAction("Index", "Accessories");
            }

            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", accessories.ImageFileName);
            System.IO.File.Delete(imageFullPath);

            context.Accessories.Remove(accessories);
            context.SaveChanges();

            return RedirectToAction("Index", "Accessories");
        }

        public IActionResult Details(int id)
        {
            var accessories = context.Accessories.Find(id);
            if (accessories == null)
            {
                return RedirectToAction("Index", "Accessories");
            }

            return View(accessories);
        }
    }
}
