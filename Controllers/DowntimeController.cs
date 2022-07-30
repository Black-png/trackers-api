//-----------------------------------------------------------------------
// <copyright file="DowntimeController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Downtime controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.RequestModels;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Downtime controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class DowntimeController : Controller
    {
        /// <summary>
        /// The downtime service
        /// </summary>
        private IDowntimeService downtimeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DowntimeController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="downtimeService">The entity service.</param>
        public DowntimeController(IOptions<ApplicationSettings> applicationSettingsConfig, IDowntimeService downtimeService)
        {
            this.downtimeService = downtimeService ?? throw new ArgumentNullException("downtimeService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Downtime</returns>
        [HttpGet]
        public async Task<IEnumerable<Downtime>> Get()
        {
            var downTime = await this.downtimeService.GetAll();
            return downTime.OrderBy(dt => dt.Created);
        }

        /// <summary>
        /// Gets the list of downtimes.
        /// </summary>
        /// <returns>The list of downtimes.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        /// <param name="equipmentId">The equipment identifier.</param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<DowntimeResponseModel>, int> Get(int pageNo, string searchText, long? equipmentId)
        {
            var downtimes = this.downtimeService.GetAllDowntimes(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount, equipmentId);
            return Tuple.Create(downtimes, totalCount);
        }

        /// <summary>
        /// Gets the list of downtimes.
        /// </summary>
        /// <returns>The list of downtimes.</returns>
        /// <param name="factoryId">The factory Id.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        [HttpPost("GetDownTime/{factoryId}")]
        public Task<Tuple<Dictionary<string, List<DownTimeModel>>, Dictionary<string, List<DownTimeModel>>>> GetDownTime(long factoryId, [FromBody]TrendRequestModel trendRequestModel)
        {
            DateTimeOffset fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime();
            DateTimeOffset toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime();

            var downtimes = this.downtimeService.GetAllDowntimesByTime(fromDate, toDate, factoryId);
            return downtimes;
        }

        /// <summary>
        /// Gets the list of downtimes.
        /// </summary>
        /// <returns>The list of downtimes.</returns>
        /// <param name="reasonId">The reason Id.</param>
        /// <param name="factoryId">The factory Id.</param>
        /// <param name="trendRequestModel">The trend request model.</param>
        [HttpPost("GetDownTimeWithToolId/{reasonId}/{factoryId}")]
        public Task<Tuple<Dictionary<string, List<DownTimeModel>>, Dictionary<long?, List<DownTimeModel>>>> GetDownTimeWithToolId(int reasonId, long factoryId, [FromBody]TrendRequestModel trendRequestModel)
        {
            DateTimeOffset fromDate = Convert.ToDateTime(trendRequestModel.FromDate).ToUniversalTime();
            DateTimeOffset toDate = Convert.ToDateTime(trendRequestModel.ToDate).ToUniversalTime();

            var downtimes = this.downtimeService.GetDownTimeWithToolId(fromDate, toDate, reasonId, factoryId);
            return downtimes;
        }

        /// <summary>
        /// Closes any open down times.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The task</returns>
        [HttpPost]
        [Route("CloseOpenDownTimes/{equipmentId}")]
        public async Task CloseAnyOpenDownTimes(long equipmentId)
        {
            await this.downtimeService.CloseAnyOpenDownTimes(equipmentId);
        }

        /// <summary>
        /// Gets the specified downtime identifier.
        /// </summary>
        /// <param name="downtimeId">The downtime identifier.</param>
        /// <returns>Downtime</returns>
        [HttpGet("{downtimeId}")]
        public async Task<Downtime> Get(long downtimeId)
        {
            return await this.downtimeService.Get(downtimeId);
        }

        /// <summary>
        /// Gets the downtime list.
        /// </summary>
        /// <param name="equipmentId">The equipemnt identifier.</param>
        /// <returns> Downtime list</returns>
        [HttpGet("equipment/{equipmentId}")]
        public async Task<IEnumerable<Downtime>> GetByEquipmentId(string equipmentId)
        {
            return await this.downtimeService.GetDowntimeList(equipmentId);
        }

        /// <summary>
        /// Gets the downtime list.
        /// </summary>
        /// <param name="equipmentId">The equipemnt identifier.</param>
        /// <returns> Downtime list</returns>
        [HttpGet("GetByEquipmentId/{equipmentId}")]
        public IEnumerable<DowntimeResponseModel> GetByEquipmentId(long equipmentId)
        {
            return this.downtimeService.GetDowntimeList(equipmentId);
        }

        /// <summary>
        /// Get the equipment downtime for Current Shift.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>Down time</returns>
        [HttpGet("currentShift/{equipmentId}")]
        public async Task<IEnumerable<Downtime>> GetEquipmentDowntimesForCurrentShift(long equipmentId)
        {
            return await this.downtimeService.GetEquipmentDowntimesForCurrentShift(equipmentId);
        }

        /// <summary>
        /// Gets the equipment downtimes for time range.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        /// <returns>The list of downtimes.</returns>
        [HttpGet("CheckExistingDowntimes/{equipmentId}/{fromTime}/{toTime?}")]
        public async Task<List<Downtime>> GetEquipmentDowntimesForTimeRange(long equipmentId, DateTimeOffset fromTime, DateTimeOffset? toTime)
        {
            return await this.downtimeService.GetEquipmentDowntimesForTimeRange(equipmentId, fromTime, toTime);
        }

        /// <summary>
        /// Posts the specified downtime.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>
        /// The task
        /// </returns>
        [HttpGet("CheckIfIndefiniteDowntimeExists/{equipmentId}")]
        public async Task<bool> CheckIfIndefiniteDowntimeExists(long equipmentId)
        {
            return await this.downtimeService.CheckIfIndefiniteDowntimeExists(equipmentId, null);
        }

        /// <summary>
        /// Posts the specified downtime.
        /// </summary>
        /// <param name="equipmentId">The equipment.</param>
        /// <param name="fromdate">The fromdate .</param>
        /// <param name="todate">The todate .</param>
        /// <returns>
        /// The task
        /// </returns>
        [HttpGet("DowntimeRangeCheck/{equipmentId}/{fromdate}/{todate}")]
        public async Task<bool> DowntimeRangeCheck(long equipmentId, DateTimeOffset fromdate, DateTimeOffset todate)
        {
            return await this.downtimeService.CheckIfDateTimeRangeDowntimeExists(equipmentId, fromdate, todate);
        }

        /// <summary>
        /// Posts the specified downtime.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="downtimeId">The downtime identifier.</param>
        /// <returns>
        /// The task
        /// </returns>
        [HttpGet("CheckIfIndefiniteDowntimeExists/{equipmentId}/{downtimeId}")]
        public async Task<bool> CheckIfIndefiniteDowntimeExists(long equipmentId, long? downtimeId)
        {
            return await this.downtimeService.CheckIfIndefiniteDowntimeExists(equipmentId, downtimeId);
        }

        /// <summary>
        /// Posts the specified downtime.
        /// </summary>
        /// <param name="downTimeModel">The downTimeModel.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Downtime> Post([FromBody]DownTimeRequestModel downTimeModel)
        {
            if (!downTimeModel.AllEquipments)
            {
                return await this.downtimeService.CreateDowntime(downTimeModel.DownTime);
            }
            else
            {
                return await this.downtimeService.Create(downTimeModel.DownTime, downTimeModel.FactoryId);
            }
        }

        /// <summary>
        /// Posts the specified downtime.
        /// </summary>
        /// <param name="downTime">The downTime.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost("createopendowntime")]
        public async Task<Downtime> CreateOpenDowntime([FromBody]Downtime downTime)
        {
                return await this.downtimeService.CreateOpenDowntime(downTime);
        }

        /// <summary>
        /// Adds the unplanned downtime.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The Task</returns>
        [HttpPost("AddUnplannedDowntime")]
        public async Task AddUnplannedDowntime([FromBody]UnplannedDowntimeRequestModel model)
        {
            await this.downtimeService.AddUnplannedDowntime(model.EquipmentId, model.PlannedStart);
        }

        /// <summary>
        /// Puts the specified downtime.
        /// </summary>
        /// <param name="downtime">The downtime.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task<Downtime> Put([FromBody]Downtime downtime)
        {
            if (downtime == null)
            {
                throw new ArgumentNullException("downtime");
            }

            if (downtime.Id < 1)
            {
                throw new ArgumentNullException("downtime.Id");
            }

            if (downtime.DowntimeReasonId < 1)
            {
                throw new ArgumentNullException("downtime.DowntimeReasonId");
            }

            if (downtime.EquipmentId < 1)
            {
                throw new ArgumentNullException("downtime.EquipmentId");
            }

            return await this.downtimeService.UpdateDowntime(downtime);
        }

        /// <summary>
        /// Puts the specified downtime.
        /// </summary>
        /// <param name="downtime">The downtime.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut("updateReason")]
        public async Task UpdateReason([FromBody]Downtime downtime)
        {
            if (downtime == null)
            {
                throw new ArgumentNullException("downtime");
            }

            if (downtime.DowntimeReasonId < 1)
            {
                throw new ArgumentNullException("downtime.DowntimeReasonId");
            }

            if (downtime.EquipmentId < 1)
            {
                throw new ArgumentNullException("downtime.EquipmentId");
            }

            await this.downtimeService.UpdateReason(downtime);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.downtimeService.Delete(id);
        }
    }
}