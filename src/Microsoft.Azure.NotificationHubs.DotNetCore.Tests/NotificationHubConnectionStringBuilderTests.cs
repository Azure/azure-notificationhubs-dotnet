//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Tests
{
    using Xunit;

    public class NotificationHubConnectionStringBuilderTests
    {
        [Fact]
        public void ToStringShouldReturnAllProperties()
        {
            var b = new NotificationHubConnectionStringBuilder("Endpoint=sb://my-namespace.servicebus.windows.net/")
            {
                SharedAccessKeyName = "key-name",
                SharedAccessKey = "secret"
            };
            
            Assert.Equal(
                "Endpoint=sb://my-namespace.servicebus.windows.net/;SharedAccessKeyName=key-name;SharedAccessKey=secret",
                b.ToString());
        }
    }
}
