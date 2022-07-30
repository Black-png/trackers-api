// <copyright file="ComplaintController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>

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
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    [Route("api/[Controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        /// <summary>
        /// The complaint service
        /// </summary>
        private readonly IComplaintService complaintService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="complaintService">The complaint service.</param
        public ComplaintController(IOptions<ApplicationSettings> applicationSettingsConfig, IComplaintService complaintService)
        {
            this.complaintService = complaintService ?? throw new ArgumentNullException("complaintService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>contact</returns>
        [HttpGet]
        public async Task<IEnumerable<Complaint>> Get()
        {
            return await this.complaintService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The product</returns>
        [HttpGet("{id}")]
        public async Task<Complaint> Get(long id)
        {
            return await this.complaintService.Get(id);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>contact</returns>
        [HttpGet("complaintSource")]
        public async Task<IEnumerable<ComplaintSource>> GetComplaintSource()
        {
            return await this.complaintService.GetAllComplaintSource();
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of contact.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Complaint>, int> GetSearched(int pageNo, string searchText)
        {
            var contacts = this.complaintService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(contacts, totalCount);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="factoryId">The factoryId text.</param>
        /// <returns>The list of contact.</returns>
        [HttpGet("GetComplaintFactory")]
        public Tuple<IEnumerable<Complaint>, int> GetComplaintbyFactory(int pageNo, string searchText, long factoryId)
        {
            var contacts = this.complaintService.GetAllComplaintByFactory(pageNo, this.ApplicationSettings.PageSize, searchText, factoryId, out int totalCount);
            return Tuple.Create(contacts, totalCount);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of contact.</returns>
        [HttpGet("GetSearchedComplaintSource")]
        public Tuple<IEnumerable<ComplaintSource>, int> GetSearchedComplaintSource(int pageNo, string searchText)
        {
            var contacts = this.complaintService.GetSearchedComplaintSource(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(contacts, totalCount);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The product</returns>
        [HttpGet("complaintSource/{id}")]
        public async Task<ComplaintSource> GetComplaintSource(long id)
        {
            return await this.complaintService.GetComplaintSourceById(id);
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <param name="complaintId">The complaint identifier.</param>
        /// <returns>task</returns>
        [HttpGet("GetAllFiles/{complaintId}")]
        public async Task<IEnumerable<BlobImageModel>> GetAllFiles(long complaintId)
        {
            return await this.complaintService.GetAllFiles(complaintId);
        }

        /// <summary>
        /// Posts the specified complaint.
        /// </summary>
        /// <param name="complaint">The complaint.</param>
        /// <param name="file">The file.</param>
        /// <returns>
        /// newly created complaint task
        /// </returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Complaint> Post([FromBody] Complaint complaint)
        {
            return await this.complaintService.Create(complaint);
        }

        /// <summary>
        /// Posts the specified file.
        /// </summary>
        /// <param name="fileList">The file.</param>
        /// <param name="complaintId">The complaint identifier.</param>
        /// <returns>
        /// ss
        /// </returns>
        [HttpPost("UploadFiles/{complaintId}")]
        public async Task Post(IList<IFormFile> fileList, long complaintId)
        {
            await this.complaintService.UploadAllFiles(fileList, complaintId);
        }

        /// <summary>
        /// Posts the specified complaint source.
        /// </summary>
        /// <param name="complaintSource">The complaint source.</param>
        /// <returns>complaint source</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost("ComplaintSource")]
        public async Task<ComplaintSource> Post([FromBody] ComplaintSource complaintSource)
        {
            return await this.complaintService.CreateComplaintSource(complaintSource);
        }

        /// <summary>
        /// Posts the specified complaint source.
        /// </summary>
        /// <param name="complaintSource">The complaint source.</param>
        /// <returns>complaint source</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut("ComplaintSource")]
        public async Task<ComplaintSource> Put([FromBody] ComplaintSource complaintSource)
        {
            return await this.complaintService.UpdateComplaintSource(complaintSource);
        }

        /// <summary>
        /// Puts the specified complaint.
        /// </summary>
        /// <param name="complaint">The complaint.</param>
        /// <returns>updated complaint task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task<Complaint> Put([FromBody] Complaint complaint)
        {
            return await this.complaintService.Update(complaint);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.complaintService.Delete(id);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpDelete("DeleteComplaintSource/{id}")]
        public async Task DeleteComplaintSource(long id)
        {
            await this.complaintService.DeleteComplaintSource(id);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="complaintRefNo">The complaintRefNo.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpGet("CheckComplaintRefNoExist")]
        public async Task<bool> ComplaintRefNoExist(string complaintRefNo, long id)
        {
           return await this.complaintService.IscomplaintRefNoExist(complaintRefNo, id);
        }
    }
}
