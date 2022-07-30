// <copyright file="WebApiAuthentication.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>

namespace TT.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// WebApiAuthentication
    /// </summary>
    public class WebApiAuthentication
    {
        /// <summary>
        /// The aad instance
        /// </summary>
        private static readonly string AadInstance = "https://login.microsoftonline.com/{0}";

        /// <summary>
        /// The tenant
        /// </summary>
        private readonly string tenant;

        /// <summary>
        /// The next
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The azure options
        /// </summary>
        private readonly AzureAdOptions azureOptions;

        /// <summary>
        /// The mobile audience
        /// </summary>
        private readonly string mobileAudience;

        /// <summary>
        /// The face audience
        /// </summary>
        private readonly string faceRegaAudience;

        /// <summary>
        /// The web audience
        /// </summary>
        private readonly string webAudience;

        /// <summary>
        /// The authority
        /// </summary>
        private readonly string authority;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiAuthentication" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="azureOptions">The azure options.</param>
        public WebApiAuthentication(RequestDelegate next, IOptions<AzureAdOptions> azureOptions)
        {
            this.next = next;
            this.azureOptions = azureOptions.Value;
            this.tenant = this.azureOptions.TenantId;
            this.mobileAudience = this.azureOptions.MobileAppClientId;
            this.faceRegaAudience = this.azureOptions.EmployeeFacialRegClientId;
            this.webAudience = this.azureOptions.WebAppClientId;
            this.authority = string.Format(CultureInfo.InvariantCulture, AadInstance, this.tenant);
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                Console.WriteLine("hitting the container");
                var authheader = context.Request.Headers["Authorization"].FirstOrDefault();
                var authQS = context.Request.Query["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authQS))
                {
                    authheader = string.IsNullOrEmpty(authheader) ? authQS : authheader;
                    var token = authheader.Replace("Bearer", string.Empty).Trim();
                    Console.WriteLine(authheader);
                    IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{this.authority}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
                    OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
                    TokenValidationParameters validationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = $"https://sts.windows.net/{this.tenant}/",
                        ValidAudiences = new[] { this.mobileAudience, this.webAudience, this.faceRegaAudience },
                        IssuerSigningKeys = openIdConfig.SigningKeys
                    };

                    SecurityToken validatedToken;
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    var claims = handler.ValidateToken(token, validationParameters, out validatedToken);
                    context.User = claims;
                    Console.WriteLine("success");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                throw ex;
            }

            await this.next.Invoke(context);
        }
    }
}
