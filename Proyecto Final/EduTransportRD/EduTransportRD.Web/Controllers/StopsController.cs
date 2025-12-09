using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using EduTransportRD.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class StopsController : Controller
    {
        private readonly DataContext _context;

        public StopsController(DataContext context)
        {
            _context = context;
        }

        private void LoadRoutes()
        {
            var routes = _context.Routes
                .Select(r => new { r.Id, r.RouteName })
                .ToList();

            ViewBag.RoutesList = new SelectList(routes, "Id", "RouteName");
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.Stops
                .Include(s => s.Route)          
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s =>
                    s.StopName.Contains(search) ||
                    s.Route.RouteName.Contains(search)); 

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.StopOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new Paginator<Stop>(items, total, page, pageSize);

            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;

            return View(model);
        }

        public IActionResult Create()
        {
            LoadRoutes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Stop stop)
        {
            if (!ModelState.IsValid)
            {
                LoadRoutes();
                return View(stop);
            }

            _context.Add(stop);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var stop = await _context.Stops.FindAsync(id);

            if (stop == null)
                return NotFound();

            LoadRoutes();
            return View(stop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Stop stop)
        {
            if (id != stop.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadRoutes();
                return View(stop);
            }

            _context.Update(stop);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var stop = await _context.Stops
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stop == null)
                return NotFound();

            return View(stop);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var stop = await _context.Stops
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stop == null)
                return NotFound();

            return View(stop);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stop = await _context.Stops.FindAsync(id);

            if (stop != null)
            {
                _context.Stops.Remove(stop);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
