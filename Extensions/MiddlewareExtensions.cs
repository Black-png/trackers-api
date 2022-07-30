// <copyright file="MiddlewareExtensions.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>

namespace TT.Core.Api.Extensions
{
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// MiddlewareExtensions
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Uses the web API authentication.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseWebApiAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebApiAuthentication>();
        }
    }
}
