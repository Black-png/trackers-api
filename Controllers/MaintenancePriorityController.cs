//-----------------------------------------------------------------------
// <copyright file="MaintenancePriorityController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Maintenance priority controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The maintenance priority controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class MaintenancePriorityController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IMaintenancePriorityService maintenancePriorityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenancePriorityController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="maintenancePriorityService">The entity service.</param>
        public MaintenancePriorityController(IOptions<ApplicationSettings> applicationSettingsConfig, IMaintenancePriorityService maintenancePriorityService)
        {
            this.maintenancePriorityService = maintenancePriorityService ?? throw new ArgumentNullException("maintenancePriorityService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Posts the specified maintenance priority.
        /// </summary>
        /// <param name="maintenancePriority">The maintenance priority.</param>
        /// <returns>The maintenance priority</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaintenancePriority> Post([FromBody]MaintenancePriority maintenancePriority)
        {
            return await this.maintenancePriorityService.Create(maintenancePriority);
        }

        /// <summary>
        /// Puts the specified maintenance priority.
        /// </summary>
        /// <param name="maintenancePriority">The maintenance priority.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaintenancePriority maintenancePriority)
        {
            await this.maintenancePriorityService.Update(maintenancePriority);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.maintenancePriorityService.Delete(id);
        }

        /// <summary>
        /// Gets the priorities.
        /// </summary>
        /// <returns>The list of maintenance priorities.</returns>
        [HttpGet("getpriorities")]
        public async Task<List<DataSelectionModel>> GetPriorities()
        {
            return await this.maintenancePriorityService.GetPriorityList();
        }
    }
}