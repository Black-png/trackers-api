//-----------------------------------------------------------------------
// <copyright file="AuthorisationController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Authorisation controller class.</summary>
//-----------------------------------------------------------------------
namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Api.Controllers;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Department controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class AuthorisationController : Controller
    {
        /// <summary>
        /// The entity service
        /// </summary>
        private IAuthorisationService authorisationService;

        /// <summary>
        /// The package services
        /// </summary>
        private IPackageServices packageServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorisationController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application setting from configuration</param>
        /// <param name="authorisationService">The entity service.</param>
        /// <param name="packageServices">The package services.</param>
        public AuthorisationController(IOptions<ApplicationSettings> applicationSettingsConfig, IAuthorisationService authorisationService, IPackageServices packageServices)
        {
            this.ApplicationSettings = applicationSettingsConfig.Value;
            this.authorisationService = authorisationService ?? throw new ArgumentNullException("authorisationService");
            this.packageServices = packageServices ?? throw new ArgumentNullException("packageServices");
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Authorised Users</returns>
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            Console.WriteLine("calling GetListOfAuthorisedUsers from line number 67");
            var users = await this.authorisationService.GetListOfAuthorisedUsers();
            return users.Where(x => x.Email != this.User.Identity.Name);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Authorised Users</returns>
        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await this.authorisationService.GetAll();
            return users;
        }

        /// <summary>
        /// Gets the list  of a Authorised User.
        /// Get the objectid of login user
        /// Gets the level of a given objectid.
        /// </summary>
        /// <returns>User level Http response</returns>
        [HttpGet("GetStartupAuth")]
        public async Task<ContentResult> GetStartupAuth()
        {
            ContentResult contentResult = new ContentResult();

            var user = await this.GetAuthorisationLevel();

            if (user != null)
            {
                string host = string.Empty;
                if (!string.IsNullOrEmpty(user.Email))
                {
                    MailAddress userEmail = new MailAddress(user.Email);
                    host = userEmail.Host;
                }

                string script = "(function () {window.auth_level = " + user.RoleId + " || 0; window.auth_id = " + user.Id + " || 0,window.auth_host =  \'" + host + "\' || 0, window.showReleaseDialogue = " + user.ShowReleaseDialogue.ToString().ToLower() + ", window.showFirstLoginDialogue = " + user.ShowFirstLoginDialogue.ToString().ToLower() + "; })();";
                contentResult.Content = script;
                contentResult.ContentType = "application/javascript";
            }

            return contentResult;
        }

        /// <summary>
        /// Gets the list  of a Authorised User.
        /// Get the objectid of login user
        /// Gets the level of a given objectid.
        /// </summary>
        /// <returns>User level Http response</returns>
        [HttpGet("GetUserAuthData")]
        public async Task<UserAuthModel> GetUserAuthData()
        {
            UserAuthModel userAuthModel = new UserAuthModel();

            var user = await this.GetAuthorisationLevel();
            var activePackage = await this.packageServices.GetActivePackage();
            if (user != null)
            {
                string host = string.Empty;
                if (!string.IsNullOrEmpty(user.Email))
                {
                    MailAddress userEmail = new MailAddress(user.Email);
                    host = userEmail.Host;
                }

                string script = "(function () {window.auth_level = " + user.RoleId + "|| window.packageLevel = " + activePackage.Id + " || 0; window.auth_id = " + user.Id + " || 0,window.auth_host =  \'" + host + "\' || 0, window.showReleaseDialogue = " + user.ShowReleaseDialogue.ToString().ToLower() + ", window.showFirstLoginDialogue = " + user.ShowFirstLoginDialogue.ToString().ToLower() + "; })();";
                userAuthModel.ScriptStartUp = script;
                userAuthModel.RoleId = user.RoleId;
                userAuthModel.RoleName = user.Role?.Name;
                userAuthModel.Id = user.Id;
                userAuthModel.UserName = user.Name;
                userAuthModel.UserEmail = user.Email;
                userAuthModel.NotifyEmail = user.NotifyEmail;
                userAuthModel.PhoneNumber = user.PhoneNumber;
                userAuthModel.PackageLevel = activePackage.Level;
                userAuthModel.ShowReleaseDialogue = user.ShowReleaseDialogue;
                userAuthModel.ShowFirstLoginDialogue = user.ShowFirstLoginDialogue;
                userAuthModel.EmailHost = host;
                userAuthModel.UserId = user.UserId;
            }

            return userAuthModel;
        }

        /// <summary>
        /// Gets the list  of a Authorised User.
        /// Get the objectid of login user
        /// Gets the level of a given objectid.
        /// </summary>
        /// <returns>user level</returns>
        [HttpGet("GetAuthorisation")]
        public async Task<User> GetAuthorisationLevel()
        {
            if (this.User == null || this.User.Claims == null)
            {
                throw new Exception("Claim not found for authorised user");
            }

            string claimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            var userId = this.User.Claims
                                  .Where(c => c.Type == claimType)
                                  .Select(c => c.Value)
                                  .SingleOrDefault();

            return await this.Get(userId);
        }

        /// <summary>
        /// Updates user when he seen the release dialog.
        /// </summary>
        /// <param name="dialogType">Type of dialog to be updated.</param>
        /// <returns>The task</returns>
        [HttpGet("updateUserDialogInfo/{dialogType}")]
        public async Task UpdateReleaseDialogForUser(string dialogType)
        {
            if (string.IsNullOrEmpty(dialogType))
            {
                throw new ArgumentNullException("dialogType");
            }

            var user = await this.GetAuthorisationLevel();
            if (user != null)
            {
                if (dialogType == "release")
                {
                    user.ShowReleaseDialogue = false;
                }
                else
                {
                    user.ShowFirstLoginDialogue = false;
                }

                await this.authorisationService.Update(user);
            }
        }

        /// <summary>
        /// Gets the level of a given userid.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>user level</returns>
        [HttpGet("Getuserdetails/{userId}")]
        public async Task<User> GetUserDetails(long userId)
        {
            if (userId == 0)
            {
                return null;
            }

            return await this.authorisationService.GetUserDetailsById(userId);
        }

        /// <summary>
        /// Gets the level of a given userid.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>user level</returns>
        [HttpGet("{userId}")]
        public async Task<User> Get(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var authData = await this.authorisationService.GetUserDetailsByUserId(userId);
            if (authData == null)
            {
                Console.WriteLine("calling GetListOfAuthorisedUsers from line number 238");
                await this.authorisationService.GetListOfAuthorisedUsers();
                authData = await this.authorisationService.GetUserDetailsByUserId(userId);
                if (authData == null)
                {
                    throw new Exception("Authorised user is not found load the A.D users");
                }
            }

            return authData;
        }

        /// <summary>
        /// Gets the level of a given userid.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>user level</returns>
        [HttpGet("{id}")]
        public async Task<long> GetLevel(long id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException("user Id");
            }

            var authData = await this.authorisationService.Get(id);
            if (authData == null)
            {
                Console.WriteLine("calling GetListOfAuthorisedUsers from line number 266");
                await this.authorisationService.GetListOfAuthorisedUsers();
                authData = await this.authorisationService.Get(id);
                if (authData == null)
                {
                    throw new Exception("Authorised user is not found load the A.D users");
                }
            }

            return authData.RoleId.Value;
        }

        /// <summary>
        /// Gets the user area details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The list of user area role details.</returns>
        [HttpGet("getuserareadetails/{id}")]
        public async Task<List<UserAreaRoleResponseModel>> GetUserAreaDetails(long id)
        {
            return await this.authorisationService.GetUserAreaDetails(id);
        }

        /// <summary>
        /// Posts the specified User.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The task</returns>
        [HttpPost]
        public async Task<User> Post([FromBody] User user)
        {
            return await this.authorisationService.Create(user);
        }

        /// <summary>
        /// Puts the specified User.
        /// </summary>
        /// <param name="user">The downtime.</param>
        /// <returns>The task</returns>
        [HttpPut]
        public async Task Put([FromBody] User user)
        {
            await this.authorisationService.Update(user);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task</returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id)
        {
            await this.authorisationService.Delete(id);
        }
    }
}
