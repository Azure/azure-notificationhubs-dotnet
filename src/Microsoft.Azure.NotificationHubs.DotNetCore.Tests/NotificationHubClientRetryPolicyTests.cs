//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using RichardSzalay.MockHttp;
using Xunit;

namespace Microsoft.Azure.NotificationHubs.DotNetCore.Tests
{
    public class NotificationHubClientRetryPolicyTests
    {
        private readonly string _connectionString;
        private readonly string _hubName;
        private NotificationHubClient _nhClient;
        private MockHttpMessageHandler _mockHttp;

        public NotificationHubClientRetryPolicyTests()
        {
            _connectionString = "Endpoint=sb://sample.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=xxxxxx";
            _hubName = "hub-name";
            _mockHttp = new MockHttpMessageHandler();
            _nhClient = new NotificationHubClient(_connectionString, _hubName, new NotificationHubSettings
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
        public async Task RetryPolicyRetriesOnTransientErrorInSend(HttpStatusCode errorCode)
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(errorCode);
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(HttpStatusCode.OK);

            await _nhClient.SendDirectNotificationAsync(new FcmNotification("{}"), "123");

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        public async Task RetryPolicyRetriesOnTransientErrorInRegister(HttpStatusCode errorCode)
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/registrations")
                    .Respond(errorCode);
            var registrationXml = "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:a=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>https://sample.servicebus.windows.net/hub-name/registrations/123456?api-version=2017-04</id><title type=\"text\">4757098718499783238-6462592605842469809-1</title><published>2019-05-13T17:12:18Z</published><updated>2019-05-13T17:12:18Z</updated><link rel=\"self\" href=\"https://sdk-sample-namespace.servicebus.windows.net/sdk-sample-nh/registrations/4757098718499783238-6462592605842469809-1?api-version=2017-04\"/><content type=\"application/xml\"><GcmRegistrationDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><ETag>2</ETag><ExpirationTime>9999-12-31T23:59:59.999</ExpirationTime><RegistrationId>4757098718499783238-6462592605842469809-1</RegistrationId><Tags>tag2</Tags><GcmRegistrationId>amzn1.adm-registration.v2.123</GcmRegistrationId></GcmRegistrationDescription></content></entry>";
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/registrations")
                    .Respond("application/atom+xml", registrationXml);

            var registration = await _nhClient.CreateFcmNativeRegistrationAsync("123456");

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RetryPolicyRetriesConnectionErrors()
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Throw(new TimeoutException());
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Throw(new HttpRequestException("", new SocketException((int)SocketError.TimedOut)));
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(HttpStatusCode.OK);

            await _nhClient.SendDirectNotificationAsync(new FcmNotification("{}"), "123");

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RetryPolicyRetriesOnThrottling()
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond((HttpStatusCode)403, new Dictionary<string, string> { { "Retry-After", "1" }}, new StringContent(""));
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond((HttpStatusCode)429, new Dictionary<string, string> { { "Retry-After", "1" } }, new StringContent(""));
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(HttpStatusCode.OK);

            await _nhClient.SendDirectNotificationAsync(new FcmNotification("{}"), "123");

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task RetryPolicyRethrowsNonTransientErrors()
        {
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(HttpStatusCode.NotFound);
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(HttpStatusCode.OK);

            await Assert.ThrowsAsync<MessagingEntityNotFoundException>(() => _nhClient.SendDirectNotificationAsync(new FcmNotification("{}"), "123"));
        }

        [Fact]
        public async Task RetryPolicyGivesUpAfterTimeout()
        {
            _nhClient = new NotificationHubClient(_connectionString, _hubName, new NotificationHubSettings
            {
                MessageHandler = _mockHttp,
                RetryOptions = new NotificationHubRetryOptions
                {
                    Delay = TimeSpan.FromMilliseconds(10),
                    MaxRetries = 1
                }
            });

            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Throw(new TimeoutException());
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Throw(new TimeoutException());
            _mockHttp.Expect("https://sample.servicebus.windows.net/hub-name/messages")
                    .Respond(HttpStatusCode.OK);

            await Assert.ThrowsAsync<TimeoutException>(() => _nhClient.SendDirectNotificationAsync(new FcmNotification("{}"), "123"));
        }
    }
}
