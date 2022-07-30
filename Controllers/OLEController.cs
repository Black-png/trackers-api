//-----------------------------------------------------------------------
// <copyright file="OLEController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>OLE controller class.</summary>
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
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// OLE controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/OLE")]
    public class OLEController : Controller
    {
        /// <summary>
        /// The factory area service
        /// </summary>
        private IOleService oleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OLEController"/> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="oleService">The OLE service.</param>
        /// <exception cref="System.ArgumentNullException">factoryService</exception>
        public OLEController(IOptions<ApplicationSettings> applicationSettingsConfig, IOleService oleService)
        {
            this.oleService = oleService ?? throw new ArgumentNullException("factoryService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>OLE.</returns>
        [HttpGet]
        public async Task<IEnumerable<Ole>> Get()
        {
            return await this.oleService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>OLE.</returns>
        [HttpGet("{Id}")]
        public async Task<Ole> Get(int id)
        {
            return await this.oleService.Get(id);
        }
    }
}