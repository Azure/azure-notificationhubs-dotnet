// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBackend
{
    public class AuthenticationTestMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationTestMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader != null && authorizationHeader
                .StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
            {
                var authorizationUserAndPwdBase64 =
                    authorizationHeader.Substring("Basic ".Length);
                var authorizationUserAndPwd = Encoding.Default
                    .GetString(Convert.FromBase64String(authorizationUserAndPwdBase64));
                var user = authorizationUserAndPwd.Split(':')[0];
                var password = authorizationUserAndPwd.Split(':')[1];

                if (verifyUserAndPwd(user, password))
                {
                    // Attach the new principal object to the current HttpContext object
                    context.User =
                        new GenericPrincipal(new GenericIdentity(user), new string[0]);
                    Thread.CurrentPrincipal =
                        context.User;
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next.Invoke(context);
        }

        private bool verifyUserAndPwd(string user, string password)
        {
            // This is not a real authentication scheme.
            return user == password;
        }
    }
}
