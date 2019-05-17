// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

namespace ParseFeedbackSample
{
    public enum PnsError
    {
        InvalidCredentials = 1,
        PnsUnreachable = 2,
        PnsInterfaceError = 3,
        BadChannel = 4,
        ExpiredChannel = 5,
        PnsServerError = 6,
        PnsUnavailable = 7,
        Throttled = 8,
        TokenProviderInterfaceError = 9,
        TokenProviderUnreachable = 10,
        InvalidToken = 11,
        WrongToken = 12,
        InvalidNotificationFormat = 13,
        InvalidNotificationSize = 14,
        ChannelThrottled = 15,
        ChannelDisconnected = 16,
        Dropped = 17,
        WrongChannel = 18,
        RefreshToken = 19,
        AbandonedNotificationMessages = 96,
        NoTargets = 97,
        Skipped = 98,
        UnknownError = 99
    }
}
