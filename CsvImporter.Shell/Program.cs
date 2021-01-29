using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CsvImporter.Shell.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            try
            {

                BlobServiceClient blobServiceClient = new BlobServiceClient(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString());
                Console.WriteLine("Bienvenido a CsvImporter");
                Console.WriteLine($"Blob: {blobServiceClient.AccountName}");
                Console.WriteLine("Lista de Contenedores");
                AzureStorageClient azure = new AzureStorageClient();
                var containers = azure.ListContainers(blobServiceClient, "").GetAwaiter().GetResult();

                #region selecting a container from the list

                var containerSelected = SelectContainer(containers);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerSelected.Name);
                Console.WriteLine($"Contenedor Seleccionado: {containerSelected.Name}");
                #endregion

                #region selecting a File from the list
                Console.WriteLine("Lista de Archivos");
                var blobs = await azure.ListBlobsFlatListing(containerClient);
                AlertMessage("Una vez seleccionado el archivo la descarga comenzara automaticamente.");
                var selectedBlob = SelectFile(blobs);
                var blogClient = containerClient.GetBlobClient(selectedBlob.Name);
                #endregion

                #region Download Selected Blob
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string curFile = $@"{path}blobs";

                if (!Directory.Exists(curFile))
                    Directory.CreateDirectory(curFile);

                var filePath = $@"{curFile}\{blogClient.Name}";

                azure.DownloadBlob(blogClient, filePath);
                Console.WriteLine($"archivo {blogClient.Name} descaragdo con exito");

                Console.WriteLine($"¿Desea Limpiar la tabla antes de la migracion? S/N");
                var resp = Console.ReadLine();
                #endregion

                if (resp.ToUpper() == "S")
                    CleanDB();

                #region migrate the data to the database
                var itemsList = azure.LoadFileInMemory(filePath);
                Console.WriteLine($"archivo {blogClient.Name} cargado en memoria");
                Console.WriteLine($"{blogClient.Name} Inicio de migracion");
                await azure.MigrationBlob(itemsList);
                Console.WriteLine($"archivo {blogClient.Name} guardado en db");
                #endregion

            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }
        }

        private static BlobContainerItem SelectContainer(List<BlobContainerItem> containers)
        {
            #region selecting a container from the list

            string c;
            do
            {
                Console.WriteLine("Seleccione un Contenedor");
                c = Console.ReadLine();
            } while (!validateInt(c, containers.Count));

            var fileSeledted = int.Parse(c);

            return containers[fileSeledted];

            #endregion
        }

        private static BlobItem SelectFile(List<BlobItem> blobs)
        {
            #region selecting a blob from the list

            string f;
            do
            {
                Console.WriteLine("Seleccione un Archivo");
                f = Console.ReadLine();
            } while (!validateInt(f, blobs.Count));

            var fileSeledted = int.Parse(f);

            return blobs[fileSeledted];

            #endregion
        }

        private static void CleanDB()
        {
            using (DataContext context = new DataContext())
            {
                // Creamos una DB en limpio 
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        private static void AlertMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static bool validateInt(string input, int leght)
        {
            int valor;

            if (int.TryParse(input, out valor))
            {
                if (valor <= leght)
                {
                    if (valor >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        ErrorMessage("No se admiten numeros negativos.");
                        return false;
                    }
                }
                else
                {
                    ErrorMessage("El Número debe encontrarse en la lista.");
                    return false;
                }
            }
            else
            {
                ErrorMessage("Debe Ingresar Un Número.");
                return false;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
             .ConfigureServices((hostContext, services) =>
             {
                 services.AddDbContext<DataContext>();
             });
    }
}
