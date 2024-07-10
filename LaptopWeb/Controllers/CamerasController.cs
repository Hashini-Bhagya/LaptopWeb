using LaptopWeb.Models;
using LaptopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaptopWeb.Controllers
{
    public class CamerasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public CamerasController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        // List all cameras
        public IActionResult Index()
        {
            var cameras = context.Cameras.OrderByDescending(c => c.Id).ToList();
            return View(cameras);
        }

        // Display form to create a new camera
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission to create a new camera
        [HttpPost]
        public IActionResult Create(CamerasDto camerasDto)
        {
            if (camerasDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(camerasDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(camerasDto.ImageFile!.FileName);
            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                camerasDto.ImageFile.CopyTo(stream);
            }

            var camera = new Cameras
            {
                Name = camerasDto.Name,
                Brand = camerasDto.Brand,
                Category = camerasDto.Category,
                Price = camerasDto.Price,
                Description = camerasDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Cameras.Add(camera);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Display details of a camera
        public IActionResult Details(int id)
        {
            var camera = context.Cameras.Find(id);
            if (camera == null)
            {
                return RedirectToAction("Index");
            }

            return View(camera);
        }

        // Display form to edit an existing camera
        public IActionResult Edit(int id)
        {
            var camera = context.Cameras.Find(id);
            if (camera == null)
            {
                return RedirectToAction("Index");
            }

            var camerasDto = new CamerasDto
            {
                Id = camera.Id,
                Name = camera.Name,
                Brand = camera.Brand,
                Category = camera.Category,
                Price = camera.Price,
                Description = camera.Description
            };

            ViewData["ImageFileName"] = camera.ImageFileName;
            return View(camerasDto);
        }

        // Handle form submission to edit an existing camera
        [HttpPost]
        public IActionResult Edit(int id, CamerasDto camerasDto)
        {
            if (id != camerasDto.Id)
            {
                return BadRequest();
            }

            var camera = context.Cameras.Find(id);
            if (camera == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ImageFileName"] = camera.ImageFileName;
                return View(camerasDto);
            }

            string newFileName = camera.ImageFileName;
            if (camerasDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(camerasDto.ImageFile.FileName);
                string imageFullPath = Path.Combine(environment.WebRootPath, "Images", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    camerasDto.ImageFile.CopyTo(stream);
                }

                string oldImageFullPath = Path.Combine(environment.WebRootPath, "Images", camera.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            camera.Name = camerasDto.Name;
            camera.Brand = camerasDto.Brand;
            camera.Category = camerasDto.Category;
            camera.Price = camerasDto.Price;
            camera.Description = camerasDto.Description;
            camera.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Confirm and delete a camera
        public IActionResult Delete(int id)
        {
            var camera = context.Cameras.Find(id);
            if (camera == null)
            {
                return RedirectToAction("Index");
            }

            string imageFullPath = Path.Combine(environment.WebRootPath, "Images", camera.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            context.Cameras.Remove(camera);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
