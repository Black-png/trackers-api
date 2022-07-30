// <copyright file="ContainmentMetricsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// ContainmentMetricsController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ContainmentMetricsController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IContainmentMetricsService containmentMetricsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainmentMetricsController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="containmentMetricsService">The ContainmentMetrics services.</param>
        /// <exception cref="ArgumentNullException">IContainmentMetricsService</exception>
        public ContainmentMetricsController(IOptions<ApplicationSettings> applicationSettingsConfig, IContainmentMetricsService containmentMetricsService)
        {
            this.containmentMetricsService = containmentMetricsService ?? throw new ArgumentNullException("containmentMetricsService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Data and count</returns>
        [HttpGet]
        public async Task<IEnumerable<ContainmentMetrics>> Get()
        {
            var metrics = await this.containmentMetricsService.GetAll();
            return metrics.OrderBy(c => c.Id);
        }

        /// <summary>
        /// Gets the list of Labour Job Title.
        /// </summary>
        /// <returns>The list of containment Location</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<ContainmentMetrics>, int> GetSearched(int pageNo, string searchText)
        {
            var containmentMetrics = this.containmentMetricsService.GetSearchedData(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(containmentMetrics, totalCount);
        }

        /// <summary>
        /// Gets the specified identifier. GET api/ContainmentMetrics/{id}
        /// </summary>
        /// <param name="id">The Metrics identifier.</param>
        /// <returns>The Metrics</returns>
        [HttpGet("{id}")]
        public async Task<ContainmentMetrics> Get(long id)
        {
            return await this.containmentMetricsService.Get(id);
        }

        /// <summary>
        /// Gets the specified identifier. GET api/ContainmentMetrics/GetMetricsTypeList
        /// </summary>
        /// <returns>The Get Metrics Type List</returns>
        [HttpGet("GetMetricsTypeList")]
        public List<MetricsTypeViewModel> GetMetricsTypeList()
        {
            return this.containmentMetricsService.GetMetricsTypeList();
        }

        /// <summary>
        /// Posts the specified Metrics.
        /// </summary>
        /// <param name="containmentMetrics">The Metrics.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<ContainmentMetrics> Post([FromBody] ContainmentMetrics containmentMetrics)
        {
            return await this.containmentMetricsService.Create(containmentMetrics);
        }

        /// <summary>
        /// Puts the specified Metrics.
        /// </summary>
        /// <param name="containmentMetrics">The Metrics.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody] ContainmentMetrics containmentMetrics)
        {
            await this.containmentMetricsService.Update(containmentMetrics);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.containmentMetricsService.Delete(id);
        }
    }
}
