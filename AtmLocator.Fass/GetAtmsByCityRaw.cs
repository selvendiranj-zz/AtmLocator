
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AtmLocator.Fass
{
    public static class GetAtmsByCityRaw
    {
        [FunctionName("GetAtmsByCityRaw")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string name = req.Query["name"];

                string requestBody = new StreamReader(req.Body).ReadToEnd();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;
                if (name == null)
                {
                    return new BadRequestObjectResult("Please pass a city name {name} on the query string or in the request body");
                }

                // pass empty staticFilesDir to access files from app root
                string filePath = FileHelper.GetFilePath("", "allAtms.json", log);
                log.LogInformation("filePath: " + filePath);
                FileStream stream = new FileStream(filePath, FileMode.Open);
                string atms = new StreamReader(stream).ReadToEnd();
                var allAtms = JsonConvert.DeserializeObject<List<AtmLocation>>(atms);
                var atmsByCity = allAtms.Where(l => l.Address.City.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (atms != null)
                {
                    return (ActionResult)new OkObjectResult(atmsByCity);
                }
                else
                {
                    return new BadRequestObjectResult("no ATM locations found for the city");
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }
    }
}
