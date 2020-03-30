//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;

    /// <summary>
    /// This class can be used to construct a connection string to be used
    /// in creating messaging client entities. It can also be used to perform
    /// basic validation on an existing connection string.
    /// </summary>

    public class NotificationHubConnectionStringBuilder
    {
        /// <summary>
        /// Create an instance of the connection builder using
        /// an existing connection as a reference.
        /// </summary>
        /// <param name="connectionString">an connection string, which can be obtained
        /// from azure management portal.</param>
        /// <list type="bullet">
        ///     <item> 
        ///     <description>Connection string missing endpoints</description> 
        ///     </item> 
        ///     <item> 
        ///     <description>Connection string does not have enough information for forming a token provider.
        ///     E.g. if user supplied a SasIssuer name but not a SasIssuer key.</description> 
        ///     </item> 
        ///     <item> 
        ///     <description>OperationTimeout value is not a valid TimeSpan format</description> 
        ///     </item> 
        ///     <item> 
        ///     <description>RuntimePort or ManagementPort value is not in an integer format</description> 
        ///     </item> 
        /// </list>

        public NotificationHubConnectionStringBuilder(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                this.InitializeFromConnectionString(connectionString);
            }
        }

        /// <summary>
        /// Gets the collection of service endpoint. Each endpoint 
        /// must reference the same ServiceBus namespace.
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Gets or sets the key name for Sas token
        /// </summary>
        public string SharedAccessKeyName { get; set; }

        /// <summary>
        /// Gets or sets the access key for Sas token
        /// </summary>
        public string SharedAccessKey { get; set; }

        private void InitializeFromConnectionString(string connectionString)
        {
            this.InitializeFromKeyValueManager(new KeyValueConfigurationManager(connectionString));
        }

        private void InitializeFromKeyValueManager(KeyValueConfigurationManager kvmManager)
        {
            try
            {
                // we should avoid caching KeyValueCofigurationManager because it internally
                // cache the connection string which might contain password information.
                kvmManager.Validate();

                // Endpoint
                var endpoint = kvmManager.connectionProperties[KeyValueConfigurationManager.EndpointConfigName];
                if(!string.IsNullOrEmpty(endpoint))
                {
                    this.Endpoint = new Uri(endpoint);
                }

                var sasKeyName = kvmManager.connectionProperties[KeyValueConfigurationManager.SharedAccessKeyName];
                if (!string.IsNullOrWhiteSpace(sasKeyName))
                {
                    this.SharedAccessKeyName = sasKeyName;
                }

                var sasValue = kvmManager.connectionProperties[KeyValueConfigurationManager.SharedAccessValueName];
                if (!string.IsNullOrWhiteSpace(sasValue))
                {
                    this.SharedAccessKey = sasValue;
                }
            }
            catch (Exception exception)
            {
                throw new ArgumentException(exception.Message, "connectionString", exception);
            }
        }

        /// <summary>
        /// Returns string representation of parsed connection string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Endpoint={Endpoint};SharedAccessKeyName={SharedAccessKeyName};SharedAccessKey={SharedAccessKey}";
        }
    }
}
