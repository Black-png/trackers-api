//-----------------------------------------------------------------------
// <copyright file="RawMaterialController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>RawMaterial Controllerr class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Class RawMaterialController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class RawMaterialController : Controller
    {
        /// <summary>
        /// The Assignment service
        /// </summary>
        private IRawMaterialService rawMaterialService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="rawMaterialService">The raw material service.</param>
        /// <exception cref="ArgumentNullException">assignmentService</exception>
        public RawMaterialController(IOptions<ApplicationSettings> applicationSettingsConfig, IRawMaterialService rawMaterialService)
        {
            this.rawMaterialService = rawMaterialService ?? throw new ArgumentNullException("rawMaterialService");
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
        public async Task<IEnumerable<RawMaterial>> Get()
        {
            var rawMaterial = await this.rawMaterialService.GetAll();
            return rawMaterial.OrderBy(e => e.Id);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        [HttpGet("{id}")]
        public async Task<RawMaterial> Get(long id)
        {
            return await this.rawMaterialService.GetRawMaterialByIdAsync(id);
        }

        /// <summary>
        /// Gets the list of assignment.
        /// </summary>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text</param>
        /// <returns>The list of equipments.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<RawMaterial>, int> GetRawMaterial(int pageNo, string searchText)
        {
            var rawMaterial = this.rawMaterialService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(rawMaterial, totalCount);
        }

        /// <summary>
        /// Posts the specified value.
        /// </summary>
        /// <param name="rawMaterial">The raw material.</param>
        /// <returns>Task&lt;Assignment&gt;.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<RawMaterial> Post([FromBody]RawMaterial rawMaterial)
        {
            return await this.rawMaterialService.Create(rawMaterial);
        }

        /// <summary>
        /// Puts the specified identifier.
        /// </summary>
        /// <param name="rawMaterial">The raw material.</param>
        /// <returns>Task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]RawMaterial rawMaterial)
        {
            await this.rawMaterialService.Update(rawMaterial);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.rawMaterialService.Delete(id);
        }
    }
}