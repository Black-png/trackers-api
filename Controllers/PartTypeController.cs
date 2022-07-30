//-----------------------------------------------------------------------
// <copyright file="PartTypeController.cs" company="ThingTrax UK Ltd">
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
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// PartType controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class PartTypeController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IPartTypeService partTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartTypeController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="partTypeService">The entity service.</param>
        public PartTypeController(IOptions<ApplicationSettings> applicationSettingsConfig, IPartTypeService partTypeService)
        {
            this.partTypeService = partTypeService ?? throw new ArgumentNullException("partTypeService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the specified page no.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The searchtext.</param>
        /// <returns>Part type list.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<PartType>, int> GetSearched(int pageNo, string searchText)
        {
            var partTypes = this.partTypeService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(partTypes, totalCount);
        }

        /// <summary>
        /// Posts the specified part type.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <returns>The part type.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<PartType> Post([FromBody]PartType partType)
        {
            return await this.partTypeService.Create(partType);
        }

        /// <summary>
        /// Puts the specified part type.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]PartType partType)
        {
            await this.partTypeService.Update(partType);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.partTypeService.Delete(id);
        }

        /// <summary>
        /// Gets the part types.
        /// </summary>
        /// <returns>The list of part types.</returns>
        [HttpGet("getparttypes")]
        public async Task<List<DataSelectionModel>> GetPartTypes()
        {
            return await this.partTypeService.GetPartTypes();
        }
    }
}