//-----------------------------------------------------------------------
// <copyright file="LabourJobTitleController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Labour Job Title controller class.</summary>
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
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Labour Job Title controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class LabourJobTitleController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private ILabourJobTitleService labourJobTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabourJobTitleController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="labourJobTitle">The entity service.</param>
        public LabourJobTitleController(IOptions<ApplicationSettings> applicationSettingsConfig, ILabourJobTitleService labourJobTitle)
        {
            this.labourJobTitle = labourJobTitle ?? throw new ArgumentNullException("labourJobTitleService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Posts the specified labour Job Title.
        /// </summary>
        /// <param name="labourJobTitle">The labour Job Title.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<LabourJobTitle> Post([FromBody] LabourJobTitle labourJobTitle)
        {
            labourJobTitle.Colour = "#000000";
            labourJobTitle.Created = DateTimeOffset.UtcNow;
            return await this.labourJobTitle.Create(labourJobTitle);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Data and count</returns>
        [HttpGet]
        public async Task<IEnumerable<LabourJobTitle>> Get()
        {
            var labourJobTitle = await this.labourJobTitle.GetAll();
            return labourJobTitle.OrderBy(e => e.Name);
        }

        /// <summary>
        /// Gets the list of Labour Job Title.
        /// </summary>
        /// <returns>The list of Labour Job Title</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<LabourJobTitle>, int> GetSearched(int pageNo, string searchText)
        {
            var labourJobTitle = this.labourJobTitle.GetSearchedData(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(labourJobTitle, totalCount);
        }

        /// <summary>
        /// Puts the specified job Labour Title.
        /// </summary>
        /// <param name="jobLabourTitle">The job Labour Title.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody] LabourJobTitle jobLabourTitle)
        {
            if (jobLabourTitle.Id <= 0)
            {
                throw new ArgumentNullException("jobLabourTitle.Id");
            }

            await this.labourJobTitle.Update(jobLabourTitle);
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
            await this.labourJobTitle.Delete(id);
        }
    }
}
