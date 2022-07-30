//-----------------------------------------------------------------------
// <copyright file="UserAreaController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>User area controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The user area controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class UserAreaController : Controller
    {
        /// <summary>
        /// The role service.
        /// </summary>
        private readonly IUserAreaService userAreaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAreaController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="userAreaService">The user area service.</param>
        /// <exception cref="ArgumentNullException">userAreaService</exception>
        public UserAreaController(IOptions<ApplicationSettings> applicationSettingsConfig, IUserAreaService userAreaService)
        {
            this.ApplicationSettings = applicationSettingsConfig.Value;
            this.userAreaService = userAreaService ?? throw new ArgumentNullException("userAreaService");
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the user area role details.
        /// </summary>
        /// <returns>The list of user area role details.</returns>
        [HttpGet("GetUserRoleDetails")]
        public async Task<IEnumerable<UserAreaRoleResponseModel>> GetUserAreaRoleDetails()
        {
            var userRolesDetails = await this.userAreaService.GetUserAreaRoleDetails();
            return userRolesDetails;
        }

        /// <summary>
        /// Gets the user areas.
        /// </summary>
        /// <returns>The list of user areas.</returns>
        [HttpGet("getuserareas")]
        public async Task<List<DataSelectionModel>> GetUserAreas()
        {
            return await this.userAreaService.GetUserAreas();
        }

        /// <summary>
        /// Updates the specified user area roles.
        /// </summary>
        /// <param name="userAreaRoles">The user area roles.</param>
        /// <returns>The task.</returns>
        [HttpPut("updateuserarearoles")]
        public async Task Update([FromBody]List<UserAreaRole> userAreaRoles)
        {
            await this.userAreaService.Update(userAreaRoles);
        }
    }
}