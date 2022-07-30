//-----------------------------------------------------------------------
// <copyright file="EquipmentController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Equipment controller class.</summary>
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
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Equipment controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class EquipmentController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IEquipmentService equipmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="equipmentService">The entity service.</param>
        public EquipmentController(IOptions<ApplicationSettings> applicationSettingsConfig, IEquipmentService equipmentService)
        {
            this.equipmentService = equipmentService ?? throw new ArgumentNullException("equipmentService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Data and count</returns>
        [HttpGet]
        public async Task<IEnumerable<Equipment>> Get()
        {
            var equipment = await this.equipmentService.GetAll();
            return equipment.OrderBy(e => e.EquipmentId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="equipmentId">The equipmentId identifier.</param>
        /// <returns>Data and count</returns>
        [HttpGet("packing/getpackingdevices")]
        public async Task<IEnumerable<GetAllPackingDevicesResponseModel>> GetAllPackingDevices(long equipmentId)
        {
            var equipment = await this.equipmentService.GetAllPackingDevices(equipmentId);
            return equipment.OrderBy(e => e.Id);
        }

        /// <summary>
        /// Gets the specified identifier. GET api/equipment/{id}
        /// </summary>
        /// <param name="id">The equipment identifier.</param>
        /// <returns>The equipment</returns>
        [HttpGet("{id}")]
        public async Task<Equipment> Get(long id)
        {
            return await this.equipmentService.GetEquipmentById(id);
        }

        /// <summary>
        /// Gets the list of equipments.
        /// </summary>
        /// <returns>The list of equipments.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Equipment>, int> Get(int pageNo, string searchText)
        {
            var equipments = this.equipmentService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(equipments, totalCount);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("factory/{factoryId}")]
        public async Task<IEnumerable<Equipment>> GetByFactoryId(long factoryId)
        {
            return await this.equipmentService.GetEquipmentsByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("factory/{factoryId}/{groupId}")]
        public async Task<IEnumerable<Equipment>> GetByGroupId(long factoryId, long groupId)
        {
            return await this.equipmentService.GetEquipmentsByGroupId(factoryId, groupId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("IsIdAvailable/{equipmentId}")]
        public async Task<bool> IsIdAvailable(string equipmentId)
        {
            return await this.equipmentService.IsEquipmentAvailable(equipmentId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="deviceId">The deviceId identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("IsDeviceIdAvailable/{deviceId}")]
        public async Task<bool> IsDeviceIdAvailable(string deviceId)
        {
            return await this.equipmentService.IsDeviceIdAvailable(deviceId);
        }

        /// <summary>
        /// Posts the specified equipment.
        /// </summary>
        /// <param name="equipment">The equipment.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Equipment> Post([FromBody]Equipment equipment)
        {
          return await this.equipmentService.Create(equipment);
        }

        /// <summary>
        /// Puts the specified equipment.
        /// </summary>
        /// <param name="equipment">The equipment.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Equipment equipment)
        {
            await this.equipmentService.Update(equipment);
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
            await this.equipmentService.Delete(id);
        }

        /// <summary>
        /// Gets the equipments by group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The list of equipmentid and id list.</returns>
        [HttpGet("getequipmentsbygroup/{groupId}")]
        public async Task<List<DataSelectionModel>> GetEquipmentsByGroup(long groupId)
        {
            return await this.equipmentService.GetEquipmentsByGroup(groupId);
        }

        /// <summary>
        /// Gets the equipment runtime.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The equipment cumulative runtime</returns>
        [HttpGet("getequipmentruntime/{equipmentId}")]
        public double GetEquipmentRuntime(long equipmentId)
        {
            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            return this.equipmentService.GetEquipmentRuntime(equipmentId);
        }
    }
}
