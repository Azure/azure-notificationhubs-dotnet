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

namespace BasicSample
{
    class Program
    {

        private const string GcmSampleNotificationContent = "{\"data\":{\"message\":\"Notification Hub test notification from SDK sample\"}}";
        private const string AppleSampleNotificationContent = "{\"aps\":{\"alert\":\"Notification Hub test notification from SDK sample\"}}";

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
                var apnsCredsSplit = config.ApnsCreds.Replace("\\n","\n").Split(";");
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

            // Getting connection key from the new resource
            var connectionString = await nhManagemntClient.NotificationHubs.ListKeysAsync(config.ResourceGroupName, config.NamespaceName, config.HubName, authorizationRule);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(connectionString.PrimaryConnectionString, config.HubName);

            // Register some fake devices
            var gcmDeviceId = Guid.NewGuid().ToString();
            var gcmInstallation = new Installation
            {
                InstallationId = "fake-gcm-install-id",
                Platform = NotificationPlatform.Gcm,
                PushChannel = gcmDeviceId,
                PushChannelExpired = false,
                Tags = new [] { "gcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(gcmInstallation);
            
            var appleDeviceId = "00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0";
            var apnsInstallation = new Installation
            {
                InstallationId = "fake-apns-install-id",
                Platform = NotificationPlatform.Apns,
                PushChannel = appleDeviceId,
                PushChannelExpired = false,
                Tags = new [] { "apns" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(apnsInstallation);

            // Send notifications to all users
            var outcomeGcm = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent);
            var outcomeApns = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent);

            var outcomeGcmByTag = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent, "gcm");
            var outcomeApnsByTag = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent, "apns");

            var outcomeGcmByDeviceId = await nhClient.SendDirectNotificationAsync(CreateGcmNotification(), gcmDeviceId);
            var outcomeApnsByDeviceId = await nhClient.SendDirectNotificationAsync(CreateApnsNotification(), appleDeviceId);

            var gcmOutcomeDetails = await WaitForThePushStatusAsync("GCM", nhClient, outcomeGcm);
            var apnsOutcomeDetails = await WaitForThePushStatusAsync("APNS", nhClient, outcomeApns);
            var gcmTagOutcomeDetails = await WaitForThePushStatusAsync("GCM Tags", nhClient, outcomeGcmByTag);
            var apnsTagOutcomeDetails = await WaitForThePushStatusAsync("APNS Tags", nhClient, outcomeApnsByTag);
            var gcmDirectSendOutcomeDetails = await WaitForThePushStatusAsync("GCM direct", nhClient, outcomeGcmByDeviceId);
            var apnsDirectSendOutcomeDetails = await WaitForThePushStatusAsync("APNS direct", nhClient, outcomeApnsByDeviceId);

            PrintPushOutcome("GCM", gcmOutcomeDetails, gcmOutcomeDetails.GcmOutcomeCounts);
            PrintPushOutcome("APNS", apnsOutcomeDetails, apnsOutcomeDetails.ApnsOutcomeCounts);
            PrintPushOutcome("GCM Tags", gcmTagOutcomeDetails, gcmTagOutcomeDetails.GcmOutcomeCounts);
            PrintPushOutcome("APNS Tags", apnsTagOutcomeDetails, apnsTagOutcomeDetails.ApnsOutcomeCounts);
            PrintPushOutcome("GCM Direct", gcmDirectSendOutcomeDetails, gcmDirectSendOutcomeDetails.ApnsOutcomeCounts);
            PrintPushOutcome("APNS Direct", apnsDirectSendOutcomeDetails, apnsDirectSendOutcomeDetails.ApnsOutcomeCounts);
            Console.WriteLine("Hello World!");
        }

        private static Notification CreateGcmNotification()
        {
            return new GcmNotification(GcmSampleNotificationContent);
        }

        private static Notification CreateApnsNotification()
        {
            return new AppleNotification(AppleSampleNotificationContent);
        } 

        private static async Task<NotificationDetails> WaitForThePushStatusAsync(string pnsType, NotificationHubClient nhClient, NotificationOutcome notificationOutcome) 
        {
            var notificationId = notificationOutcome.NotificationId;
            NotificationDetails outcomeDetails = await nhClient.GetNotificationOutcomeDetailsAsync(notificationId);
            var state = outcomeDetails.State;
            var count = 0;
            while ((state == NotificationOutcomeState.Enqueued || state == NotificationOutcomeState.Processing) && ++count < 10)
            {
                Console.WriteLine($"{pnsType} status: {state}");
                outcomeDetails = await nhClient.GetNotificationOutcomeDetailsAsync(notificationId);
                state = outcomeDetails.State;
                Thread.Sleep(1000);                                                
            }
            return outcomeDetails;
        }

        private static void PrintPushOutcome(string pnsType, NotificationDetails details, NotificationOutcomeCollection collection)
        {
            if (collection != null) 
            {
                Console.WriteLine($"{pnsType} outcome: " + string.Join(",", collection.Select(kv => $"{kv.Key}:{kv.Value}")));    
            }
            else 
            {
                Console.WriteLine($"{pnsType} no outcomes.");
            }
            Console.WriteLine($"{pnsType} error details URL: {details.PnsErrorDetailsUri}");
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
