using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly DataContext _context;

        public AttendanceController(DataContext context)
        {
            _context = context;
        }

        private void LoadDropdowns()
        {
            ViewBag.Students = new SelectList(
                _context.Students
                    .OrderBy(s => s.FirstName)
                    .ThenBy(s => s.LastName)
                    .Select(s => new
                    {
                        s.Id,
                        Name = s.FirstName + " " + s.LastName
                    })
                    .ToList(),
                "Id",
                "Name");

            ViewBag.Routes = new SelectList(
                _context.Routes
                    .OrderBy(r => r.RouteName)
                    .Select(r => new { r.Id, r.RouteName })
                    .ToList(),
                "Id",
                "RouteName");
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Route)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();

                query = query.Where(a =>
                    a.Student.FirstName.ToLower().Contains(s) ||
                    a.Student.LastName.ToLower().Contains(s) ||
                    a.Route.RouteName.ToLower().Contains(s));
            }

            var list = await query
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            ViewBag.Search = search;

            return View(list);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            var model = new Attendance
            {
                Date = DateTime.Now,
                IsPresent = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(attendance);
            }

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
                return NotFound();

            LoadDropdowns();
            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance attendance)
        {
            if (id != attendance.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(attendance);
            }

            _context.Update(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Route)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
                return NotFound();

            return View(attendance);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Route)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
                return NotFound();

            return View(attendance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
