using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly DataContext _context;

        public TicketsController(DataContext context)
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

            ViewBag.Schedules = new SelectList(
                _context.Schedules
                    .Include(s => s.Route)
                    .OrderBy(s => s.WeekDay)
                    .ThenBy(s => s.DepartureTime)
                    .Select(s => new
                    {
                        s.Id,
                        Name = s.WeekDay + " - " + s.Route.RouteName
                    })
                    .ToList(),
                "Id",
                "Name");

            ViewBag.TypeList = new SelectList(new[]
            {
                "Ida",
                "Vuelta",
                "Ida y vuelta"
            });
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.Route)
                .Include(t => t.Schedule)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return View(tickets);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            var model = new Ticket
            {
                Status = "Activo"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(ticket);
            }

            var route = await _context.Routes.FindAsync(ticket.RouteId);
            if (route == null)
            {
                LoadDropdowns();
                return View(ticket);
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.StudentId == ticket.StudentId);

            if (wallet == null)
            {
                TempData["Error"] = "El estudiante no tiene una cartera asignada.";
                LoadDropdowns();
                return View(ticket);
            }

            decimal basePrice = route.TicketPrice;

            // Normalizar tipo de ticket
            string type = ticket.Type?.Trim().ToLower() ?? "";

            decimal totalCharge = type switch
            {
                "ida" => basePrice,
                "vuelta" => basePrice,
                "ida y vuelta" => basePrice * 2,
                _ => basePrice
            };

            if (wallet.Balance < totalCharge)
            {
                TempData["Error"] = "Fondos insuficientes en la cartera.";
                LoadDropdowns();
                return View(ticket);
            }

            wallet.Balance -= totalCharge;
            wallet.LastRecharge = DateTime.Now;

            ticket.PurchaseDate = DateTime.Now;
            ticket.Status = "Activo";
            ticket.PaidPrice = totalCharge;

            _context.Tickets.Add(ticket);

            var payment = new Payment
            {
                StudentId = ticket.StudentId,
                Amount = totalCharge,
                Method = "Wallet",
                PaymentDate = DateTime.Now,
                Description = "Compra de ticket (" + ticket.Type + ")",
                Status = "Completado"
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.Route)
                .Include(t => t.Schedule)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.Route)
                .Include(t => t.Schedule)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
