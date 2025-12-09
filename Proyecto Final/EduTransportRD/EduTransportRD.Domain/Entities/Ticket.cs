namespace EduTransportRD.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        public int RouteId { get; set; }
        public Route? Route { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal PaidPrice { get; set; }

        public string? Status { get; set; } = "Activo";

        public string Type { get; set; }
    }
}


