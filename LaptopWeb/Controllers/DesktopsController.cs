using LaptopWeb.Models;
using LaptopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace LaptopWeb.Controllers
{
    public class DesktopsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public DesktopsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        // List all desktops
        public IActionResult Index()
        {
            var desktops = context.Desktops.OrderByDescending(d => d.Id).ToList();
            return View(desktops);
        }

        // Display form to create a new desktop
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission to create a new desktop
        [HttpPost]
        public IActionResult Create(DesktopDto desktopDto)
        {
            if (desktopDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(desktopDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(desktopDto.ImageFile!.FileName);
            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                desktopDto.ImageFile.CopyTo(stream);
            }

            var desktop = new Desktop
            {
                Name = desktopDto.Name,
                Brand = desktopDto.Brand,
                Category = desktopDto.Category,
                Price = desktopDto.Price,
                Description = desktopDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            context.Desktops.Add(desktop);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Display details of a desktop
        public IActionResult Details(int id)
        {
            var desktop = context.Desktops.Find(id);
            if (desktop == null)
            {
                return RedirectToAction("Index");
            }

            return View(desktop);
        }

        // Display form to edit an existing desktop
        public IActionResult Edit(int id)
        {
            var desktop = context.Desktops.Find(id);
            if (desktop == null)
            {
                return RedirectToAction("Index");
            }

            var desktopDto = new DesktopDto
            {
                Id = desktop.Id,
                Name = desktop.Name,
                Brand = desktop.Brand,
                Category = desktop.Category,
                Price = desktop.Price,
                Description = desktop.Description
            };

            ViewData["ImageFileName"] = desktop.ImageFileName;
            return View(desktopDto);
        }

        // Handle form submission to edit an existing desktop
        [HttpPost]
        public IActionResult Edit(int id, DesktopDto desktopDto)
        {
            if (id != desktopDto.Id)
            {
                return BadRequest();
            }

            var desktop = context.Desktops.Find(id);
            if (desktop == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ImageFileName"] = desktop.ImageFileName;
                return View(desktopDto);
            }

            string newFileName = desktop.ImageFileName;
            if (desktopDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(desktopDto.ImageFile.FileName);
                string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    desktopDto.ImageFile.CopyTo(stream);
                }

                string oldImageFullPath = Path.Combine(environment.WebRootPath, "Images", desktop.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            desktop.Name = desktopDto.Name;
            desktop.Brand = desktopDto.Brand;
            desktop.Category = desktopDto.Category;
            desktop.Price = desktopDto.Price;
            desktop.Description = desktopDto.Description;
            desktop.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Confirm and delete a desktop
        public IActionResult Delete(int id)
        {
            var desktop = context.Desktops.Find(id);
            if (desktop == null)
            {
                return RedirectToAction("Index");
            }

            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", desktop.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            context.Desktops.Remove(desktop);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
