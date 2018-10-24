using System;

namespace BasicSample
{
    public class SampleConfiguration
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; } = "dotnet-sdk-sampl";
        public string Location { get; set; } = "West US";
        public string NamespaceName { get; set; }
        public string HubName { get; set; }
        public string GcmCreds { get; set; }
        public string ApnsCreds { get; set; }
    }
}