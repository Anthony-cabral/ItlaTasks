using System;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a number: ");
            int number = int.Parse(Console.ReadLine());

            if (number % 2 == 0)
            {
                Console.WriteLine($"The number entered is {number} and it is Even.");
            }
            else
            {
                Console.WriteLine($"The number entered is {number} and it is Odd.");
            }

            Console.ReadKey();
        }
    }
}