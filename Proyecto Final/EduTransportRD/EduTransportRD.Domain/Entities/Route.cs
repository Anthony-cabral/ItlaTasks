namespace EduTransportRD.Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }

        public string RouteName { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public decimal TicketPrice { get; set; } = 50;

        public string Status { get; set; }

        public List<Schedule>? Schedules { get; set; }
        public List<Stop>? Stops { get; set; }

        public List<Ticket>? Tickets { get; set; }
        public List<Attendance>? Attendances { get; set; }
    }
}


