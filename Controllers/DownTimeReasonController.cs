//-----------------------------------------------------------------------
// <copyright file="DownTimeReasonController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Downtime reason controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models.Configurations;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// DownTime reason controller class.
    /// </summary>
    [Route("api/[controller]")]
    public class DowntimeReasonController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IDowntimeReasonService downTimeReasonService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DowntimeReasonController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">the application settings configuration </param>
        /// <param name="downTimeReasonService">The entity service.</param>
        public DowntimeReasonController(IOptions<ApplicationSettings> applicationSettingsConfig, IDowntimeReasonService downTimeReasonService)
        {
            this.downTimeReasonService = downTimeReasonService ?? throw new ArgumentNullException("downTimeReasonService");
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
        public async Task<IEnumerable<DowntimeReason>> Get()
        {
            var downtimeReasons = await this.downTimeReasonService.GetAll();
            return downtimeReasons.OrderBy(e => e.Name);
        }

        /// <summary>
        /// Gets the list of downtime reasons.
        /// </summary>
        /// <returns>The list of downtime reasons.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<DowntimeReason>, int> GetSearched(int pageNo, string searchText)
        {
            var downtimeReasons = this.downTimeReasonService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(downtimeReasons, totalCount);
        }

        /// <summary>
        /// Gets the specified entity identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <returns>The entity</returns>
        [HttpGet("{id}")]
        public async Task<DowntimeReason> Get(long id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException("id");
            }

            return await this.downTimeReasonService.Get(id);
        }

        /// <summary>
        /// Posts the specified entity.
        /// </summary>
        /// <param name="model">The entity.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = RolesConstant.Setter)]
        [HttpPost]
        public async Task<DowntimeReason> Post([FromBody]DowntimeReason model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (model.IsDefault)
            {
                throw new InvalidOperationException("User cannot add default downtime reason");
            }

            return await this.downTimeReasonService.Create(model);
        }

        /// <summary>
        /// Puts the specified entity.
        /// </summary>
        /// <param name="model">The entity.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = RolesConstant.Setter)]
        [HttpPut]
        public async Task Put([FromBody]DowntimeReason model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("entity");
            }

            await this.downTimeReasonService.Update(model);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        ////[Authorize(Policy = RolesConstant.Admin)]
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException("id");
            }

            await this.downTimeReasonService.Delete(id);
        }

        /// <summary>
        /// Gets the specified entity identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <returns>The entity</returns>
        [HttpGet("defaultdowntimereason/{id}")]
        public async Task<DowntimeReason> GetDefaultDowntimeReason(long id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException("id");
            }

            return await this.downTimeReasonService.GetDefaultDowntimeReason(id);
        }

        /// <summary>
        /// Determines whether [is downtimereason exists] [the specified code].
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns> true or false value.</returns>
        [HttpGet("checkdowntimereasoncode/{code}")]
        public async Task<bool> IsDowntimereasonExists(string code)
        {
            return await this.downTimeReasonService.ReasonCodeExists(code);
        }
    }
}
