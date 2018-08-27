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

            //string atms = File.ReadAllText("allATM.json");
            string atms = GetResponseString("https://www.ing.nl/api/locator/atms/").Result;
            atms = atms.Substring(5, atms.Length - 5);

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
                //return new JsonResult(HttpStatusCode.OK)
                //{
                //    ContentType = "application/json",
                //    StatusCode = StatusCodes.Status200OK,
                //    Value = new StringContent(JsonConvert.SerializeObject(atmsSmpl))
                //};
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
