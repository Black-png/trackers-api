//-----------------------------------------------------------------------
// <copyright file="MaintenanceStatusController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Maintenance status controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The maintenance status controllelr class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class MaintenanceStatusController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IMaintenanceStatusService maintenanceStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceStatusController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="maintenanceStatusService">The entity service.</param>
        public MaintenanceStatusController(IOptions<ApplicationSettings> applicationSettingsConfig, IMaintenanceStatusService maintenanceStatusService)
        {
            this.maintenanceStatusService = maintenanceStatusService ?? throw new ArgumentNullException("maintenanceStatusService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of maintenance status.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<MaintenanceStatus>, int> GetSearched(int pageNo, string searchText)
        {
            var maintenanceStatus = this.maintenanceStatusService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(maintenanceStatus, totalCount);
        }

        /// <summary>
        /// Posts the specified maintenace status.
        /// </summary>
        /// <param name="maintenaceStatus">The maintenace status.</param>
        /// <returns>The  maintenance status.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaintenanceStatus> Post([FromBody]MaintenanceStatus maintenaceStatus)
        {
            return await this.maintenanceStatusService.Create(maintenaceStatus);
        }

        /// <summary>
        /// Puts the specified maintenance status.
        /// </summary>
        /// <param name="maintenanceStatus">The maintenance status.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaintenanceStatus maintenanceStatus)
        {
            await this.maintenanceStatusService.Update(maintenanceStatus);
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
            await this.maintenanceStatusService.Delete(id);
        }

        /// <summary>
        /// Gets the maintenance status.
        /// </summary>
        /// <returns>The list of maintenance status.</returns>
        [HttpGet("getmaintenancestatus")]
        public async Task<List<DataSelectionModel>> GetMaintenanceStatus()
        {
            return await this.maintenanceStatusService.GetMaintenanceStatus();
        }
    }
}