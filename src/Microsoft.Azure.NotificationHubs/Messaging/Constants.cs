//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    static class Constants
    {
        public static readonly int MaximumTagSize = 120;
        public static readonly int DefaultMaxDeliveryCount = 10;
        public static readonly TimeSpan DefaultRegistrationTtl = TimeSpan.MaxValue;
        public static readonly TimeSpan MinimumRegistrationTtl = TimeSpan.FromDays(1);

        public const string NotificationHub = "NotificationHub";
        public const string PathDelimiter = @"/";
        public const string SubQueuePrefix = "$";
        public const string EntityDelimiter = @"|";
        public const string EmptyEntityDelimiter = @"||";
        public const string ColonDelimiter = ":";
        public const string PartDelimiter = ColonDelimiter;

        public const string ContinuationTokenHeaderName = "x-ms-continuationtoken";
        public const string ContinuationTokenQueryName = "continuationtoken";
        public const int NotificationHubNameMaximumLength = 260;
        public const string HttpErrorSubCodeFormatString = "SubCode={0}";

        // HTTP Constants
        public const string HttpLocationHeaderName = "Location";
        public const string HttpUserAgentHeaderName = "User-Agent";
        public const string HttpTrackingIdHeaderName = "TrackingId";
    }
}
