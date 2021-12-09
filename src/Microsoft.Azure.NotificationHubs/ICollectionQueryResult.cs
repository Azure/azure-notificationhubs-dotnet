//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
        /// <summary>
    /// Represents a collection query result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public interface ICollectionQueryResult<T> : IEnumerable<T> where T : EntityDescription
    {
        /// <summary>
        /// Gets the continuation token.
        /// </summary>
        /// <value>
        /// The continuation token, which is null or empty when there is no additional data available in the query.
        /// </value>
        string ContinuationToken { get; }
    }
}
