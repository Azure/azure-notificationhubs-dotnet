//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security;
using System.Text.RegularExpressions;
using Microsoft.Azure.NotificationHubs.Auth;

namespace Microsoft.Azure.NotificationHubs
{
    internal class KeyValueConfigurationManager
    {
        public const string ServiceBusConnectionKeyName = @"Microsoft.Azure.NotificationHubs.ConnectionString";
        public const string OperationTimeoutConfigName = @"OperationTimeout";
        public const string EntityPathConfigName = @"EntityPath";
        public const string EndpointConfigName = @"Endpoint";
        public const string SharedSecretIssuerConfigName = @"SharedSecretIssuer";
        public const string SharedSecretValueConfigName = @"SharedSecretValue";
        public const string SharedAccessKeyName = @"SharedAccessKeyName";
        public const string SharedAccessValueName = @"SharedAccessKey";
        public const string RuntimePortConfigName = @"RuntimePort";
        public const string ManagementPortConfigName = @"ManagementPort";
        public const string StsEndpointConfigName = @"StsEndpoint";
        public const string WindowsDomainConfigName = @"WindowsDomain";
        public const string WindowsUsernameConfigName = @"WindowsUsername";
        public const string WindowsPasswordConfigName = @"WindowsPassword";
        public const string OAuthDomainConfigName = @"OAuthDomain";
        public const string OAuthUsernameConfigName = @"OAuthUsername";
        public const string OAuthPasswordConfigName = @"OAuthPassword";

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
        internal string _connectionString;

        public KeyValueConfigurationManager(string connectionString)
        {
            Initialize(connectionString);
        }

        private void Initialize(string connection)
        {
            _connectionString = connection;
            connectionProperties = CreateNameValueCollectionFromConnectionString(_connectionString);
        }

        public string this[string key] => connectionProperties[key];

        private static NameValueCollection CreateNameValueCollectionFromConnectionString(string connectionString)
        {
            var settings = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = KeyDelimiter + connectionString;
                var keyValues = Regex.Split(connection, KeyDelimiterRegexString, RegexOptions.IgnoreCase);
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
            if (string.IsNullOrWhiteSpace(connectionProperties[EndpointConfigName]))
            {
                throw new ConfigurationException(SRClient.AppSettingsConfigMissingSetting(EndpointConfigName, ServiceBusConnectionKeyName));
            }
        }

        public NamespaceManager CreateNamespaceManager()
        {
            Validate();

            string operationTimeout = connectionProperties[OperationTimeoutConfigName];
            IEnumerable<Uri> endpoints = GetEndpointAddresses(connectionProperties[EndpointConfigName], connectionProperties[ManagementPortConfigName]);
            IEnumerable<Uri> stsEndpoints = GetEndpointAddresses(connectionProperties[StsEndpointConfigName], null);
            string issuerName = connectionProperties[SharedSecretIssuerConfigName];
            string issuerKey = connectionProperties[SharedSecretValueConfigName];
            string sasKeyName = connectionProperties[SharedAccessKeyName];
            string sasKey = connectionProperties[SharedAccessValueName];
            string windowsDomain = connectionProperties[WindowsDomainConfigName];
            string windowsUsername = connectionProperties[WindowsUsernameConfigName];
            SecureString windowsPassword = GetWindowsPassword();
            string oauthDomain = connectionProperties[OAuthDomainConfigName];
            string oauthUsername = connectionProperties[OAuthUsernameConfigName];
            SecureString oauthPassword = GetOAuthPassword();

            try
            {
                TokenProvider provider = CreateTokenProvider(stsEndpoints, issuerName, issuerKey, sasKeyName, sasKey, windowsDomain, windowsUsername, windowsPassword, oauthDomain, oauthUsername, oauthPassword);
                if (string.IsNullOrEmpty(operationTimeout))
                {
                    return new NamespaceManager(endpoints, provider);
                }

                return new NamespaceManager(
                    endpoints,
                    new NamespaceManagerSettings()
                    {
                        TokenProvider = provider
                    });
            }
            catch (ArgumentException e)
            {
                throw new ConfigurationErrorsException(
                    SRClient.AppSettingsCreateManagerWithInvalidConnectionString(e.Message),
                    e);
            }
            catch (UriFormatException e)
            {
                throw new ConfigurationErrorsException(
                    SRClient.AppSettingsCreateManagerWithInvalidConnectionString(e.Message),
                    e);
            }
        }

        public SecureString GetWindowsPassword()
        {
            return GetSecurePassword(WindowsPasswordConfigName);
        }

        public SecureString GetOAuthPassword()
        {
            return GetSecurePassword(OAuthPasswordConfigName);
        }

        internal TokenProvider CreateTokenProvider()
        {
            IList<Uri> endpointAddresses = GetEndpointAddresses(connectionProperties["StsEndpoint"], (string)null);
            string connectionProperty1 = connectionProperties["SharedSecretIssuer"];
            string connectionProperty2 = connectionProperties["SharedSecretValue"];
            string connectionProperty3 = connectionProperties["SharedAccessKeyName"];
            string connectionProperty4 = connectionProperties["SharedAccessKey"];
            string connectionProperty5 = connectionProperties["WindowsDomain"];
            string connectionProperty6 = connectionProperties["WindowsUsername"];
            SecureString windowsPassword1 = GetWindowsPassword();
            string connectionProperty7 = connectionProperties["OAuthDomain"];
            string connectionProperty8 = connectionProperties["OAuthUsername"];
            SecureString oauthPassword1 = GetOAuthPassword();
            string issuerName = connectionProperty1;
            string issuerKey = connectionProperty2;
            string sharedAccessKeyName = connectionProperty3;
            string sharedAccessKey = connectionProperty4;
            string windowsDomain = connectionProperty5;
            string windowsUser = connectionProperty6;
            SecureString windowsPassword2 = windowsPassword1;
            string oauthDomain = connectionProperty7;
            string oauthUser = connectionProperty8;
            SecureString oauthPassword2 = oauthPassword1;
            return CreateTokenProvider((IEnumerable<Uri>)endpointAddresses,
                issuerName,
                issuerKey,
                sharedAccessKeyName,
                sharedAccessKey,
                windowsDomain,
                windowsUser,
                windowsPassword2,
                oauthDomain,
                oauthUser,
                oauthPassword2);
        }

        private static TokenProvider CreateTokenProvider(
            IEnumerable<Uri> stsEndpoints,
            string issuerName,
            string issuerKey,
            string sharedAccessKeyName,
            string sharedAccessKey,
            string windowsDomain,
            string windowsUser,
            SecureString windowsPassword,
            string oauthDomain,
            string oauthUser,
            SecureString oauthPassword)
        {
            if (!string.IsNullOrWhiteSpace(sharedAccessKey))
            {
                return TokenProvider.CreateSharedAccessSignatureTokenProvider(sharedAccessKeyName, sharedAccessKey);
            }
            if (string.IsNullOrWhiteSpace(issuerName))
            {
                int num = stsEndpoints == null ? 0 : (stsEndpoints.Any<Uri>() ? 1 : 0);
                bool flag1 = !string.IsNullOrWhiteSpace(windowsUser) && windowsPassword != null && windowsPassword.Length > 0;
                bool flag2 = !string.IsNullOrWhiteSpace(oauthUser) && oauthPassword != null && oauthPassword.Length > 0;
                if (num == 0)
                    return (TokenProvider)null;
                if (flag2)
                {
                    NetworkCredential credential = string.IsNullOrWhiteSpace(oauthDomain)
                        ? new NetworkCredential(oauthUser, oauthPassword)
                        : new NetworkCredential(oauthUser, oauthPassword, oauthDomain);
                    return TokenProvider.CreateOAuthTokenProvider(stsEndpoints, credential);
                }
                if (!flag1)
                {
                    return TokenProvider.CreateWindowsTokenProvider(stsEndpoints);
                }
                NetworkCredential credential1 = string.IsNullOrWhiteSpace(windowsDomain)
                    ? new NetworkCredential(windowsUser, windowsPassword)
                    : new NetworkCredential(windowsUser, windowsPassword, windowsDomain);
                return TokenProvider.CreateWindowsTokenProvider(stsEndpoints, credential1);
            }

            if (stsEndpoints != null && stsEndpoints.Any<Uri>())
            {
                return TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerKey, stsEndpoints.First<Uri>());
            }
            return TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerKey);
        }

        private IList<Uri> GetEndpointAddresses()
        {
            return GetEndpointAddresses(connectionProperties[EndpointConfigName], connectionProperties[RuntimePortConfigName]);
        }

        public static IList<Uri> GetEndpointAddresses(string uriEndpoints, string portString)
        {
            List<Uri> addresses = new List<Uri>();
            if (string.IsNullOrWhiteSpace(uriEndpoints))
            {
                return addresses;
            }

            string[] endpoints = uriEndpoints.Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (endpoints == null || endpoints.Length == 0)
            {
                return addresses;
            }

            if (!int.TryParse(portString, out int port))
            {
                port = -1;
            }

            foreach (string endpoint in endpoints)
            {
                var address = new UriBuilder(endpoint);
                if (port > 0)
                {
                    address.Port = port;
                }

                addresses.Add(address.Uri);
            }

            return addresses;
        }

        SecureString GetSecurePassword(string configName)
        {
            SecureString password = null;
            string passwordString = connectionProperties[configName];
            if (!string.IsNullOrWhiteSpace(passwordString))
            {
                unsafe
                {
                    char[] array = passwordString.ToCharArray();
                    fixed (char* pChars = array)
                    {
                        password = new SecureString(pChars, array.Length);
                    }
                }
            }

            return password;
        }
    }
}
