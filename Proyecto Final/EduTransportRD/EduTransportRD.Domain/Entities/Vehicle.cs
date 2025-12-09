namespace EduTransportRD.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        public string Plate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public int Capacity { get; set; }

        public string Status { get; set; }

        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }

        public List<Route>? Routes { get; set; }
    }
}
