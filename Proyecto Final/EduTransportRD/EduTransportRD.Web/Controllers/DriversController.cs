using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using EduTransportRD.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class DriversController : Controller
    {
        private readonly DataContext _context;

        public DriversController(DataContext context)
        {
            _context = context;
        }

        private void LoadStatusList()
        {
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Disponible",     Text = "Disponible"     },
                new { Value = "No Disponible",  Text = "No Disponible"  },
                new { Value = "Suspendido",     Text = "Suspendido"     }
            }, "Value", "Text");
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.Drivers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();

                query = query.Where(d =>
                    d.FirstName.Contains(search) ||
                    d.LastName.Contains(search) ||
                    d.DocumentId.Contains(search) ||
                    d.LicenseNumber.Contains(search));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new Paginator<Driver>(items, totalItems, page, pageSize);

            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        public IActionResult Create()
        {
            LoadStatusList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Driver driver)
        {
            if (!ModelState.IsValid)
            {
                LoadStatusList();
                return View(driver);
            }

            _context.Add(driver);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound();

            LoadStatusList();
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Driver driver)
        {
            if (id != driver.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadStatusList();
                return View(driver);
            }

            try
            {
                _context.Update(driver);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Drivers.AnyAsync(d => d.Id == id);
                if (!exists)
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
