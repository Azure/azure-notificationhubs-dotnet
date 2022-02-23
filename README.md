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
NotificationHubDescription hub = await namespaceManager.GetNotificationHubAsync("hubname", CancellationToken.None);
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

Using this SDK, you can do these Installation API operations.  For example, we can create an installation for an Amazon Kindle Fire.

```java
AdmInstallation installation = new AdmInstallation("installation-id", "adm-push-channel");
hub.createOrUpdateInstallation(installation);
```

An installation can have multiple tags and multiple templates with its own set of tags and headers.

```java
installation.addTag("foo");
installation.addTemplate("template1", new InstallationTemplate("{\"data\":{\"key1\":\"$(value1)\"}}","tag-for-template1"));
installation.addTemplate("template2", new InstallationTemplate("{\"data\":{\"key2\":\"$(value2)\"}}","tag-for-template2"));
hub.createOrUpdateInstallation(installation);
```

For advanced scenarios we have partial update capability which allows to modify only particular properties of the installation object. Basically partial update is subset of [JSON Patch](https://tools.ietf.org/html/rfc6902/) operations you can run against Installation object.

```java
PartialUpdateOperation addChannel = new PartialUpdateOperation(UpdateOperationType.Add, "/pushChannel", "adm-push-channel2");
PartialUpdateOperation addTag = new PartialUpdateOperation(UpdateOperationType.Add, "/tags", "bar");
PartialUpdateOperation replaceTemplate = new PartialUpdateOperation(UpdateOperationType.Replace, "/templates/template1", new InstallationTemplate("{\"data\":{\"key3\":\"$(value3)\"}}","tag-for-template1")).toJson());
hub.patchInstallation("installation-id", addChannel, addTag, replaceTemplate);
```

**Create an Azure Notification Hub Client:**

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
