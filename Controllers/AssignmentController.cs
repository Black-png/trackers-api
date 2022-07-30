//-----------------------------------------------------------------------
// <copyright file="AssignmentController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>AssignmentController class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Class AssignmentController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/Assignment")]
    public class AssignmentController : Controller
    {
        /// <summary>
        /// The Assignment service
        /// </summary>
        private IAssignmentService assignmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="assignmentService">The assignment service.</param>
        /// <exception cref="ArgumentNullException">assignmentService</exception>
        public AssignmentController(IOptions<ApplicationSettings> applicationSettingsConfig, IAssignmentService assignmentService)
        {
            this.assignmentService = assignmentService ?? throw new ArgumentNullException("assignmentService");
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
        public async Task<IEnumerable<Assignment>> Get()
        {
            var assignment = await this.assignmentService.GetAll();
            return assignment.OrderBy(e => e.Id);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        [HttpGet("{id}")]
        public async Task<Assignment> Get(long id)
        {
            return await this.assignmentService.GetAssignmentByIdAsync(id);
        }

        /// <summary>
        /// Gets the list of assignment.
        /// </summary>
        /// <returns>The list of equipments.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Assignment>, int> GetAssignment(int pageNo, string searchText)
        {
            var assignment = this.assignmentService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(assignment, totalCount);
        }

        /// <summary>
        /// Calculates the planned start.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The planned start date for new assignment.</returns>
        [HttpGet("CalculatePlannedStart/{equipmentId}")]
        public async Task<Dictionary<string, DateTimeOffset>> CalculatePlannedStart(long equipmentId)
        {
            return await this.assignmentService.CalculatePlannedStart(equipmentId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <param name="groupId">The group identifier</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("dashboard/{factoryId}")]
        public async Task<IEnumerable<AssignmentDashboardResponseModel>> GetByFactoryId(long factoryId, long? groupId)
        {
            return await this.assignmentService.GetAssignmentsByFactoryId(factoryId, groupId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("equipment/{equipmentId}")]
        public async Task<IEnumerable<Assignment>> GetByEquipmentId(long equipmentId)
        {
            return await this.assignmentService.GetActiveAssignmentsByEquipmentId(equipmentId);
        }

        /// <summary>
        /// Posts the specified value.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <returns>Task&lt;Assignment&gt;.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Assignment> Post([FromBody]Assignment assignment)
        {
            return await this.assignmentService.Create(assignment);
        }

        /// <summary>
        /// Puts the specified identifier.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <returns>Task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Assignment assignment)
        {
            await this.assignmentService.Update(assignment);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.assignmentService.Delete(id);
        }
    }
}
