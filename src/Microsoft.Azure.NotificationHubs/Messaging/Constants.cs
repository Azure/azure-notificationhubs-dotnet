//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    static class Constants
    {
        public const bool DefaultIsAnonymousAccessible = false;
        public static readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan MaxOperationTimeout = TimeSpan.FromDays(1);
        public static readonly TimeSpan TokenRequestOperationTimeout = TimeSpan.FromMinutes(3);
        public const string DefaultOperationTimeoutString = "0.00:01:00.00";
        public static readonly int ServicePointMaxIdleTimeMilliSeconds = 50000; // This should be lower than the server keep-alive connection time ( < 1 min)
        
        public static readonly int MaximumTagSize = 120;
        
        public static readonly int DefaultMaxDeliveryCount = 10;
        public static readonly TimeSpan DefaultRegistrationTtl = TimeSpan.MaxValue;
        public static readonly TimeSpan MinimumRegistrationTtl = TimeSpan.FromDays(1);

        public const string NotificationHub = "NotificationHub";
       
        ////// AuthZ related constants                
        public const string AuthClaimType = "net.windows.servicebus.action";
        public const string ManageClaim = "Manage";
        public const string SendClaim = "Send";
        public const string ListenClaim = "Listen";
        public static List<string> SupportedClaims = new List<string>() { ManageClaim, SendClaim, ListenClaim };
        public const int SupportedClaimsCount = 3;
        public const string ClaimSeparator = ",";

        public const string PathDelimiter = @"/";
        public const string SubQueuePrefix = "$";
        public const string EntityDelimiter = @"|";
        public const string EmptyEntityDelimiter = @"||";
        public const string ColonDelimiter = ":";
        public const string PartDelimiter = ColonDelimiter;
                
        public const string IsAnonymousAccessibleHeader = "X-MS-ISANONYMOUSACCESSIBLE";
        public const string ServiceBusSupplementartyAuthorizationHeaderName = "ServiceBusSupplementaryAuthorization";
        public const string ServiceBusDlqSupplementaryAuthorizationHeaderName = "ServiceBusDlqSupplementaryAuthorization";
        
        public const string ContinuationTokenHeaderName = "x-ms-continuationtoken";
        public const string ContinuationTokenQueryName = "continuationtoken";
               
        public const int MaxJobIdLength = 128;

        public const int NotificationHubNameMaximumLength = 260;

        public const string BrokerInvalidOperationPrefix = "BR0012";

        public const string InternalServiceFault = "InternalServiceFault";
        public const string ConnectionFailedFault = "ConnectionFailedFault";
        public const string EndpointNotFoundFault = "EndpointNotFoundFault";
        public const string AuthorizationFailedFault = "AuthorizationFailedFault";
        public const string NoTransportSecurityFault = "NoTransportSecurityFault";
        public const string QuotaExceededFault = "QuotaExceededFault";
        public const string PartitionNotOwnedFault = "PartitionNotOwnedException";
        public const string UndeterminableExceptionType = "UndeterminableExceptionType";
        public const string InvalidOperationFault = "InvalidOperationFault";
        public const string SessionLockLostFault = "SessionLockLostFault";
        public const string TimeoutFault = "TimeoutFault";
        public const string ArgumentFault = "ArgumentFault";
        public const string MessagingEntityDisabledFault = "MessagingEntityDisabledFault";
        public const string ServerBusyFault = "ServerBusyFault";
        public const int MaximumUserMetadataLength = 1024;
        public const int MaxNotificationHubPathLength = 290;
        public static readonly TimeSpan DefaultRetryMinBackoff = TimeSpan.FromSeconds(0);
        public static readonly TimeSpan DefaultRetryMaxBackoff = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan DefaultRetryDeltaBackoff = TimeSpan.FromSeconds(3);
        public static readonly TimeSpan DefaultRetryTerminationBuffer = TimeSpan.FromSeconds(5);
        public const string HttpErrorSubCodeFormatString = "SubCode={0}";

        // HTTP Constants
        public const string HttpLocationHeaderName = "Location";
        public const string HttpUserAgentHeaderName = "User-Agent";
        public const string HttpTrackingIdHeaderName = "TrackingId";

        /// <summary>
        /// The default value for <see cref="NotificationHubDescription.IsDisabled"/>
        /// </summary>
        public const bool DefaultHubIsDisabled = false;

        public const string PublicKeyPhrase = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae";
        public const string PublicTestKeyPhrase = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae";

    }
}
