//-----------------------------------------------------------------------
// <copyright file="LabourSkillController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Labour skill controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Labour skill controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class LabourSkillController : Controller
    {
        /// <summary>
        /// The labour service
        /// </summary>
        private ILabourSkillService labourSkillService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabourSkillController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="labourSkillService">The labour skill service.</param>
        /// <exception cref="ArgumentNullException">labourSkillService</exception>
        public LabourSkillController(IOptions<ApplicationSettings> applicationSettingsConfig, ILabourSkillService labourSkillService)
        {
            this.labourSkillService = labourSkillService ?? throw new ArgumentNullException("labourSkillService");
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
        public async Task<IEnumerable<LabourSkill>> Get()
        {
            return await this.labourSkillService.GetAll();
        }

        /// <summary>
        /// Gets the list of factories.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<LabourSkillsResponseModel>, int> GetSearched(int pageNo, string searchText)
        {
            var labours = this.labourSkillService.GetSearched(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(labours, totalCount);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The labour</returns>
        [HttpGet("{id}")]
        public async Task<LabourSkill> Get(long id)
        {
            return await this.labourSkillService.Get(id);
        }

        /// <summary>
        /// Posts the specified labour skill.
        /// </summary>
        /// <param name="labourSkill">The labour skill.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentNullException">
        /// labourSkill.DepartmentId
        /// or
        /// labourSkill.DepartmentId
        /// </exception>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<LabourSkill> Post([FromBody]LabourSkill labourSkill)
        {
            if (labourSkill.LabourId <= 0)
            {
                throw new ArgumentNullException("labourSkill.DepartmentId");
            }

            if (labourSkill.ToolId <= 0)
            {
                throw new ArgumentNullException("labourSkill.DepartmentId");
            }

            labourSkill.LastUpdated = DateTimeOffset.UtcNow;

            return await this.labourSkillService.Create(labourSkill);
        }

        /// <summary>
        /// Puts the specified labour skill.
        /// </summary>
        /// <param name="labourSkill">The labour skill.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentNullException">
        /// labour.DepartmentId
        /// or
        /// labour.DepartmentId
        /// </exception>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]LabourSkill labourSkill)
        {
            if (labourSkill.LabourId <= 0)
            {
                throw new ArgumentNullException("labourSkill.DepartmentId");
            }

            if (labourSkill.ToolId <= 0)
            {
                throw new ArgumentNullException("labourSkill.DepartmentId");
            }

            labourSkill.LastUpdated = DateTimeOffset.UtcNow;
            await this.labourSkillService.Update(labourSkill);
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
            await this.labourSkillService.Delete(id);
        }

        /// <summary>
        /// Posts the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost("importlabourskills/{factoryId}")]
        public async Task<List<string>> PostFile(IFormFile file, long factoryId)
        {
            List<string> errorList = new List<string>();
            if (file == null)
            {
                errorList.Add(string.Format("File is null."));
            }

            var stream = file.OpenReadStream();
            errorList = await this.labourSkillService.ReadDataFromLabourSkillFile(stream, factoryId);
            return errorList;
        }

        /// <summary>
        /// Determines whether [is labour skilled] [the specified labour identifier].
        /// </summary>
        /// <param name="labourId">The labour identifier.</param>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>Return true or false.</returns>
        [HttpGet("GetSkill/{labourId}/{toolId}")]
        public async Task<List<LabourSkill>> GetLabourSkill(long labourId, long toolId)
        {
            return await this.labourSkillService.GetLabourSkill(labourId, toolId);
        }

        /// <summary>
        /// Determines whether [is labour skilled] [the specified labour identifier].
        /// </summary>
        /// <param name="labourId">The labour identifier.</param>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>Return true or false.</returns>
        [HttpGet("IsLabourSkilled/{labourId}/{toolId}")]
        public async Task<bool> IsLabourSkilled(long labourId, long toolId)
        {
            return await this.labourSkillService.IsLabourSkilled(labourId, toolId);
        }
    }
}