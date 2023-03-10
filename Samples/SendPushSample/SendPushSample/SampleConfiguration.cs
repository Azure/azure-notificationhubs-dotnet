// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

namespace SendPushSample
{
    public class SampleConfiguration
    {
        public string PrimaryConnectionString { get; set; }
        public string HubName { get; set; }
        public string Tag {get; set; }
        public string FcmDeviceId {get; set; }
        public string AppleDeviceId {get; set; }
        public string XiaomiDeviceId { get; set; }
        public string SendType {get; set; }
        public string AppleGroupId { get; set; }

        public enum Operation {
            Broadcast, 
            SendByTag,
            SendByDevice
        }
    }
}
