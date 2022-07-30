//-----------------------------------------------------------------------
// <copyright file="AssemblyLineController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Dashboards controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Models.Telemetry;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The dashboards controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class AssemblyLineController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IAssemblyLineService assemblyLineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLineController"/> class.
        /// </summary>
        /// <param name="assemblyLineService">The assembly line service.</param>
        /// <exception cref="ArgumentNullException">assemblyLineService</exception>
        public AssemblyLineController(IAssemblyLineService assemblyLineService)
        {
            this.assemblyLineService = assemblyLineService ?? throw new ArgumentNullException("assemblyLineService");
        }

        /// <summary>
        /// Gets the assembly line.
        /// </summary>
        /// <param name="factoryId">The factory id identifier.</param>
        /// <param name="groupId">The group id identifier.</param>
        /// <param name="state">The equipment running state</param>
        /// <param name="reasonCode">The downtime reason code.</param>
        /// <returns>Assembly Line</returns>
        /// <exception cref="System.ArgumentNullException">equipmentId</exception>
        [HttpGet("assembly")]
        public async Task<OeeDashboardResponseModel> GetAssemblyLine(long factoryId, long? groupId, int? state, string reasonCode)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.assemblyLineService.GetAssemblyLine(factoryId, groupId, state, reasonCode);
        }

        /// <summary>
        /// Gets the assembly line.
        /// </summary>
        /// <param name="equipId">The equipment id identifier.</param>
        /// <param name="fromDate">The from Date identifier.</param>
        /// <param name="toDate">The to Date identifier</param>
        /// <returns>Assembly Line</returns>
        /// <exception cref="System.ArgumentNullException">equipmentId</exception>
        [HttpGet("assemblyTrend")]
        public async Task<AssemblyLineTelemetryModel> GetAssemblyLineTrend(long equipId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            return await this.assemblyLineService.GetAssemblyLineTrendChart(equipId, fromDate, toDate);
        }
    }
}
