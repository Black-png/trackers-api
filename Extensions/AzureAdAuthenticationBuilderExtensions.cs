//-----------------------------------------------------------------------
// <copyright file="AzureAdAuthenticationBuilderExtensions.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Azure AD authentication builder class.</summary>
//-----------------------------------------------------------------------

namespace Microsoft.AspNetCore.Authentication
{
    using System;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Azure AD authentication builder class.
    /// </summary>
    public static class AzureAdAuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds the azure ad.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The builder</returns>
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        /// <summary>
        /// Adds the azure ad.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns>The builder</returns>
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
            builder.AddJwtBearer();
            return builder;
        }
    }
}
