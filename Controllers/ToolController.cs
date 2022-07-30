//-----------------------------------------------------------------------
// <copyright file="ToolController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Department controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Repository.Sql.ResponseQueryEntities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Tool controller class.
    /// </summary>
    [Route("api/[controller]")]
    public class ToolController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private readonly IToolService toolService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration</param>
        /// <param name="toolService">The tool service.</param>
        public ToolController(IOptions<ApplicationSettings> applicationSettingsConfig, IToolService toolService)
        {
            this.ApplicationSettings = applicationSettingsConfig.Value;
            this.toolService = toolService ?? throw new ArgumentNullException("toolService");
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Tool</returns>
        [HttpGet]
        public async Task<IEnumerable<Tool>> Get()
        {
            return await this.toolService.GetAll();
        }

        /// <summary>
        /// Gets the specified Tool identifier.
        /// </summary>
        /// <param name="toolid">The tool identifier.</param>
        /// <returns>Department</returns>
        [HttpGet("{toolid}")]
        public async Task<Tool> Get(long toolid)
        {
            return await this.toolService.Get(toolid);
        }

        /// <summary>
        /// Gets the list of tools.
        /// </summary>
        /// <returns>The list of tools.</returns>
        /// <param name="pageNo">Page Number.</param>
        /// <param name="searchText">Serach text.</param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Tool>, int> GetSearched(int pageNo, string searchText)
        {
            var tools = this.toolService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(tools, totalCount);
        }

        /// <summary>
        /// Posts the specified department.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Tool> Post([FromBody]Tool tool)
        {
            return await this.toolService.Create(tool);
        }

        /// <summary>
        /// Puts the specified department.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Tool tool)
        {
            await this.toolService.Update(tool);
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
            await this.toolService.Delete(id);
        }

        /// <summary>
        /// Determines whether [is tool number exists] [the specified tool number].
        /// </summary>
        /// <param name="toolNumber">The tool number.</param>
        /// <returns>true or false</returns>
        [HttpGet("istoolnumberexists/{toolNumber}")]
        public async Task<bool> IsToolNumberExists(string toolNumber)
        {
            return await this.toolService.IsToolNumberExists(toolNumber);
        }

        /// <summary>
        /// Gets the tools by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of tools.</returns>
        [HttpGet("gettoolsbyfactory/{factoryId}")]
        public async Task<List<Tool>> GetToolsByFactoryId(long factoryId)
        {
            return await this.toolService.GetToolsByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the tools data by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The tools names and Id's for factory.</returns>
        [HttpGet("gettoolsdataforfactory/{factoryId}")]
        public async Task<List<DataSelectionModel>> GetToolsDataByFactoryId(long factoryId)
        {
            return await this.toolService.GetToolsDataByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the tools by measurement unit id.
        /// </summary>
        /// <param name="measurementUnitId">The factory identifier.</param>
        /// <returns>The tools associated with the measurement unit.</returns>
        [HttpGet("gettoolsbymeasurementunit/{measurementUnitId}")]
        public async Task<IEnumerable<Tool>> Get(int measurementUnitId)
        {
            return await this.toolService.GetToolsByMeasurementUnitId(measurementUnitId);
        }

        /// <summary>
        /// Gets the tool shot reading.
        /// </summary>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>The tool shot reading</returns>
        [HttpGet("gettoolshotreading/{toolId}")]
        public int GetToolShotReading(long toolId)
        {
            return this.toolService.GetToolShotReading(toolId);
        }

        /// <summary>
        /// Determines whether [is tool deletable] [the specified tool identifier].
        /// </summary>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>The bool value which indicates if tool is deletable or not.</returns>
        [HttpGet("istooldeletable/{toolId}")]
        public async Task<bool> IsToolDeletable(long toolId)
        {
            return await this.toolService.IsToolDeletable(toolId);
        }

        /// <summary>
        /// Gets the product average cycle time.
        /// </summary>
        /// <param name="fromdate">The from date.</param>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>
        /// task
        /// </returns>
        [HttpGet("GetToolAvgCycleTime")]
        public async Task<IList<ToolAvgCycleTimeResponseQueryModel>> GetToolAvgCycleTime(DateTimeOffset? fromdate, int pageNo, string searchText)
        {
            return await this.toolService.GetToolAvgCycleTime(fromdate, pageNo, this.ApplicationSettings.PageSize, searchText);
        }

        /// <summary>
        /// Gets the tool average cycle time.
        /// </summary>
        /// <param name="toolId">The tool identifier.</param>
        /// <param name="cycleTime">The cycle time.</param>
        /// <returns>task</returns>
        [HttpPut("UpdateToolCycleTime")]
        public async Task<Tool> UpdateToolCycleTime(long toolId, double cycleTime)
        {
            return await this.toolService.UpdateToolCycleTime(toolId, cycleTime);
        }

        /// <summary>
        /// Post File.
        /// </summary>
        /// <param name="file">The tool file.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The task</returns>
        [HttpPost("postToolfile/{factoryId}")]
        public async Task<List<string>> PostFile(IFormFile file, long factoryId)
        {
            List<string> errorList = new List<string>();
            if (file == null)
            {
                errorList.Add(string.Format("File is null."));
            }

            var stream = file.OpenReadStream();
            errorList = await this.toolService.ReadDataFromFile(stream, factoryId);
            return errorList;
        }
    }
}
