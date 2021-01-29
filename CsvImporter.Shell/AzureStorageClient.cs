namespace CsvImporter.Shell
{
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using CsvImporter.Shell.Domain;
    using CsvImporter.Shell.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class AzureStorageClient
    {

        public async Task<List<BlobContainerItem>> ListContainers(BlobServiceClient blobServiceClient, string prefix)
        {
            try
            {
                // Call the listing operation and enumerate the result segment.
                var resultSegment =
                    blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix, default)
                    .AsPages(default);
                var containers = new List<BlobContainerItem>();
                var i = 0;

                await foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
                {
                    foreach (BlobContainerItem containerItem in containerPage.Values)
                    {
                        Console.WriteLine($"{i}- Container name: {containerItem.Name}");
                        containers.Add(containerItem);
                        i++;
                    }

                    Console.WriteLine();
                }

                return containers;
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task AddContainerMetadataAsync(BlobContainerClient container)
        {
            try
            {
                IDictionary<string, string> metadata =
                   new Dictionary<string, string>();

                // Add some metadata to the container.
                metadata.Add("docType", "textDocuments");
                metadata.Add("category", "guidance");

                // Set the container's metadata.
                await container.SetMetadataAsync(metadata);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public static async Task ReadContainerMetadataAsync(BlobContainerClient container)
        {
            try
            {
                var properties = await container.GetPropertiesAsync();

                // Enumerate the container's metadata.
                Console.WriteLine("Container metadata:");
                foreach (var metadataItem in properties.Value.Metadata)
                {
                    Console.WriteLine($"\tKey: {metadataItem.Key}");
                    Console.WriteLine($"\tValue: {metadataItem.Value}");
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public async Task<List<BlobItem>> ListBlobsFlatListing(BlobContainerClient blobContainerClient)
        {
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = blobContainerClient.GetBlobsAsync()
                    .AsPages(default);

                var selectedFiles = new List<BlobItem>();
                var i = 0;
                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                {
                    foreach (BlobItem blobItem in blobPage.Values)
                    {

                        Console.WriteLine($"{i} - Blob name: {blobItem.Name}");
                        selectedFiles.Add(blobItem);
                        i++;
                    }
                }

                return selectedFiles;


            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public void DownloadBlob(BlobClient blob, string pathToDownload)
        {

            //Get the blob client instance and call the method Download().
            //Note: At this line we are not download the file, we are just getting the download stream of the file with additional metadata to download.
            var blobToDownload = blob.Download().Value; //Step 1
            var outputFile = File.OpenWrite(pathToDownload); //Step 2

            var downloadBuffer = new byte[81920]; //Choose an appropriate buffer size
            int bytesRead;
            int totalBytesDownloaded = 0;

            //Read(Download) the file in bytes
            while ((bytesRead = blobToDownload.Content.Read(downloadBuffer, 0, downloadBuffer.Length)) != 0) //Step 3
            {
                outputFile.Write(downloadBuffer, 0, bytesRead); // Write the download bytes from source stream to destination stream. //Step 3
                totalBytesDownloaded += bytesRead;//Increment the total downloaded counter. This is used for percentage calculation //Step 4

                Console.WriteLine($"downloaded {blobToDownload.ContentLength} of {totalBytesDownloaded} bytes. {GetProgressPercentage(blobToDownload.ContentLength, totalBytesDownloaded)} % complete...");//Step 5
            }

            //Close both the source and destination stream
            blobToDownload.Content.Close();
            outputFile.Close();
        }

        public List<StockItem> LoadFileInMemory(string pach)
        {
            var listItems = new List<StockItem>();

            using (var streamRdr = new StreamReader(pach))
            {
                var csvReader = new CsvReader(streamRdr, ";");
                var item = new StockItem();
                var aux = false;

                while (csvReader.Read())
                {
                    item = new StockItem();

                    for (int j = 0; j < csvReader.FieldsCount; j++)
                    {
                        if (aux)
                            switch (j)
                            {
                                case 0:
                                    item.PointOfSale = int.Parse(csvReader[j]);
                                    break;
                                case 1:
                                    item.Product = csvReader[j];
                                    break;
                                case 2:
                                    item.Date = DateTime.Parse(csvReader[j]);
                                    break;
                                case 3:
                                    item.Stock = int.Parse(csvReader[j]);
                                    break;
                                default:
                                    break;
                            }
                    }

                    if (aux)
                        listItems.Add(item);
                    else
                        aux = true;
                }
            }

            return listItems;
        }

        public async Task MigrationBlob(List<StockItem> listItems)
        {

            using (DataContext context = new DataContext())
            {
                // Creamos una DB en limpio 
                await context.BulkInsertAsync(listItems);

            }

        }
        private double GetProgressPercentage(double totalSize, double currentSize)
        {
            return Math.Round(((currentSize / totalSize) * 100));
        }

        public async Task ListBlobsHierarchicalListing(BlobContainerClient container, string prefix)
        {
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/")
                    .AsPages(default);

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
                {
                    // A hierarchical listing may return both virtual directories and blobs.
                    foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
                    {
                        if (blobhierarchyItem.IsPrefix)
                        {
                            // Write out the prefix of the virtual directory.
                            Console.WriteLine("Virtual directory prefix: {0}", blobhierarchyItem.Prefix);

                            // Call recursively with the prefix to traverse the virtual directory.
                            await ListBlobsHierarchicalListing(container, blobhierarchyItem.Prefix);
                        }
                        else
                        {
                            // Write out the name of the blob.
                            Console.WriteLine("Blob name: {0}", blobhierarchyItem.Blob.Name);
                        }
                    }

                    Console.WriteLine();
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        private static async Task GetBlobPropertiesAsync(BlobClient blob)
        {
            try
            {
                // Get the blob properties
                Azure.Storage.Blobs.Models.BlobProperties properties = await blob.GetPropertiesAsync();

                // Display some of the blob's property values
                Console.WriteLine($" ContentLanguage: {properties.ContentLanguage}");
                Console.WriteLine($" ContentType: {properties.ContentType}");
                Console.WriteLine($" CreatedOn: {properties.CreatedOn}");
                Console.WriteLine($" LastModified: {properties.LastModified}");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
