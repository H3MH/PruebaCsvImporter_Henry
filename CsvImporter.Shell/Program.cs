using Azure.Storage.Blobs;
using System;
using System.Configuration;
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
            BlobServiceClient blobServiceClient = new BlobServiceClient(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString());
            Console.WriteLine("Bienvenido a CsvImporter");
            Console.WriteLine($"Blob: {blobServiceClient.AccountName}");
            Console.WriteLine("Lista de Contenedores");
            //AzureStorageClient azure = new AzureStorageClient();
            //var containers = azure.ListContainers(blobServiceClient, "").GetAwaiter().GetResult();
        }

        private static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
