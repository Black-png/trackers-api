//-----------------------------------------------------------------------
// <copyright file="RecurrenceJobsController.cs" company="ThingTrax UK Ltd">
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
    /// The recurrence job controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class RecurrenceJobsController : Controller
    {
        private readonly IRecurrenceJobService recurrenceJobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurrenceJobsController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The appsettings.</param>
        /// <param name="recurrenceJobService">The context.</param>
        public RecurrenceJobsController(IOptions<ApplicationSettings> applicationSettingsConfig, IRecurrenceJobService recurrenceJobService)
        {
            this.recurrenceJobService = recurrenceJobService ?? throw new ArgumentNullException("recurrenceJobService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        ////// GET: api/RecurrenceJobs
        ////[HttpGet]
        ////public IEnumerable<RecurrenceJob> GetRecurrenceJobs()
        ////{
        ////    return _context.RecurrenceJobs;
        ////}

        /// <summary>
        /// Get the recurrence job.
        /// </summary>
        /// <param name="recurrenceJobId">The recurrence job identifier.</param>
        /// <returns>The recurrence job.</returns>
        // GET: api/RecurrenceJobs/5
        [HttpGet("{recurrenceJobId}")]
        public async Task<RecurrenceJob> GetRecurrenceJob([FromRoute] long recurrenceJobId)
        {
            return await this.recurrenceJobService.GetRecurrenceJob(recurrenceJobId);
        }

        /// <summary>
        /// Updates the specified recurrence job.
        /// </summary>
        /// <param name="recurrenceJob">The recurrence job.</param>
        /// <returns>The updated recurrence job.</returns>
        [HttpPut]
        [Authorize(Policy = "CustomAuthorization")]
        public async Task<RecurrenceJob> Update([FromBody] RecurrenceJob recurrenceJob)
        {
            return await this.recurrenceJobService.Update(recurrenceJob);
        }

        /// <summary>
        /// Posts the recurrence job.
        /// </summary>
        /// <param name="recurrenceJob">The recurrence job.</param>
        /// <returns>The created recurrence job.</returns>
        [HttpPost]
        [Authorize(Policy = "CustomAuthorization")]
        public async Task<RecurrenceJob> PostRecurrenceJob([FromBody] RecurrenceJob recurrenceJob)
        {
            return await this.recurrenceJobService.Create(recurrenceJob);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "CustomAuthorization")]
        public async Task Delete([FromRoute] long id)
        {
            await this.recurrenceJobService.Delete(id);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of recurrence jobs.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<RecurrenceJobResponseModel>, int> GetSearched(int pageNo, string searchText)
        {
            var recurrenceJobs = this.recurrenceJobService.GetAllRecurrenceJobs(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(recurrenceJobs, totalCount);
        }
    }
}