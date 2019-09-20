// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using Microsoft.Azure.NotificationHubs;

namespace AppBackend.Models
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            // Please update the following connection string with your hub's DefaultFullSharedAccessSignature with the full access such as Listen, Manage, Send
            Hub = NotificationHubClient.CreateClientFromConnectionString("<your hub's DefaultFullSharedAccessSignature>",
                                                                            "<hub name>");
        }
    }
}
