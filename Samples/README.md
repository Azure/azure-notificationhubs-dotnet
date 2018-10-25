# Samples 
## Building Samples
These samples require [.NET Core tooling](https://www.microsoft.com/net/download).

```
cd <SampleName>
dotnet build
```

## CreateHubSample
Samples CLI for creating Azure Notification Hubs using Management SDK (Microsoft.Azure.Management.NotificationHubs).

To run the sample update the `config.json` file and execute the sample as such:

```
cd CreateHubSample
dotnet run [--gcmCreds "<GCM credentials>" --apnsCreds "<APNS Credentials>"]
```

## SendPushSample
Samples CLI for sending push notifications and reading notification feedback.

To run the sample update the `config.json` file and execute the sample as such:

```
cd SendPushSample
dotnet run --primaryConnectionString "primaryConnectionString"
```