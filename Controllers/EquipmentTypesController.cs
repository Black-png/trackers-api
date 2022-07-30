//-----------------------------------------------------------------------
// <copyright file="EquipmentTypesController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Dashboards controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The equipment types controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[Controller]")]
    public class EquipmentTypesController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IEquipmentTypeService equipmentTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentTypesController"/> class.
        /// </summary>
        /// <param name="equipmentTypeService">The equipment type service.</param>
        /// <exception cref="ArgumentNullException">equipmentTypeService</exception>
        public EquipmentTypesController(IEquipmentTypeService equipmentTypeService)
        {
            this.equipmentTypeService = equipmentTypeService ?? throw new ArgumentNullException("equipmentTypeService");
        }

        /// <summary>
        /// Gets the equipment types asynchronous.
        /// </summary>
        /// <returns>The list of equipment types</returns>
        [HttpGet]
        public async Task<IEnumerable<EquipmentType>> GetEquipmentTypesAsync()
        {
            return await this.equipmentTypeService.GetAllEquipmentTypes();
        }
    }
}