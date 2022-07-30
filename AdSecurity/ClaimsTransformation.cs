//-----------------------------------------------------------------------
// <copyright file="ClaimsTransformation.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Claims Transformation class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.AdSecurity
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Caching.Memory;
    using TT.Core.Models.Constants;
    using TT.Core.Models.Enums;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Claims Transformation class.
    /// </summary>
    public class ClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// The authorisation service
        /// </summary>
        private IAuthorisationService authorisationService;

        /// <summary>
        /// The memory cache
        /// </summary>
        private IMemoryCache memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsTransformation" /> class.
        /// </summary>
        /// <param name="authorisationService">The authorisation service.</param>
        /// <param name="memoryCache">The memory cache.</param>
        public ClaimsTransformation(IAuthorisationService authorisationService, IMemoryCache memoryCache)
        {
            this.authorisationService = authorisationService ?? throw new ArgumentNullException("authorisationService");
            this.memoryCache = memoryCache ?? throw new ArgumentNullException("memoryCache");
        }

        /// <summary>
        /// Transform the principle
        /// </summary>
        /// <param name="principal">The principl</param>
        /// <returns>The transformed principal</returns>
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            string claimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            var userObjectIdClaim = principal.Claims.FirstOrDefault(p => p.Type == claimType);
            if (userObjectIdClaim == null || string.IsNullOrEmpty(principal.Identity.Name))
            {
                return principal;
            }

            var user = await this.authorisationService.Get(u => u.UserId == userObjectIdClaim.Value);
            if (user == null)
            {
                Console.WriteLine("calling GetListOfAuthorisedUsers from line number 63 claim");
                await this.authorisationService.GetListOfAuthorisedUsers();
                user = await this.authorisationService.Get(u => u.UserId == userObjectIdClaim.Value);
                if (user == null)
                {
                    throw new Exception("Authorised user is not found load the A.D users");
                }
            }

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Save data in cache.
            this.memoryCache.Set<string>(CacheKeyConstants.LoggedInUserEmail, user.Email);
            this.memoryCache.Set<string>(CacheKeyConstants.LoggedInUserId, user.UserId);
            this.memoryCache.Set<string>(CacheKeyConstants.CustomerEnvironment, environmentName);

            Roles role = (Roles)user.Level;
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Level", role.ToString()));
            return principal;
        }
    }
}
