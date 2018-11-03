//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Specifies the error codes of the exceptions.
    /// </summary>
    public enum ExceptionErrorCodes
    {
        /// <summary>
        /// Specifies that bad request error ocured
        /// </summary>
        BadRequest = 40000,

        /// <summary>
        /// Specifies that request was not authorized
        /// </summary>
        UnauthorizedGeneric = 40100,

        /// <summary>
        /// Specifies that error occured because no transport layer security used
        /// </summary>
        NoTransportSecurity = 40101,

        /// <summary>
        /// Specifies that authorization token is missing
        /// </summary>
        MissingToken = 40102,

        /// <summary>
        /// Specifies that signature could not be verified
        /// </summary>
        InvalidSignature = 40103,

        /// <summary>
        /// Specifies that audience requested is invalid
        /// </summary>
        InvalidAudience = 40104,

        /// <summary>
        /// Specifies that authorization token is malformed
        /// </summary>
        MalformedToken = 40105,

        /// <summary>
        /// Specifies that authorization token is expired
        /// </summary>
        ExpiredToken = 40106,

        /// <summary>
        /// Specifies that audience requested does not exist
        /// </summary>
        AudienceNotFound = 40107,

        /// <summary>
        /// Specifies that expiration is not set for authorization token
        /// </summary>
        ExpiresOnNotFound = 40108,

        /// <summary>
        /// Specifies that issuer used was not found
        /// </summary>
        IssuerNotFound = 40109,

        /// <summary>
        /// Specifies that signature was not found
        /// </summary>
        SignatureNotFound = 40110,

        /// <summary>
        /// Specifies that request forbidden error ocured
        /// </summary>
        ForbiddenGeneric = 40300,

        /// <summary>
        /// Specifies that endpoint was not found
        /// </summary>
        EndpointNotFound = 40400,

        /// <summary>
        /// Specifies that destination is invalid
        /// </summary>
        InvalidDestination = 40401,

        /// <summary>
        /// Specifies that namespace was not found
        /// </summary>
        NamespaceNotFound = 40402,

        /// <summary>
        /// Specifies that store lock was lost
        /// </summary>
        StoreLockLost = 40500,

        /// <summary>
        /// Specifies that SQL filters exceeded its allowable maximum number
        /// </summary>
        SqlFiltersExceeded = 40501,

        /// <summary>
        /// Specifies that correlation filters exceeded its allowable maximum number
        /// </summary>
        CorrelationFiltersExceeded = 40502,

        /// <summary>
        /// Specifies that subscriptions exceeded its allowable maximum number
        /// </summary>
        SubscriptionsExceeded = 40503,

        /// <summary>
        /// Specifies that conflict during updating occurred
        /// </summary>
        UpdateConflict = 40504,

        /// <summary>
        /// Specifies that event hub has reached its full capacity
        /// </summary>
        EventHubAtFullCapacity = 40505,

        /// <summary>
        /// Specifies that conflict occured
        /// </summary>
        ConflictGeneric = 40900,

        /// <summary>
        /// Specifies that another operation is in progress
        /// </summary>
        ConflictOperationInProgress = 40901,


        /// <summary>
        /// Specifies that entity was not found
        /// </summary>
        EntityGone = 41000,


        /// <summary>
        /// Specifies that unknown internal error ocured
        /// </summary>
        UnspecifiedInternalError = 50000,

        /// <summary>
        /// Specifies that data communication error ocured
        /// </summary>
        DataCommunicationError = 50001,

        /// <summary>
        /// Specifies that internal failure occured
        /// </summary>
        InternalFailure = 50002,

        /// <summary>
        /// Specifies that provider is unreachable
        /// </summary>
        ProviderUnreachable = 50003,

        /// <summary>
        /// Specifies that server is busy
        /// </summary>
        ServerBusy = 50004,

        /// <summary>
        /// Specifies that bad gateway error occured
        /// </summary>
        BadGatewayFailure = 50200,


        /// <summary>
        /// Specifies that gateway timeout error ocured
        /// </summary>
        GatewayTimeoutFailure = 50400,

        // This exception detail will be used for those exceptions that are thrown without specific any explicit exception detail
        /// <summary>
        /// Specifies that unknown error ocured
        /// </summary>
        UnknownExceptionDetail = 60000,
    }
}

