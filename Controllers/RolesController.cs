//-----------------------------------------------------------------------
// <copyright file="RolesController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Roles controller class.</summary>
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
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The roles controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        /// <summary>
        /// The role service.
        /// </summary>
        private readonly IRoleService roleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="roleService">The role service.</param>
        /// <exception cref="ArgumentNullException">roleService</exception>
        public RolesController(IOptions<ApplicationSettings> applicationSettingsConfig, IRoleService roleService)
        {
            this.ApplicationSettings = applicationSettingsConfig.Value;
            this.roleService = roleService ?? throw new ArgumentNullException("roleService");
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the specified role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>The role.</returns>
        [HttpGet("{roleId}")]
        public async Task<Role> Get(long roleId)
        {
            return await this.roleService.Get(roleId);
        }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <returns>The list of roles.</returns>
        [HttpGet("getroles")]
        public async Task<List<DataSelectionModel>> GetRoles()
        {
            return await this.roleService.GetRoles();
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of roles.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Role>, int> GetSearched(int pageNo, string searchText)
        {
            var roles = this.roleService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(roles, totalCount);
        }

        /// <summary>
        /// Posts the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>The role.</returns>
        [HttpPost]
        public async Task<Role> Post([FromBody]Role tool)
        {
            return await this.roleService.Create(tool);
        }

        /// <summary>
        /// Puts the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>The role.</returns>
        [HttpPut]
        public async Task Put([FromBody]Role tool)
        {
            await this.roleService.Update(tool);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.roleService.Delete(id);
        }
    }
}