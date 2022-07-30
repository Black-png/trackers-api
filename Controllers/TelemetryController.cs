//-----------------------------------------------------------------------
// <copyright file="TelemetryController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Telemetry controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models;
    using TT.Core.Models.RequestModels;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Models.Telemetry;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Telemetry controller class.
    /// </summary>
    [Route("api/[controller]")]
    public class TelemetryController : Controller
    {
        /// <summary>
        /// The telemetry service
        /// </summary>
        private ITelemetryService telemetryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryController"/> class.
        /// </summary>
        /// <param name="telemetryService">The telemetry service.</param>
        /// <exception cref="System.ArgumentNullException">factoryService</exception>
        public TelemetryController(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService ?? throw new ArgumentNullException("telemetryService");
        }

        /// <summary>
        /// Gets the telemetry.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The telemetry</returns>
        [HttpGet]
        [Route("{equipmentId}")]
        public async Task<EquipmentTelemetryModel> GetTelemetry(long equipmentId)
        {
            var result = await this.telemetryService.GetTelemetry(equipmentId);

            if (result == null)
            {
                return new EquipmentTelemetryModel();
            }

            return result;
        }

        /// <summary>
        /// Gets the Dashboard Data.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The DashboardResponseModel</returns>
        [HttpGet]
        [Route("dashboard/{equipmentId}")]
        public async Task<DashboardResponseModel> GetDashboardData(long equipmentId)
        {
            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            return await this.telemetryService.GetDashboardData(equipmentId);
        }

        /// <summary>
        /// Gets the telemetry trend.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        /// <returns>The trend data</returns>
        [HttpPost]
        [Route("tagtrend/{equipmentId}/{tagName}")]
        public async Task<EquipmentTrendModel> GetTagTrend(long equipmentId, string tagName, [FromBody]TrendRequestModel trendRequestModel)
        {
            string fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime().ToString("o");
            string toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime().ToString("o");

            return await this.telemetryService.GetTagTrend(equipmentId, fromDate, toDate, tagName);
        }

        /// <summary>
        /// Gets the telemetry trend.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        /// <returns>The trend data</returns>
        [HttpPost]
        [Route("alltrends/{equipmentId}")]
        public async Task<IEnumerable<EquipmentTrendModel>> GetAllTagTrend(long equipmentId, [FromBody]TrendRequestModel trendRequestModel)
        {
            string fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime().ToString("o");
            string toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime().ToString("o");

            var tagsTrend = await this.telemetryService.GetAllTagsTrend(equipmentId, fromDate, toDate);
            return tagsTrend;
        }

        /// <summary>
        /// Gets the telemetry trend.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        /// <returns>The trend data</returns>
        [HttpPost]
        [Route("equipmenttrend/{equipmentId}")]
        public async Task<EquipmentTrendResponseModel> GetEquipmentTrend(long equipmentId, [FromBody]TrendRequestModel trendRequestModel)
        {
            string fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime().ToString("o");
            string toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime().ToString("o");

            var tagsTrend = await this.telemetryService.GetEquipmentTrend(equipmentId, fromDate, toDate);
            return tagsTrend;
        }

        /// <summary>
        /// Puts the specified rejection.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        /// <returns>The task</returns>
        [HttpPut]
        public async Task Put([FromBody]TelemetryViewModel telemetry)
        {
            if (telemetry.EquipmentId < 0)
            {
                throw new ArgumentNullException("telemetry.EquipmentId");
            }

            if (string.IsNullOrEmpty(telemetry.CycleDatetime))
            {
                throw new ArgumentNullException("telemetry.CycleDatetime");
            }

            if (string.IsNullOrEmpty(telemetry.Cycletime))
            {
                throw new ArgumentNullException("telemetry.Cycletime");
            }

            if (string.IsNullOrEmpty(telemetry.Remark))
            {
                throw new ArgumentNullException("telemetry.Remark");
            }

            await this.telemetryService.UpdateRemarks(telemetry);
        }

        /// <summary>
        /// Gets the telemetry trend.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        /// <returns>The trend data</returns>
        [HttpPost]
        [Route("equipmenttrends/{equipmentId}")]
        public async Task<EquipmentTrendResponseModel> GetEquipmentTrends(long equipmentId, [FromBody]TrendRequestModel trendRequestModel)
        {
            string fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime().ToString("o");
            string toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime().ToString("o");
            var tagsTrend = await this.telemetryService.GetEquipmentTrends(equipmentId, fromDate, toDate);
            return tagsTrend;
        }
    }
}