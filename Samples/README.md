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
Sample UWP application for sending push notifications 

Create UwpSample\~Secrets.cs file with the following body:

```
namespace UwpSample
{
    public class Secrets
    {
        public const string HubName = "<hub name>";
        public const string HubConnectionString = "<connection string>";
    }
}
```

To run the sample open the solution, select UwpSample as a startup project, and hit F5.

