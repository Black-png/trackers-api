//-----------------------------------------------------------------------
// <copyright file="ConfigureAzureOptions.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Configure Azure options.</summary>
//-----------------------------------------------------------------------

namespace Microsoft.AspNetCore.Authentication
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Configure Azure options
    /// </summary>
    public class ConfigureAzureOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        /// <summary>
        /// The azure options
        /// </summary>
        private readonly AzureAdOptions azureOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureAzureOptions"/> class.
        /// </summary>
        /// <param name="azureOptions">The azure options.</param>
        public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
        {
            this.azureOptions = azureOptions.Value;
        }

        /// <summary>
        /// Invoked to configure a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void Configure(string name, JwtBearerOptions options)
        {
            options.Audience = this.azureOptions.ClientId;
            options.Authority = $"{this.azureOptions.Instance}{this.azureOptions.TenantId}";
            options.RequireHttpsMetadata = false;
            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = this.OnMessageReceived
            };
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>task</returns>
        public Task OnMessageReceived(MessageReceivedContext context)
        {
            var accessToken = context.Request.Query["Authorization"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hub"))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when [challenge].
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>taskreturns>
        public Task OnChallenge(JwtBearerChallengeContext arg)
        {
            arg.HandleResponse();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when [authentication failed].
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>task</returns>
        public Task OnAuthenticationFailed(AuthenticationFailedContext arg)
        {
            Console.WriteLine("fail");
            arg.Response.StatusCode = 500;
            arg.Response.ContentType = "text/plain";
            arg.Response.WriteAsync(arg.Exception.ToString()).Wait();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tokens the validated.
        /// </summary>
        /// <param name="tokenArg">The token argument.</param>
        /// <returns>task</returns>
        public Task TokenValidated(TokenValidatedContext tokenArg)
        {
            var authHeader = tokenArg.Request.Headers["Authorization"];
            var authQS = tokenArg.Request.Query["Authorization"];
            return Task.FromResult(0);
        }

        /// <summary>
        /// Invoked to configure a TOptions instance.
        /// </summary>
        /// <param name="options">The options instance to configure.</param>
        public void Configure(JwtBearerOptions options)
        {
            this.Configure(Options.DefaultName, options);
        }

        /// <summary>
        /// Called when [remote failure].
        /// </summary>
        /// <param name="remoteFailureContext">The remote failure context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task OnRemoteFailure(RemoteFailureContext remoteFailureContext)
        {
            remoteFailureContext.Response.Redirect("/Home/Index");
            remoteFailureContext.HandleResponse();
            return Task.FromResult(0);
        }
    }
}
