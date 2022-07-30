// <copyright file="PackageController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// PackageController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PackageController : Controller
    {
        /// <summary>
       /// The entity service
       /// </summary>
        private IPackageServices packageServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="packageServices">The package services.</param>
        /// <exception cref="ArgumentNullException">IPackageServices</exception>
        public PackageController(IOptions<ApplicationSettings> applicationSettingsConfig, IPackageServices packageServices)
        {
            this.packageServices = packageServices ?? throw new ArgumentNullException("packageServices");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the active package.
        /// </summary>
        /// <returns>the task</returns>
        [HttpGet("GetActivePackage")]
        public async Task<Package> GetActivePackage()
        {
            return await this.packageServices.GetActivePackage();
        }
    }
}
