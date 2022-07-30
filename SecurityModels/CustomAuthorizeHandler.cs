//-----------------------------------------------------------------------
// <copyright file="CustomAuthorizeHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>The custom authorize handler class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.SecurityModels
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using TT.Core.Repository.Sql;

    /// <summary>
    /// The factory Area Access requirement attribute model.
    /// </summary>
    public class CustomAuthorizeHandler : AuthorizationHandler<CustomAuthorizeRequirement>, IAuthorizationRequirement
    {
        /// <summary>
        /// The core context
        /// </summary>
        private readonly CoreContext coreContext;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorizeHandler" /> class.
        /// </summary>
        /// <param name="coreContext">The core context.</param>
        /// <param name="configuration">The configuration.</param>
        public CustomAuthorizeHandler(CoreContext coreContext, IConfiguration configuration)
        {
            this.coreContext = coreContext;
            this.configuration = configuration;
        }

        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <returns>The task.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizeRequirement requirement)
        {
            bool isAnonymousUser = Convert.ToBoolean(this.configuration.GetSection("anonymousUser").Value);
            if (isAnonymousUser)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            var mvcContext = context.Resource as AuthorizationFilterContext;
            var actionName = string.Empty;
            var controllerName = string.Empty;
            if (context.Resource is Endpoint endpoint)
            {
                var actionDescriptor = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
                var httpMethodMetadat = endpoint.Metadata.GetMetadata<HttpMethodMetadata>();
                if (actionDescriptor != null)
                {
                    controllerName = actionDescriptor.ControllerName;
                    actionName = httpMethodMetadat.HttpMethods[0];
                }
            }
            else if (mvcContext?.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                actionName = mvcContext.HttpContext.Request.Method;
                controllerName = descriptor.ControllerName;
            }

            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
            {
                var user = this.coreContext.Users
                           .Include(usr => usr.Role)
                           .ThenInclude(role => role.UserAreaDetails)
                           .ThenInclude(uad => uad.UserArea)
                           .FirstOrDefault(usr => usr.UserId == userId);

                if (user != null)
                {
                    if (controllerName == "MaintenanceJob"
                    || controllerName == "MaintenancePriority"
                    || controllerName == "MaintenanceReason"
                    || controllerName == "MaintenanceStatus"
                    || controllerName == "MaintenanceType"
                    || controllerName == "RecurrenceJobs"
                    || controllerName == "RecurrenceInstances"
                    || controllerName == "Part"
                    || controllerName == "PartType")
                    {
                        controllerName = "Maintenance";
                    }
                    else if (controllerName == "Contact" || controllerName == "Complaint" || controllerName == "EightdProcess")
                    {
                        controllerName = "QualityAssurance";
                    }
                    else if (controllerName == "Contact" || controllerName == "Complaint")
                    {
                        controllerName = "Job";
                    }
                    else if (controllerName == "MaterialAllocation"
                        || controllerName == "MaterialConsumption")
                    {
                        controllerName = "Assignment";
                    }
                    else if (controllerName == "EquipmentShift")
                    {
                        controllerName = "Equipment";
                    }
                    else if (controllerName == "LabourSkill" || controllerName == "Labour" || controllerName == "JobLabour" || controllerName == "LabourJobTitle")
                    {
                        controllerName = "Labour";
                    }

                    // else if (controllerName == "UnidentifiedOperator")
                    // {
                    //     controllerName = "UnidentifiedOperator";
                    // }
                    var userArea = user.Role.UserAreaDetails.FirstOrDefault(uad => uad.UserArea.Name == controllerName);

                    if (userArea != null)
                    {
                        if (actionName.ToLower() == "post" && userArea.Create)
                        {
                            context.Succeed(requirement);
                        }
                        else if (actionName.ToLower() == "put" && userArea.Edit)
                        {
                            context.Succeed(requirement);
                        }
                        else if (actionName.ToLower() == "delete" && userArea.Delete)
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return Task.FromException(new UnauthorizedAccessException());
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
