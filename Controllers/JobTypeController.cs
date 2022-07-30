// <copyright file="JobTypeController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// JobTypeController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class JobTypeController : Controller
    {
        /// <summary>
        /// The jobtype service
        /// </summary>
        private IJobTypeService jobTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobTypeController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings</param>
        /// <param name="jobTypeService">The job type service.</param>
        /// <exception cref="ArgumentNullException">labourService</exception>
        public JobTypeController(IOptions<ApplicationSettings> applicationSettingsConfig, IJobTypeService jobTypeService)
        {
            this.jobTypeService = jobTypeService ?? throw new ArgumentNullException("jobTypeService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the list of jobs.
        /// </summary>
        /// <returns>The list of jobs</returns>
        [HttpGet]
        public async Task<IEnumerable<JobType>> Get()
        {
            return await this.jobTypeService.GetAll();
        }

        /// <summary>
        /// Gets the list of equipments.
        /// </summary>
        /// <returns>The list of equipments.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<JobType>, int> Get(int pageNo, string searchText)
        {
            var jobType = this.jobTypeService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(jobType, totalCount);
        }

        /// <summary>
        /// Posts the specified job.
        /// </summary>
        /// <param name="jobType">Type of the job.</param>
        /// <returns>the task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<JobType> Post([FromBody]JobType jobType)
        {
            if (string.IsNullOrEmpty(jobType.Name))
            {
                throw new ArgumentNullException("jobtype name cannot be empty");
            }

            return await this.jobTypeService.Create(jobType);
        }

        /// <summary>
        /// Puts the specified job.
        /// </summary>
        /// <param name="jobType">Type of the job.</param>
        /// <returns>Task</returns>
        /// <exception cref="ArgumentNullException">jobtype name cannot be empty</exception>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]JobType jobType)
        {
            if (string.IsNullOrEmpty(jobType.Name))
            {
                throw new ArgumentNullException("jobtype name cannot be empty");
            }

            await this.jobTypeService.Update(jobType);
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
            await this.jobTypeService.Delete(id);
        }
    }
}