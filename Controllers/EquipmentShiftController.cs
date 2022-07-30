//-----------------------------------------------------------------------
// <copyright file="EquipmentShiftController.cs" company="ThingTrax UK Ltd">
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
    /// Equipment shift controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class EquipmentShiftController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IEquipmentShiftService equipmentShiftService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentShiftController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="equipmentShiftService">The shift service.</param>
        /// <exception cref="ArgumentNullException">shiftService</exception>
        public EquipmentShiftController(IOptions<ApplicationSettings> applicationSettingsConfig, IEquipmentShiftService equipmentShiftService)
        {
            this.equipmentShiftService = equipmentShiftService ?? throw new ArgumentNullException("factoryShiftService");
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
        public async Task<IEnumerable<EquipmentShift>> Get()
        {
            return await this.equipmentShiftService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("{id}")]
        public async Task<EquipmentShift> Get(long id)
        {
            return await this.equipmentShiftService.Get(id);
        }

        /// <summary>
        /// Gets the list of equipment shifts.
        /// </summary>
        /// <returns>The list of equipment shifts.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<EquipmentShift>, int> GetSearched(int pageNo, string searchText)
        {
            var equipmentShift = this.equipmentShiftService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(equipmentShift, totalCount);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="equipmentId">The factory identifier.</param>
        /// <returns>current equipment shift Shift</returns>
        [HttpGet("currentshift/{equipmentId}")]
        public async Task<EquipmentShift> GetCurrentShiftByEquipmentId(long equipmentId)
        {
            return await this.equipmentShiftService.GetCurrentShiftByEquipmentId(equipmentId);
        }

        /// <summary>
        /// Gets the equipment shift by equipment identifier.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="dateTimeOffset">The date.</param>
        /// <returns>The equipment shift</returns>
        [HttpGet("GetEquipmentShift/{equipmentId}/{dateTimeOffset}")]
        public async Task<EquipmentShift> GetEquipmentShiftByEquipmentId(long equipmentId, DateTimeOffset dateTimeOffset)
        {
            return await this.equipmentShiftService.GetShift(equipmentId, dateTimeOffset);
        }

        /// <summary>
        /// Posts the specified factory shift.
        /// </summary>
        /// <param name="equipmentShift">The factory shift.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<EquipmentShift> Post([FromBody]EquipmentShift equipmentShift)
        {
            return await this.equipmentShiftService.Create(equipmentShift);
        }

        /// <summary>
        /// Puts the specified factory shift.
        /// </summary>
        /// <param name="equipmentShift">The factory shift.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]EquipmentShift equipmentShift)
        {
            await this.equipmentShiftService.Update(equipmentShift);
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
            await this.equipmentShiftService.Delete(id);
        }
    }
}
