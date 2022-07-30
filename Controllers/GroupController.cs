//-----------------------------------------------------------------------
// <copyright file="GroupController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Factory controller class.</summary>
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
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Group controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class GroupController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IGroupService groupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="groupService">The group service.</param>
        /// <exception cref="ArgumentNullException">factoryService</exception>
        public GroupController(IOptions<ApplicationSettings> applicationSettingsConfig, IGroupService groupService)
        {
            this.groupService = groupService ?? throw new ArgumentNullException("groupService");
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
        public async Task<IEnumerable<Group>> Get()
        {
            var group = await this.groupService.GetAll();
            return group.OrderBy(f => f.Name);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("{id}")]
        public async Task<Group> Get(long id)
        {
            return await this.groupService.Get(id);
        }

        /// <summary>
        /// Gets the list of groups.
        /// </summary>
        /// <returns>The list of groups.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Group>, int> GetSearched(int pageNo, string searchText)
        {
            var equipments = this.groupService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(equipments, totalCount);
        }

        /// <summary>
        /// Posts the specified factory.
        /// </summary>
        /// <param name="group">The factory.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Group> Post([FromBody]Group group)
        {
            return await this.groupService.Create(group);
        }

        /// <summary>
        /// Puts the specified factory.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Group group)
        {
            await this.groupService.Update(group);
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
            await this.groupService.Delete(id);
        }

        /// <summary>
        /// Gets the groups for factory.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>Returns group id and name list.</returns>
        [HttpGet("getgroupsbyfactory/{factoryId}")]
        public async Task<List<DataSelectionModel>> GetGroupsForFactory(long factoryId)
        {
            return await this.groupService.GetGroupsForFactory(factoryId);
        }

        /// <summary>
        /// Determines whether [is equipment exists in group] [the specified group identifier].
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>Returns boolean vlaue</returns>
        [HttpGet("isequipmentexists/{groupId}")]
        public async Task<bool> IsEquipmentExistsInGroup(long groupId)
        {
            return await this.groupService.IsEquipmetExistsInGroup(groupId);
        }
    }
}
