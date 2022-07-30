//-----------------------------------------------------------------------
// <copyright file="FactoryShiftController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Factory shift controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Factory shift controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class FactoryShiftController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IFactoryShiftService factoryShiftService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryShiftController" /> class.
        /// </summary>
        /// <param name="factoryShiftService">The factory shift service.</param>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        public FactoryShiftController(IOptions<ApplicationSettings> applicationSettingsConfig, IFactoryShiftService factoryShiftService)
        {
            this.factoryShiftService = factoryShiftService ?? throw new ArgumentNullException("factoryShiftService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Groups.</returns>
        [HttpGet]
        public async Task<IEnumerable<FactoryShift>> Get()
        {
            return await this.factoryShiftService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("{id}")]
        public async Task<FactoryShift> Get(long id)
        {
            return await this.factoryShiftService.Get(id);
        }

        /// <summary>
        /// Gets the factory shift by shift and date.
        /// </summary>
        /// <param name="shiftId">The shift identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <returns>The factory shift.</returns>
        [HttpGet("factoryshiftbyshiftanddate/{shiftId}/{fromDate}")]
        public async Task<FactoryShift> GetFactoryShiftByShiftAndDate(long shiftId, DateTimeOffset fromDate)
        {
            return await this.factoryShiftService.GetFactoryShiftByShiftAndDate(shiftId, fromDate);
        }

        /// <summary>
        /// Gets the list of factory shifts.
        /// </summary>
        /// <returns>The list of factory shifts.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<FactoryShift>, int> GetSearched(int pageNo, string searchText)
        {
            var equipments = this.factoryShiftService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(equipments, totalCount);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>current factory Shift</returns>
        [HttpGet("currentshift/{factoryId}")]
        public async Task<FactoryShift> GetCurrentShiftByFactoryId(long factoryId)
        {
            return await this.factoryShiftService.GetCurrentShiftByFactoryId(factoryId);
        }

        /// <summary>
        /// Posts the specified factory shift.
        /// </summary>
        /// <param name="factoryShift">The factory shift.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = RolesConstant.Admin)]
        [HttpPost]
        public async Task<FactoryShift> Post([FromBody]FactoryShift factoryShift)
        {
            return await this.factoryShiftService.Create(factoryShift);
        }

        /// <summary>
        /// Puts the specified factory shift.
        /// </summary>
        /// <param name="factoryShift">The factory shift.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = RolesConstant.Admin)]
        [HttpPut]
        public async Task Put([FromBody]FactoryShift factoryShift)
        {
            await this.factoryShiftService.Update(factoryShift);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = RolesConstant.Admin)]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.factoryShiftService.Delete(id);
        }

        /// <summary>
        /// Gets the factory shift by time range.
        /// </summary>
        /// <param name="factoryId">factory Id.</param>
        /// <param name="fromDateTime">From date time.</param>
        /// <param name="toDateTime">To date time.</param>
        /// <returns>The factory shift.</returns>
        [HttpGet("GetFactoryshiftByTimeRange/{factoryId}/{fromDateTime}/{toDateTime}")]
        public async Task<FactoryShift> GetFactoryShiftByTimeRange(long factoryId, DateTimeOffset fromDateTime, DateTimeOffset toDateTime)
        {
            return await this.factoryShiftService.GetFactoryshiftByTimeRange(factoryId, fromDateTime, toDateTime);
        }
    }
}
