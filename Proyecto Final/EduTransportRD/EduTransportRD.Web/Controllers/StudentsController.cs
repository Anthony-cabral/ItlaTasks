using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using EduTransportRD.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly DataContext _context;

        public StudentsController(DataContext context)
        {
            _context = context;
        }

        private void LoadDropdowns()
        {
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = "Active",   Text = "Active"   },
                new { Value = "Inactive", Text = "Inactive" }
            }, "Value", "Text");

            ViewBag.BloodTypes = new SelectList(new[]
            {
                "A+", "A-", "B+", "B-",
                "AB+", "AB-", "O+", "O-"
            });
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.FirstName.Contains(search) ||
                    s.LastName.Contains(search) ||
                    s.Enrollment.Contains(search) ||
                    s.DocumentId.Contains(search));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new Paginator<Student>(items, totalItems, page, pageSize);

            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(student);
            }

            _context.Add(student);
            await _context.SaveChangesAsync();

            var wallet = new Wallet
            {
                StudentId = student.Id,
                Balance = 0,
                LastRecharge = DateTime.Now
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound();

            LoadDropdowns();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(student);
            }

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Students.AnyAsync(e => e.Id == id);
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

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student != null)
                _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
