using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using EduTransportRD.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly DataContext _context;

        public VehiclesController(DataContext context)
        {
            _context = context;
        }

        private void LoadDropdowns(int? selectedDriverId = null)
        {
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Disponible",       Text = "Disponible"       },
                new { Value = "En Ruta",          Text = "En Ruta"          },
                new { Value = "En Mantenimiento", Text = "En Mantenimiento" },
                new { Value = "Fuera de Servicio",Text = "Fuera de Servicio"}
            }, "Value", "Text");

            var driversQuery = _context.Drivers.AsQueryable();

            if (selectedDriverId.HasValue)
            {
                driversQuery = driversQuery.Where(d =>
                    d.Status == "Disponible" || d.Id == selectedDriverId.Value);
            }
            else
            {
                driversQuery = driversQuery.Where(d => d.Status == "Disponible");
            }

            var driversList = driversQuery
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .Select(d => new
                {
                    d.Id,
                    FullName = d.FirstName + " " + d.LastName
                })
                .ToList();

            ViewBag.DriverList = new SelectList(driversList, "Id", "FullName", selectedDriverId);
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();

                query = query.Where(v =>
                    v.Plate.Contains(search) ||
                    v.Brand.Contains(search) ||
                    v.Model.Contains(search) ||
                    v.Status.Contains(search));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(v => v.Plate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new Paginator<Vehicle>(items, totalItems, page, pageSize);

            var driverNames = await _context.Drivers
                .Select(d => new
                {
                    d.Id,
                    FullName = d.FirstName + " " + d.LastName
                })
                .ToDictionaryAsync(d => d.Id, d => d.FullName);

            ViewBag.DriverNames = driverNames;
            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return NotFound();

            if (vehicle.DriverId.HasValue)
            {
                var driver = await _context.Drivers
                    .FirstOrDefaultAsync(d => d.Id == vehicle.DriverId.Value);

                if (driver != null)
                    ViewBag.DriverName = driver.FirstName + " " + driver.LastName;
            }

            return View(vehicle);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(vehicle.DriverId);
                return View(vehicle);
            }

            _context.Add(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
                return NotFound();

            LoadDropdowns(vehicle.DriverId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns(vehicle.DriverId);
                return View(vehicle);
            }

            try
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Vehicles.AnyAsync(v => v.Id == id);
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

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
