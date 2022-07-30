//-----------------------------------------------------------------------
// <copyright file="CustomAuthorizeRequirement.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>The custom authorize requirement class.</summary>

namespace TT.Core.Api.SecurityModels
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// The custom authorize requirement class.
    /// </summary>
    /// <seealso cref="IAuthorizationRequirement" />
    public class CustomAuthorizeRequirement : IAuthorizationRequirement
    {
    }
}
