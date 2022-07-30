//-----------------------------------------------------------------------
// <copyright file="EquipmentAlertsController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Equipment alert controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The equipment alerts controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class EquipmentAlertsController : Controller
    {
        /// <summary>
        /// The equipment alert service
        /// </summary>
        private IEquipmentAlertService equipmentAlertService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentAlertsController"/> class.
        /// </summary>
        /// <param name="equipmentAlertService">The equipment alert service.</param>
        /// <exception cref="ArgumentNullException">equipmentAlertService</exception>
        public EquipmentAlertsController(IEquipmentAlertService equipmentAlertService)
        {
            this.equipmentAlertService = equipmentAlertService ?? throw new ArgumentNullException("equipmentAlertService");
        }

        /// <summary>
        /// Gets the equipmet active alerts for a factory.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The list of active alerts.</returns>
        [HttpGet("EquipmentActiveAlerts")]
        public async Task<IEnumerable<EquipmentAlertDashboardResponseModel>> GetEquipmentActiveAlerts(long factoryId, long? equipmentId, DateTimeOffset? fromdate, DateTimeOffset? toDate)
        {
            return await this.equipmentAlertService.GetEquipmentActiveAlerts(factoryId, equipmentId, fromdate, toDate);
        }
    }
}