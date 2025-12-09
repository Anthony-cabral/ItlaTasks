
namespace EduTransportRD.Domain.Entities
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentId { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime? LicenseExpiration { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
}
