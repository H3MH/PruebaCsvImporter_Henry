using System;
using System.Threading.Tasks;

namespace CsvImporter.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Menu().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task Menu() 
        {
            Console.WriteLine("Bienvenido a CsvImporter");
        }

        private static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
