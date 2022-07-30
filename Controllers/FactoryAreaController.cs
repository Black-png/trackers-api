//-----------------------------------------------------------------------
// <copyright file="FactoryAreaController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Factory area controller class.</summary>
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
    /// Factory area controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/FactoryArea")]
    public class FactoryAreaController : Controller
    {
        /// <summary>
        /// The factory area service
        /// </summary>
        private IFactoryAreaService factoryAreaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryAreaController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="factoryAreaService">The factory area service.</param>
        /// <exception cref="System.ArgumentNullException">factoryService</exception>
        public FactoryAreaController(IOptions<ApplicationSettings> applicationSettingsConfig, IFactoryAreaService factoryAreaService)
        {
            this.factoryAreaService = factoryAreaService ?? throw new ArgumentNullException("factoryService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Factory areas.</returns>
        [HttpGet]
        public async Task<IEnumerable<FactoryArea>> Get()
        {
            return await this.factoryAreaService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Factory area.</returns>
        [HttpGet("{Id}")]
        public async Task<FactoryArea> Get(int id)
        {
            return await this.factoryAreaService.Get(id);
        }

        /// <summary>
        /// Gets the factory area.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("factory/{factoryId}")]
        public async Task<IEnumerable<FactoryArea>> GetFactoryArea(int factoryId)
        {
            return await this.factoryAreaService.GetFactoryAreasByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>Task.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<FactoryArea>, int> GetSearched(int pageNo, string searchText)
        {
            var factoryArea = this.factoryAreaService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(factoryArea, totalCount);
        }

        /// <summary>
        /// Posts the specified factory area.
        /// </summary>
        /// <param name="factoryArea">The factory area.</param>
        /// <returns>Factory area</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<FactoryArea> Post([FromBody]FactoryArea factoryArea)
        {
            return await this.factoryAreaService.Create(factoryArea);
        }

        /// <summary>
        /// Puts the specified factory area.
        /// </summary>
        /// <param name="factoryArea">The factory area.</param>
        /// <returns>Task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]FactoryArea factoryArea)
        {
            await this.factoryAreaService.Update(factoryArea);
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
            await this.factoryAreaService.Delete(id);
        }
    }
}