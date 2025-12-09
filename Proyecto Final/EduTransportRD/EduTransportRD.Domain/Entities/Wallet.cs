namespace EduTransportRD.Domain.Entities
{
    public class Wallet
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public decimal Balance { get; set; }

        public DateTime LastRecharge { get; set; }
    }
}
