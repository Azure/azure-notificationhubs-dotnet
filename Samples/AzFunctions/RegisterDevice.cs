using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.NotificationHubs;

namespace NHubSamples
{
    public static class RegisterDevice
    {
        [FunctionName("RegisterDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string hubName = req.Query["hubName"];
            hubName = hubName ?? data?.hubName;
            if(hubName == null){
                return new BadRequestObjectResult("Please pass a Hub name (\"hubName\") on the query string or in the request body");
            }
            string token = req.Query["token"];
            token = token ?? data?.token;
            if(token == null){
                return new BadRequestObjectResult("Please pass a token (\"token\") on the query string or in the request body");
            }
            string tags = req.Query["tags"];
            tags = tags ?? data?.tags;
            var hub = NotificationHubClient.CreateClientFromConnectionString(
                Parameters.ConnectionString, 
                hubName);

            Installation installation = new Installation();
            installation.InstallationId = Guid.NewGuid().ToString();
            installation.Platform = NotificationPlatform.Apns;
            installation.PushChannel = token;
            if(tags != null){
                string[] tagArray = tags.Split(',');
                ArraySegment<string> tagsList = new ArraySegment<string>(tagArray);
                installation.Tags = tagsList;
            }
            try{
                hub.CreateOrUpdateInstallation(installation);
                string response = $"{{ \"installationId\" = \"{installation.InstallationId}\" }}";

                return  (ActionResult)new OkObjectResult(response);
            }catch(Exception e){
                return new BadRequestObjectResult(e.Message);

            }
        }
    }
}
