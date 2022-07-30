//-----------------------------------------------------------------------
// <copyright file="SilosController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Silos controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.RequestModels;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Models.Telemetry;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The silos controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class SilosController : Controller
    {
        /// <summary>
        /// The telemetry service
        /// </summary>
        private ISilosTelemetryService silosTelemetryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SilosController"/> class.
        /// </summary>
        /// <param name="silosTelemetryService">The silos telemetry service.</param>
        /// <exception cref="ArgumentNullException">telemetryService</exception>
        public SilosController(ISilosTelemetryService silosTelemetryService)
        {
            this.silosTelemetryService = silosTelemetryService ?? throw new ArgumentNullException("silosTelemetryService");
        }

        /// <summary>
        /// Gets the tag trend.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        /// <returns>The list of silos trends.</returns>
        [HttpPost]
        [Route("trend/{equipmentId}")]
        public async Task<IEnumerable<SilosTelemetryModel>> GetTagTrend(long equipmentId, [FromBody]TrendRequestModel trendRequestModel)
        {
            string fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime().ToString("o");
            string toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime().ToString("o");

            return await this.silosTelemetryService.GetTrend(equipmentId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the dashboard data.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The silos equipment data.</returns>
        [HttpGet]
        [Route("dashboard/{equipmentId}")]
        public async Task<SilosEquipmentDashboardResponseModel> GetDashboardData(long equipmentId)
        {
            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            return await this.silosTelemetryService.GetSilosEquipmentData(equipmentId);
        }
    }
}