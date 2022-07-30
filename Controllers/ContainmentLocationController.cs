// <copyright file="ContainmentLocationController.cs" company="PlaceholderCompany">
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
    using TT.Core.Models.Configurations;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// ContainmentLocationController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ContainmentLocationController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IContainmentLocationService containmentLocationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainmentLocationController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="containmentLocationService">The ContainmentLocation services.</param>
        /// <exception cref="ArgumentNullException">IContainmentLocationService</exception>
        public ContainmentLocationController(IOptions<ApplicationSettings> applicationSettingsConfig, IContainmentLocationService containmentLocationService)
        {
            this.containmentLocationService = containmentLocationService ?? throw new ArgumentNullException("containmentLocationService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the list of Labour Job Title.
        /// </summary>
        /// <returns>The list of containment Location</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<ContainmentLocation>, int> GetSearched(int pageNo, string searchText)
        {
            var containmentLocation = this.containmentLocationService.GetSearchedData(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(containmentLocation, totalCount);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Data and count</returns>
        [HttpGet]
        public async Task<IEnumerable<ContainmentLocation>> Get()
        {
            var locations = await this.containmentLocationService.GetAll();
            return locations.OrderBy(c => c.Id);
        }

        /// <summary>
        /// Gets the specified identifier. GET api/ContainmentLocation/{id}
        /// </summary>
        /// <param name="id">The Location identifier.</param>
        /// <returns>The Location</returns>
        [HttpGet("{id}")]
        public async Task<ContainmentLocation> Get(long id)
        {
            return await this.containmentLocationService.Get(id);
        }

        /// <summary>
        /// Posts the specified Location.
        /// </summary>
        /// <param name="containmentLocation">The Location.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<ContainmentLocation> Post([FromBody] ContainmentLocation containmentLocation)
        {
            return await this.containmentLocationService.Create(containmentLocation);
        }

        /// <summary>
        /// Puts the specified Location.
        /// </summary>
        /// <param name="containmentLocation">The Location.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody] ContainmentLocation containmentLocation)
        {
            await this.containmentLocationService.Update(containmentLocation);
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
            await this.containmentLocationService.Delete(id);
        }
    }
}
