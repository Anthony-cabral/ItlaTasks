namespace ContactesPOO
{
    public class Contact
    {
        public int Id;
        public string Name;
        public string Phone;
        public string Email;
        public string Address;

        public Contact(int id, string name, string phone, string email, string address)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
        }
    }
}
