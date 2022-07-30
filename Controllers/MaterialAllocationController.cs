//-----------------------------------------------------------------------
// <copyright file="MaterialAllocationController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Material allocation controller class.</summary>
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
    /// The material allocation ciontroller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/MaterialAllocation")]
    public class MaterialAllocationController : Controller
    {
        private readonly IMaterialAllocationService materialAllocationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialAllocationController"/> class.
        /// </summary>
        /// <param name="materialAllocationService">The material allocation service.</param>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        public MaterialAllocationController(IOptions<ApplicationSettings> applicationSettingsConfig, IMaterialAllocationService materialAllocationService)
        {
            this.materialAllocationService = materialAllocationService ?? throw new ArgumentNullException("materialAllocationService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the allocations list by params.
        /// </summary>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        /// <param name="assignmentId">Assignment Id text </param>
        /// <returns>The list of material allocations.</returns>
        [HttpGet("GetSearched")]
        public IList<MaterialAllocation> GetAllocationsByParams([FromQuery]int pageNo, string searchText, long assignmentId)
        {
            var result = this.materialAllocationService.GetAllocationsByParams(pageNo, this.ApplicationSettings.PageSize, searchText, assignmentId, out int totalCount);
            return result;
        }

        /// <summary>
        /// Posts the specified equipment.
        /// </summary>
        /// <param name="materialAllocation">The material allocation.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaterialAllocation> Post([FromBody]MaterialAllocation materialAllocation)
        {
            return await this.materialAllocationService.Create(materialAllocation);
        }

        /// <summary>
        /// Puts the specified equipment.
        /// </summary>
        /// <param name="materialAllocation">The equipment.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaterialAllocation materialAllocation)
        {
            await this.materialAllocationService.Update(materialAllocation);
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
            await this.materialAllocationService.Delete(id);
        }

        /// <summary>
        /// Gets the allocations by assignment identifier.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <returns>The list of material allocations.</returns>
        [HttpGet("GetAllocations/{assignmentId}")]
        public async Task<IEnumerable<MaterialAllocation>> GetAllocationsByAssignmentID(long assignmentId)
        {
            return await this.materialAllocationService.GetAllocationsByAssignmentID(assignmentId);
        }
    }
}