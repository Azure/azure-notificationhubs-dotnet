//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions.Configuration;

    public class TestServerProxy : HttpClientHandler
    {
        private const string ServerUriKey = "##ServerUri##";
        private ConcurrentQueue<ProxyResponsePayload> _history = new ConcurrentQueue<ProxyResponsePayload>();
        private ConcurrentQueue<Guid> _guids = new ConcurrentQueue<Guid>();
        public TestServerSession Session { get => new TestServerSession { Guids = _guids.ToArray(), HttpCalls = _history.ToArray() }; }
        public bool RecordingMode { get; set; } = false;
        public string BaseUri { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (RecordingMode)
            {
                var result = await base.SendAsync(request, cancellationToken);
                if (BaseUri == null)
                {
                    BaseUri = $"{result.RequestMessage.RequestUri}://{result.RequestMessage.RequestUri.Host}";
                }
                var responsePayload = new ProxyResponsePayload();
                responsePayload.StatusCode = result.StatusCode;
                responsePayload.RequestUriPath = result.RequestMessage.RequestUri.PathAndQuery;
                foreach (var header in result.Headers)
                {
                    responsePayload.Headers[header.Key] = string.Join(",", header.Value);
                }
                if (result.Content != null)
                {
                    foreach (var header in result.Content.Headers)
                    {
                        responsePayload.ContentHeaders[header.Key] = string.Join(",", header.Value);
                    }
                    var responseBody = await result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        responsePayload.Content = responseBody.Replace(BaseUri, ServerUriKey);
                    }
                }
                _history.Enqueue(responsePayload);
                return result;
            }
            else
            {
                var httpResponse = new HttpResponseMessage();
                if (_history.TryDequeue(out var payload))
                {
                    httpResponse.StatusCode = payload.StatusCode;
                    httpResponse.RequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{BaseUri}/{payload.RequestUriPath}");
                    foreach (var header in payload.Headers)
                    {
                        httpResponse.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    if (payload.Content != null)
                    {
                        httpResponse.Content = new StringContent(payload.Content.Replace(ServerUriKey, BaseUri));
                    }
                    else
                    {
                        httpResponse.Content = new StringContent("");
                    }
                    foreach (var header in payload.ContentHeaders)
                    {
                        httpResponse.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
                return httpResponse;
            }
        }

        public Guid NewGuid()
        {
            if (RecordingMode)
            {
                var g = Guid.NewGuid();
                _guids.Enqueue(g);
                return g;
            }
            else
            {
                if (_guids.TryDequeue(out var g))
                {
                    return g;
                }
                return Guid.NewGuid();
            }
        }

        public void LoadResponses(TestServerSession session)
        {
            foreach (var response in session.HttpCalls)
            {
                _history.Enqueue(response);
            }
            foreach (var guid in session.Guids)
            {
                _guids.Enqueue(guid);
            }
        }
    }

    public class TestServerSession
    {
        public ProxyResponsePayload[] HttpCalls { get; set; }
        public Guid[] Guids { get; set; }
    }

    public class ProxyResponsePayload
        {
            public string RequestUriPath { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
            public Dictionary<string, string> ContentHeaders { get; set; } = new Dictionary<string, string>();
            public string Content { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.AppendLine($"Status: {StatusCode}");
                sb.AppendLine($"URI: {RequestUriPath}");
                foreach (var header in Headers)
                {
                    sb.AppendLine($"Header '{header.Key}': {string.Join(",", header.Value)}");
                }
                if (!string.IsNullOrEmpty(Content))
                {
                    foreach (var header in ContentHeaders)
                    {
                        sb.AppendLine($"Content header '{header.Key}': {string.Join(",", header.Value)}");
                    }
                    sb.AppendLine("-----------------------");
                    sb.AppendLine(Content);
                }

                return sb.ToString();
            }
        }
}
