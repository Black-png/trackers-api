//-----------------------------------------------------------------------
// <copyright file="GlobalDashboardController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Dashboards controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The global dashboard controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalDashboardController : ControllerBase
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IGlobalDashboardService globalDashboardService;

        /// <summary>
        /// The entity service
        /// </summary>
        private IDashboardsService dashboardsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalDashboardController"/> class.
        /// </summary>
        /// <param name="globalDashboardService">The global dashboard service.</param>
        /// <param name="dashboardsService">The global dashboard.</param>
        /// <exception cref="ArgumentNullException">globalDashboardService</exception>
        public GlobalDashboardController(IGlobalDashboardService globalDashboardService, IDashboardsService dashboardsService)
        {
            this.globalDashboardService = globalDashboardService ?? throw new ArgumentNullException("globalDashboardService");
            this.dashboardsService = dashboardsService ?? throw new ArgumentNullException("dashboardsService");
        }

        /// <summary>
        /// Gets the global dashboard KPI data.
        /// </summary>
        /// <param name="fromDateTime">The global dashboard KPI data start datetime .</param>
        /// <param name="toDateTime">The global dashboard KPI data end datetime.</param>
        /// <returns>Global Dashboard KPI Data</returns>
        // /// <exception cref="System.ArgumentNullException">equipmentId</exception>
        [HttpGet]
        public async Task<GlobalDashboardResponseModel> GetGlobalDashboardDataAsync(DateTimeOffset fromDateTime, DateTimeOffset toDateTime)
        {
            return await this.globalDashboardService.GetGlobalDashboardDataAsync(fromDateTime, toDateTime);
        }

        /// <summary>
        /// Gets the global dashboard KPI data.
        /// </summary>
        /// <param name="fromDateTime">The global dashboard KPI data start datetime .</param>
        /// <param name="toDateTime">The global dashboard KPI data end datetime.</param>
        /// <param name="id">The Factory id.</param>
        /// <returns>Global Dashboard KPI Data</returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
        [HttpGet("GetFactoryDataById")]
        public async Task<GlobalDashboardResponseModel> GetFactory(DateTimeOffset fromDateTime, DateTimeOffset toDateTime, long id)
        {
            return await this.globalDashboardService.GetFactoryDataById(fromDateTime, toDateTime, id);
        }

        /// <summary>
        /// Gets the Compare factory Results data.
        /// </summary>
        /// <returns>Compare factory Results Data</returns>
        [HttpGet("GetGlobalDashboard")]
        public async Task<GlobalDashboardModel> GetGlobaldashboardResult()
        {
            return await this.globalDashboardService.GetGlobalDashboard();
        }

        /// <summary>
        /// Gets the Compare factory Results data.
        /// </summary>
        /// <returns>Compare factory Results Data</returns>
        [HttpGet("GetFactoryConfigurations")]
        public async Task<List<FactoriesConfiguration>> GetFactoryConfigurations()
        {
            return await this.globalDashboardService.GetFactoryConfigurations();
        }

        /// <summary>
        /// Gets the last twelve months availability data.
        /// </summary>
        /// <returns>The LTM data</returns>
        [HttpGet("ltmAvailability")]
        public async Task<IActionResult> GetLTMAvailabilityAsync()
        {
            return this.Ok(await this.globalDashboardService.GetLTMAvailabilityAsync());
        }

        /// <summary>
        /// Gets the top five plants oee data.
        /// </summary>
        /// <param name="fromDateTime">start period</param>
        /// <param name="toDateTime">end period</param>
        /// <returns>The top five plants oee data</returns>
        [HttpGet("topfiveplants")]
        public IActionResult GetTopFivePerformingPlantsAsync(DateTimeOffset fromDateTime, DateTimeOffset toDateTime)
        {
            return this.Ok(this.globalDashboardService.GetTopFivePerformingPlants(fromDateTime, toDateTime));
        }
    }
}
