using EduTransportRD.Domain.Entities;
using EduTransportRD.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace EduTransportRD.Web.Controllers
{
    public class StudentPortalController : Controller
    {
        private readonly DataContext _context;

        public StudentPortalController(DataContext context)
        {
            _context = context;
        }

        public IActionResult CompleteProfile()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.UserAccounts.First(x => x.Id == id);

            var model = new Student
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                StudentEmail = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteProfile(Student model)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.UserAccounts.First(x => x.Id == id);

            var student = new Student
            {
                Enrollment = model.Enrollment,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DocumentId = model.DocumentId,
                StudentEmail = model.StudentEmail,
                StudentPhone = model.StudentPhone,
                Address = model.Address,
                District = model.District,
                City = model.City,
                BirthDate = model.BirthDate,
                BloodType = model.BloodType,
                MedicalConditions = model.MedicalConditions,
                Grade = model.Grade,
                GuardianName = model.GuardianName,
                GuardianPhone = model.GuardianPhone,
                GuardianEmail = model.GuardianEmail,
                Status = "Activo"
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            user.StudentId = student.Id;
            _context.UserAccounts.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.UserAccounts.First(x => x.Id == id);
            var student = _context.Students.First(x => x.Id == user.StudentId);

            return View(student);
        }
    }
}
