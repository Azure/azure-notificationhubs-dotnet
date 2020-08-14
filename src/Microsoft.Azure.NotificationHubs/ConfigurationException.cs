// ----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
// ----------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represnets an exception based upon invalid configuration.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Creates a ConfigurationException with a message.
        /// </summary>
        /// <param name="message">The message for the ConfigurationException instance.</param>
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}