//-----------------------------------------------------------------------
// <copyright file="OEEController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>OEE controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// OEE controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class OEEController : Controller
    {
        /// <summary>
        /// The oee service
        /// </summary>
        private IOeeService oeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OEEController"/> class.
        /// </summary>
        /// <param name="oeeService">The oee service.</param>
        public OEEController(IOeeService oeeService)
        {
            this.oeeService = oeeService ?? throw new ArgumentNullException("oeeService");
        }

        /// <summary>
        /// Gets the equipment oee.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>Equipment OEE</returns>
        /// <exception cref="System.ArgumentNullException">equipmentId</exception>
        [HttpGet("{equipmentId}")]
        public async Task<OeeResponseModel> GetEquipmentOEE(long equipmentId)
        {
            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            return await this.oeeService.GetEquipmentOee(equipmentId);
        }

        /// <summary>
        /// Gets the oee shift by job equipment identifier.
        /// </summary>
        /// <param name="jobEquipmentId">The job equipment identifier.</param>
        /// <returns>
        /// list of shift
        /// </returns>
        /// <exception cref="System.ArgumentNullException">jobEquipmentId</exception>
        [HttpGet("GetOeeShiftByJobEquipmentId/{jobEquipmentId}")]
        public async Task<IList<EquipmentShift>> GetOeeShiftByJobEquipmentId(long jobEquipmentId)
        {
            if (jobEquipmentId <= 0)
            {
                throw new ArgumentNullException("jobEquipmentId");
            }

            return await this.oeeService.GetOeeShiftByJobEquipmentId(jobEquipmentId);
        }

        /////// <summary>
        /////// Gets the equipment oee.
        /////// </summary>
        /////// <param name="factoryId">The equipment identifier.</param>
        /////// <param name="groupId">The group identifier.</param>
        /////// <param name="state">The equipment running state</param>
        /////// <param name="reasonCode">The downtime reason code.</param>
        /////// <returns>Equipment OEE</returns>
        /////// <exception cref="System.ArgumentNullException">equipmentId</exception>
        ////[HttpGet("factory")]
        ////public async Task<OeeDashboardResponseModel> GetFactoryOEE(long factoryId, long? groupId, int? state, string reasonCode)
        ////{
        ////    if (factoryId <= 0)
        ////    {
        ////        throw new ArgumentNullException("factoryId");
        ////    }

        ////    return await this.oeeService.GetFactoryOee(factoryId, groupId, state, reasonCode);
        ////}
    }
}