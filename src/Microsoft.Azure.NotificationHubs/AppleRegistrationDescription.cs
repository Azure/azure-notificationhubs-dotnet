//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents the description of apple registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.AppleRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class AppleRegistrationDescription : RegistrationDescription
    {
        internal static Regex DeviceTokenRegex = new Regex("^[a-fA-F0-9]+$");
        internal const string ExpiryHeader = "ServiceBusNotification-Apns-Expiry";
        internal const string PriorityHeader = "X-Apns-Priority";
        internal const string ApnsHeaderPrefix = "apns-";
        internal const string ApnsExpiryHeader = "apns-expiration";
        internal const string ApnsPriorityHeader = "apns-priority";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public AppleRegistrationDescription(AppleRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.DeviceToken = sourceRegistration.DeviceToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleRegistrationDescription"/> class.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        public AppleRegistrationDescription(string deviceToken)
            : this(string.Empty, deviceToken, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleRegistrationDescription"/> class.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="tags">The tags.</param>
        public AppleRegistrationDescription(string deviceToken, IEnumerable<string> tags)
            : this(string.Empty, deviceToken, tags)
        {
        }

        internal AppleRegistrationDescription(string notificationHubPath, string deviceToken, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (string.IsNullOrWhiteSpace(deviceToken))
            {
                throw new ArgumentNullException("deviceToken");
            }

            this.DeviceToken = deviceToken;

            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        internal override string AppPlatForm
        {
            get
            {
                return ApnsCredential.AppPlatformName;
            }
        }

        internal override string RegistrationType
        {
            get
            {
                return ApnsCredential.AppPlatformName;
            }
        }

        internal override string PlatformType
        {
            get
            {
                return ApnsCredential.AppPlatformName;
            }
        }

        /// <summary>
        /// Gets or sets the device token.
        /// </summary>
        /// 
        /// <returns>
        /// The device token.
        /// </returns>
        [DataMember(Name = ManagementStrings.DeviceToken, Order = 2001, IsRequired = true)]
        public string DeviceToken { get; set; }

        internal override void OnValidate(ApiVersion version)
        {
            if (string.IsNullOrWhiteSpace(this.DeviceToken))
            {
                throw new InvalidDataContractException(SRClient.DeviceTokenIsEmpty);
            }

            if (!AppleRegistrationDescription.DeviceTokenRegex.IsMatch(this.DeviceToken) || this.DeviceToken.Length % 2 != 0)
            {
                throw new InvalidDataContractException(SRClient.DeviceTokenHexaDecimalDigitError);
            }
        }

        internal byte[] GetDeviceTokenBytes()
        {
            return GetDeviceTokenBytes(this.DeviceToken);
        }

        internal static byte[] GetDeviceTokenBytes(string deviceToken)
        {
            if (string.IsNullOrWhiteSpace(deviceToken))
            {
                throw new InvalidDataContractException(SRClient.DeviceTokenIsEmpty);
            }

            if (!AppleRegistrationDescription.DeviceTokenRegex.IsMatch(deviceToken) || deviceToken.Length % 2 != 0)
            {
                throw new InvalidDataContractException(SRClient.DeviceTokenHexaDecimalDigitError);
            }

            byte[] result = new byte[deviceToken.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(deviceToken.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return result;
        }

        internal override string GetPnsHandle()
        {
            return this.DeviceToken.ToUpperInvariant();
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            this.DeviceToken = pnsHandle;
        }

        internal override RegistrationDescription Clone()
        {
            return new AppleRegistrationDescription(this);
        }
    }
}