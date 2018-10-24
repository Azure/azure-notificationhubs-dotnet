using System;
using Microsoft.Rest.ClientRuntime.Azure;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.Management.NotificationHubs;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.ResourceManager;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.NotificationHubs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.NotificationHubs;

namespace AzureNHDotnet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "1950a258-227b-4e31-a9cf-717495945fc2"; // Unfortunately a "well-known" value: https://blogs.technet.microsoft.com/keithmayer/2014/12/30/leveraging-the-azure-service-management-rest-api-with-azure-active-directory-and-powershell-list-azure-administrators/
            var config = LoadConfiguration(args);
            var authorizationRule = "DefaultFullSharedAccessSignature";

            var creds = await UserTokenProvider.CreateCredentialsFromCache(clientId, "microsoft.com", "kykamper");
            if (creds == null) {
                creds = await UserTokenProvider.LoginByDeviceCodeAsync(clientId, (deviceCodeResult) =>
                {
                    Console.WriteLine(deviceCodeResult.Message);
                    return true;
                });
            }

            // Creating an Azure resource                            
            // var resourceClient = new ResourceManagementClient(creds);
            // resourceClient.SubscriptionId = config.SubscriptionId;
            // await resourceClient.ResourceGroups.CreateOrUpdateAsync(config.ResourceGroupName, new ResourceGroup(config.Location));

            var nhManagemntClient = new NotificationHubsManagementClient(creds);

            nhManagemntClient.SubscriptionId = config.SubscriptionId;
            await nhManagemntClient.Namespaces.CreateOrUpdateAsync(config.ResourceGroupName, config.NamespaceName, new NamespaceCreateOrUpdateParameters(config.Location));
            var nhResource = await nhManagemntClient.NotificationHubs.CreateOrUpdateAsync(config.ResourceGroupName, config.NamespaceName, config.HubName, new NotificationHubCreateOrUpdateParameters(config.Location));

            // Getting connection key from the new resource
            var connectionString = await nhManagemntClient.NotificationHubs.ListKeysAsync(config.ResourceGroupName, config.NamespaceName, config.HubName, authorizationRule);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(connectionString.PrimaryConnectionString, $"https://{config.NamespaceName}.notificationhubs.windows.net/{config.HubName}");
            
            Console.WriteLine("Hello World!");
        }

        private static SampleConfiguration LoadConfiguration(string[] args) 
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("config.json", true);
            var configRoot = configurationBuilder.Build();
            var sampleConfig = new SampleConfiguration();
            configRoot.Bind(sampleConfig);
            return sampleConfig;
        }
    }
}
