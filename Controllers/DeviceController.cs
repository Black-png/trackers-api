//-----------------------------------------------------------------------
// <copyright file="DeviceController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Device controller class.</summary>
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
    using TT.Core.Repository.Entities;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Downtime controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class DeviceController : Controller
    {
        /// <summary>
        /// The telemetry service
        /// </summary>
        private IHealthTelemetryService healthTelemetryService;

        /// <summary>
        /// The telemetry service
        /// </summary>
        private IDeviceService deviceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceController"/> class.
        /// </summary>
        /// <param name="healthTelemetryService">The telemetry service.</param>
        /// <param name="deviceService">The device service.</param>
        /// <exception cref="System.ArgumentNullException">factoryService</exception>
        public DeviceController(IHealthTelemetryService healthTelemetryService, IDeviceService deviceService)
        {
            this.healthTelemetryService = healthTelemetryService ?? throw new ArgumentNullException("healthTelemetryService");
            this.deviceService = deviceService ?? throw new ArgumentNullException("deviceService");
        }

        /// <summary>
        /// Gets the Health.
        /// </summary>
        /// <returns>
        /// Device Health
        /// </returns>
        [Authorize]
        [HttpGet("GetAllDevicesHealth")]
        public async Task<List<DeviceStatus>> GetAllDevicesHealth()
        {
            var devices = await this.deviceService.GetAllDevicesHealth();
            return devices;
        }

        /// <summary>
        /// Gets the Health.
        /// </summary>
        /// <param name="deviceId">The deviceid identifier.</param>
        /// <returns>
        /// Device Health
        /// </returns>
        [Authorize]
        [HttpGet("GetHealth/{deviceId}")]
        public async Task<List<DeviceStatus>> GetDevicesHealthById(string deviceId)
        {
            var device = await this.deviceService.GetDeviceHealthById(deviceId);
            return device;
        }

        /// <summary>
        /// Gets the Health.
        /// </summary>
        /// <returns>
        /// Device Health
        /// </returns>
        [Authorize]
        [HttpGet("ComputeHealth")]
        public async Task ComputeHealth()
        {
            await this.deviceService.ComputeDeviceStatus();
        }
    }
}