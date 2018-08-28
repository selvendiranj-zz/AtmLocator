using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AtmLocator.Fass
{
    public class FileHelper
    {
        static string defaultPage = string.IsNullOrEmpty(GetEnvironmentVariable("DEFAULT_PAGE")) ?
                                    "index.html" : GetEnvironmentVariable("DEFAULT_PAGE");
        const EnvironmentVariableTarget ENV_HOME = EnvironmentVariableTarget.Process;
        const string APP_ROOT = @"site\wwwroot";
        const string QUERY_PARAM = "file";
        static string staticFilesDir = "";

        public static string GetScriptPath()
        {
            return Path.Combine(GetEnvironmentVariable("HOME"), APP_ROOT);
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, ENV_HOME);
        }

        public static string GetFilePath(string staticFilesDir, HttpRequest req, ILogger log)
        {
            FileHelper.staticFilesDir = staticFilesDir;
            string file = req.Query[QUERY_PARAM];
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            file = file ?? data?.name;

            var staticFilesPath = Path.GetFullPath(Path.Combine(GetScriptPath(), FileHelper.staticFilesDir));
            var fullPath = Path.GetFullPath(Path.Combine(staticFilesPath, file));

            if (!IsInDirectory(staticFilesPath, fullPath))
            {
                throw new ArgumentException("Invalid path");
            }

            var isDirectory = Directory.Exists(fullPath);
            if (isDirectory)
            {
                fullPath = Path.Combine(fullPath, defaultPage);
            }

            return fullPath;
        }

        public static string GetFilePath(string staticFilesDir, string fileName, ILogger log)
        {
            FileHelper.staticFilesDir = staticFilesDir;
            string file = fileName;
            
            var staticFilesPath = Path.GetFullPath(Path.Combine(GetScriptPath(), FileHelper.staticFilesDir));
            var fullPath = Path.GetFullPath(Path.Combine(staticFilesPath, file));

            if (!IsInDirectory(staticFilesPath, fullPath))
            {
                throw new ArgumentException("Invalid path");
            }

            var isDirectory = Directory.Exists(fullPath);
            if (isDirectory)
            {
                fullPath = Path.Combine(fullPath, defaultPage);
            }

            return fullPath;
        }

        public static bool IsInDirectory(string parentPath, string childPath)
        {
            var parent = new DirectoryInfo(parentPath);
            var child = new DirectoryInfo(childPath);

            var dir = child;
            do
            {
                if (dir.FullName == parent.FullName)
                {
                    return true;
                }
                dir = dir.Parent;
            } while (dir != null);

            return false;
        }

        public static string GetMimeType(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return MimeTypeMap.GetMimeType(fileInfo.Extension);
        }

        public static async Task<string> GetResponseString(string url)
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
