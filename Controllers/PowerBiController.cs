//-----------------------------------------------------------------------
// <copyright file="PowerBiController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>PowerBiController class.</summary>
//-----------------------------------------------------------------------
namespace TT.Core.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Api.Models;
    using TT.Core.Models.PowerBIEmbed;
    using TT.Core.Services.Extensions;
    using TT.Core.Services.InetegrationServices;
    using TT.Core.Services.InetegrationServices.Interfaces;

    /// <summary>
    /// Power Bi controller is for external contraction
    /// between app and Ms Power Bi server to get Token
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class PowerBiController : Controller
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        private readonly IPowerBiEmbedService powerBiEmbedService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerBiController" /> class.
        /// </summary>
        /// <param name="powerBiEmbedService">The Power BI embed service.</param>
        public PowerBiController(IPowerBiEmbedService powerBiEmbedService)
        {
            this.powerBiEmbedService = powerBiEmbedService;
        }

        [HttpGet("GetPowerBiCustomerEmbedParams/{reportName}")]
        public async Task<EmbedParams> GetPowerBiCustomerEmbedParams(string reportName)
        {
            EmbedParams embedParams = await this.powerBiEmbedService.GetEmbedParams(reportName);
            return embedParams;
        }

        [HttpGet("RefreshDatasetData/{reportName}")]
        public async Task<bool> RefreshDatasetData(string reportName)
        {
            return await this.powerBiEmbedService.RefresahDatasetData(reportName);
        }
    }
}
