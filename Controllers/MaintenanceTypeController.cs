//-----------------------------------------------------------------------
// <copyright file="MaintenanceTypeController.cs" company="ThingTrax UK Ltd">
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
    /// The maintenance type controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class MaintenanceTypeController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IMaintenanceTypeService maintenanceTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceTypeController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="maintenanceTypeService">The entity service.</param>
        public MaintenanceTypeController(IOptions<ApplicationSettings> applicationSettingsConfig, IMaintenanceTypeService maintenanceTypeService)
        {
            this.maintenanceTypeService = maintenanceTypeService ?? throw new ArgumentNullException("maintenanceTypeService");
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
        /// <returns>The list of maintenance types.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<MaintenanceType>, int> GetSearched(int pageNo, string searchText)
        {
            var maintenanceTypes = this.maintenanceTypeService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(maintenanceTypes, totalCount);
        }

        /// <summary>
        /// Posts the specified maintenace type.
        /// </summary>
        /// <param name="maintenaceType">Type of the maintenace.</param>
        /// <returns>The maintenance type</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaintenanceType> Post([FromBody]MaintenanceType maintenaceType)
        {
            return await this.maintenanceTypeService.Create(maintenaceType);
        }

        /// <summary>
        /// Puts the specified maintenance type.
        /// </summary>
        /// <param name="maintenanceType">Type of the maintenance.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaintenanceType maintenanceType)
        {
            await this.maintenanceTypeService.Update(maintenanceType);
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
            await this.maintenanceTypeService.Delete(id);
        }

        /// <summary>
        /// Gets the maintenance types.
        /// </summary>
        /// <returns>The list of maintenance types.</returns>
        [HttpGet("getmaintenancetypes")]
        public async Task<List<DataSelectionModel>> GetMaintenanceTypes()
        {
            return await this.maintenanceTypeService.GetMaintenanceTypes();
        }
    }
}