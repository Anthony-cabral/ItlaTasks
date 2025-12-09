namespace EduTransportRD.Domain.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        public int RouteId { get; set; }
        public Route? Route { get; set; }

        public string WeekDay { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        public List<Ticket>? Tickets { get; set; }
    }
}
