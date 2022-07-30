//-----------------------------------------------------------------------
// <copyright file="OnBoardingController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>OnBoardingController class.</summary>
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
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.MobileAppModel;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Class OnBoardingController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/OnBoarding")]
    public class OnBoardingController : Controller
    {
        /// <summary>
        /// The Assignment service
        /// </summary>
        private IStepService stepService;

        /// <summary>
        /// The Assignment service
        /// </summary>
        private IOnBoardingService onboardingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnBoardingController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">The application settings configuration.</param>
        /// <param name="stepService">The step service.</param>
        /// <param name="onboardingService">The onboarding service.</param>
        /// <exception cref="ArgumentNullException">stepService</exception>
        /// <exception cref="ArgumentNullException">onboardingService</exception>
        public OnBoardingController(IOptions<ApplicationSettings> applicationSettingsConfig, IStepService stepService, IOnBoardingService onboardingService)
        {
            this.stepService = stepService ?? throw new ArgumentNullException("stepService");
            this.onboardingService = onboardingService ?? throw new ArgumentNullException("onboardingService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this All Steps.
        /// </summary>
        /// <returns>Data and count</returns>
        [HttpGet("GetAllSteps")]
        public async Task<IEnumerable<Step>> GetAllSteps()
        {
            var steps = await this.stepService.GetAll();
            return steps.OrderBy(e => e.Id);
        }

        /// <summary>
        /// Gets the specified OnBoarding identifier.
        /// </summary>
        /// <param name="factoryId">The factoryId.</param>
        /// <returns>OnBoarding</returns>
        [HttpGet("GetAllBoarding/{factoryId}")]
        public async Task<IEnumerable<OnBoardingDataModel>> GetAllBoarding(long factoryId)
        {
            return await this.onboardingService.GetByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the specified OnBoarding identifier.
        /// </summary>
        /// <param name="equipmentId">The equipmentId.</param>
        /// <returns>OnBoarding</returns>
        [HttpGet("GetBoarding/{equipmentId}")]
        public async Task<IEnumerable<OnBoardingModel>> GetBoarding(long equipmentId)
        {
            return await this.onboardingService.GetByEquipmentId(equipmentId);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="equipmentTypeId">The identifier.</param>
        /// <returns>System.String.</returns>
        [HttpGet("GetEquipmentSteps/{equipmentTypeId}")]
        public async Task<EquipmentStepVm> GetEquipmentSteps(long equipmentTypeId)
        {
            return await this.stepService.GetByEquipmentTypeId(equipmentTypeId);
        }

        /// <summary>
        /// Posts the specified value.
        /// </summary>
        /// <param name="boarding">The onboarding.</param>
        /// <returns>Task&lt;onboarding&gt;.</returns>
        [HttpPost]
        public async Task<OnBoarding> Post([FromBody]OnBoarding boarding)
        {
            if (boarding == null)
            {
                throw new ArgumentException("boarding");
            }

            return await this.onboardingService.Create(boarding);
        }

        /// <summary>
        /// Puts the specified boarding.
        /// </summary>
        /// <param name="boarding">The boarding.</param>
        /// <returns>The task</returns>
        [HttpPut]
        public async Task Put([FromBody]OnBoarding boarding)
        {
            if (boarding == null)
            {
                throw new ArgumentException("boarding");
            }

            await this.onboardingService.Update(boarding);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.onboardingService.Delete(id);
        }
    }
}
