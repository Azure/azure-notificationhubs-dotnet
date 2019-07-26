//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    internal static class MinimalApiVersionFor
    {
        public static ApiVersion BaiduSupport
        {
            get { return ApiVersion.Eleven; }
        }

        public static ApiVersion AdmSupport
        {
            get { return ApiVersion.Eight; }
        }

        public static ApiVersion NamespacesWithoutACS
        {
            get { return ApiVersion.Nine; }
        }

        public static ApiVersion InstallationApi
        {
            get { return ApiVersion.Nine; }
        }

        public static ApiVersion DisableNotificationHub
        {
            get { return ApiVersion.Ten; }
        }

        public static ApiVersion PropertyBagSupport
        {
            get { return ApiVersion.Thirteen; }
        }

        public static ApiVersion MessagingPremiumSKU
        {
            get { return ApiVersion.Fourteen; }
        }

        public static ApiVersion ApnsTokenCredential
        {
            get { return ApiVersion.Seventeen; }
        }
    }
}

