//-----------------------------------------------------------------------
// <copyright file="AzureAdOptions.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Configure Azure options.</summary>
//-----------------------------------------------------------------------

namespace Microsoft.AspNetCore.Authentication
{
    /// <summary>
    /// Azure AD options
    /// </summary>
    public class AzureAdOptions
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the callback path.
        /// </summary>
        /// <value>
        /// The callback path.
        /// </value>
        public string CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the mobile application client identifier.
        /// </summary>
        /// <value>
        /// The mobile application client identifier.
        /// </value>
        public string MobileAppClientId { get; set; }

        /// <summary>
        /// Gets or sets the web application client identifier.
        /// </summary>
        /// <value>
        /// The web application client identifier.
        /// </value>
        public string WebAppClientId { get; set; }

        /// <summary>
        /// Gets or sets the emp reg client identifier.
        /// </summary>
        /// <value>
        /// The web application client identifier.
        /// </value>
        public string EmployeeFacialRegClientId { get; set; }
    }
}
