[![NuGet](https://img.shields.io/nuget/v/Microsoft.Azure.NotificationHubs.svg)](https://www.nuget.org/packages/Microsoft.Azure.NotificationHubs/)

# .NET Client for Azure Notification Hubs

This repository contains source code for Azure Notification Hubs .NET SDK as well as samples for using the client.  This library is also available via NuGet as part of [`Microsoft.Azure.NotificationHubs`](https://www.nuget.org/packages/Microsoft.Azure.NotificationHubs/).

## Building Code

To build the `Microsoft.Azure.NotificationHubs`, you need support for [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).  This requires a minimum of .NET Core 3.1, .NET Framework 4.6.2 or Mono 5.4.  This project ships with two unit test files, one for .NET Core 6.0, and one for .NET Framework 4.6.2 or Mono. This library ships binaries for .NET Standard 2.0, .NET Standard 2.1 and .NET 6.0.

## Getting Started

To get started, you can find all the classes in the `Microsoft.Azure.NotificationHubs` namespace, for example:

```csharp
using Microsoft.Azure.NotificationHubs;
```

The Azure Notification Hubs SDK for .NET support both synchronous and asynchronous operations on `NotificationHubClient` and `/NamespaceManagerClient`.  The asynchronous APIs have the `Async` suffix returning `Task` objects.

```csharp
// Synchronous
var hub = new NotificationHubDescription("hubname");
hub.WnsCredential = new WnsCredential("sid","key");
hub = namespaceManager.CreateNotificationHub(hub);

// Asynchronous
var hub = new NotificationHubDescription("hubname");
hub.WnsCredential = new WnsCredential("sid","key");
hub = await namespaceManager.CreateNotificationHubAsync(hub, CancellationToken.None);
```

## Azure Notification Hubs Management Operations

This section details the usage of the Azure Notification Hubs SDK for .NET management operations for CRUD operations on Notification Hubs and Notification Hub Namespaces.

### Create a namespace manager

```csharp
var namespaceManager = new NamespaceManagerClient("connection string");
```

### Create an Azure Notification Hub

```csharp
var hub = new NotificationHubDescription("hubname");
hub.WnsCredential = new WnsCredential("sid","key");
hub = await namespaceManager.CreateNotificationHubAsync(hub);
```

### Get a Azure Notification Hub

```csharp
var hub = await namespaceManager.GetNotificationHubAsync("hubname", CancellationToken.None);
```

### Update an Azure Notification Hub

```csharp
hub.FcmCredential = new FcmCredential("key");
hub = await namespaceManager.UpdateNotificationHubAsync(hub, CancellationToken.None);
```

### Delete an Azure Notification Hub

```csharp
await namespaceManager.DeleteNotificationHubAsync("hubname", CancellationToken.None);
```

## Azure Notification Hubs Operations

The `NotificationHubClient` class and `INotificationHubClient` interface is the main entry point for installations/registrations, but also sending push notifications.  To create a `NotificationHubClient`, you need the connection string from your Access Policy with the desired permissions such as `Listen`, `Manage` and `Send`, and in addition, the hub name to use.

```csharp
INotificationHubClient hub = new NotificationHubClient("connection string", "hubname");
```

## Azure Notification Hubs Installation API

An Installation is an enhanced registration that includes a bag of push related properties. It is the latest and best approach to registering your devices.

The following are some key advantages to using installations:

- Creating or updating an installation is fully idempotent. So you can retry it without any concerns about duplicate registrations.
- The installation model supports a special tag format `($InstallationId:{INSTALLATION_ID})` that enables sending a notification directly to the specific device. For example, if the app's code sets an installation ID of `joe93developer` for this particular device, a developer can target this device when sending a notification to the `$InstallationId:{joe93developer}` tag. This enables you to target a specific device without having to do any additional coding.
- Using installations also enables you to do partial registration updates. The partial update of an installation is requested with a PATCH method using the JSON-Patch standard. This is useful when you want to update tags on the registration. You don't have to pull down the entire registration and then resend all the previous tags again.

Using this SDK, you can do these Installation API operations.  For example, we can create an installation for an Amazon Kindle Fire using the `Installation` class.

```csharp
var installation = new Installation
{
    InstallationId = "installation-id",
    PushChannel = "adm-push-channel",
    Platform = NotificationPlatform.Adm
};
await hub.CreateOrUpdateInstallationAsync(installation);
```

Alternatively, we can use specific installation classes per type for example `AdmInstallation` for Amazon Kindle Fire devices.

```csharp
var installation = new AdmInstallation("installation-id", "adm-push-channel");
await hub.CreateOrUpdateInstallationAsync(installation);
```

An installation can have multiple tags and multiple templates with its own set of tags and headers.

```csharp
installation.Tags = new List<string> { "foo" };
installation.Templates = new Dictionary<string, InstallationTemplate>
{
    { "template1", new InstallationTemplate { Body = "{\"data\":{\"key1\":\"$(value1)\"}}" } },
    { "template2", new InstallationTemplate { Body = "{\"data\":{\"key2\":\"$(value2)\"}}" } }
};
await hub.CreateOrUpdateInstallationAsync(installation);
```

For advanced scenarios we have partial update capability which allows to modify only particular properties of the installation object. Basically partial update is subset of [JSON Patch](https://tools.ietf.org/html/rfc6902/) operations you can run against Installation object.

```csharp
var addChannel = new PartialUpdateOperation
{ 
    Operation = UpdateOperationType.Add, ,
    Path = "/pushChannel", 
    Value = "adm-push-channel2"
};
var addTag = new PartialUpdateOperation
{
    Operation = UpdateOperationType.Add, 
    Path = "/tags", 
    Value = "bar"
};
var replaceTemplate = new PartialUpdateOperation
{
    Operation = UpdateOperationType.Replace, 
    Path = "/templates/template1",
    Value = new InstallationTemplate { Body = "{\"data\":{\"key3\":\"$(value3)\"}}" }.ToJson()
};
await hub.PatchInstallationAsync(
    "installation-id", 
    new List<PartialUpdateOperation> { addChannel, addTag, replaceTemplate }
);
```

### Delete an Installation

```csharp
await hub.DeleteinstallationAsync("installation-id");
```

Keep in mind that `CreateOrUpdateInstallationAsync`, `PatchInstallationAsync` and `DeleteInstallationAsync` are eventually consistent with `GetInstallationAsync`. In fact operation just goes to the system queue during the call and will be executed in background. Moreover Get is not designed for main runtime scenario but just for debug and troubleshooting purposes, it is tightly throttled by the service.

## Azure Notification Hub Registration API

A registration associates the Platform Notification Service (PNS) handle for a device with tags and possibly a template. The PNS handle could be a ChannelURI, device token, or FCM registration ID. Tags are used to route notifications to the correct set of device handles. Templates are used to implement per-registration transformation.  The Registration API handles requests for these operations.

### Create a Windows Registration

```csharp
var tags = new HashSet<string> { "platform_uwp", "os_windows10" };
var channelUri = new Uri("https://notify.windows.net/mychannel");
WindowsRegistrationDescription created = await hub.CreateWindowsNativeRegistrationAsync(channelUri, tags);
```

### Create an Apple Registration

```java
var deviceToken = "device-token";
var tags = new HashSet<string> { "platform_ios", "os_tvos" };
AppleRegistrationDescription created = await hub.CreateAppleNativeRegistrationAsync(deviceToken, tags);
```

Analogous for Android (FCM), Windows Phone (MPNS), and Kindle Fire (ADM).

### Create Template Registrations

```csharp
var deviceToken = "device-token";
var jsonBody = "{\"aps\": {\"alert\": \"$(message)\"}}";
AppleTemplateRegistrationDescription created = await hub.CreateAppleTemplateRegistrationAsync(deviceToken, jsonBody);
```

Create registrations using create registrationid+upsert pattern (removes duplicates deriving from lost responses if registration ids are stored on the device):

```csharp
var deviceToken = "device-token";
var registrationId = await hub.CreateRegistrationIdAsync();
var jsonBody = "{\"aps\": {\"alert\": \"$(message)\"}}";
var reg = new AppleTemplateRegistrationDescription(deviceToken, jsonBody) { RegistrationId = registrationId };
AppleTemplateRegistrationDescription upserted = await hub.CreateOrUpdateRegistrationAsync(reg);
```

### Update a Registration

```csharp
await hub.UpdateRegistrationAsync(reg);
```

### Delete a Registration

```csharp
await hub.DeleteRegistrationAsync(registrationId);
```

### Get a Single Registration

```csharp
AppleRegistrationDescription registration = hub.GetRegistrationAsync(registrationId);
```

### Get Registrations With a Given Tag

This query support $top and continuation tokens.

```csharp
var registrations = await hub.GetRegistrationsByTagAsync("platform_ios");
```

### Get Registrations By Channel

This query support $top and continuation tokens.

```csharp
var registrations = await hub.GetRegistrationsByChannelAsync("devicetoken");
```

## Send Notifications

The Notification object is simply a body with headers, some utility methods help in building the native and template notifications objects.

### Send an Apple Push Notification

```csharp
var jsonBody = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
var n = new AppleNotification(jsonBody);
NotificationOutcome outcome = await hub.SendNotificationAsync(n);
```

Analogous for Android, Windows, Windows Phone, Kindle Fire and Baidu PNS.

### Send a Template Notification

```csharp
var props =  new Dictionary<string, string>
{
    { "prop1", "v1" },
    { "prop2", "v2" }
};
var n = new TemplateNotification(props);
NotificationOutcome outcome = hub.SendNotificationAsync(n);
```

### Send To An Installation ID

Send flow for Installations is the same as for Registrations. We've just introduced an option to target notification to the particular Installation - just use tag "$InstallationId:{desired-id}". For case above it would look like this:

```csharp
var jsonBody = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
var n = new AppleNotification(jsonBody);
var tags = new List<string> { "$InstallationId:{installation-id}" };
NotificationOutcome outcome = await hub.SendNotificationAsync(n, tags);
```

### Send to a User ID

With the [Installation API](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#installations) we now have a new feature that allows you to associate a user ID with an installation and then be able to target it with a send to all devices for that user.  To set the user ID for the installation, set the `UserId` property of the `Installation`.

```csharp
var installation = new AppleInstallation("installation-id", "device-token");
installation.UserId = "user1234";

await hub.CreateOrUpdateInstallationAsync(installation);
```

The user can then be targeted to send a notification with the tag format of `$UserId:{USER_ID}`, for example like the following:

```csharp
var jsonPayload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
var n = new AppleNotification(jsonPayload);
NotificationOutcome outcome = await hub.SendNotificationAsync(n, "$UserId:user1234");
```

### Send To An Installation Template For An Installation

```csharp
var props = new Dictionary<string, string>
{
    { "value3", "some value" }
};
var n = new TemplateNotification(prop);
NotificationOutcome outcome = await hub.SendNotificationAsync(n, "$InstallationId:{installation-id} && tag-for-template1");
```

## Scheduled Send Operations

**Note: This feature is only available for [STANDARD Tier](http://azure.microsoft.com/en-us/pricing/details/notification-hubs/).**

Scheduled send operations are similar to a normal send operations, with a scheduledTime parameter which says when notification should be delivered. The Azure Notification Hubs Service accepts any point of time between now + 5 minutes and now + 7 days.

### Schedule Apple Native Send Operation

```csharp
var scheduledDate = DateTimeOffset.UtcNow.AddHours(12);

var jsonPayload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
var n = new AppleNotification(jsonPayload);

ScheduledNotification outcome = await hub.ScheduleNotificationAsync(n, scheduledDate);
```

### Cancel Scheduled Notification

```csharp
await hub.CancelNotificationAsync(outcome.ScheduledNotificationId);
```

## Import and Export Registrations

**Note: This feature is only available for [STANDARD Tier](http://azure.microsoft.com/en-us/pricing/details/notification-hubs/).**

Sometimes it is required to perform bulk operation against registrations. Usually it is for integration with another system or just to update the tags. It is strongly not recommended to use Get/Update flow if you are modifying thousands of registrations. Import/Export capability is designed to cover the scenario. You provide an access to some blob container under your storage account as a source of incoming data and location for output.

### Submit an Export Job

```csharp
var job = new NotificationHubJob
{
    JobType = NotificationHubJobType.ExportRegistrations,
    OutputContainerUri = new Uri("container uri with SAS signature"),
};

job = await hub.SubmitNotificationHubJobAsync(job);
```

### Submit an Import Job

```csharp
var job = new NotificationHubJob
{
    JobType = NotificationHubJobType.ImportCreateRegistrations,
    ImportFileUri = new Uri("input file uri with SAS signature"),
    OutputContainerUri = new Uri("container uri with SAS signature")
};

job = await hub.SubmitNotificationHubJobAsync(job);

```

### Wait for Job Completion

```csharp
while (true) {
    await Task.Delay(1000);
    job = await hub.GetNotificationHubJobAsync(job.JobId);
    if (job.Status == NotificationHubJobStatus.Completed) {
        break;
    }
}
```

### Get All jobs

```csharp
var allJobs = await hub.GetNotificationHubJobsAsync()
```

## References

[Microsoft Azure Notification Hubs Docs](https://docs.microsoft.com/en-us/azure/notification-hubs/)

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
