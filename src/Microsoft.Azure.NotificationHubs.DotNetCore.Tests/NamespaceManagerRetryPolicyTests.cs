//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.DotNetCore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Microsoft.Azure.NotificationHubs.Messaging;
    using RichardSzalay.MockHttp;
    using Xunit;

    public class NamespaceManagerRetryPolicyTests
    {
        private readonly string _connectionString;
        private readonly string _hubName;
        private NamespaceManager _namespaceClient;
        private MockHttpMessageHandler _mockHttp;
        private const string _hubResponse = "<entry xmlns=\"http://www.w3.org/2005/Atom\"><id>https://sample.servicebus.windows.net/sample?api-version=2017-04</id><title type=\"text\">sample</title><published>2020-07-02T18:03:10Z</published><updated>2020-07-02T18:03:11Z</updated><author><name>sample</name></author><link rel=\"self\" href=\"https://sample.servicebus.windows.net/sample?api-version=2017-04\"/><content type=\"application/xml\"><NotificationHubDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><RegistrationTtl>P10675199DT2H48M5.4775807S</RegistrationTtl><AuthorizationRules><AuthorizationRule i:type=\"SharedAccessAuthorizationRule\"><ClaimType>SharedAccessKey</ClaimType><ClaimValue>None</ClaimValue><Rights><AccessRights>Listen</AccessRights></Rights><CreatedTime>2020-07-02T18:03:10.772227Z</CreatedTime><ModifiedTime>2020-07-02T18:03:10.772227Z</ModifiedTime><KeyName>DefaultListenSharedAccessSignature</KeyName><PrimaryKey>xxxx</PrimaryKey><SecondaryKey>xxxx</SecondaryKey></AuthorizationRule><AuthorizationRule i:type=\"SharedAccessAuthorizationRule\"><ClaimType>SharedAccessKey</ClaimType><ClaimValue>None</ClaimValue><Rights><AccessRights>Listen</AccessRights><AccessRights>Manage</AccessRights><AccessRights>Send</AccessRights></Rights><CreatedTime>2020-07-02T18:03:10.772227Z</CreatedTime><ModifiedTime>2020-07-02T18:03:10.772227Z</ModifiedTime><KeyName>DefaultFullSharedAccessSignature</KeyName><PrimaryKey>xxxx</PrimaryKey><SecondaryKey>xxxx</SecondaryKey></AuthorizationRule></AuthorizationRules></NotificationHubDescription></content></entry>";

        public NamespaceManagerRetryPolicyTests()
        {
            _connectionString = "Endpoint=sb://sample.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=xxxxxx";
            _hubName = "hub-name";
            _mockHttp = new MockHttpMessageHandler();
            _namespaceClient = new NamespaceManager(_connectionString, new NotificationHubSettings
            {
                HttpClient = _mockHttp.ToHttpClient(),
                RetryOptions = new NotificationHubRetryOptions
                {
                    Delay = TimeSpan.FromMilliseconds(10)
                }
            });
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        public async Task RetryPolicyRetriesOnTransientErrorInPut(HttpStatusCode errorCode)
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(errorCode);
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(HttpStatusCode.OK, "application/xml", _hubResponse);

            await _namespaceClient.CreateNotificationHubAsync(_hubName);

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RetryPolicyRetriesConnectionErrors()
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Throw(new TimeoutException());
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Throw(new HttpRequestException("", new SocketException((int)SocketError.TimedOut)));
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(HttpStatusCode.OK, "application/xml", _hubResponse);

            await _namespaceClient.CreateNotificationHubAsync(_hubName);

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RetryPolicyRetriesOnThrottling()
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond((HttpStatusCode)403, new Dictionary<string, string> { { "Retry-After", "1" }}, new StringContent(""));
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond((HttpStatusCode)429, new Dictionary<string, string> { { "Retry-After", "1" } }, new StringContent(""));
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(HttpStatusCode.OK, "application/xml", _hubResponse);

            await _namespaceClient.CreateNotificationHubAsync(_hubName);

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RetryPolicyRethrowsNonTransientErrors()
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(HttpStatusCode.NotFound);
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(HttpStatusCode.OK, "application/xml", _hubResponse);

            await Assert.ThrowsAsync<MessagingEntityNotFoundException>(() => _namespaceClient.CreateNotificationHubAsync(_hubName));
        }

        [Fact]
        public async Task RetryPolicyGivesUpAfterTimeout()
        {
            _namespaceClient = new NamespaceManager(_connectionString, new NotificationHubSettings
            {
                MessageHandler = _mockHttp,
                RetryOptions = new NotificationHubRetryOptions
                {
                    Delay = TimeSpan.FromMilliseconds(10),
                    MaxRetries = 1
                }
            });

            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Throw(new TimeoutException());
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Throw(new TimeoutException());
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name")
                    .Respond(HttpStatusCode.OK, "application/xml", _hubResponse);

            await Assert.ThrowsAsync<TimeoutException>(() => _namespaceClient.CreateNotificationHubAsync(_hubName));
        }
    }
}
