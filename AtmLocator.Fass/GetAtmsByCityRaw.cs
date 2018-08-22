
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

namespace AtmLocator.Fass
{
    public static class GetAtmsByCityRaw
    {
        [FunctionName("GetAtmsByCityRaw")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
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
            
            string atms = File.ReadAllText("allATM.json");
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
    }
}
