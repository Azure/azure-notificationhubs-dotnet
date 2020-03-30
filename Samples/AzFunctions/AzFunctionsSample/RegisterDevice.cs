using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsSample
{
    public static class RegisterDevice
    {
        [FunctionName("RegisterDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string hubName = req.Query["hubName"];
            hubName = hubName ?? data?.hubName;
            if(hubName == null)
            {
                return new BadRequestObjectResult("Please pass a Hub name (\"hubName\") on the query string or in the request body");
            }
            string token = req.Query["token"];
            token = token ?? data?.token;
            if(token == null)
            {
                return new BadRequestObjectResult("Please pass a token (\"token\") on the query string or in the request body");
            }
            string tags = req.Query["tags"];
            tags = tags ?? data?.tags;
            var hub = NotificationHubClient.CreateClientFromConnectionString(Parameters.ConnectionString, hubName);

            var installation = new Installation
            {
                InstallationId = Guid.NewGuid().ToString(),
                Platform = NotificationPlatform.Apns,
                PushChannel = token
            };
            if(tags != null){
                var tagArray = tags.Split(',');
                var tagsList = new ArraySegment<string>(tagArray);
                installation.Tags = tagsList;
            }
            try
            {
                hub.CreateOrUpdateInstallation(installation);
                var response = $"{{ \"installationId\" = \"{installation.InstallationId}\" }}";

                return new OkObjectResult(response);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);

            }
        }
    }
}
