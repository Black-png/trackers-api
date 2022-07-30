//-----------------------------------------------------------------------
// <copyright file="MaintenanceJobController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Department controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The maintenance job controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class MaintenanceJobController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IMaintenanceJobService maintenanceJobService;

        /// <summary>
        /// The maintenance image service.
        /// </summary>
        private IMaintenanceImageService maintenanceImageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceJobController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="maintenanceJobService">The entity service.</param>
        /// <param name="maintenanceImageService">The maintenance image service.</param>
        public MaintenanceJobController(
            IOptions<ApplicationSettings> applicationSettingsConfig,
            IMaintenanceJobService maintenanceJobService,
            IMaintenanceImageService maintenanceImageService)
        {
            this.maintenanceJobService = maintenanceJobService ?? throw new ArgumentNullException("maintenanceJobService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
            this.maintenanceImageService = maintenanceImageService ?? throw new ArgumentNullException("maintenanceImageService");
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
        /// <param name="typeId">The type id.</param>
        /// <param name="assignedTo">The assigned to.</param>
        /// <param name="priorityId">The priority id.</param>
        /// <returns>The list of maintenance jobs.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<MaintenanceJobResponseModel>, int> GetSearched(int pageNo, string searchText, long typeId, long assignedTo, long priorityId)
        {
            var maintenanceStatus = this.maintenanceJobService.GetSearchdata(pageNo, this.ApplicationSettings.PageSize, searchText, typeId, assignedTo, priorityId, out int totalCount);
            return Tuple.Create(maintenanceStatus, totalCount);
        }

        /// <summary>
        /// Gets the specified maintenance job identifier.
        /// </summary>
        /// <param name="maintenanceJobId">The maintenance job identifier.</param>
        /// <returns>The maintenance job</returns>
        [HttpGet("{maintenanceJobId}")]
        public async Task<MaintenanceJob> Get(long maintenanceJobId)
        {
            return await this.maintenanceJobService.GetMaintenanceJobById(maintenanceJobId);
        }

        /// <summary>
        /// Posts the specified maintenace job.
        /// </summary>
        /// <param name="maintenaceJob">The maintenace job.</param>
        /// <returns>The maintenance job.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaintenanceJob> Post([FromBody]MaintenanceJob maintenaceJob)
        {
            return await this.maintenanceJobService.Create(maintenaceJob);
        }

        /// <summary>
        /// Puts the specified maintenance job.
        /// </summary>
        /// <param name="maintenanceJob">The maintenance job.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaintenanceJob maintenanceJob)
        {
            await this.maintenanceJobService.Update(maintenanceJob);
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
            await this.maintenanceJobService.Delete(id);
        }

        /// <summary>
        /// Ges the maintenance dashboard data.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">searchText</param>
        /// <param name="typeId">typeId</param>
        /// <param name="assignedTo">assignedTo</param>
        /// <param name="priorityId">priorityId</param>
        /// <returns>The maintenance dashboard data.</returns>
        [HttpGet("Dashboard")]
        public async Task<Tuple<MaintenanceDashboardResponseModel, int>> GeMaintenanceDashboardData(long factoryId, int pageNo, string searchText, long typeId, long assignedTo, long priorityId)
        {
            var dashboardData = await this.maintenanceJobService.GetDashboardData(factoryId, pageNo, this.ApplicationSettings.PageSize, searchText, typeId, assignedTo, priorityId);
            return dashboardData;
        }

        /// <summary>
        /// Gets the maintenance master data.
        /// </summary>
        /// <returns>The maintenance module masters data.</returns>
        [HttpGet("maintenancedata")]
        public async Task<MaintenanceMasterDataResponseModel> GetMaintenanceMasterData()
        {
            return await this.maintenanceJobService.GetMaintenanceMasterData();
        }

        /// <summary>
        /// Ges the maintenance dashboard searched data.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="state">The state of maintenance job.</param>
        /// <returns>The maintenance dashboard searched data.</returns>
        [HttpGet("DashboardSearched")]
        public async Task<Tuple<IEnumerable<MaintenanceJobResponseModel>, int>> GeMaintenanceDashboardData(long factoryId, int pageNo, string searchText, string state)
        {
            var dashboardData = await this.maintenanceJobService.GetDashboardSearchedData(factoryId, pageNo, this.ApplicationSettings.PageSize, searchText, state);

            return dashboardData;
        }

        /// <summary>
        /// Actives the maintenance job status.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="toolId">The tool identifier.</param>
        /// <param name="plannedStartdate">The planned startdate.</param>
        /// <param name="plannedCompletionDate">The planned completion date.</param>
        /// <returns>
        /// The list of maintenance notification model.
        /// </returns>
        [HttpGet("IsActiveMaintenaceJob")]
        public async Task<List<MaintenanceNotificationModel>> ActiveMaintenanceJobStatus(long? equipmentId, long factoryId, long? toolId, DateTimeOffset plannedStartdate, DateTimeOffset plannedCompletionDate)
        {
            return await this.maintenanceJobService.ActiveMaintenanceJobStatus(equipmentId, factoryId, toolId, plannedStartdate, plannedCompletionDate);
        }

        /// <summary>
        /// Uploads the maintenance images.
        /// </summary>
        /// <param name="maintenanceImages">The maintenance images.</param>
        /// <returns>The list of created images for maintenance.</returns>
        [HttpPost("UploadImages")]
        public async Task<IEnumerable<MaintenanceImage>> UploadMaintenanceImages([FromBody]List<MaintenanceImage> maintenanceImages)
        {
            return await this.maintenanceImageService.StoreMultipleImages(maintenanceImages);
        }

        /// <summary>
        /// Gets the maintenmance images.
        /// </summary>
        /// <param name="maintenanceJobId">The maintenance job identifier.</param>
        /// <returns>The list of maintenance images.</returns>
        [HttpGet("maintenanceimages/{maintenanceJobId}")]
        public async Task<IEnumerable<MaintenanceImage>> GetMaintenmanceImages(long maintenanceJobId)
        {
            return await this.maintenanceImageService.GetImagesByMaintenanceId(maintenanceJobId);
        }
    }
}