
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

                var cityName = RequestHelper.GetParamValue("name", req);

                // pass empty staticFilesDir to access files from app root
                string filePath = FileHelper.GetFilePath("", "allAtms.json");
                log.LogInformation("filePath: " + filePath);
                // allAtms.json file reading START
                string atms;
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        atms = reader.ReadToEnd();
                    }
                } // allAtms.json file reading END
                var allAtms = JsonConvert.DeserializeObject<List<AtmLocation>>(atms);
                var atmsByCity = allAtms.Where(l => l.Address.City.Equals(cityName, StringComparison.InvariantCultureIgnoreCase));

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
