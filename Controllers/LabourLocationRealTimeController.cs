//-----------------------------------------------------------------------
// <copyright file="LabourLocationRealTimeController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Labour location real time controller class.</summary>
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
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Labour location real time controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class LabourLocationRealTimeController : Controller
    {
        /// <summary>
        /// The labour location real time service
        /// </summary>
        private ILabourLocationRealTimeService labourLocationRealTimeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabourLocationRealTimeController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="labourLocationRealTimeService">The labour location real time service.</param>
        /// <exception cref="ArgumentNullException">labourService</exception>
        public LabourLocationRealTimeController(IOptions<ApplicationSettings> applicationSettingsConfig, ILabourLocationRealTimeService labourLocationRealTimeService)
        {
            this.labourLocationRealTimeService = labourLocationRealTimeService ?? throw new ArgumentNullException("labourService");
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
        public async Task<IEnumerable<LabourLocationRealTime>> Get()
        {
            return await this.labourLocationRealTimeService.GetAll();
        }

        /// <summary>
        /// Gets the list of factories.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<LabourLocationRealTime>, int> GetSearched(int pageNo, string searchText)
        {
            var labours = this.labourLocationRealTimeService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(labours, totalCount);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The labour</returns>
        [HttpGet("{id}")]
        public async Task<LabourLocationRealTime> Get(long id)
        {
            return await this.labourLocationRealTimeService.Get(id);
        }

        /// <summary>
        /// Gets the kpi report.
        /// </summary>
        /// <param name="factoryId">The factory Identifier.</param>
        /// <param name="labourTypeId">The labour type identifier.</param>
        /// <returns>
        /// The list of kpi data.
        /// </returns>
        /// <exception cref="ArgumentNullException">productId</exception>
        [HttpGet("livelabour/{factoryId}/{labourTypeId}")]
        public async Task<LabourDasboardResponseModel> GetLiveLabour(long factoryId, int? labourTypeId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.labourLocationRealTimeService.GetLiveLabourLocation(factoryId, labourTypeId);
        }
    }
}