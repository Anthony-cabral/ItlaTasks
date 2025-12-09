using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class WalletsController : Controller
    {
        private readonly DataContext _context;

        public WalletsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var query = _context.Wallets
                .Include(w => w.Student)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();

                query = query.Where(w =>
                    w.Student.FirstName.ToLower().Contains(s) ||
                    w.Student.LastName.ToLower().Contains(s) ||
                    w.Student.Enrollment.ToLower().Contains(s));
            }

            ViewBag.Search = search;

            var list = query
                .OrderBy(w => w.Student.FirstName)
                .ThenBy(w => w.Student.LastName)
                .ToList();

            return View(list);
        }

        public async Task<IActionResult> AddBalance(int studentId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.Student)
                .FirstOrDefaultAsync(w => w.StudentId == studentId);

            if (wallet == null)
                return NotFound();

            var model = new Payment
            {
                StudentId = studentId,
                Method = "Efectivo"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBalance(Payment payment)
        {
            if (payment.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "El monto debe ser mayor a 0.");
                return View(payment);
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.StudentId == payment.StudentId);

            if (wallet == null)
                return NotFound();

            wallet.Balance += payment.Amount;
            wallet.LastRecharge = DateTime.Now;

            payment.PaymentDate = DateTime.Now;
            payment.Description = "Recarga de saldo";
            payment.Status = "Completado";

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
