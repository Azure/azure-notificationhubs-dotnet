using System;
using System.Linq;
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

namespace CreateHubSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "1950a258-227b-4e31-a9cf-717495945fc2"; // Unfortunately a "well-known" value: https://blogs.technet.microsoft.com/keithmayer/2014/12/30/leveraging-the-azure-service-management-rest-api-with-azure-active-directory-and-powershell-list-azure-administrators/
            var config = LoadConfiguration(args);
            var authorizationRule = "DefaultFullSharedAccessSignature";

            var creds = await UserTokenProvider.LoginByDeviceCodeAsync(clientId, (deviceCodeResult) =>
            {
                Console.WriteLine(deviceCodeResult.Message);
                return true;
            });

            // Creating resource group                           
            var resourceClient = new ResourceManagementClient(creds);
            resourceClient.SubscriptionId = config.SubscriptionId;
            await resourceClient.ResourceGroups.CreateOrUpdateAsync(config.ResourceGroupName, new ResourceGroup(config.Location));

            var nhManagemntClient = new NotificationHubsManagementClient(creds);
            nhManagemntClient.SubscriptionId = config.SubscriptionId;

            // Create namespace
            await nhManagemntClient.Namespaces.CreateOrUpdateAsync(config.ResourceGroupName, config.NamespaceName, new NamespaceCreateOrUpdateParameters(config.Location)
            {
                Sku = new Microsoft.Azure.Management.NotificationHubs.Models.Sku("standard")
            });
            // Create hub
            Microsoft.Azure.Management.NotificationHubs.Models.GcmCredential gcmCreds = null;
            Microsoft.Azure.Management.NotificationHubs.Models.ApnsCredential apnsCreds = null;
            if (config.GcmCreds != null)
            {
                gcmCreds = new Microsoft.Azure.Management.NotificationHubs.Models.GcmCredential
                {
                    GoogleApiKey = config.GcmCreds
                };
            }
            if (config.ApnsCreds != null)
            {
                var apnsCredsSplit = config.ApnsCreds.Replace("\\n", "\n").Split(";");
                apnsCreds = new Microsoft.Azure.Management.NotificationHubs.Models.ApnsCredential
                {
                    KeyId = apnsCredsSplit[0],
                    // Id
                    AppName = apnsCredsSplit[1],
                    // Prefix
                    AppId = apnsCredsSplit[2],
                    Token = apnsCredsSplit[3],
                    Endpoint = "https://api.development.push.apple.com:443/3/device"
                };
            }
            await nhManagemntClient.NotificationHubs.CreateOrUpdateAsync(config.ResourceGroupName, config.NamespaceName, config.HubName, new NotificationHubCreateOrUpdateParameters(config.Location)
            {
                GcmCredential = gcmCreds,
                ApnsCredential = apnsCreds
            });

            Console.WriteLine($"Create NotificationHub {config.HubName}.");
        }

        private static SampleConfiguration LoadConfiguration(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("config.json", true);
            configurationBuilder.AddCommandLine(args);
            var configRoot = configurationBuilder.Build();
            var sampleConfig = new SampleConfiguration();
            configRoot.Bind(sampleConfig);
            return sampleConfig;
        }
    }
}
