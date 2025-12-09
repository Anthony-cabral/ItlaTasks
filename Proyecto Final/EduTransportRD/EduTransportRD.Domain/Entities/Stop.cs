namespace EduTransportRD.Domain.Entities
{
    public class Stop
    {
        public int Id { get; set; }

        public string StopName { get; set; }

        public string Address { get; set; }

        public int RouteId { get; set; }
        public Route? Route { get; set; }

        public int StopOrder { get; set; }
    }
}
