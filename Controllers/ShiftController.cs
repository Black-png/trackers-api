//-----------------------------------------------------------------------
// <copyright file="ShiftController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Factory shift controller class.</summary>
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
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Shift controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class ShiftController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IShiftService shiftService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="shiftService">The shift service.</param>
        /// <exception cref="ArgumentNullException">shiftService</exception>
        public ShiftController(IOptions<ApplicationSettings> applicationSettingsConfig, IShiftService shiftService)
        {
            this.shiftService = shiftService ?? throw new ArgumentNullException("shiftService");
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
        public async Task<IEnumerable<Shift>> Get()
        {
            var group = await this.shiftService.GetAll();
            return group.OrderBy(f => f.Name);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("{id}")]
        public async Task<Shift> Get(long id)
        {
            return await this.shiftService.Get(id);
        }

        /// <summary>
        /// Gets the list of shifts.
        /// </summary>
        /// <returns>The list of shifts.</returns>
        /// <param name="pageNo">Page Number.</param>
        /// <param name="searchText">Serach text.</param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Shift>, int> GetSearched(int pageNo, string searchText)
        {
            var shifts = this.shiftService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(shifts, totalCount);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("GetShiftByFactoryId/{factoryId}")]
        public async Task<IEnumerable<Shift>> GetShiftByFactoryId(long factoryId)
        {
            return await this.shiftService.GetShiftByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="beginDatetime">The begin date time.</param>
        /// <param name="endDatetime">The end date time.</param>
        /// <param name="id">The factoryshift identifier.</param>
        /// <returns>The falg according to availbility</returns>
        [HttpGet("isShiftAvailable/{factoryId}/{beginDatetime}/{endDatetime}/{id}")]
        public async Task<bool> IsShiftAvailable(long factoryId, DateTimeOffset beginDatetime, DateTimeOffset endDatetime, long id)
        {
            return await this.shiftService.IsShiftAvailable(factoryId, id, beginDatetime, endDatetime);
        }

        /// <summary>
        /// Posts the specified factory.
        /// </summary>
        /// <param name="shift">The factory.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Shift> Post([FromBody]Shift shift)
        {
            return await this.shiftService.Create(shift);
        }

        /// <summary>
        /// Puts the specified factory.
        /// </summary>
        /// <param name="shift">The group.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Shift shift)
        {
            await this.shiftService.Update(shift);
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
            await this.shiftService.Delete(id);
        }
    }
}
