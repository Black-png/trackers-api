//-----------------------------------------------------------------------
// <copyright file="RejectionController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Rejection controller class.</summary>
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
    using TT.Core.Repository.Sql.ResponseQueryEntities;

    /// <summary>
    /// Rejection controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class RejectionController : Controller
    {
        /// <summary>
        /// The rejection service
        /// </summary>
        private IRejectionService rejectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RejectionController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">App setting configuration</param>
        /// <param name="rejectionService">The entity service.</param>
        public RejectionController(IOptions<ApplicationSettings> applicationSettingsConfig, IRejectionService rejectionService)
        {
            this.rejectionService = rejectionService ?? throw new ArgumentNullException("rejectionService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Rejections</returns>
        [HttpGet]
        public async Task<IEnumerable<Rejection>> Get()
        {
            return await this.rejectionService.GetAll();
        }

        /// <summary>
        /// Gets the specified job identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Productions</returns>
        [HttpGet("job/{jobId}")]
        public async Task<IEnumerable<RejectionResponseModel>> GetByJobId(long jobId)
        {
            if (jobId <= 0)
            {
                throw new ArgumentNullException("jobId");
            }

            return await this.rejectionService.GetByJobId(jobId);
        }

        /// <summary>
        /// Gets the list of rejections.
        /// </summary>
        /// <returns>The list of rejections</returns>
        /// <param name="fromTime">The from time</param>
        /// <param name="toTime">The to time</param>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<RejectionResponseQueryModel>, int> GetSearched(DateTimeOffset? fromTime, DateTimeOffset? toTime, int pageNo, string searchText)
        {
            var productions = this.rejectionService.GetSearchdata(fromTime, toTime, pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(productions, totalCount);
        }

        /// <summary>
        /// Gets the specified rejection identifier.
        /// </summary>
        /// <param name="rejectionId">The rejection identifier.</param>
        /// <returns>Rejection</returns>
        [HttpGet("{rejectionId}")]
        public async Task<Rejection> Get(long rejectionId)
        {
            return await this.rejectionService.Get(rejectionId);
        }

        /// <summary>
        /// Posts the specified rejection.
        /// </summary>
        /// <param name="rejection">The rejection.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Rejection> Post([FromBody]Rejection rejection)
        {
            if (rejection.JobEquipmentId <= 0)
            {
                throw new ArgumentNullException("rejection.JobEquipmentId");
            }

            if (rejection.ProductId <= 0)
            {
                throw new ArgumentNullException("rejection.ProductId");
            }

            if (rejection.EquipmentShiftId <= 0)
            {
                throw new ArgumentNullException("rejection.EquipmentShiftId");
            }

            return await this.rejectionService.Create(rejection);
        }

        /// <summary>
        /// Puts the specified rejection.
        /// </summary>
        /// <param name="rejection">The rejection.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Rejection rejection)
        {
            if (rejection.JobEquipmentId <= 0)
            {
                throw new ArgumentNullException("rejection.JobEquipmentId");
            }

            if (rejection.ProductId <= 0)
            {
                throw new ArgumentNullException("rejection.ProductId");
            }

            if (rejection.EquipmentShiftId <= 0)
            {
                throw new ArgumentNullException("rejection.EquipmentShiftId");
            }

            await this.rejectionService.Update(rejection);
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

            await this.rejectionService.Delete(id);
        }
    }
}
