//-----------------------------------------------------------------------
// <copyright file="GlobalExceptionLogger.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Global exception logger class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.WindowsAzure.Storage;
    using Serilog;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.RequestModels;
    using TT.Core.Repository;
    using TT.Core.Repository.Interfaces;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// GlobalExceptionLogger class
    /// </summary>
    public class GlobalExceptionLogger : IExceptionFilter
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private ITableStorageRepository<TableStorageErrorModel> tableStorageRepository;

        /// <summary>
        /// The email service
        /// </summary>
        private IEmailService emailService;

        /// <summary>
        /// The memory cache
        /// </summary>
        private IMemoryCache memoryCache;

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The telemetry client
        /// </summary>
        private TelemetryClient telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionLogger" /> class.
        /// </summary>
        /// <param name="productionStorageConnectionString">The production storage connection string.</param>
        /// <param name="apiErrorTableName">Name of the API error table.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="memoryCache">The memory cache.</param>
        /// <param name="logger">The logger.</param>
        public GlobalExceptionLogger(string productionStorageConnectionString, string apiErrorTableName, IEmailService emailService, IMemoryCache memoryCache, ILogger logger)
        {
            // Parse the connection string and return a reference to the storage account.
            AzureStorageConfig azureStorageConfig = new AzureStorageConfig
            {
                ConnectionString = productionStorageConnectionString,
            };

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageConfig.ConnectionString);
            this.tableStorageRepository = new TableStorageRepository<TableStorageErrorModel>(azureStorageConfig, apiErrorTableName);

            this.emailService = emailService;
            this.memoryCache = memoryCache ?? throw new ArgumentNullException("memoryCache");
            this.logger = logger ?? throw new ArgumentNullException("logger");
        }

        /// <summary>
        /// Exception implementation.
        /// </summary>
        /// <param name="context">The ExceptionContext.</param>
        public void OnException(ExceptionContext context)
        {
            try
            {
                var errorEntity = new TableStorageErrorModel()
                {
                    Created = DateTime.UtcNow,
                    Component = "TT.Core.Web",
                    ErrorMessage = context.Exception.ToString(),
                    RowKey = Guid.NewGuid().ToString(),
                    PartitionKey = "1",
                };

                var task = this.tableStorageRepository.InsertEntities(new List<TableStorageErrorModel>() { errorEntity });
                Task.WaitAll(task);

                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (Startup.EmailErrors)
                {
                    var emailId = context.HttpContext.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                    if (string.IsNullOrEmpty(emailId))
                    {
                        emailId = context.HttpContext.User?.FindFirst("name")?.Value;
                    }

                    string emailBody = $"Environment: {environmentName}" + Environment.NewLine +
                                       $"Time: {DateTimeOffset.UtcNow}" + Environment.NewLine +
                                       $"Email: {emailId}" + Environment.NewLine +
                                       $"Client: {context.HttpContext.Request.Headers["client"].FirstOrDefault()}" + Environment.NewLine +
                                       $"Error: {context.Exception.ToString()}";

                    this.emailService.SendErrorEmail(null, "TT.Core.Api", "notificationgroup@thingtrax.com", "ThingTrax Support", $"Error in environment: {environmentName}. Portal Api.", emailBody);
                }

                this.logger.Error("Error. {@ErrorDetails}{@Severity}{@UserID}", context.Exception.ToString(), "High", this.memoryCache.Get<string>(CacheKeyConstants.LoggedInUserEmail));
                this.telemetryClient.TrackException(context.Exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GlobalExceptionHandler. {ex.ToString()}");
                this.logger.Error("Error.{@ErrorDetails}{@Severity}", ex.ToString(), "High");
            }
            finally
            {
                HttpStatusCode status = HttpStatusCode.InternalServerError;
                if (context.Exception is InvalidOperationException)
                {
                    status = HttpStatusCode.Forbidden;
                }
                else if (context.Exception is NotImplementedException)
                {
                    status = HttpStatusCode.NotImplemented;
                }
                else if (context.Exception is ArgumentNullException)
                {
                    status = HttpStatusCode.BadRequest;
                }

                context.HttpContext.Response.StatusCode = (int)status;
                context.Result = new JsonResult(context.Exception.Message);
            }
        }
    }
}