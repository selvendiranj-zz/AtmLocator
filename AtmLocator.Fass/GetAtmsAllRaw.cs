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
using System.Net.Http;
using System.Threading.Tasks;

namespace AtmLocator.Fass
{
    public static class GetAtmsAllRaw
    {
        [FunctionName("GetAtmsAllRaw")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string atms = File.ReadAllText("allATM.json");
            string atms = GetResponseString("https://www.ing.nl/api/locator/atms/").Result;
            atms = atms.Substring(5, atms.Length - 5);
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

        private static async Task<string> GetResponseString(string url)
        {
            var httpClient = new HttpClient();

            //var parameters = new Dictionary<string, string>();
            //parameters["text"] = text;

            //var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(parameters));
            var response = await httpClient.GetAsync(url);
            var contents = await response.Content.ReadAsStringAsync();

            return contents;
        }
    }
}
