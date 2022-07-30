//-----------------------------------------------------------------------
// <copyright file="DepartmentController.cs" company="ThingTrax UK Ltd">
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
    using Services.Interfaces;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Department controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IDepartmentService departmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration </param>
        /// <param name="departmentService">The entity service.</param>
        public DepartmentController(IOptions<ApplicationSettings> applicationSettingsConfig, IDepartmentService departmentService)
        {
            this.departmentService = departmentService ?? throw new ArgumentNullException("departmentService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Departments</returns>
        [HttpGet]
        public async Task<IEnumerable<Department>> Get()
        {
            return await this.departmentService.GetAll();
        }

        /// <summary>
        /// Gets the specified department identifier.
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <returns>Department</returns>
        [HttpGet("{departmentId}")]
        public async Task<Department> Get(long departmentId)
        {
            return await this.departmentService.Get(departmentId);
        }

        /// <summary>
        /// Gets the specified page no.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The searchtext.</param>
        /// <returns>Departments</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<Department>, int> GetSearched(int pageNo, string searchText)
        {
            var departments = this.departmentService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(departments, totalCount);
        }

        /// <summary>
        /// Gets the department by factory.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>Return departments.</returns>
        [HttpGet("factory/{factoryId}")]
        public async Task<List<DataSelectionModel>> GetDepartmentsByFactory(long factoryId)
        {
            return await this.departmentService.GetDepartmentsByFactory(factoryId);
        }

        /// <summary>
        /// Posts the specified department.
        /// </summary>
        /// <param name="department">The department.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Department> Post([FromBody]Department department)
        {
           return await this.departmentService.Create(department);
        }

        /// <summary>
        /// Puts the specified department.
        /// </summary>
        /// <param name="department">The department.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]Department department)
        {
            await this.departmentService.Update(department);
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
            await this.departmentService.Delete(id);
        }
    }
}
