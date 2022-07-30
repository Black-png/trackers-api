// <copyright file="SocialDVController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// SocialDVController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class SocialDVController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private readonly ISocialDistanceViolationService socialDistanceViolationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialDVController" /> class.
        /// </summary>
        /// <param name="socialDistanceViolationService">The tool service.</param>
        public SocialDVController(ISocialDistanceViolationService socialDistanceViolationService)
        {
            this.socialDistanceViolationService = socialDistanceViolationService ?? throw new ArgumentNullException("socialDistanceViolationService");
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns> the data</returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Gets a social distans violations by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>
        /// task
        /// </returns>
        [HttpGet("GetSociaDVByFactoryId/{factoryId}")]
        public async Task<IList<SocialDVDashboardModel>> GetSocialDistansViolationsByFactoryId(long factoryId)
        {
            return await this.socialDistanceViolationService.GetAllSocialDistansViolationsByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets a social distans violations by factory identifier.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>
        /// task
        /// </returns>
        [HttpGet("GetDetailSociaDVByEquipmentId/{equipmentId}")]
        public async Task<SocialDVDetailDashboardModel> GetDetailSocialDistansViolationsByequipmentId(long equipmentId)
        {
            return await this.socialDistanceViolationService.GetDetailSocialDVByEquipmentId(equipmentId);
        }
    }
}
