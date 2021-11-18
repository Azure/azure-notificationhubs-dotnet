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

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
