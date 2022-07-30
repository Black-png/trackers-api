//-----------------------------------------------------------------------
// <copyright file="JobLabourShiftPlanController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Job labour shift plan controller class.</summary>
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
    using Services.Interfaces;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Job labour shift plan controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/JobLabourShiftPlan")]
    public class JobLabourShiftPlanController : Controller
    {
        /// <summary>
        /// The factory area service
        /// </summary>
        private IJobLabourShiftPlanService jobLabourShiftPlanService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobLabourShiftPlanController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="jobLabourShiftPlanService">The job labour shift plan service.</param>
        /// <exception cref="System.ArgumentNullException">jobLabourShiftPlanService</exception>
        public JobLabourShiftPlanController(IOptions<ApplicationSettings> applicationSettingsConfig, IJobLabourShiftPlanService jobLabourShiftPlanService)
        {
            this.jobLabourShiftPlanService = jobLabourShiftPlanService ?? throw new ArgumentNullException("jobLabourShiftPlanService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Task.</returns>
        [HttpGet]
        public async Task<IEnumerable<JobLabourShiftPlan>> Get()
        {
            return await this.jobLabourShiftPlanService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        [HttpGet("{Id}")]
        public async Task<JobLabourShiftPlan> Get(int id)
        {
            return await this.jobLabourShiftPlanService.Get(id);
        }

        /// <summary>
        /// Posts the specified job labour shift plan.
        /// </summary>
        /// <param name="jobLabourShiftPlan">The job labour shift plan.</param>
        /// <returns>Job labour shift plan.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<JobLabourShiftPlan> Post([FromBody]JobLabourShiftPlan jobLabourShiftPlan)
        {
            return await this.jobLabourShiftPlanService.Create(jobLabourShiftPlan);
        }

        /// <summary>
        /// Puts the specified job labour shift plan.
        /// </summary>
        /// <param name="jobLabourShiftPlan">The job labour shift plan.</param>
        /// <returns>Task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]JobLabourShiftPlan jobLabourShiftPlan)
        {
            await this.jobLabourShiftPlanService.Update(jobLabourShiftPlan);
        }
    }
}