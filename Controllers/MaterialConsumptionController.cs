//-----------------------------------------------------------------------
// <copyright file="MaterialConsumptionController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Material consumption controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models.Constants;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The material consumption controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class MaterialConsumptionController : Controller
    {
        /// <summary>
        /// Gets or sets the material consumpton service.
        /// </summary>
        /// <value>
        /// The material consumpton service.
        /// </value>
        private readonly IMaterialConsumptionService materialConsumptonService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialConsumptionController"/> class.
        /// </summary>
        /// <param name="materialConsumptionService">The material consumption service.</param>
        /// <exception cref="ArgumentNullException">materialConsumptonService</exception>
        public MaterialConsumptionController(IMaterialConsumptionService materialConsumptionService)
        {
            this.materialConsumptonService = materialConsumptionService ?? throw new ArgumentNullException("materialConsumptonService");
        }

        /// <summary>
        /// Posts the specified equipment.
        /// </summary>
        /// <param name="materialConsumption">The equipment.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<MaterialConsumption> Post([FromBody]MaterialConsumption materialConsumption)
        {
            return await this.materialConsumptonService.Create(materialConsumption);
        }

        /// <summary>
        /// Puts the specified equipment.
        /// </summary>
        /// <param name="materialConsumption">The material consumption.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody]MaterialConsumption materialConsumption)
        {
            await this.materialConsumptonService.Update(materialConsumption);
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
            await this.materialConsumptonService.Delete(id);
        }

        /// <summary>
        /// Gets the material consumptions.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <returns>The list of material consumption.</returns>
        [HttpGet("getconsumption/{assignmentId}")]
        public async Task<IEnumerable<MaterialConsumptionResponseModel>> GetMaterialConsumptions(long assignmentId)
        {
            return await this.materialConsumptonService.GetConsumptionsByAssignmentIdAsync(assignmentId);
        }
    }
}