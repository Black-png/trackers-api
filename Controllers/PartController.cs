//-----------------------------------------------------------------------
// <copyright file="PartController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Department controller class.</summary>
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
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The part controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class PartController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IPartService partService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="partService">The entity service.</param>
        public PartController(IOptions<ApplicationSettings> applicationSettingsConfig, IPartService partService)
        {
            this.partService = partService ?? throw new ArgumentNullException("partService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of parts.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Part>, int> GetSearched(int pageNo, string searchText)
        {
            var parts = this.partService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(parts, totalCount);
        }

        /// <summary>
        /// Posts the specified part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns>The part</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Part> Post([FromBody]Part part)
        {
            return await this.partService.Create(part);
        }

        /// <summary>
        /// Puts the specified part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Part part)
        {
            await this.partService.Update(part);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.partService.Delete(id);
        }

        /// <summary>
        /// Gets the part types.
        /// </summary>
        /// <param name="partTypeId">The part type identifier.</param>
        /// <returns>The list of parts.</returns>
        [HttpGet("getpartsbyparttype/{partTypeId}")]
        public async Task<List<DataSelectionModel>> GetPartTypes(long partTypeId)
        {
            return await this.partService.GetPartByPartTypeId(partTypeId);
        }

        /// <summary>
        /// Gets the parts.
        /// </summary>
        /// <returns>The list of parts.</returns>
        [HttpGet("getparts")]
        public async Task<List<DataSelectionModel>> GetParts()
        {
            return await this.partService.GetParts();
        }
    }
}