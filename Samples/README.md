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
dotnet run --primaryConnectionString "primaryConnectionString" --sendType "[broadcast|sendByTag|sendByDevice]" [--gcmDeviceId "id"] [--appleDeviceId "id"] [--tag "tag"]
```

## ParseFeedbackSample
Samples CLI for sending push notifications and parsing per message telemetry.

To run the sample update the `config.json` file and execute the sample as such:

```
cd ParseFeedbackSample
dotnet run --primaryConnectionString "primaryConnectionString"
```

## RegistrationSample
Samples CLI for adding and deleting registrations and installations. 

To run the sample update the `config.json` file and execute the sample as such:

```
cd RegistrationSample
dotnet run --primaryConnectionString "primaryConnectionString"
```

## UWPSample
Sa,ple UWP application for sending push notifications 

Create Secrets.cs file with the following properties
public const string HubName = "<hub name>";
public const string HubConnectionString = "<connection string>"

```
cd UWPSample
Hit F5
```