//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Notification outcome dictionary
    /// </summary>
    [CollectionDataContract(
        Name = ManagementStrings.NotificationOutcomeCollection,
        ItemName = "Outcome",
        KeyName = "Name",
        ValueName = "Count",
        Namespace = ManagementStrings.Namespace)]
    public sealed class NotificationOutcomeCollection : Dictionary<string, long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationOutcomeCollection"/> class.
        /// </summary>
        public NotificationOutcomeCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        internal NotificationOutcomeCollection(IEnumerable<string> notificationOutcomes)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            foreach (var notificationOutcome in notificationOutcomes)
            {
                if (this.ContainsKey(notificationOutcome))
                {
                    this[notificationOutcome] = this[notificationOutcome] + 1;
                }
                else
                {
                    this[notificationOutcome] = 1;
                }
            }
        }

        internal void AddRange(IEnumerable<string> notificationOutcomes)
        {
            foreach (var notificationOutcome in notificationOutcomes)
            {
                if (this.ContainsKey(notificationOutcome))
                {
                    this[notificationOutcome] = this[notificationOutcome] + 1;
                }
                else
                {
                    this[notificationOutcome] = 1;
                }
            }
        }

        internal NotificationOutcomeCollection Add(NotificationOutcomeCollection outcomeCollection)
        {
            foreach (var kvp in outcomeCollection)
            {
                if (this.ContainsKey(kvp.Key))
                {
                    this[kvp.Key] = this[kvp.Key] + kvp.Value;
                }
                else
                {
                    this[kvp.Key] = kvp.Value;
                }
            }

            return this;
        }

        internal static NotificationOutcomeCollection Rollup(IEnumerable<NotificationOutcomeCollection> outcomeCollection)
        {
            var combinedOutcome = new NotificationOutcomeCollection();
            foreach (var outcome in outcomeCollection)
            {
                foreach (var kvp in outcome)
                {
                    if (combinedOutcome.ContainsKey(kvp.Key))
                    {
                        combinedOutcome[kvp.Key] = combinedOutcome[kvp.Key] + kvp.Value;
                    }
                    else
                    {
                        combinedOutcome[kvp.Key] = kvp.Value;
                    }
                }
            }

            return combinedOutcome;
        }
    }
}
