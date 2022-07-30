//-----------------------------------------------------------------------
// <copyright file="JobController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Job controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.RequestModels;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Product planning controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        /// <summary>
        /// The job service
        /// </summary>
        private IJobService jobService;
        private IDowntimeService downtimeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings</param>
        /// <param name="jobService">The entity service.</param>
        /// <param name="downtimeService">The downtime service.</param>
        public JobController(IOptions<ApplicationSettings> applicationSettingsConfig, IJobService jobService, IDowntimeService downtimeService)
        {
            this.jobService = jobService ?? throw new ArgumentNullException("jobService");
            this.downtimeService = downtimeService ?? throw new ArgumentNullException("downtimeService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the list of jobs.
        /// </summary>
        /// <returns>The list of jobs</returns>
        [HttpGet]
        public async Task<IEnumerable<Job>> Get()
        {
            return await this.jobService.GetAll();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The product</returns>
        [HttpGet("{id}")]
        public async Task<Job> Get(long id)
        {
            return await this.jobService.Get(id);
        }

        /// <summary>
        /// Keeps the alive session.
        /// </summary>
        /// <returns>string</returns>
        [HttpGet("KeepAliveSession")]
        public ActionResult<IEnumerable<string>> KeepAliveSession()
        {
            return new string[] { "Active", "Alive" };
        }

        /// <summary>
        /// Gets the list of jobs.
        /// </summary>
        /// <returns>The list of jobs.</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Job>, int> GetSearched(int pageNo, string searchText)
        {
            var jobs = this.jobService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(jobs, totalCount);
        }

        /// <summary>
        /// Gets the jobs by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of jobs</returns>
        [HttpGet("factory/{factoryId}")]
        public async Task<IEnumerable<JobResponseModel>> GetJobsByFactoryId(long factoryId)
        {
            return await this.jobService.GetJobsByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the jobs by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of jobs</returns>
        [HttpGet("runningjobs/{factoryId}")]
        public async Task<IEnumerable<RunningMachineResponseModel>> GetRunningJobsByFactoryId(long factoryId)
        {
            return await this.jobService.GetRunningJobsByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the jobs by equipment identifier.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The list of jobs</returns>
        [HttpGet("equipmentjobs/{equipmentId}")]
        public async Task<List<DataSelectionModel>> GetEquipmentJobs(long equipmentId)
        {
            return await this.jobService.GetEquipmentJob(equipmentId);
        }

        /// <summary>
        /// Gets the jobs by factory identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="equipmentGroupId">The equipment group identifier.</param>
        /// <returns>The list of jobs</returns>
        [HttpGet("factory/{factoryId}/{equipmentGroupId}")]
        public async Task<IEnumerable<JobResponseModel>> GetJobsByFactoryId(int factoryId, int equipmentGroupId)
        {
            return await this.jobService.GetJobsByFactoryId(factoryId, equipmentGroupId);
        }

        /// <summary>
        /// Gets the current job.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The current job</returns>
        [HttpGet("CurrentJob/{equipmentId}")]
        public async Task<Job> GetCurrentJob(long equipmentId)
        {
            return await this.jobService.GetCurrentJob(equipmentId);
        }

        /// <summary>
        /// Gets the current job.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>The product list</returns>
        [HttpGet("Job/{equipmentId}")]
        public async Task<IEnumerable<JobDashboardResponseModel>> GetJob(long equipmentId)
        {
            return await this.jobService.GetEquipmentJobs(equipmentId);
        }

        /// <summary>
        /// Gets the current job.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="shiftId">shiftId</param>
        /// <param name="date">date</param>
        /// <returns>The product list</returns>
        [HttpGet("Job/{equipmentId}/{shiftId}/{date}")]
        public async Task<IEnumerable<JobDashboardResponseModel>> GetJob(long equipmentId, long shiftId, DateTimeOffset date)
        {
            return await this.jobService.GetEquipmentModalJobs(equipmentId, date, shiftId);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <returns>List of Equipment</returns>
        [HttpGet("IsJobAssigned/equipment/{equipmentId}")]
        public async Task<bool> IsJobAssigned(long equipmentId)
        {
            return await this.jobService.AnyJobExistsInEquipment(equipmentId);
        }

        /// <summary>
        /// Gets the  job history.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <returns>The job </returns>
        [HttpGet("history/{factoryId}/{pageNo}")]
        public Tuple<IEnumerable<JobDashboardResponseModel>, int> GetJobHistory(long factoryId, int pageNo)
        {
            var jobs = this.jobService.GetJobHistory(factoryId, pageNo, this.ApplicationSettings.PageSize, string.Empty, out int totalCount);
            return Tuple.Create(jobs, totalCount);
        }

        /// <summary>
        /// Gets the job history.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <param name="toolNumber">The search Text.</param>
        /// <returns>The job </returns>
        [HttpGet("history/{factoryId}/{equipmentId}/{toolNumber}/{pageNo}")]
        public Tuple<IEnumerable<JobDashboardResponseModel>, int> GetJobHistory(long factoryId, long equipmentId, int pageNo, string toolNumber)
        {
            IEnumerable<JobDashboardResponseModel> jobs = null;
            if (equipmentId < 1)
            {
                jobs = this.jobService.GetJobHistory(factoryId, pageNo, this.ApplicationSettings.PageSize, toolNumber, out int totalCount);
                return Tuple.Create(jobs, totalCount);
            }
            else
            {
                jobs = this.jobService.GetJobHistory(factoryId, equipmentId, pageNo, this.ApplicationSettings.PageSize, toolNumber, out int totalCount);
                return Tuple.Create(jobs, totalCount);
            }
        }

        /// <summary>
        /// Posts the specified job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Job> Post([FromBody]Job job)
        {
            if (job.JobProducts.Count <= 0)
            {
                throw new ArgumentNullException("job.Products");
            }

            return await this.jobService.Create(job);
        }

        /// <summary>
        /// Puts the specified job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Job job)
        {
            if (job.JobProducts.Count <= 0)
            {
                throw new ArgumentNullException("job.Products");
            }

            await this.jobService.Update(job);
        }

        /// <summary>
        /// Puts the specified job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <exception cref="ArgumentNullException">job.Products</exception>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut("MovejobToUnassigned")]
        public async Task MovejobToUnassigned([FromBody] Job job)
        {
            if (job.JobProducts.Count <= 0)
            {
                throw new ArgumentNullException("job.Products");
            }

            await this.jobService.MoveJobAssignedToUnassigned(job);
        }

        /// <summary>
        /// Gets the job by equipment identifier.
        /// </summary>
        /// <param name="request">The move job request model.</param>
        /// <returns>The JobUpdateResponse product </returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut("MoveJob")]
        public async Task<MoveJobResponseModel> MoveJob([FromBody]MoveJobRequestModel request)
        {
            return await this.jobService.MoveJob(request);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("{id}")]
        public async Task<List<Job>> Delete(long id)
        {
            return await this.jobService.Delete(id);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpDelete("DeleteJobById/{id}")]
        public async Task<IList<JobDashboardResponseModel>> DeleteJobById(long id)
        {
            return await this.jobService.DeleteJobById(id);
        }

        /// <summary>
        /// Gets the list of jobs.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of jobs</returns>
        [HttpGet("GetUnassignedJobs/{factoryId}")]
        public async Task<IEnumerable<Job>> GetUnassignedJobs(long factoryId)
        {
            return await this.jobService.GetUnassignedJobs(factoryId);
        }

        /// <summary>
        /// Gets the jobs by factory identifier.
        /// </summary>
        /// <param name="factoryId">The equipment identifier.</param>
        /// <returns>The list of jobs</returns>
        [HttpGet("factoryjobs/{factoryId}")]
        public async Task<List<DataSelectionModel>> GetFactoryJobs(long factoryId)
        {
            return await this.jobService.GetFactoryJobs(factoryId, true);
        }

        /// <summary>
        /// Calculates the job timings.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="plannedCycleTime">The planned cycle time.</param>
        /// <param name="cavity">The cavity.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="setupTime">The setup time.</param>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>
        /// The dictionary containing planned start time and planned completion time for job.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">priority</exception>
        [HttpGet("CalculateJobTimings/{quantity}/{plannedCycleTime}/{cavity}/{equipmentId}/{productId}/{jobId}/{setupTime}/{toolId}")]
        public async Task<Dictionary<string, string>> CalculateJobTimings(
            long quantity,
            double plannedCycleTime,
            double cavity,
            long equipmentId,
            long productId,
            long? jobId,
            double setupTime,
            long? toolId)
        {
            return await this.jobService.CalculateJobTimings(
                quantity,
                plannedCycleTime,
                cavity,
                equipmentId,
                productId,
                jobId,
                setupTime,
                toolId);
        }

        /// <summary>
        /// Posts the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The task.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost("importjobs/{factoryId}")]
        public async Task<List<string>> PostFile(IFormFile file, long factoryId)
        {
            List<string> errorList = new List<string>();
            if (file == null)
            {
                errorList.Add(string.Format("File is null."));
            }

            var stream = file.OpenReadStream();
            errorList = await this.jobService.ReadDataFromJobFile(stream, factoryId);
            return errorList;
        }

        /// <summary>
        /// Exports the factory jobs.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The file stream result.</returns>
        [HttpGet("ExportJobs")]
        public async Task<FileStreamResult> ExportFactoryJobs(long factoryId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            var dataString = await this.jobService.ExportFactoryActiveJobs(factoryId);
            MemoryStream memStream = new MemoryStream();
            string contentType = "text/csv";
            try
            {
                byte[] fileData = System.Text.Encoding.UTF8.GetBytes(dataString);
                memStream.Write(fileData, 0, fileData.Length);
                memStream.Position = 0;

                this.Response.Headers.Add("Content-Type", contentType);
                return new FileStreamResult(memStream, contentType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates the state of the job.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="state">The state.</param>
        /// <returns>The updated job.</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut("updatestate/{jobId}/{state}")]
        public async Task<Job> UpdateJobState(long jobId, string state)
        {
            return await this.jobService.UpdateJobState(jobId, state);
        }
    }
}
