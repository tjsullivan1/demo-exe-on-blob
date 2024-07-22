using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TestExe
{
    public class Blob
    {
        private readonly ILogger<Blob> _logger;

        public Blob(ILogger<Blob> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Blob))]
        public async Task Run([BlobTrigger("myexe/{name}", Connection = "AZURE_STORAGE")] string blobTrigger, string name, [BlobInput("myexe/{name}", Connection = "AZURE_STORAGE")] string blobContent, BlobClient blobClient)
        {
            _logger.LogInformation("starting program...");
            // Connect to a blob storage account using the connection string tored in an environment variable called AZURE_STORAGE
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE");
            _logger.LogInformation("Building service client...");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to a blob
            _logger.LogInformation("Building container client...");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("myexe");
            _logger.LogInformation("Building blob client...");
            BlobClient blobClient2 = containerClient.GetBlobClient("helloWorld.exe");

            _logger.LogInformation("starting download...");
            await blobClient2.DownloadToAsync("helloWorld.exe");

            _logger.LogInformation("download successful...");
            string output;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "helloWorld.exe";
            _logger.LogInformation("starting process...");
    

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            _logger.LogInformation("process started");

            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            _logger.LogInformation($"{output}");
         }
    }
}
