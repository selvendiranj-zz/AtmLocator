using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AtmLocator.Fass
{
    public static class GetAtmsAllRaw
    {
        [FunctionName("GetAtmsAllRaw")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string atms = File.ReadAllText("allATM.json");
            var allAtms = JsonConvert.DeserializeObject<List<AtmLocation>>(atms);
            
            if (atms != null)
            {
                return (ActionResult)new OkObjectResult(allAtms);
            }
            else
            {
                return new BadRequestObjectResult("no ATM locations found for the bank");
            }
        }
    }
}
