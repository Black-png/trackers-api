//-----------------------------------------------------------------------
// <copyright file="LabourController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Product controller class.</summary>
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
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Equipment controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class LabourController : Controller
    {
        /// <summary>
        /// The labour service
        /// </summary>
        private ILabourService labourService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabourController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="labourService">The labour service.</param>
        public LabourController(IOptions<ApplicationSettings> applicationSettingsConfig, ILabourService labourService)
        {
            this.labourService = labourService ?? throw new ArgumentNullException("labourService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the list of labour.
        /// </summary>
        /// <returns>The list of labour</returns>
        [HttpGet]
        public async Task<IEnumerable<Labour>> Get()
        {
            return await this.labourService.GetAll();
        }

        /// <summary>
        /// Gets the list of labour.
        /// </summary>
        /// <param name="titleId">The title identifier.</param>
        /// <returns>The list of labour</returns>
        [HttpGet("labours/{titleId?}")]
        public async Task<List<DataSelectionModel>> GetLabours(long? titleId)
        {
            return await this.labourService.GetLabours(titleId);
        }

        /// <summary>
        /// Gets the list of factories.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Labour>, int> GetSearched(int pageNo, string searchText)
        {
            var labours = this.labourService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(labours, totalCount);
        }

        /// <summary>
        /// Gets the un identified operator.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of unidentfied operator's.</returns>
        [HttpGet("GetUnidentifiedOperators")]
        public Tuple<IEnumerable<Labour>, int> GetUnIdentifiedOperator(int pageNo, string searchText)
        {
            var labours = this.labourService.GetUnidentifiedOperators(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(labours, totalCount);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The labour</returns>
        [HttpGet("{id}")]
        public async Task<Labour> Get(long id)
        {
            return await this.labourService.Get(id);
        }

        /// <summary>
        /// Posts the specified labour.
        /// </summary>
        /// <param name="labour">The labour.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Labour> Post([FromBody]Labour labour)
        {
            if (labour.DepartmentId <= 0)
            {
                throw new ArgumentNullException("labour.DepartmentId");
            }

            return await this.labourService.Create(labour);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="labourId">The labour identifier.</param>
        /// <returns>List of labour</returns>
        [HttpGet("IsIdAvailable/{labourId}")]
        public async Task<bool> IsIdAvailable(long labourId)
        {
            return await this.labourService.IsLabourIdAvailable(labourId);
        }

        /// <summary>
        /// Puts the specified labour.
        /// </summary>
        /// <param name="labour">The labour.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Labour labour)
        {
            if (labour.DepartmentId <= 0)
            {
                throw new ArgumentNullException("labour.DepartmentId");
            }

            await this.labourService.Update(labour);
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
            await this.labourService.Delete(id);
        }

        /// <summary>
        /// Gets the labours by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of LabourId and id fields.</returns>
        [HttpGet("getlaboursbyfactoryid/{factoryId}")]
        public async Task<List<DataSelectionModel>> GetLaboursByFactoryId(long factoryId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.labourService.GetLaboursByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the labours by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of LabourId and id fields.</returns>
        [HttpGet("getfactorylaboursbyid/{factoryId}")]
        public async Task<List<Labour>> GetFactoryLaboursById(long factoryId)
       {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.labourService.GetFactoryLaboursById(factoryId);
        }

        /// <summary>
        /// Gets the labours by department.
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <returns>Retun labours by department id.</returns>
        [HttpGet("department/{departmentId}")]
        public async Task<List<DataSelectionModel>> GetLaboursByDepartment(long departmentId)
        {
            if (departmentId <= 0)
            {
                throw new ArgumentNullException("departmentId");
            }

            return await this.labourService.GetLaboursByDepartment(departmentId);
        }

        /// <summary>
        /// Gets the labours by job title.
        /// </summary>
        /// <param name="jobTitleId">The job title identifier.</param>
        /// <returns>The list of labours for job title.</returns>
        /// <exception cref="ArgumentNullException">jobTitleId</exception>
        [HttpGet("laboursbyjobtitle/{jobTitleId}")]
        public async Task<List<DataSelectionModel>> GetLaboursByJobTitle(long jobTitleId)
        {
            if (jobTitleId <= 0)
            {
                throw new ArgumentNullException("jobTitleId");
            }

            return await this.labourService.GetLaboursByJobTitle(jobTitleId);
        }

        /// <summary>
        /// Determines whether [is labour identifier exists] [the specified labour identifier].
        /// </summary>
        /// <param name="labourId">The labour identifier.</param>
        /// <returns>true and false</returns>
        [HttpGet("islabourexists/{labourId}")]
        public async Task<bool> IsLabourIdExists(string labourId)
        {
            if (string.IsNullOrEmpty(labourId))
            {
                throw new ArgumentNullException("labourId");
            }

            return await this.labourService.IsLabourIdExists(labourId);
        }

        /// <summary>
        /// Determines whether [is labour identifier exists] [the specified labour identifier].
        /// </summary>
        /// <param name="trackerId">The labour identifier.</param>
        /// <returns>true and false</returns>
        [HttpGet("istrackereidxists/{trackerId}")]
        public async Task<Labour> IsTrackerIdExists(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentNullException("trackerId");
            }

            return await this.labourService.IsTrackerIdExists(trackerId);
        }

        /// <summary>
        /// Gets the labours by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="departmentId">The department identifier.</param>
        /// <returns>The list of LabourId and id fields.</returns>
        [HttpGet("getlaboursbyfactoryanddepartment")]
        public async Task<List<DataSelectionModel>> GetLaboursByFactoryAndDepartment(long factoryId, long? departmentId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.labourService.GetLaboursByFactoryAndDepartment(factoryId, departmentId);
        }

        /// <summary>
        /// Gets the new tracker identifier.
        /// </summary>
        /// <returns>The new tracker id.</returns>
        [HttpGet("newtrackerid")]
        public async Task<string> GetNewTrackerId()
        {
            return await this.labourService.GetNewTrackerId();
        }

        /// <summary>
        /// Gets the job titles.
        /// </summary>
        /// <returns>The list of job titles.</returns>
        [HttpGet("JobTitles")]
        public async Task<List<DataSelectionModel>> GetJobTitles()
        {
            return await this.labourService.GetJobTitles();
        }
    }
}
