//-----------------------------------------------------------------------
// <copyright file="ProductionController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Production controller class.</summary>
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
    using TT.Core.Models.Constants;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Production controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class ProductionController : Controller
    {
        /// <summary>
        /// The production service
        /// </summary>
        private IProductionService productionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionController" /> class.
        /// </summary>
        /// <param name="productionService">The entity service.</param>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        public ProductionController(IOptions<ApplicationSettings> applicationSettingsConfig, IProductionService productionService)
        {
            this.productionService = productionService ?? throw new ArgumentNullException("productionService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Productions</returns>
        [HttpGet]
        public async Task<IEnumerable<Production>> Get()
        {
            return await this.productionService.GetAll();
        }

        /// <summary>
        /// Gets the specified production identifier.
        /// </summary>
        /// <param name="productionId">The production identifier.</param>
        /// <returns>Production</returns>
        [HttpGet("{productionId}")]
        public async Task<Production> Get(long productionId)
        {
            return await this.productionService.Get(productionId);
        }

        /// <summary>
        /// Gets the specified job identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="pageNo">The page no</param>
        /// <returns>Productions</returns>
        [HttpGet("GetProductionByJobId/{jobId}/{pageNo}")]
        public Tuple<IEnumerable<ProductionResponseModel>, int> GetByJobId(long jobId, int pageNo)
        {
            if (jobId <= 0)
            {
                throw new ArgumentNullException("jobId");
            }

            var productions = this.productionService.GetByJobId(jobId, pageNo, this.ApplicationSettings.PageSize, out int totalCount);

            return Tuple.Create(productions, totalCount);
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="fromTime">The from time</param>
        /// <param name="toTime">The to time</param>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<ProductionResponseModel>, int> GetSearched(DateTimeOffset? fromTime, DateTimeOffset? toTime, int pageNo, string searchText)
        {
            var productions = this.productionService.GetSearchdata(fromTime, toTime, pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(productions, totalCount);
        }

        /// <summary>
        /// Posts the specified production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Production> Post([FromBody] Production production)
        {
            if (production.JobEquipmentId <= 0)
            {
                throw new ArgumentNullException("productionData.JobEquipmentId");
            }

            if (production.ProductId <= 0)
            {
                throw new ArgumentNullException("productionData.ProductId");
            }

            if (production.EquipmentShiftId <= 0)
            {
                throw new ArgumentNullException("productionData.EquipmentShiftId");
            }

            return await this.productionService.Create(production);
        }

        /// <summary>
        /// Puts the specified production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody] Production production)
        {
            if (production == null)
            {
                throw new ArgumentNullException("productionData");
            }

            if (production.JobEquipment == null)
            {
                throw new ArgumentNullException("productionData.JobEquipment");
            }

            if (production.ProductId <= 0)
            {
                throw new ArgumentNullException("productionData.ProductId");
            }

            if (production.EquipmentShiftId <= 0)
            {
                throw new ArgumentNullException("productionData.EquipmentShiftId");
            }

            await this.productionService.Update(production);
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
            if (id <= 0)
            {
                throw new ArgumentNullException("id");
            }

            await this.productionService.Delete(id);
        }
    }
}
