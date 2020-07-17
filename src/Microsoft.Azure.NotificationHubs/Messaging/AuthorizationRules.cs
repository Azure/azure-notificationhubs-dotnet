//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// Represents a collection of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/>.
    /// </summary>
    [CollectionDataContract(
        Name = ManagementStrings.AuthorizationRules,
        ItemName = "AuthorizationRule",
        Namespace = ManagementStrings.Namespace)]    
    public class AuthorizationRules : ICollection<AuthorizationRule>
    {
        /// <summary>
        /// The serializer
        /// </summary>
        public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof(AuthorizationRules));
        /// <summary>
        /// The inner collection
        /// </summary>
        public readonly ICollection<AuthorizationRule> innerCollection;
        readonly IDictionary<string, SharedAccessAuthorizationRule> nameToSharedAccessAuthorizationRuleMap;
        bool duplicateAddForSharedAccessAuthorizationRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRules"/> class.
        /// </summary>
        public AuthorizationRules()
        {
            this.nameToSharedAccessAuthorizationRuleMap = new Dictionary<string, SharedAccessAuthorizationRule>(StringComparer.OrdinalIgnoreCase);
            this.innerCollection = new List<AuthorizationRule>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRules"/> class with a list of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/>.
        /// </summary>
        /// <param name="enumerable">The list of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/>.</param>
        public AuthorizationRules(IEnumerable<AuthorizationRule> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            this.nameToSharedAccessAuthorizationRuleMap = new Dictionary<string, SharedAccessAuthorizationRule>(StringComparer.OrdinalIgnoreCase);
            this.innerCollection = new List<AuthorizationRule>();

            foreach (AuthorizationRule rule in enumerable)
            {
                this.Add(rule);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<AuthorizationRule> GetEnumerator()
        {
            return this.innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.innerCollection).GetEnumerator();
        }

        /// <summary>
        /// Adds the specified <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/> into the collection.
        /// </summary>
        /// <param name="item">The <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/> to be added.</param>
        public void Add(AuthorizationRule item)
        {
            if (item is SharedAccessAuthorizationRule newRule)
            {
                SharedAccessAuthorizationRule existingRule;
                
                if (this.nameToSharedAccessAuthorizationRuleMap.TryGetValue(newRule.KeyName, out existingRule))
                {
                    this.nameToSharedAccessAuthorizationRuleMap.Remove(newRule.KeyName);
                    this.innerCollection.Remove(existingRule);
                    this.duplicateAddForSharedAccessAuthorizationRule = true;
                }
                
                this.nameToSharedAccessAuthorizationRuleMap.Add(newRule.KeyName, newRule);
            }

            this.innerCollection.Add(item);
        }

        /// <summary>
        /// Clears all elements in the collection.
        /// </summary>
        public void Clear()
        {
            this.nameToSharedAccessAuthorizationRuleMap.Clear();
            this.innerCollection.Clear();
        }

        /// <summary>
        /// Determines whether the specified item exists in the collection.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified item is found; otherwise, false.
        /// </returns>
        /// <param name="item">The item to search in the collection.</param>
        public bool Contains(AuthorizationRule item)
        {
            return this.innerCollection.Contains(item);
        }

        /// <summary>
        /// Copies the elements into an array starting at the specified array index.
        /// </summary>
        /// <param name="array">The array to hold the copied elements.</param><param name="arrayIndex">The zero-based index at which copying starts.</param>
        public void CopyTo(AuthorizationRule[] array, int arrayIndex)
        {
            this.innerCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/> from the collection.
        /// </summary>
        /// 
        /// <returns>
        /// true if the operation succeeded; otherwise, false.
        /// </returns>
        /// <param name="item">The item to remove.</param>
        public bool Remove(AuthorizationRule item)
        {
            return this.innerCollection.Remove(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get { return this.innerCollection.Count; }
        }

        /// <summary>
        /// Gets the sets of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The sets of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/> that match the specified value.
        /// </returns>
        /// <param name="match">The authorization rule to match the specified value.</param>
        public List<AuthorizationRule> GetRules(Predicate<AuthorizationRule> match)
        {
            return ((List<AuthorizationRule>)this.innerCollection).FindAll(match);
        }

        /// <summary>
        /// Gets the rule associated with the specified key.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRules"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="keyName">The name of the key.</param><param name="rule">The rule associated with the specified key.</param>
        public bool TryGetSharedAccessAuthorizationRule(string keyName, out SharedAccessAuthorizationRule rule)
        {
            return this.nameToSharedAccessAuthorizationRuleMap.TryGetValue(keyName, out rule);
        }

        /// <summary>
        /// Gets the set of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/> that matches the specified value.
        /// </summary>
        /// 
        /// <returns>
        /// The sets of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/> that match the specified value.
        /// </returns>
        /// <param name="claimValue">The value to search for.</param>
        public List<AuthorizationRule> GetRules(string claimValue)
        {
            return ((List<AuthorizationRule>)this.innerCollection).FindAll(rule => string.Equals(claimValue, rule.ClaimValue, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get{ return this.innerCollection.IsReadOnly; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRules"/> has equal runtime behavior as this current object.
        /// </summary>
        /// 
        /// <returns>
        /// true if the they are the equal runtime behavior; otherwise, false.
        /// </returns>
        /// <param name="comparand">The <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRules"/> to compare to the current object.</param>
        public bool HasEqualRuntimeBehavior(AuthorizationRules comparand)
        {
            if (comparand == null)
            {
                return false;
            }

            AuthorizationRuleEqualityComparer equalityComparer = new AuthorizationRuleEqualityComparer();
            HashSet<AuthorizationRule> thisRules = new HashSet<AuthorizationRule>(this.innerCollection, equalityComparer);
            HashSet<AuthorizationRule> comparandRules = new HashSet<AuthorizationRule>(comparand.innerCollection, equalityComparer);

            if (thisRules.Count != comparandRules.Count)
            {
                return false;
            }

            foreach (AuthorizationRule rule in thisRules)
            {
                if (!comparandRules.Contains(rule))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether [requires encryption].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires encryption]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresEncryption
        {
            get { return this.nameToSharedAccessAuthorizationRuleMap.Any(); }
        }

        internal void Validate()
        {
            foreach (AuthorizationRule rule in this.innerCollection)
            {
                rule.Validate();
            }

            if (this.duplicateAddForSharedAccessAuthorizationRule)
            {
                throw new InvalidDataContractException(SRClient.CannotHaveDuplicateSAARule);
            }
        }

        internal void UpdateForVersion(ApiVersion version, AuthorizationRules existingAuthorizationRules = null)
        {
            if (version < ApiVersion.Three)
            {
                // API V2 does not understand SharedAccessAuthorizationRule, so we explicitly
                // remove it.
                foreach (var rule in this.nameToSharedAccessAuthorizationRuleMap.Values)
                {
                    this.innerCollection.Remove(rule);
                }

                this.nameToSharedAccessAuthorizationRuleMap.Clear();

                if (existingAuthorizationRules != null)
                {
                    // Given an existing rule set (we get existing ruleset in update scenarios), 
                    // for version 2 and below we need to merge back the SharedAccess rule that 
                    // is not understood by these version.
                    foreach (SharedAccessAuthorizationRule rule in existingAuthorizationRules.nameToSharedAccessAuthorizationRuleMap.Values)
                    {
                        this.Add(rule);
                    }
                }
            }
        }

        internal bool IsValidForVersion(ApiVersion version)
        {
            if (version < ApiVersion.Three)
            {
                return !this.nameToSharedAccessAuthorizationRuleMap.Any();
            }

            return true;
        }
    }
}
