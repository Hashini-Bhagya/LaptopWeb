using LaptopWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using LaptopWeb.Services;

namespace LaptopWeb.Controllers
{
    public class PrintersScannersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PrintersScannersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // List all printers and scanners
        public IActionResult Index()
        {
            var printersScanners = _context.PrintersScanners.OrderByDescending(p => p.Id).ToList();
            return View(printersScanners);
        }

        // Display form to create a new printer or scanner
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission to create a new printer or scanner
        [HttpPost]
        public IActionResult Create(PrintersScanners printersScanners)
        {
            if (!ModelState.IsValid)
            {
                return View(printersScanners);
            }

            _context.Add(printersScanners);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Display details of a printer or scanner
        public IActionResult Details(int id)
        {
            var printersScanners = _context.PrintersScanners.FirstOrDefault(p => p.Id == id);
            if (printersScanners == null)
            {
                return NotFound();
            }

            return View(printersScanners);
        }

        // Display form to edit an existing printer or scanner
        public IActionResult Edit(int id)
        {
            var printersScanners = _context.PrintersScanners.Find(id);
            if (printersScanners == null)
            {
                return NotFound();
            }

            return View(printersScanners);
        }

        // Handle form submission to edit an existing printer or scanner
        [HttpPost]
        public IActionResult Edit(int id, PrintersScanners printersScanners)
        {
            if (id != printersScanners.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(printersScanners);
            }

            try
            {
                _context.Update(printersScanners);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrintersScannersExists(printersScanners.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // Confirm and delete a printer or scanner
        public IActionResult Delete(int id)
        {
            var printersScanners = _context.PrintersScanners.Find(id);
            if (printersScanners == null)
            {
                return NotFound();
            }

            _context.PrintersScanners.Remove(printersScanners);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool PrintersScannersExists(int id)
        {
            return _context.PrintersScanners.Any(e => e.Id == id);
        }
    }
}
