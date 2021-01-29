using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.NUnitTest
{
    public class AzureStorageTest : BaseAzure
    {
        /// <summary>
        /// Upload a file to a blob.
        /// </summary>
        [Test]
        public async Task UploadAsync()
        {
            // Create a temporary Lorem Ipsum file on disk that we can upload
            string path = CreateTempFile(SampleFileContent);

            string connectionString = ConnectionString;


            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            await container.CreateAsync();
            try
            {

                BlobClient blob = container.GetBlobClient(Randomize("sample-file"));


                using (FileStream file = File.OpenRead(path))
                {
                    await blob.UploadAsync(file);
                }


                BlobProperties properties = await blob.GetPropertiesAsync();
                Assert.AreEqual(SampleFileContent.Length, properties.ContentLength);
            }
            finally
            {
                // Clean up after the test when we're finished
                await container.DeleteAsync();
            }
        }

        /// <summary>
        /// Download a blob to a file.
        /// </summary>
        [Test]
        public async Task DownloadAsync()
        {

            string originalPath = CreateTempFile(SampleFileContent);


            string downloadPath = CreateTempPath();


            string connectionString = ConnectionString;

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            await container.CreateAsync();
            try
            {
                // Get a reference to a blob named "sample-file"
                BlobClient blob = container.GetBlobClient(Randomize("sample-file"));

                // First upload something the blob so we have something to download
                await blob.UploadAsync(File.OpenRead(originalPath));

                // Download the blob's contents and save it to a file
                BlobDownloadInfo download = await blob.DownloadAsync();
                using (FileStream file = File.OpenWrite(downloadPath))
                {
                    await download.Content.CopyToAsync(file);
                }

                // Verify the contents
                Assert.AreEqual(SampleFileContent, File.ReadAllText(downloadPath));
            }
            finally
            {
                // Clean up after the test when we're finished
                await container.DeleteAsync();
            }
        }

        /// <summary>
        /// Download our sample image.
        /// </summary>
        [Test]
        public async Task DownloadImageAsync()
        {
            string downloadPath = CreateTempPath();
            #region Snippet:SampleSnippetsBlob_Async
            // Get a temporary path on disk where we can download the file
            //@@ string downloadPath = "hello.jpg";

            // Download the public blob at https://aka.ms/bloburl
            await new BlobClient(new Uri("https://aka.ms/bloburl")).DownloadToAsync(downloadPath);
            #endregion

            Assert.IsTrue(File.ReadAllBytes(downloadPath).Length > 0);
            File.Delete("hello.jpg");
        }

        /// <summary>
        /// List all the blobs in a container.
        /// </summary>
        [Test]
        public async Task ListAsync()
        {
            // Get a connection string to our Azure Storage account.
            string connectionString = ConnectionString;

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            await container.CreateAsync();
            try
            {
                // Upload a couple of blobs so we have something to list
                await container.UploadBlobAsync("first", File.OpenRead(CreateTempFile()));
                await container.UploadBlobAsync("second", File.OpenRead(CreateTempFile()));
                await container.UploadBlobAsync("third", File.OpenRead(CreateTempFile()));

                // List all the blobs
                List<string> names = new List<string>();
                await foreach (BlobItem blob in container.GetBlobsAsync())
                {
                    names.Add(blob.Name);
                }

                Assert.AreEqual(3, names.Count);
                Assert.Contains("first", names);
                Assert.Contains("second", names);
                Assert.Contains("third", names);
            }
            finally
            {
                // Clean up after the test when we're finished
                await container.DeleteAsync();
            }
        }

        /// <summary>
        /// Trigger a recoverable error.
        /// </summary>
        [Test]
        public async Task ErrorsAsync()
        {
            // Get a connection string to our Azure Storage account.
            string connectionString = ConnectionString;

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            await container.CreateAsync();

            try
            {
                // Try to create the container again
                await container.CreateAsync();
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.ContainerAlreadyExists)
            {
                // Ignore any errors if the container already exists
            }
            catch (RequestFailedException ex)
            {
                Assert.Fail($"Unexpected error: {ex}");
            }

            // Clean up after the test when we're finished
            await container.DeleteAsync();
        }
    }
}
