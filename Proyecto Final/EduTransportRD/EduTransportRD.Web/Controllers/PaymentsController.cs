using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly DataContext _context;

        public PaymentsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Payments
                .Include(p => p.Student)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }
    }
}
