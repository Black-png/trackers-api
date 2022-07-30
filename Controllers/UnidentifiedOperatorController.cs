//-----------------------------------------------------------------------
// <copyright file="UnidentifiedOperatorController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Product controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Equipment controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class UnidentifiedOperatorController : Controller
    {
        /// <summary>
        /// The labour service
        /// </summary>
        private ILabourService labourService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnidentifiedOperatorController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="labourService">The labour service.</param>
        public UnidentifiedOperatorController(IOptions<ApplicationSettings> applicationSettingsConfig, ILabourService labourService)
        {
            this.labourService = labourService ?? throw new ArgumentNullException("labourService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Puts the specified labour.
        /// </summary>
        /// <param name="labour">The labour.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody] Labour labour)
        {
            if (labour.DepartmentId <= 0)
            {
                throw new ArgumentNullException("labour.DepartmentId");
            }

            await this.labourService.Update(labour);
        }
    }
}