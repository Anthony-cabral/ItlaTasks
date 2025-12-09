using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using EduTransportRD.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using DomainRoute = EduTransportRD.Domain.Entities.Route;

namespace EduTransportRD.Web.Controllers
{
    public class RoutesController : Controller
    {
        private readonly DataContext _context;

        public RoutesController(DataContext context)
        {
            _context = context;
        }

        private void LoadDropdowns(int? vehicleId = null)
        {
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Disponible", Text = "Disponible" },
                new { Value = "En Ruta",   Text = "En Ruta"   },
                new { Value = "Inactiva",  Text = "Inactiva"  }
            }, "Value", "Text");

            var query = _context.Vehicles.AsQueryable();

            if (vehicleId.HasValue)
                query = query.Where(v => v.Status == "Disponible" || v.Id == vehicleId.Value);
            else
                query = query.Where(v => v.Status == "Disponible");

            var list = query
                .OrderBy(v => v.Plate)
                .Select(v => new
                {
                    v.Id,
                    Name = v.Plate + " - " + v.Brand + " " + v.Model
                })
                .ToList();

            ViewBag.VehicleList = new SelectList(list, "Id", "Name", vehicleId);
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.Routes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r => r.RouteName.Contains(search));

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.RouteName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new Paginator<DomainRoute>(items, totalItems, page, pageSize);

            var vehicles = await _context.Vehicles
                .ToDictionaryAsync(v => v.Id, v => v.Plate);

            ViewBag.Vehicles = vehicles;
            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound();

            if (route.VehicleId.HasValue)
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == route.VehicleId.Value);

                if (vehicle != null)
                    ViewBag.Vehicle = vehicle.Plate + " - " + vehicle.Brand + " " + vehicle.Model;
            }

            return View(route);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DomainRoute route)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(route.VehicleId);
                return View(route);
            }

            _context.Add(route);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var route = await _context.Routes.FindAsync(id);

            if (route == null)
                return NotFound();

            LoadDropdowns(route.VehicleId);

            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DomainRoute route)
        {
            if (id != route.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns(route.VehicleId);
                return View(route);
            }

            _context.Update(route);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound();

            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var route = await _context.Routes.FindAsync(id);

            if (route != null)
            {
                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
