using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using EduTransportRD.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EduTransportRD.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

       
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.UserAccounts.FirstOrDefault(x => x.Email == email && x.Password == password);


            if (user == null)
            {
                TempData["Error"] = "Credenciales incorrectas.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);

            if (user.Role == "Student")
            {
                return RedirectToAction("Index", "StudentPortal");
            }

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string firstname, string lastname, string email, string password, string confirm)
        {
            if (password != confirm)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return View();
            }

            var exist = _context.UserAccounts.Any(x => x.Email == email);

            if (exist)
            {
                TempData["Error"] = "Este correo ya está registrado.";
                return View();
            }

            var user = new UserAccount
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                Password = password,
                Role = "Student"
            };

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Cuenta creada correctamente.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


