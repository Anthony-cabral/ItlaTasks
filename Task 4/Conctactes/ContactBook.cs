using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ContactesPOO
{
    public class ContactBook
    {
        Database db = new Database();

        public void AddContact()
        {
            Console.WriteLine("Let's add a new contact.");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Address: ");
            string address = Console.ReadLine();

            int newId = GetNextId();

            Contact c = new Contact(newId, name, phone, email, address);
            db.InsertContact(c);

            Console.WriteLine("Contact saved.\n");
        }

        public int GetNextId()
        {
            int id = 1;

            SqlDataReader dr = db.GetContacts();
            while (dr.Read())
            {
                id = (int)dr["Id"] + 1;
            }
            dr.Close();

            return id;
        }

        public void ViewContacts()
        {
            SqlDataReader dr = db.GetContacts();
            Console.WriteLine("ID   Name   Phone   Email   Address");
            Console.WriteLine("-------------------------------------");

            while (dr.Read())
            {
                Console.WriteLine($"{dr["Id"]}   {dr["Name"]}   {dr["Phone"]}   {dr["Email"]}   {dr["Address"]}");
            }

            dr.Close();
        }

        public void SearchContact()
        {
            Console.Write("Enter ID to search: ");
            int id = Convert.ToInt32(Console.ReadLine());

            SqlDataReader dr = db.GetContacts();

            while (dr.Read())
            {
                if ((int)dr["Id"] == id)
                {
                    Console.WriteLine("Name: " + dr["Name"]);
                    Console.WriteLine("Phone: " + dr["Phone"]);
                    Console.WriteLine("Email: " + dr["Email"]);
                    Console.WriteLine("Address: " + dr["Address"]);
                    dr.Close();
                    return;
                }
            }

            Console.WriteLine("Contact not found.");
            dr.Close();
        }

        public void EditContact()
        {
            Console.Write("Enter ID to edit: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("New Name: ");
            string name = Console.ReadLine();

            Console.Write("New Phone: ");
            string phone = Console.ReadLine();

            Console.Write("New Email: ");
            string email = Console.ReadLine();

            Console.Write("New Address: ");
            string address = Console.ReadLine();

            Contact c = new Contact(id, name, phone, email, address);
            db.UpdateContact(c);

            Console.WriteLine("Contact updated.\n");
        }

        public void DeleteContact()
        {
            Console.Write("Enter ID to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            db.DeleteContact(id);

            Console.WriteLine("Contact deleted.");
        }
    }
}
