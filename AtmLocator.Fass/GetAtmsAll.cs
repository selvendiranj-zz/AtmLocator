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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Headers;

namespace AtmLocator.Fass
{
    public static class GetAtmsAll
    {
        [FunctionName("GetAtmsAll")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                // pass empty staticFilesDir to access files from app root
                string filePath = FileHelper.GetFilePath("", "allAtms.json", log);
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
                var atmsSmpl = allAtms.Select(a => new AtmSimplified()
                {
                    HouseNumber = a.Address.HouseNumber,
                    Street = a.Address.Street,
                    City = a.Address.City,
                    PostalCode = a.Address.PostalCode
                });

                if (atms != null)
                {
                    return (ActionResult)new OkObjectResult(atmsSmpl);
                }
                else
                {
                    return new BadRequestObjectResult("no ATM locations found for the bank");
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
