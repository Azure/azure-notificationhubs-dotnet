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
    public static class ListRegistrations
    {
        [FunctionName("ListRegistrations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string hubName = req.Query["hubName"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            hubName = hubName ?? data?.hubName;
            if(hubName == null){
                return new BadRequestObjectResult("Please pass a Hub name on the query string or in the request body");
            }

            var hub = NotificationHubClient.CreateClientFromConnectionString(
                Parameters.ConnectionString, 
                hubName);

            CollectionQueryResult<RegistrationDescription> regs = await hub.GetAllRegistrationsAsync(0);
            string result = "";int quantity = 0;
            foreach(RegistrationDescription reg in regs){
                result += $"{reg.RegistrationId}";
                if(reg.Tags != null){
                    result += $"-> {reg.Tags.Count} tags: {reg.Tags.ToString()}";
                } else{
                    result += "-> No tags";
                }
                result += "\n";
                quantity++;
            }
            return (ActionResult) new OkObjectResult($"There are {quantity} registrations:\n{result}");
        }
    }
}
