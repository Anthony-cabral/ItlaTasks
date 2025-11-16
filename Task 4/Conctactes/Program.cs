using System;

namespace ContactesPOO
{
    class Program
    {
        static void Main()
        {
            ContactBook book = new ContactBook();
            bool running = true;

            Console.WriteLine("My Contact Book");
            Console.WriteLine("Welcome to your contact list");

            while (running)
            {
                Console.Write("1. Add Contact    ");
                Console.Write("2. View Contacts    ");
                Console.Write("3. Search Contact    ");
                Console.Write("4. Edit Contact    ");
                Console.Write("5. Delete Contact    ");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");

                int option = Convert.ToInt32(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        book.AddContact();
                        break;

                    case 2:
                        book.ViewContacts();
                        break;

                    case 3:
                        book.SearchContact();
                        break;

                    case 4:
                        book.EditContact();
                        break;

                    case 5:
                        book.DeleteContact();
                        break;

                    case 6:
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
