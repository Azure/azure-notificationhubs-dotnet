# Notification Hubs usage in Azure Functions

The goal of these samples is to show how to register devices from a backend, using serverless Azure Functions.

## Building samples

Temporarily, before deploying the function, you will need to add a file named `Parameters.cs` to define your `ConnectionString`. Here is an example:
```csharp
namespace NHubSamples
{
    public static class Parameters{
        public static string ConnectionString = "<your Notification Hubs ConnectionString>";
    }
}
```

## Usage

There are 2 HTTP functions that can be used either as POST or GET. They take their parameters either from the query string or from the request body as a json object.

### RegisterDevices parameters

QueryString example:
`hubName=nhub-ios&token=1234567890abcdef&tags=Football,level2,LoggedIn`

Body example:
```json
{
    "hubName" : "nhub-ios",
    "token" : "1234567890abcdef",
    "tags" : "Football,level2,LoggedIn"
}
```

#### hubName

Mandatory. The name of the Notification Hubs in the namespace referenced by the Connection String mentioned above

#### token

Mandatory. The device identifier provided by the operating system when registering for push notification. This is called token by APNS and FCM or PushChannel by WNS.

#### tags

Optional. A comma separated list of tags to attach to the installation

### ListRegistrations parameters

QueryString example:
`hubName=nhub-ios`

Body example:
```json
{
    "hubName" : "nhub-ios"
}
```

#### hubName

Mandatory. The name of the Notification Hubs in the namespace referenced by the Connection String mentioned above