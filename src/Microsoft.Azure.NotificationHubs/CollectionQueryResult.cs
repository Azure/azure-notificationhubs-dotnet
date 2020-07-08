//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System.Collections.Generic;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents a collection query result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public sealed class CollectionQueryResult<T> : IEnumerable<T> where T : EntityDescription
    {
        private IEnumerable<T> results;

        internal CollectionQueryResult(IEnumerable<T> results, string continuationToken)
        {
            this.results = results;
            if (this.results == null)
            {
                this.results = new T[0];
            }
            
            this.ContinuationToken = continuationToken;
        }

        /// <summary>
        /// Gets the continuation token.
        /// </summary>
        /// <value>
        /// The continuation token, which is null or empty when there is no additional data available in the query.
        /// </value>
        public string ContinuationToken
        {
            get;
            private set;
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator that iterates through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
                return this.results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.results.GetEnumerator();
        }
    }
}