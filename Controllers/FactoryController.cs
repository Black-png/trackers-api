//-----------------------------------------------------------------------
// <copyright file="FactoryController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Factory controller class.</summary>
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
    /// Factory controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class FactoryController : Controller
    {
        /// <summary>
        /// The factory service
        /// </summary>
        private IFactoryService factoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="factoryService">The entity service.</param>
        public FactoryController(IOptions<ApplicationSettings> applicationSettingsConfig, IFactoryService factoryService)
        {
            this.factoryService = factoryService ?? throw new ArgumentNullException("factoryService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Factories</returns>
        [HttpGet]
        public async Task<IEnumerable<Factory>> Get()
        {
            var factory = await this.factoryService.GetAll();
            return factory.OrderBy(f => f.Name);
        }

        /// <summary>
        /// Gets the specified factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>Factory</returns>
        [HttpGet("{factoryId}")]
        public async Task<Factory> Get(int factoryId)
        {
            return await this.factoryService.Get(factoryId);
        }

        /// <summary>
        /// Gets the list of factories.
        /// </summary>
        /// <returns>The list of factories</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Factory>, int> GetSearched(int pageNo, string searchText)
        {
            var factories = this.factoryService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(factories, totalCount);
        }

        /// <summary>
        /// Posts the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Factory> Post([FromBody]Factory factory)
        {
           return await this.factoryService.Create(factory);
        }

        /// <summary>
        /// Puts the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Factory factory)
        {
            await this.factoryService.Update(factory);
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
            await this.factoryService.Delete(id);
        }

        /// <summary>
        /// Gets the factory list.
        /// </summary>
        /// <returns>The list of factories to be used to populate factory dropdown.</returns>
        [HttpGet("factories")]
        public async Task<List<DataSelectionModel>> GetFactoryList()
        {
            return await this.factoryService.GetFactories();
        }

        /// <summary>
        /// Gets the factory labels detail by userid.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>
        /// the task
        /// </returns>
        [HttpGet("GetFactoryLabelsDetailByUserid")]
        public async Task<FactoryLabelsDetailModel> GetFactoryLabelsDetailByUserid(int userid)
        {
            return await this.factoryService.GetFactoryLabelsDetailByUserid(userid);
        }

        /// <summary>
        /// Gets the aux factories.
        /// </summary>
        /// <param name="factoryId">The Factory id.</param>
        /// <param name="fromDateTime">The fromDateTime.</param>
        /// <param name="toDateTime">The toDateTime.</param>
        /// <returns>The list of auxilliary factories.</returns>
        [HttpGet("GetFactoryDashboard/{factoryId}/{fromDateTime}/{toDateTime}")]
        public async Task<FactoryDashboardKpiResponseModel> GetFactoryDashboardKpi(long factoryId, DateTimeOffset fromDateTime, DateTimeOffset toDateTime)
        {
            return await this.factoryService.GetFactoryDashboardKpi(factoryId, fromDateTime, toDateTime);
        }

        /// <summary>
        /// Gets the oee factories.
        /// </summary>
        /// <returns>The list of oee factories.</returns>
        [HttpGet("oeefactories")]
        public async Task<List<DataSelectionModel>> GetOeeFactories()
        {
            return await this.factoryService.GetOeeFactories();
        }

        /// <summary>
        /// Gets the silos factories.
        /// </summary>
        /// <returns>The list of silos factories.</returns>
        [HttpGet("silosfactories")]
        public async Task<List<DataSelectionModel>> GetSilosFactories()
        {
            return await this.factoryService.GetSilosFactories();
        }

        /// <summary>
        /// Gets the aux factories.
        /// </summary>
        /// <returns>Thelist of auxilliary factories.</returns>
        [HttpGet("auxfactories")]
        public async Task<List<DataSelectionModel>> GetAuxFactories()
        {
            return await this.factoryService.GetAuxFactories();
        }
    }
}
