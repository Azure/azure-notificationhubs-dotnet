using System;

namespace SendPushSample
{
    public class SampleConfiguration
    {
        public string PrimaryConnectionString { get; set; }
        public string HubName { get; set; }
        public string Tag {get; set; }
        public string DeviceId {get; set; }
        public Operation SendType {get; set; }

        public enum Operation {
            Broadcast, 
            SendByTag,
            SendByDevice
        }
    }
}