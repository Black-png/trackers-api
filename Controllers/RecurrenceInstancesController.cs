//-----------------------------------------------------------------------
// <copyright file="RecurrenceInstancesController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Recurrence jobs controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// The recurrence instance controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class RecurrenceInstancesController : Controller
    {
        private readonly IRecurrenceInstanceService recurrenceInstanceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurrenceInstancesController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="recurrenceInstanceService">The recurrence instance service.</param>
        /// <exception cref="ArgumentNullException">recurrenceInstanceService</exception>
        public RecurrenceInstancesController(IOptions<ApplicationSettings> applicationSettingsConfig, IRecurrenceInstanceService recurrenceInstanceService)
        {
            this.recurrenceInstanceService = recurrenceInstanceService ?? throw new ArgumentNullException("recurrenceInstanceService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the recurrence instance.
        /// </summary>
        /// <param name="recurrenceInstanceId">The recurrence instance identifier.</param>
        /// <returns>The recurrence instance.</returns>
        [HttpGet("{recurrenceInstanceId}")]
        public async Task<RecurrenceInstance> GetRecurrenceInstance([FromRoute] long recurrenceInstanceId)
        {
            return await this.recurrenceInstanceService.GetRecurrenceInstance(recurrenceInstanceId);
        }

        /// <summary>
        /// Updates the specified recurrence instance.
        /// </summary>
        /// <param name="recurrenceInstance">The recurrence instance.</param>
        /// <returns>The updated recurrence instance.</returns>
        [HttpPut]
        [Authorize(Policy = "CustomAuthorization")]
        public async Task<RecurrenceInstance> Update([FromBody] RecurrenceInstance recurrenceInstance)
        {
            return await this.recurrenceInstanceService.Update(recurrenceInstance);
        }

        /// <summary>
        /// Creates the specified recurrence instance.
        /// </summary>
        /// <param name="recurrenceInstance">The recurrence instance.</param>
        /// <returns>The created recurrence instance.</returns>
        [HttpPost]
        [Authorize(Policy = "CustomAuthorization")]
        public async Task<RecurrenceInstance> Create([FromBody] RecurrenceInstance recurrenceInstance)
        {
            return await this.recurrenceInstanceService.Create(recurrenceInstance);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "CustomAuthorization")]
        public async Task Delete(long id)
        {
            await this.recurrenceInstanceService.Delete(id);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="recurrenceJobId">The recurrence job identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of recurrence jobs.</returns>
        [HttpGet("GetSearched")]
        public Tuple<RecurrenceInstanceResponseModel, int> GetSearched(long recurrenceJobId, int pageNo, string searchText)
        {
            var recurrenceInstances = this.recurrenceInstanceService.GetAllRecurrenceForJobId(recurrenceJobId, pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(recurrenceInstances, totalCount);
        }
    }
}