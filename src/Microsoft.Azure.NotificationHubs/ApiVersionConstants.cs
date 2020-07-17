//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System.Diagnostics.CodeAnalysis;

    class ApiVersionConstants
    {
        public const string Name = "api-version";
        public const string VersionTwo = "2012-03";
        public const string VersionThree = "2012-08";
        public const string VersionFour = "2013-04";
        public const string VersionFive = "2013-07";
        public const string VersionSix = "2013-08";
        public const string VersionSeven = "2013-10";
        public const string VersionEight = "2014-01";
        public const string VersionNine = "2014-05";
        public const string VersionTen = "2014-08";
        public const string VersionEleven = "2014-09";
        public const string VersionTwelve = "2015-01";
        public const string VersionThirteen = "2015-04";
        public const string VersionFourteen = "2015-08";
        public const string VersionFifteen = "2016-03";
        public const string VersionSixteen = "2016-07";
        public const string VersionSeventeen = "2017-04";
        public const string VersionEighteen = "2020-06";
        public const string MaxSupportedApiVersion = VersionEighteen;

        public const string OldRuntimeVersion = VersionFive; // Upto this version runtime operations did not include version
        public const string PartitionedEntityMinimumRuntimeApiVersionText = VersionSeven;
        public const string SubscriptionPartitioningMinimumRuntimeApiVersionText = VersionEight;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Assembly links this file but does not use this field. Microsoft.Cloud.ServiceBus.ResourceProvider.ACISClient.dll")]
        public static readonly string SupportedVersions = string.Join(",",
            ApiVersionConstants.VersionTwo,
            ApiVersionConstants.VersionThree,
            ApiVersionConstants.VersionFour,
            ApiVersionConstants.VersionFive,
            ApiVersionConstants.VersionSix,
            ApiVersionConstants.VersionSeven,
            ApiVersionConstants.VersionEight,
            ApiVersionConstants.VersionNine,
            ApiVersionConstants.VersionTen,
            ApiVersionConstants.VersionEleven,
            ApiVersionConstants.VersionTwelve,
            ApiVersionConstants.VersionThirteen,
            ApiVersionConstants.VersionFourteen,
            ApiVersionConstants.VersionFifteen,
            ApiVersionConstants.VersionSixteen,
            ApiVersionConstants.VersionSeventeen,
            ApiVersionConstants.VersionEighteen);
    }
}
