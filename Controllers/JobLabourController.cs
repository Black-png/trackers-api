//-----------------------------------------------------------------------
// <copyright file="JobLabourController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Job labour controller class.</summary>
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
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Repository.Sql.ResponseQueryEntities;

    /// <summary>
    /// Labour planning controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class JobLabourController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IJobLabourService jobLabourService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobLabourController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">App setting configuration</param>
        /// <param name="equipmentLabourService">The entity service.</param>
        public JobLabourController(IOptions<ApplicationSettings> applicationSettingsConfig, IJobLabourService equipmentLabourService)
        {
            this.jobLabourService = equipmentLabourService ?? throw new ArgumentNullException("equipmentLabourService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the list of equipment labour.
        /// </summary>
        /// <returns>The list of equipment labour</returns>
        [HttpGet]
        public async Task<IEnumerable<JobLabour>> Get()
        {
            return await this.jobLabourService.GetAll();
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">serach text </param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="labourId">The labour identifier.</param>
        /// <param name="fromTime">The from time.</param>
        /// <param name="toTime">The to time.</param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<JobLabourDashboardResponseQueryModel>, int> GetSearched(int pageNo, string searchText, long? equipmentId, long? labourId, DateTimeOffset? fromTime, DateTimeOffset? toTime)
        {
            var jobLabours = this.jobLabourService.GetSearchedData(pageNo, this.ApplicationSettings.PageSize, searchText, equipmentId ?? 0, labourId ?? 0, fromTime, toTime, out int totalCount);
            return Tuple.Create(jobLabours, totalCount);
        }

        /// <summary>
        /// Gets the by job identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Return list of equipment labour based on job id</returns>
        [HttpGet("job/{jobId}")]
        public async Task<IEnumerable<JobLabourResponseModel>> GetByJobId(long jobId)
        {
            if (jobId <= 0)
            {
                throw new ArgumentNullException("jobId");
            }

            return await this.jobLabourService.GetLaboursByjobId(jobId);
        }

        /// <summary>
        /// Gets the by job identifier.
        /// </summary>
        /// <param name="jobLabourId">The job Labour Id.</param>
        /// <returns>Return list of Job Labour History based on job labour id</returns>
        [HttpGet("GetJobLabourHistoryByJobLabourId/{jobLabourId}")]
        public async Task<IEnumerable<LabourJourney>> GetJobLabourHistoryByJobLabourId(long jobLabourId)
        {
            if (jobLabourId <= 0)
            {
                throw new ArgumentNullException("jobLabourId");
            }

            return await this.jobLabourService.GetJobLabourHistoryByJobLabourId(jobLabourId);
        }

        /// <summary>
        /// Gets the current equipment labour.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The current equipment labour</returns>
        [HttpGet("CurrentEquipmentLabour/{equipmentId}")]
        public async Task<JobLabour> GetCurrentEquipmentLabour(long equipmentId)
        {
            return await this.jobLabourService.GetCurrentEquipmentLabour(equipmentId);
        }

        /// <summary>
        /// check the  operator availbility.
        /// </summary>
        /// <param name="operatorid">The operatior identifier.</param>
        /// <param name="fromDatetime">The from date time.</param>
        /// <param name="toDatetime">The to date time.</param>
        /// <returns>The falg according to availbility</returns>
        [HttpGet("IsOperatorAvailable/{operatorid}/{fromdatetime}/{todatetime}")]
        public async Task<bool> IsOperatorAvailable(long operatorid, DateTimeOffset fromDatetime, DateTimeOffset toDatetime)
        {
            return await this.jobLabourService.IsOperatorAvailable(operatorid, fromDatetime, toDatetime);
        }

        /// <summary>
        /// check the  operator availbility.
        /// </summary>
        /// <param name="operatorid">The operatior identifier.</param>
        /// <returns>The falg according to availbility</returns>
        [HttpGet("IsOperatorAssinged/{operatorid}")]
        public async Task<bool> IsOperatorAssinged(long operatorid)
        {
            return await this.jobLabourService.IsOperatorAssinged(operatorid);
        }

        /// <summary>
        /// Posts the specified equipment labour.
        /// </summary>
        /// <param name="jobLabour">The job labour.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<JobLabour> Post([FromBody]JobLabour jobLabour)
        {
            if (jobLabour == null)
            {
                throw new ArgumentNullException("jobLabour");
            }

            if (jobLabour.LabourId <= 0)
            {
                throw new ArgumentNullException("jobLabour.LabourId");
            }

            if (jobLabour.JobId <= 0)
            {
                throw new ArgumentNullException("jobLabour.JobId");
            }

            return await this.jobLabourService.Create(jobLabour);
        }

        /// <summary>
        /// Puts the specified equipment labour.
        /// </summary>
        /// <param name="jobLabour">The equipment labour.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]JobLabour jobLabour)
        {
            if (jobLabour.LabourId <= 0)
            {
                throw new ArgumentNullException("jobLabour.LabourId");
            }

            await this.jobLabourService.Update(jobLabour);
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
            await this.jobLabourService.Delete(id);
        }
    }
}
