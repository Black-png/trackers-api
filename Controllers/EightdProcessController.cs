// <copyright file="EightdProcessController.cs" company="ThingTrax UK Ltd">
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

    [Route("api/[controller]")]
    [ApiController]
    public class EightdProcessController : ControllerBase
    {
        /// <summary>
        /// The eight d process service
        /// </summary>
        private readonly IEightDProcessService eightDProcessService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EightdProcessController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="eightDProcessService">The eight D ProcessService service.</param
        public EightdProcessController(IOptions<ApplicationSettings> applicationSettingsConfig, IEightDProcessService eightDProcessService)
        {
            this.eightDProcessService = eightDProcessService ?? throw new ArgumentNullException("eightDProcessService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>eight d process</returns>
        [HttpGet]
        public async Task<IEnumerable<EightDProcess>> Get()
        {
            return await this.eightDProcessService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>eight d process entity</returns>
        [HttpGet("{id}")]
        public async Task<EightDProcess> Get(long id)
        {
            return await this.eightDProcessService.Get(id);
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of contact.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<EightDProcess>, int> GetSearched(int pageNo, string searchText)
        {
            var contacts = this.eightDProcessService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(contacts, totalCount);
        }

        /// <summary>
        /// Gets the count complaintn eightd process.
        /// </summary>
        /// <returns>task</returns>
        [HttpGet("GetQualityAssuranceTileData")]
        public async Task<QualityAssuranceTilesData> GetCountComplaintnEightdProcess()
        {
            return await this.eightDProcessService.GetCountComplaintnEightdProcess();
        }

        [HttpGet("GetAllFiles/{id}")]
        public async Task<IEnumerable<BlobImageModel>> GetAllFiles(long id)
        {
            return await this.eightDProcessService.GetAllFiles(id);
        }

        /// <summary>
        /// Uploads all files.
        /// </summary>
        /// <param name="eightdProcessId">The eightd process identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("ContainmentActionInterim")]
        public async Task<IEnumerable<ContainmentActionInterimModal>> ContainmentActionInterimList(long eightdProcessId)
        {
            return await this.eightDProcessService.ContainmentActionInterimModal(eightdProcessId);
        }

        /// <summary>
        /// Uploads all files.
        /// </summary>
        /// <returns>Task</returns>
        [HttpGet("EightDProcessRefNo")]
        public async Task<string> GenerateEightDRefNo()
        {
            return await this.eightDProcessService.GenerateEightDRefNo();
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="eightDRefNo">The eightDRefNo.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpGet("CheckEightDRefNoExist")]
        public async Task<bool> EightDRefNoExist(string eightDRefNo, long id)
        {
            return await this.eightDProcessService.IsEightDRefNoExist(eightDRefNo, id);
        }

        /// <summary>
        /// Posts the specified file.
        /// </summary>
        /// <param name="fileList">The file.</param>
        /// <param name="id">The eight d identifier.</param>
        /// <returns>
        /// ss
        /// </returns>
        [HttpPost("UploadFiles/{id}")]
        public async Task Post(IList<IFormFile> fileList, long id)
        {
            await this.eightDProcessService.UploadAllFiles(fileList, id);
        }

        /// <summary>
        /// Posts the specified eight d process.
        /// </summary>
        /// <param name="eightDProcess">The eight d process.</param>
        /// <returns>representing the asynchronous operation</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<EightDProcess> Post([FromBody] EightDProcess eightDProcess)
        {
            return await this.eightDProcessService.Create(eightDProcess);
        }

        /// <summary>
        /// Puts the specified contact.
        /// </summary>
        /// <param name="eightDProcess">The contact.</param>
        /// <returns>contact</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task<EightDProcess> Put([FromBody] EightDProcess eightDProcess)
        {
            return await this.eightDProcessService.Update(eightDProcess);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.eightDProcessService.Delete(id);
        }
    }
}
