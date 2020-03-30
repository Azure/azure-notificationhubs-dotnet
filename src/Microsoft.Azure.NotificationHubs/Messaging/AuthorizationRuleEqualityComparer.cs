//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    sealed class AuthorizationRuleEqualityComparer : EqualityComparer<AuthorizationRule>
    {
        public override bool Equals(AuthorizationRule x, AuthorizationRule y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public override int GetHashCode(AuthorizationRule obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.GetHashCode();
        }
    }
}
