//-----------------------------------------------------------------------
// <copyright file="RejectionReasonController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Rejection reason controller class.</summary>
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
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Rejection reason controller class.
    /// </summary>
    [Route("api/[controller]")]
    public class RejectionReasonController : Controller
    {
        /// <summary>
        /// The rejection reason service
        /// </summary>
        private readonly IRejectionReasonService rejectionReasonService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RejectionReasonController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">App setting</param>
        /// <param name="rejectionReasonService">The entity service.</param>
        public RejectionReasonController(IOptions<ApplicationSettings> applicationSettingsConfig, IRejectionReasonService rejectionReasonService)
        {
            this.rejectionReasonService = rejectionReasonService ?? throw new ArgumentNullException("rejectionReasonService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>The entity list</returns>
        [HttpGet]
        public async Task<IEnumerable<RejectionReason>> Get()
        {
            return await this.rejectionReasonService.GetAll();
        }

        /// <summary>
        /// Gets the list of rejection reasons.
        /// </summary>
        /// <returns>The list of rejection reasons.</returns>
        /// <param name="pageNo">Page Number.</param>
        /// <param name="searchText">Serach text.</param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<RejectionReason>, int> GetSearched(int pageNo, string searchText)
        {
            var rejectionReasons = this.rejectionReasonService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(rejectionReasons, totalCount);
        }

        /// <summary>
        /// Gets the specified entity identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <returns>The entity</returns>
        [HttpGet("{id}")]
        public async Task<RejectionReason> Get(long id)
        {
            return await this.rejectionReasonService.Get(id);
        }

        /// <summary>
        /// Posts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<RejectionReason> Post([FromBody]RejectionReason entity)
        {
           return await this.rejectionReasonService.Create(entity);
        }

        /// <summary>
        /// Puts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]RejectionReason entity)
        {
            await this.rejectionReasonService.Update(entity);
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
            await this.rejectionReasonService.Delete(id);
        }
    }
}
