using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTransportRD.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalDrivers = await _context.Drivers.CountAsync();
            ViewBag.TotalVehicles = await _context.Vehicles.CountAsync();
            ViewBag.TotalRoutes = await _context.Routes.CountAsync();
            ViewBag.TotalSchedules = await _context.Schedules.CountAsync();

            ViewBag.TotalTickets = await _context.Tickets.CountAsync();
            ViewBag.TicketsToday = await _context.Tickets
                    .CountAsync(t => t.PurchaseDate.Date == DateTime.Today);

            ViewBag.PaymentsToday = await _context.Payments
                    .Where(p => p.PaymentDate.Date == DateTime.Today)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0;

            
            ViewBag.AttendanceToday = await _context.Attendances
                    .CountAsync(a => a.Date.Date == DateTime.Today);

            
            ViewBag.TotalAttendance = await _context.Attendances.CountAsync();

            return View();
        }
    }
}
