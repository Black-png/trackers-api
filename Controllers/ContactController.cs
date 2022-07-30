// <copyright file="ContactController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc; 
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        /// <summary>
        /// The contact service
        /// </summary>
        private readonly IContactService contactService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactController"/> class.
        /// </summary>
        /// <param name="contactService">The contact service.</param>
        public ContactController(IOptions<ApplicationSettings> applicationSettingsConfig, IContactService contactService)
        {
            this.contactService = contactService ?? throw new ArgumentNullException("contactService");
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
        public async Task<IEnumerable<ExternalContact>> Get()
        {
            return await this.contactService.GetAll();
        }

        /// <summary>
        /// Gets the searched.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>The list of contact.</returns>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<ExternalContact>, int> GetSearched(int pageNo, string searchText)
        {
            var contacts = this.contactService.GetAll(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(contacts, totalCount);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The product</returns>
        [HttpGet("{id}")]
        public async Task<ExternalContact> Get(long id)
        {
            return await this.contactService.Get(id);
        }

        /// <summary>
        /// Posts the specified contact.
        /// </summary>
        /// <param name="contact">The contact.</param>
        /// <returns>contact</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<ExternalContact> Post([FromBody]ExternalContact contact)
        {
            return await this.contactService.Create(contact);
        }

        /// <summary>
        /// Puts the specified contact.
        /// </summary>
        /// <param name="contact">The contact.</param>
        /// <returns>contact</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task<ExternalContact> Put([FromBody] ExternalContact contact)
        {
            return await this.contactService.Update(contact);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.contactService.Delete(id);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>the task</returns>
        [HttpGet("CheckEmailExist")]
        public async Task<bool> ComplaintRefNoExist(string email, long id)
        {
            return await this.contactService.IsEmailExist(email, id);
        }
    }
}
