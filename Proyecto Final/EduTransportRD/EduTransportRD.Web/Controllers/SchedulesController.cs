using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly DataContext _context;

        public SchedulesController(DataContext context)
        {
            _context = context;
        }

        private void LoadDropdowns()
        {
            ViewBag.Days = new List<string>
            {
                "Lunes","Martes","Miércoles","Jueves","Viernes","Sábado",
            };

            ViewBag.Routes = new SelectList(
                _context.Routes
                    .OrderBy(r => r.RouteName)
                    .Select(r => new { r.Id, r.RouteName })
                    .ToList(),
                "Id",
                "RouteName");
        }

        public IActionResult Index(string search)
        {
            var query = _context.Schedules
                .Include(x => x.Route)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();

                query = query.Where(x =>
                    x.WeekDay.ToLower().Contains(s) ||
                    x.Route.RouteName.ToLower().Contains(s));
            }

            var schedules = query
                .OrderBy(s => s.WeekDay)
                .ThenBy(s => s.DepartureTime)
                .ToList();

            ViewBag.Search = search;

            return View(schedules);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new Schedule());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(schedule);
            }

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
                return NotFound();

            LoadDropdowns();
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Schedule schedule)
        {
            if (id != schedule.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(schedule);
            }

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return NotFound();

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
                return NotFound();

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
