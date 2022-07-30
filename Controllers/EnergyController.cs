//-----------------------------------------------------------------------
// <copyright file="EnergyController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Energy controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The energy controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class EnergyController : Controller
    {
        /// <summary>
        /// The energy service
        /// </summary>
        private IEnergyService engergyService;

        /// <summary>
        /// The equipment shift service
        /// </summary>
        private IEquipmentShiftService equipmentShiftService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyController" /> class.
        /// </summary>
        /// <param name="engergyService">The engergy service.</param>
        /// <param name="equipmentShiftService">The equipment shift service.</param>
        /// <exception cref="ArgumentNullException">engergyService</exception>
        public EnergyController(IEnergyService engergyService, IEquipmentShiftService equipmentShiftService)
        {
            this.engergyService = engergyService ?? throw new ArgumentNullException("engergyService");
            this.equipmentShiftService = equipmentShiftService ?? throw new ArgumentNullException("equipmentShiftService");
        }

        /// <summary>
        /// Gets the equipment energy.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">The from date</param>
        /// <param name="toDate">The to date</param>
        /// <returns>Equipment engergy</returns>
        /// <exception cref="System.ArgumentNullException">equipmentId</exception>
        [HttpGet("{equipmentId}")]
        public async Task<EnergyDataModel> GetEquipmentEnergy(long equipmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            var equipmentShift = this.equipmentShiftService.GetShift(equipmentId, fromDate, toDate);
            if (equipmentShift != null)
            {
                return await this.engergyService.GetEquipmentEnergy(equipmentId, equipmentShift.Id);
            }
            else
            {
                return new EnergyDataModel();
            }
        }

        /// <summary>
        /// Gets the equipment energy.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <returns>
        /// Equipment engergy
        /// </returns>
        [HttpGet("factory/{factoryId}")]
        public async Task<EnergyDashboardResponsModel> GetFactoryEnergy(long factoryId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.engergyService.GetFactoryEnergy(factoryId, 0);
        }

        /// <summary>
        /// Gets the equipment energy.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>
        /// Equipment engergy
        /// </returns>
        [HttpGet("factory/{factoryId}/{groupId}")]
        public async Task<EnergyDashboardResponsModel> GetFactoryEnergy(long factoryId, long groupId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.engergyService.GetFactoryEnergy(factoryId, groupId);
        }
    }
}
