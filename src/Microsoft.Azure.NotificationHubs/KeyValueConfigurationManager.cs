//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Text.RegularExpressions;

    internal class KeyValueConfigurationManager
    {
        public const string ServiceBusConnectionKeyName = @"Microsoft.Azure.NotificationHubs.ConnectionString";
        public const string EndpointConfigName = @"Endpoint";
        public const string EntityPathConfigName = @"EntityPath";
        public const string SharedAccessKeyName = @"SharedAccessKeyName";
        public const string SharedAccessValueName = @"SharedAccessKey";

        internal const string ValueSeparator = @",";
        internal const string KeyValueSeparator = @"=";
        internal const string KeyDelimiter = @";";
        const string KeyAttributeEnumRegexString = @"(" +
                                                   EndpointConfigName + @"|" +
                                                   SharedAccessKeyName + @"|" +
                                                   EntityPathConfigName + @"|" +
                                                   SharedAccessValueName + @")";
        const string KeyDelimiterRegexString = KeyDelimiter + KeyAttributeEnumRegexString + KeyValueSeparator;

        // This is not designed to catch any custom parsing logic that SB has. We rely on SB to do the 
        // actual string validation. Also note the following characteristics:
        // 1. We are whitelisting known setting names intentionally here for security.
        // 2. The regex here excludes spaces.
        private static readonly Regex KeyRegex = new Regex(KeyAttributeEnumRegexString, RegexOptions.IgnoreCase);

        private static readonly Regex ValueRegex = new Regex(@"([^\s]+)");

        internal NameValueCollection connectionProperties;
        internal string connectionString;

        public KeyValueConfigurationManager(string connectionString)
        {
            this.Initialize(connectionString);
        }

        private void Initialize(string connection)
        {
            this.connectionString = connection;
            this.connectionProperties = CreateNameValueCollectionFromConnectionString(this.connectionString);
        }

        public string this[string key] => this.connectionProperties[key];

        private static NameValueCollection CreateNameValueCollectionFromConnectionString(string connectionString)
        {
            var settings = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = KeyValueConfigurationManager.KeyDelimiter + connectionString;
                var keyValues = Regex.Split(connection, KeyValueConfigurationManager.KeyDelimiterRegexString, RegexOptions.IgnoreCase);
                if (keyValues.Length > 0)
                {
                    // Regex.Split returns the array that include part of the delimiters, so it will look 
                    // something like this:
                    // { "", "Endpoint", "sb://a.b.c", "OperationTimeout", "01:20:30", ...}
                    // We should always get empty string for first element (except if we found no match at all).
                    if (!string.IsNullOrWhiteSpace(keyValues[0]))
                    {
                        throw new ConfigurationException(SRClient.AppSettingsConfigSettingInvalidKey(connectionString));
                    }

                    if (keyValues.Length % 2 != 1)
                    {
                        throw new ConfigurationException(SRClient.AppSettingsConfigSettingInvalidKey(connectionString));
                    }

                    for (var i = 1; i < keyValues.Length; i++)
                    {
                        var key = keyValues[i];
                        if (string.IsNullOrWhiteSpace(key) || !KeyRegex.IsMatch(key))
                        {
                            throw new ConfigurationException(SRClient.AppSettingsConfigSettingInvalidKey(key));
                        }

                        var value = keyValues[i + 1];
                        if (string.IsNullOrWhiteSpace(value) || !ValueRegex.IsMatch(value))
                        {
                            throw new ConfigurationException(SRClient.AppSettingsConfigSettingInvalidValue(key, value));
                        }

                        if (settings[key] != null)
                        {
                            throw new ConfigurationException(SRClient.AppSettingsConfigDuplicateSetting(key));
                        }

                        settings[key] = value;
                        i++;
                    }
                }
            }

            return settings;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.connectionProperties[EndpointConfigName]))
            {
                throw new ConfigurationException(SRClient.AppSettingsConfigMissingSetting(EndpointConfigName, ServiceBusConnectionKeyName));
            }
        }
    }
}
