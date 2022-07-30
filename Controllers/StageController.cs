// <copyright file="StageController.cs" company="ThingTrax UK Ltd">
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
    /// StageController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class StageController : Controller
    {
        /// <summary>
        /// The stage service
        /// </summary>
        private IStageService stageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StageController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings</param>
        /// <param name="stageService">The stage service.</param>
        /// <exception cref="ArgumentNullException">labourService</exception>
        public StageController(IOptions<ApplicationSettings> applicationSettingsConfig, IStageService stageService)
        {
            this.stageService = stageService ?? throw new ArgumentNullException("stageService");
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
        public async Task<IEnumerable<Stage>> Get()
        {
            return await this.stageService.GetAll();
        }

        /// <summary>
        /// Gets the list of equipments.
        /// </summary>
        /// <returns>The list of equipments.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Stage>, int> Get(int pageNo, string searchText)
        {
            var stage = this.stageService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(stage, totalCount);
        }

        /// <summary>
        /// Posts the specified job.
        /// </summary>
        /// <param name="stage">Type of the job.</param>
        /// <returns>the task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Stage> Post([FromBody]Stage stage)
        {
            if (string.IsNullOrEmpty(stage.Name))
            {
                throw new ArgumentNullException("stage name cannot be empty");
            }

            if (stage.No < 1)
            {
                throw new ArgumentNullException("stage no cannot less than 1");
            }

            if (stage.JobTypeId < 1)
            {
                throw new ArgumentNullException("please select a jobtype");
            }

            return await this.stageService.Create(stage);
        }

        /// <summary>
        /// Puts the specified job.
        /// </summary>
        /// <param name="stage">Type of the job.</param>
        /// <returns>Task</returns>
        /// <exception cref="ArgumentNullException">jobtype name cannot be empty</exception>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Stage stage)
        {
            if (string.IsNullOrEmpty(stage.Name))
            {
                throw new ArgumentNullException("stage name cannot be empty");
            }

            if (stage.No < 1)
            {
                throw new ArgumentNullException("stage no cannot less than 1");
            }

            if (stage.JobTypeId < 1)
            {
                throw new ArgumentNullException("please select a jobtype");
            }

            await this.stageService.Update(stage);
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
            await this.stageService.Delete(id);
        }
    }
}