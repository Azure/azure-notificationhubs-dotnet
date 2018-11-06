//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    ///    This is a stub for auto-generated resource class, providing GetString function. Usage:
    ///
    ///        string s = SR.GetString(SR.MyIdenfitier);
    /// </summary>
    sealed partial class SR : Microsoft.Azure.NotificationHubs.Properties.Resources
    {
        internal static string GetString(string value, params object[] args)
        {            
            if (args != null && args.Length > 0)
            {
                // This is currently feeding null in as nobody sets Properties.Resources.Culture. 
                // Null will internally cause ToString to use CurrentCulture for formatting.
                // Reviewed this for DateTime.
                return string.Format(Microsoft.Azure.NotificationHubs.Properties.Resources.Culture, value, args);
            }
            
            return value;
        }        
    }
}
