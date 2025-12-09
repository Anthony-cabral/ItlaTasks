namespace EduTransportRD.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string Enrollment { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DocumentId { get; set; }

        public string StudentEmail { get; set; }

        public string StudentPhone { get; set; }

        public string Address { get; set; }

        public string District { get; set; }

        public string City { get; set; }

        public DateTime BirthDate { get; set; }

        public string BloodType { get; set; }

        public string MedicalConditions { get; set; }

        public string Grade { get; set; }

        public string GuardianName { get; set; }

        public string GuardianPhone { get; set; }

        public string GuardianEmail { get; set; }

        public string Status { get; set; }

        public List<Ticket>? Tickets { get; set; }
        public List<Payment>? Payments { get; set; }
        public List<Attendance>? Attendances { get; set; }
        public Wallet? Wallet { get; set; }
    }
}
