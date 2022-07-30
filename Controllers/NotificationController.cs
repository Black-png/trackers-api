//-----------------------------------------------------------------------
// <copyright file="NotificationController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Downtime controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.Entities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// Notification controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        /// <summary>
        /// The downtime service
        /// </summary>
        private INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController" /> class.
        /// </summary>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        /// <param name="notificationService">The entity service.</param>
        public NotificationController(
            IOptions<ApplicationSettings> applicationSettingsConfig,
            INotificationService notificationService)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException("notificationService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the specified factory identifier.
        /// </summary>
        /// <returns>Notification.</returns>
        [HttpGet("GetNotifications")]
        public async Task<IEnumerable<NotificationQueue>> Get()
        {
            return await this.notificationService.GetQueuedNotifications();
        }

        /// <summary>
        /// Gets the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Notification.</returns>
        [HttpGet("GetNotifications/{userId}")]
        public async Task<IEnumerable<NotificationQueue>> Get(long userId)
        {
            return await this.notificationService.GetQueuedNotifications(userId);
        }

        /// <summary>
        /// Gets the notification types.
        /// </summary>
        /// <returns>Task</returns>
        [HttpGet("NotificationTypes")]
        public async Task<IEnumerable<NotificationType>> GetNotificationTypes()
        {
            return await this.notificationService.GetNotificationTypes();
        }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns>Task</returns>
        [HttpGet("NotificationList")]
        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await this.notificationService.GetNotifications();
        }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("GetUserNotifications/{userId}")]
        public async Task<IEnumerable<NotificationResponseModel>> GetUserNotifications(long userId)
        {
            return await this.notificationService.GetUserNotifications(userId);
        }

        /// <summary>
        /// Gets the prefrences.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("GetPrefrences/{userId}")]
        public async Task<IEnumerable<Preference>> GetPrefrences(long userId)
        {
            return await this.notificationService.GetPrefrences(userId);
        }

        /// <summary>
        /// Gets the notification settings.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task</returns>
        [HttpGet("GetNotificationSettings/{userId}")]
        public async Task<IEnumerable<NotificationSetting>> GetNotificationSettings(long userId)
        {
            return await this.notificationService.GetNotificationSettings(userId);
        }

        /// <summary>
        /// Posts the specified factory.
        /// </summary>
        /// <param name="preference">The factory.</param>
        /// <returns>The task</returns>
        [HttpPut("AddPreference")]
        public async Task<Preference> AddPrefrence([FromBody]Preference preference)
        {
            return await this.notificationService.AddPreferences(preference);
        }

        /// <summary>
        /// Posts the specified factory.
        /// </summary>
        /// <param name="setting">The factory.</param>
        /// <returns>The task</returns>
        [HttpPut("AddSetting")]
        public async Task<NotificationSetting> AddSetting([FromBody]NotificationSetting setting)
        {
            return await this.notificationService.AddNotificationSettings(setting);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>
        /// The task
        /// </returns>
        [HttpPut("DismissNotification/{id}/{comment}")]
        public async Task DismissNotification(int id, string comment)
        {
            await this.notificationService.DismissNotification(id, comment);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>
        /// The task
        /// </returns>
        [HttpPut("DismissAllNotification/{comment}")]
        public async Task DismissAllNotification(string comment)
        {
            await this.notificationService.DismissAllNotification(comment);
        }

        /// <summary>
        /// Test notification
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="message">The message</param>
        /// <returns>The task</returns>
        [HttpGet("test/{user}/{message}")]
        public async Task Test(string user, string message)
        {
            await this.notificationService.BroadcastMessageToGroup(message, user);
        }
    }
}