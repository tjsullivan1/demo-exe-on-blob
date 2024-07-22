using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TestExe
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            _logger.LogInformation($"Name in the query string is {name}.");
            string output; 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = @"C:\home\site\wwwroot\helloWorld.exe";
            _logger.LogInformation("starting process...");
            if (!string.IsNullOrEmpty(name))
            {
                process.StartInfo.Arguments = name;
            }

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            _logger.LogInformation("process started");

            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            _logger.LogInformation($"{output}");

            return new OkObjectResult($"Welcome to Azure Functions! {output}");
        }
    }
}
