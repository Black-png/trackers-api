//-----------------------------------------------------------------------
// <copyright file="DashboardsController.cs" company="ThingTrax UK Ltd">
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
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The dashboards controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class DashboardsController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IDashboardsService dashboardsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardsController"/> class.
        /// </summary>
        /// <param name="dashboardsService">The dashboards service.</param>
        /// <exception cref="ArgumentNullException">dashboardsService</exception>
        public DashboardsController(IDashboardsService dashboardsService)
        {
            this.dashboardsService = dashboardsService ?? throw new ArgumentNullException("dashboardsService");
        }

        /// <summary>
        /// Siloses the dashboard.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The list of equipments.</returns>
        [HttpGet("silos/{factoryId}/{groupId?}")]
        public async Task<IEnumerable<SilosDashboardResponseModel>> SilosDashboard(long factoryId, long? groupId)
        {
            return await this.dashboardsService.GetSilosDashboardData(factoryId, groupId);
        }

        /// <summary>
        /// Auxiliaries the dashboard.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The list of auxiliary dashboard respnse model.</returns>
        [HttpGet("auxiliary")]
        public async Task<AuxiliaryDashboardResponseModel> AuxiliaryDashboard(long factoryId, long? groupId)
        {
            return await this.dashboardsService.GetAuxiliaryDashboardData(factoryId, groupId);
        }

        /// <summary>
        /// Gets the aux equipment dashboar data.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The auxiliary equipment dashboar response model.</returns>
        [HttpGet("auxequipmentdashboard/{equipmentId}")]
        public async Task<AuxEquipmentDashboardResponseModel> GetAuxEquipmentDashboarData(long equipmentId)
        {
            return await this.dashboardsService.GetAuxEquipmentDashboarData(equipmentId);
        }

        /// <summary>
        /// Oees the dashboard.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="state">The equipment state.</param>
        /// <param name="reasonCode">The downtime reason code.</param>
        /// <returns>The list of oee dashboard response models.</returns>
        [HttpGet("circleoee")]
        public async Task<OeeCircleDashboardResponseModel> OeeDashboard(long factoryId, long? groupId, int? state, string reasonCode)
        {
            return await this.dashboardsService.GetCircleOeeDashboardData(factoryId, groupId, state, reasonCode);
        }

        /// <summary>
        /// Oees the dashboard.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="state">The equipment state.</param>
        /// <param name="reasonCode">The downtime reason code.</param>
        /// <returns>
        /// The list of oee dashboard response models.
        /// </returns>
        [HttpGet("oeedetail")]
        public async Task<OeeDetailDashboardResponseModel> OeeDetailDashboard(long factoryId, long? groupId, int? state, string reasonCode)
        {
            return await this.dashboardsService.GetShiftOeeDetailDashboardData(factoryId, groupId, state, reasonCode);
        }

        /// <summary>
        /// Gets the equipment oee.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The equipment oee.</returns>
        [HttpGet("equipmentoee/{equipmentId}")]
        public async Task<EquipmentOeeResponseModel> GetEquipmentOee(long equipmentId)
        {
            return await this.dashboardsService.GetEquipmentOeeDetails(equipmentId);
        }

        /// <summary>
        /// Gets the equipment oee.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <param name="state">The equipment running state</param>
        /// <param name="reasonCode">The downtime reason code.</param>
        /// <returns>Equipment OEE</returns>
        /// <exception cref="System.ArgumentNullException">equipmentId</exception>
        [HttpGet("oee")]
        public async Task<OeeDashboardResponseModel> GetFactoryOEE([FromQuery] long factoryId, [FromQuery] int? state, [FromQuery] string reasonCode)
        {
            string[] stringArray = this.Request.Query["groupIds"].ToString().Split(',').Where(x => !string.IsNullOrEmpty(x) && x != "null").ToArray();
            List<long> groupIds = stringArray?.Select(x => Convert.ToInt64(x)).ToList() ?? null;
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (groupIds != null && groupIds.Count == 0)
            {
                groupIds = null;
            }

            return await this.dashboardsService.GetFactoryOee(factoryId, groupIds, state, reasonCode);
        }
    }
}