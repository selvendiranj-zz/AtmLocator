using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AtmLocator.Fass
{
    public class RequestHelper
    {
        public static string GetParamValue(string queryParam, HttpRequest req)
        {
            string paramValue = req.Query[queryParam];
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            paramValue = paramValue ?? data?[queryParam];
            if (paramValue == null)
            {
                var message = string.Format("Please pass query param '{0}' on the query string or in the request body", queryParam);
                throw new ArgumentNullException(queryParam, message);
            }
            return paramValue;
        }
    }
}
