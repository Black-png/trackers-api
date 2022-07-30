//-----------------------------------------------------------------------
// <copyright file="RegionOfInterestController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Region of interest controller class.</summary>
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
    /// Region of interest controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/RegionOfInterest")]
    public class RegionOfInterestController : Controller
    {
        /// <summary>
        /// The factory area service
        /// </summary>
        private IRegionOfInterestService regionOfInterestService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionOfInterestController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="regionOfInterestService">The region of interest service.</param>
        /// <exception cref="System.ArgumentNullException">factoryService</exception>
        public RegionOfInterestController(IOptions<ApplicationSettings> applicationSettingsConfig, IRegionOfInterestService regionOfInterestService)
        {
            this.regionOfInterestService = regionOfInterestService ?? throw new ArgumentNullException("factoryService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>regionOfInterest</returns>
        [HttpGet]
        public async Task<IEnumerable<RegionOfInterest>> Get()
        {
            return await this.regionOfInterestService.GetAll();
        }

        /// <summary>
        /// Gets the Region of Interest.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("factory/{factoryId}")]
        public async Task<IEnumerable<DataSelectionModel>> GetFactoryArea(int factoryId)
        {
            return await this.regionOfInterestService.GetFactoryROIByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>regionOfInterest</returns>
        [HttpGet("{Id}")]
        public async Task<RegionOfInterest> Get(int id)
        {
            return await this.regionOfInterestService.Get(id);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>Task.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<RegionOfInterest>, int> GetSearched(int pageNo, string searchText)
        {
            var factoryArea = this.regionOfInterestService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(factoryArea, totalCount);
        }

        /// <summary>
        /// Posts the specified region of interest.
        /// </summary>
        /// <param name="regionOfInterest">The region of interest.</param>
        /// <returns>Region Of Interest.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<RegionOfInterest> Post([FromBody]RegionOfInterest regionOfInterest)
        {
            return await this.regionOfInterestService.Create(regionOfInterest);
        }

        /// <summary>
        /// Puts the specified region of interest.
        /// </summary>
        /// <param name="regionOfInterest">The region of interest.</param>
        /// <returns>Task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]RegionOfInterest regionOfInterest)
        {
            await this.regionOfInterestService.Update(regionOfInterest);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await this.regionOfInterestService.Delete(id);
        }
    }
}