using LaptopWeb.Models;
using LaptopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace LaptopWeb.Controllers
{
    public class LaptopsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public LaptopsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        // List all laptops
        public IActionResult Index()
        {
            var laptops = context.Laptops.OrderByDescending(l => l.Id).ToList();
            return View(laptops);
        }

        // Display form to create a new laptop
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission to create a new laptop
        [HttpPost]
        public IActionResult Create(LaptopDto laptopDto)
        {
            if (laptopDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(laptopDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(laptopDto.ImageFile!.FileName);
            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                laptopDto.ImageFile.CopyTo(stream);
            }

            var laptop = new Laptop
            {
                Name = laptopDto.Name,
                Brand = laptopDto.Brand,
                Category = laptopDto.Category,
                Price = laptopDto.Price,
                Description = laptopDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            context.Laptops.Add(laptop);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Display details of a laptop
        public IActionResult Details(int id)
        {
            var laptop = context.Laptops.Find(id);
            if (laptop == null)
            {
                return RedirectToAction("Index");
            }

            return View(laptop);
        }

        // Display form to edit an existing laptop
        public IActionResult Edit(int id)
        {
            var laptop = context.Laptops.Find(id);
            if (laptop == null)
            {
                return RedirectToAction("Index");
            }

            var laptopDto = new LaptopDto
            {
                Id = laptop.Id,
                Name = laptop.Name,
                Brand = laptop.Brand,
                Category = laptop.Category,
                Price = laptop.Price,
                Description = laptop.Description
            };

            ViewData["ImageFileName"] = laptop.ImageFileName;
            return View(laptopDto);
        }

        // Handle form submission to edit an existing laptop
        [HttpPost]
        public IActionResult Edit(int id, LaptopDto laptopDto)
        {
            if (id != laptopDto.Id)
            {
                return BadRequest();
            }

            var laptop = context.Laptops.Find(id);
            if (laptop == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ImageFileName"] = laptop.ImageFileName;
                return View(laptopDto);
            }

            string newFileName = laptop.ImageFileName;
            if (laptopDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(laptopDto.ImageFile.FileName);
                string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    laptopDto.ImageFile.CopyTo(stream);
                }

                string oldImageFullPath = Path.Combine(environment.WebRootPath, "Images", laptop.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            laptop.Name = laptopDto.Name;
            laptop.Brand = laptopDto.Brand;
            laptop.Category = laptopDto.Category;
            laptop.Price = laptopDto.Price;
            laptop.Description = laptopDto.Description;
            laptop.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Confirm and delete a laptop
        public IActionResult Delete(int id)
        {
            var laptop = context.Laptops.Find(id);
            if (laptop == null)
            {
                return RedirectToAction("Index");
            }

            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", laptop.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            context.Laptops.Remove(laptop);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
