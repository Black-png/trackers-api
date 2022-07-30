//-----------------------------------------------------------------------
// <copyright file="MaintenanceReasonController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Department controller class.</summary>
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
    /// The maintenance reason controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class MaintenanceReasonController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IMaintenanceReasonService maintenanceReasonService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceReasonController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="maintenanceReasonService">The entity service.</param>
        public MaintenanceReasonController(IOptions<ApplicationSettings> applicationSettingsConfig, IMaintenanceReasonService maintenanceReasonService)
        {
            this.maintenanceReasonService = maintenanceReasonService ?? throw new ArgumentNullException("maintenanceReasonService");
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
        /// <returns>The list of maintenance reason.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<MaintenanceReason>, int> GetSearched(int pageNo, string searchText)
        {
            var maintenanceReasons = this.maintenanceReasonService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(maintenanceReasons, totalCount);
        }

        /// <summary>
        /// Posts the specified maintenance reason.
        /// </summary>
        /// <param name="maintenanceReason">The maintenance reason.</param>
        /// <returns>The maintenance reason</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaintenanceReason> Post([FromBody]MaintenanceReason maintenanceReason)
        {
            return await this.maintenanceReasonService.Create(maintenanceReason);
        }

        /// <summary>
        /// Puts the specified maintenance reason.
        /// </summary>
        /// <param name="maintenanceReason">The maintenance reason.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaintenanceReason maintenanceReason)
        {
            await this.maintenanceReasonService.Update(maintenanceReason);
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
            await this.maintenanceReasonService.Delete(id);
        }

        /// <summary>
        /// Gets the maintenance reasons.
        /// </summary>
        /// <returns>The list of maintenance reasons.</returns>
        [HttpGet("getmaintenancereasons")]
        public async Task<List<DataSelectionModel>> GetMaintenanceReasons()
        {
            return await this.maintenanceReasonService.GetMaintenanceReasons();
        }
    }
}