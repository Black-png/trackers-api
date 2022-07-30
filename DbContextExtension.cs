//-----------------------------------------------------------------------
// <copyright file="DbContextExtension.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>The startup class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;
    using TT.Core.Models.Constants;
    using TT.Core.Models.Enums;
    using TT.Core.Repository.Sql;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// The DbContext extension class.
    /// </summary>
    public static class DbContextExtension
    {
        /// <summary>
        /// Alls the migrations applied.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>True Or false.</returns>
        public static bool AllMigrationsApplied(this DbContext context)
        {
            if (context != null)
            {
                var applied = context.GetService<IHistoryRepository>()
                    .GetAppliedMigrations()
                    .Select(m => m.MigrationId);

                var total = context.GetService<IMigrationsAssembly>()
                    .Migrations
                    .Select(m => m.Key);

                return !total.Except(applied).Any();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Ensures the seeded.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void EnsureSeeded(this CoreContext context)
        {
            SeedDowntimeReasons(context);
            SeedPartType(context);
            SeedMaintenancePriorities(context);
            SeedMaintenanceStatus(context);
            SeedMaintenanceTypes(context);
            SeedMaintenanceReasons(context);
            SeedRejectionReason(context);

            SeedNotificationType(context);
            SeedNotifications(context);
            SeedPreferences(context);
            SeedNotificationSettings(context);
            SeedNotificationTemplates(context);

            SeedMaintenanceDowntimeReason(context);
            SeedEquipmentType(context);
            SeedUserRoles(context);
            SeedUserArea(context);
            SeedUserAreaRoles(context);
            SeedLabourJobTitle(context);
            SeedEquipmentSubType(context);
            SeedDepartment(context);
            SeedPackages(context);
            SeedOperations(context);
            SeedPackageOperations(context);
            SeedComplaintSource(context);
            SeedStepSource(context);
        }

        /// <summary>
        /// Seeds the package operations.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedPackageOperations(CoreContext context)
        {
            var packageOperations = context.PackageOperations.ToList();
            var packages = context.Packages.ToList();
            var operation = context.Operations.ToList();
            var packageOperationList = new List<PackageOperation>();
            IList<string[]> listOfString = new List<string[]>();
            string[] levelOne = { OperationName.OEE, OperationName.Job, OperationName.Auxiliary, OperationName.Operation, OperationName.MasterData };
            packageOperationList.AddRange(InsertPackageOperations(context, levelOne, packages, operation, packageOperations, PackageName.Starter.ToString()));
            string[] levelTwo = { OperationName.OEE, OperationName.Job, OperationName.Auxiliary, OperationName.Operation, OperationName.MasterData, OperationName.Report };
            packageOperationList.AddRange(InsertPackageOperations(context, levelTwo, packages, operation, packageOperations, PackageName.Base.ToString()));
            string[] levelThree = { OperationName.OEE, OperationName.Job, OperationName.Auxiliary, OperationName.Operation, OperationName.MasterData, OperationName.Report, OperationName.Energy };
            packageOperationList.AddRange(InsertPackageOperations(context, levelThree, packages, operation, packageOperations, PackageName.BaseEnergy.ToString()));
            string[] levelFour = { OperationName.OEE, OperationName.Job, OperationName.Auxiliary, OperationName.Operation, OperationName.MasterData, OperationName.Report, OperationName.Maintenance };
            packageOperationList.AddRange(InsertPackageOperations(context, levelFour, packages, operation, packageOperations, PackageName.BaseMaintenance.ToString()));
            string[] levelFive = { OperationName.OEE, OperationName.Job, OperationName.Auxiliary, OperationName.Operation, OperationName.MasterData, OperationName.Energy, OperationName.Report, OperationName.Maintenance };
            packageOperationList.AddRange(InsertPackageOperations(context, levelFive, packages, operation, packageOperations, PackageName.BaseEnergyMaintenance.ToString()));
            string[] levelSix = { OperationName.Complete };
            packageOperationList.AddRange(InsertPackageOperations(context, levelSix, packages, operation, packageOperations, PackageName.Complete.ToString()));
            string[] levelSeven = { OperationName.Complete };
            packageOperationList.AddRange(InsertPackageOperations(context, levelSeven, packages, operation, packageOperations, PackageName.CompleteVision.ToString()));
            string[] levelEight = { OperationName.Complete };
            packageOperationList.AddRange(InsertPackageOperations(context, levelEight, packages, operation, packageOperations, PackageName.CompleteVisionAssembly.ToString()));


            if (packageOperations.Count() <= 0)
            {
                context.PackageOperations.AddRange(packageOperationList);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Inserts the package operations.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="levelOne">The level one.</param>
        /// <param name="packages">The packages.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="packageOperation">The packageOperation.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>
        /// the list
        /// </returns>
        private static List<PackageOperation> InsertPackageOperations(CoreContext context, string[] levelOne, List<Package> packages, List<Operation> operations, List<PackageOperation> packageOperation, string packageName)
        {
            var packageOperationList = new List<PackageOperation>();
            var packageDB = packages.FirstOrDefault(p => p.PackageName == packageName);
            foreach (var item in levelOne)
            {
                var operationdb = operations.FirstOrDefault(op => op.OperationName == item);
                packageOperationList.Add(new PackageOperation { OperationId = operationdb.Id, PackageId = packageDB.Id });
                var data = packageOperation.Find(e => e.OperationId == operationdb.Id && e.PackageId == packageDB.Id);
                if (data == null)
                {
                    var packageOper = new PackageOperation { OperationId = operationdb.Id, PackageId = packageDB.Id };
                    context.PackageOperations.Add(packageOper);
                    context.SaveChanges();
                }
            }

            return packageOperationList;
        }

        /// <summary>
        /// Seeds the operations.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedOperations(CoreContext context)
        {
            var operation = context.Operations.ToList();
            if (operation.Count() <= 0)
            {
                var operationList = new List<Operation>
                {
                    new Operation { OperationName = OperationName.Complete },
                    new Operation { OperationName = OperationName.Maintenance },
                    new Operation { OperationName = OperationName.Energy },
                    new Operation { OperationName = OperationName.Report },
                    new Operation { OperationName = OperationName.MasterData },
                    new Operation { OperationName = OperationName.Operation },
                    new Operation { OperationName = OperationName.Job },
                    new Operation { OperationName = OperationName.Auxiliary },
                    new Operation { OperationName = OperationName.OEE }
                };

                context.Operations.AddRange(operationList);
                context.SaveChanges();
            }
            else
            {
                var data = operation.Find(x => x.OperationName == OperationName.Auxiliary);
                if (data == null)
                {
                    var auxiliary = new Operation { OperationName = OperationName.Auxiliary };
                    context.Operations.Add(auxiliary);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the complaint source.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedComplaintSource(CoreContext context)
        {
            var complaintSources = context.ComplaintSources.ToList();
            if (complaintSources.Count <= 0)
            {
                var complaintSourceList = new List<ComplaintSource>();
                var production = new ComplaintSource { ComplaintName = "Production", Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var job = new ComplaintSource { ComplaintName = "Job", Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var tool = new ComplaintSource { ComplaintName = "Tool", Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var product = new ComplaintSource { ComplaintName = "Product", Created = DateTimeOffset.UtcNow, IsDeleted = false };

                complaintSourceList.Add(production);
                complaintSourceList.Add(job);
                complaintSourceList.Add(tool);
                complaintSourceList.Add(product);

                context.ComplaintSources.AddRange(complaintSourceList);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seeds the packages.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedPackages(CoreContext context)
        {
            var packages = context.Packages.ToList();

            if (packages.Count() <= 0)
            {
                var packagesList = new List<Package>();
                var starter = new Package { PackageName = PackageName.Starter.ToString(), Level = (int)PackageName.Starter, Created = DateTimeOffset.UtcNow, IsActive = true, IsDeleted = false };
                var basePackage = new Package { PackageName = PackageName.Base.ToString(), Level = (int)PackageName.Base, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var baseEnergy = new Package { PackageName = PackageName.BaseEnergy.ToString(), Level = (int)PackageName.BaseEnergy, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var baseMaintenance = new Package { PackageName = PackageName.BaseMaintenance.ToString(), Level = (int)PackageName.BaseMaintenance, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var baseEnergyMaintenance = new Package { PackageName = PackageName.BaseEnergyMaintenance.ToString(), Level = (int)PackageName.BaseEnergyMaintenance, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var complete = new Package { PackageName = PackageName.Complete.ToString(), Level = (int)PackageName.Complete, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var completeVision = new Package { PackageName = PackageName.CompleteVision.ToString(), Level = (int)PackageName.CompleteVision, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                var completeVisionAssembly = new Package { PackageName = PackageName.CompleteVisionAssembly.ToString(), Level = (int)PackageName.CompleteVisionAssembly, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                packagesList.Add(completeVisionAssembly);
                packagesList.Add(completeVision);
                packagesList.Add(complete);
                packagesList.Add(baseEnergyMaintenance);
                packagesList.Add(baseMaintenance);
                packagesList.Add(baseEnergy);
                packagesList.Add(basePackage);
                packagesList.Add(starter);

                context.Packages.AddRange(packagesList);
                context.SaveChanges();
            }
            else
            {
                var data = packages.Find(x => x.PackageName == PackageName.CompleteVision.ToString());
                if (data == null)
                {
                    var completeVis = new Package { PackageName = PackageName.CompleteVision.ToString(), Level = (int)PackageName.CompleteVision, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                    context.Packages.Add(completeVis);
                    context.SaveChanges();
                }

                var dataEx = packages.Find(x => x.PackageName == PackageName.CompleteVisionAssembly.ToString());
                if (dataEx == null)
                {
                    var completeVisionAssem = new Package { PackageName = PackageName.CompleteVisionAssembly.ToString(), Level = (int)PackageName.CompleteVisionAssembly, Created = DateTimeOffset.UtcNow, IsDeleted = false };
                    context.Packages.Add(completeVisionAssem);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the department.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedDepartment(CoreContext context)
        {
            var factories = context.Factories.ToList();

            foreach (var factory in factories)
            {
                var dbdepartments = context.Departments.Where(c => c.FactoryId == factory.Id).ToList();
                List<Department> departments = new List<Department>();
                Department unidentifiedDepartment = new Department { FactoryId = factory.Id, Name = "UnIdentified", Description = "Unidentified operator's department", Created = DateTimeOffset.UtcNow, IsDeleted = false };
                departments.Add(unidentifiedDepartment);
                if (dbdepartments.Count() <= 0)
                {
                    context.Departments.AddRange(departments);
                    context.SaveChanges();
                }
                else
                {
                    var names = dbdepartments.Select(c => c.Name).ToList();
                    var departmentsnotexists = departments.Where(c => !names.Contains(c.Name)).ToList();
                    if (departmentsnotexists.Count() > 0)
                    {
                        context.Departments.AddRange(departmentsnotexists);
                        context.SaveChanges();
                    }
                }

                departments.Clear();
            }
        }

        /// <summary>
        /// Seeds the labour job title.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedLabourJobTitle(CoreContext context)
        {
            var dblabourjobtitle = context.LabourJobTitle.ToList();
            List<LabourJobTitle> labourjobtitle = new List<LabourJobTitle>();

            LabourJobTitle op = new LabourJobTitle { Name = LabourTitles.Operator.ToString(), Colour = "#5cb85c", Created = DateTimeOffset.UtcNow };
            LabourJobTitle boxMan = new LabourJobTitle { Name = LabourTitles.BoxMan.ToString(), Colour = "#d9534f", Created = DateTimeOffset.UtcNow };
            LabourJobTitle qa = new LabourJobTitle { Name = LabourTitles.QA.ToString(), Colour = "#f9f9f9", Created = DateTimeOffset.UtcNow };
            LabourJobTitle toolChanger = new LabourJobTitle { Name = LabourTitles.ToolChanger.ToString(), Colour = "#0275d8", Created = DateTimeOffset.UtcNow };
            LabourJobTitle shiftLeader = new LabourJobTitle { Name = LabourTitles.ShiftLeader.ToString(), Colour = "#727272", Created = DateTimeOffset.UtcNow };

            labourjobtitle.Add(op);
            labourjobtitle.Add(boxMan);
            labourjobtitle.Add(qa);
            labourjobtitle.Add(toolChanger);
            labourjobtitle.Add(shiftLeader);
            if (dblabourjobtitle.Count <= 0)
            {
                context.LabourJobTitle.AddRange(labourjobtitle);
                context.SaveChanges();
            }
            else
            {
                var name = dblabourjobtitle.Select(c => c.Name).ToList();
                var labourjobtitlenotexists = labourjobtitle.Where(c => !name.Contains(c.Name)).ToList();
                if (labourjobtitlenotexists.Count() > 0)
                {
                    context.LabourJobTitle.AddRange(labourjobtitlenotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the user roles.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedUserRoles(CoreContext context)
        {
            var configuration = context.FactoriesConfigurations.FirstOrDefault();
            var dbrole = context.Roles.ToList();
            List<Role> roles = new List<Role>();
            Role viewer = new Role { Name = Roles.Viewer.ToString(), Created = DateTimeOffset.UtcNow };
            roles.Add(viewer);
            Role supervisor = new Role { Name = Roles.Supervisor.ToString(), Created = DateTimeOffset.UtcNow };
            roles.Add(supervisor);
            Role admin = new Role { Name = Roles.Admin.ToString(), Created = DateTimeOffset.UtcNow };
            roles.Add(admin);
            Role setter = new Role { Name = Roles.Setter.ToString(), Created = DateTimeOffset.UtcNow };
            roles.Add(setter);
            if (configuration != null)
            {
               Role globalview = new Role { Name = Roles.GlobalViewer.ToString(), Created = DateTimeOffset.UtcNow };
               roles.Add(globalview);
            }

            if (dbrole.Count <= 0)
            {
                foreach (var item in roles)
                {
                    context.Roles.Add(item);
                    context.SaveChanges();
                }
            }
            else
            {
                var name = dbrole.Select(c => c.Name).ToList();
                var rolesnotexists = roles.Where(c => !name.Contains(c.Name)).ToList();
                ////if (configuration != null && !configuration.IsParent)
                ////{
                ////    var globalRole = rolesnotexists.FirstOrDefault(rne => rne.Name.ToLower() == Roles.GlobalViewer.ToString().ToLower());
                ////    if (globalRole != null)
                ////    {
                ////        rolesnotexists.Remove(globalRole);
                ////    }
                ////}

                if (rolesnotexists.Count() > 0)
                {
                    foreach (var item in rolesnotexists)
                    {
                        context.Roles.Add(item);
                        context.SaveChanges();
                    }

                }
            }
        }

        /// <summary>
        /// Seeds the user area.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedUserArea(CoreContext context)
        {
            var dbuserarea = context.UserAreas.ToList();
            List<UserArea> userareas = new List<UserArea>();
            UserArea factory = new UserArea { Name = "Factory", Created = DateTimeOffset.UtcNow };
            UserArea groups = new UserArea { Name = "Group", Created = DateTimeOffset.UtcNow };
            UserArea departments = new UserArea { Name = "Department", Created = DateTimeOffset.UtcNow };
            UserArea rawMaterial = new UserArea { Name = "RawMaterial", Created = DateTimeOffset.UtcNow };
            UserArea shift = new UserArea { Name = "Shift", Created = DateTimeOffset.UtcNow };
            UserArea jobStage = new UserArea { Name = "Stage", Created = DateTimeOffset.UtcNow };
            UserArea jobType = new UserArea { Name = "JobType", Created = DateTimeOffset.UtcNow };
            UserArea equipment = new UserArea { Name = "Equipment", Created = DateTimeOffset.UtcNow };
            UserArea tools = new UserArea { Name = "Tool", Created = DateTimeOffset.UtcNow };
            UserArea product = new UserArea { Name = "Product", Created = DateTimeOffset.UtcNow };
            UserArea operators = new UserArea { Name = "Labour", Created = DateTimeOffset.UtcNow };
            UserArea downtime = new UserArea { Name = "Downtime", Created = DateTimeOffset.UtcNow };
            UserArea production = new UserArea { Name = "Production", Created = DateTimeOffset.UtcNow };
            UserArea rejection = new UserArea { Name = "Rejection", Created = DateTimeOffset.UtcNow };
            UserArea factoryArea = new UserArea { Name = "FactoryArea", Created = DateTimeOffset.UtcNow };
            UserArea job = new UserArea { Name = "Job", Created = DateTimeOffset.UtcNow };
            UserArea maintenance = new UserArea { Name = "Maintenance", Created = DateTimeOffset.UtcNow };
            UserArea liveLabour = new UserArea { Name = "RegionOfInterest", Created = DateTimeOffset.UtcNow };
            UserArea assignment = new UserArea { Name = "Assignment", Created = DateTimeOffset.UtcNow };
            UserArea downtimeReason = new UserArea { Name = "DownTimeReason", Created = DateTimeOffset.UtcNow };
            UserArea rejectionReason = new UserArea { Name = "RejectionReason", Created = DateTimeOffset.UtcNow };
            UserArea unidentifiedOperator = new UserArea { Name = "UnidentifiedOperator", Created = DateTimeOffset.UtcNow };
            UserArea globaldashboard = new UserArea { Name = "GlobalDashboard", Created = DateTimeOffset.UtcNow };
            UserArea qualityAssurance = new UserArea { Name = "QualityAssurance", Created = DateTimeOffset.UtcNow };
            userareas.Add(factory);
            userareas.Add(groups);
            userareas.Add(departments);
            userareas.Add(rawMaterial);
            userareas.Add(shift);
            userareas.Add(jobStage);
            userareas.Add(jobType);
            userareas.Add(equipment);
            userareas.Add(tools);
            userareas.Add(product);
            userareas.Add(operators);
            userareas.Add(downtime);
            userareas.Add(production);
            userareas.Add(rejection);
            userareas.Add(factoryArea);
            userareas.Add(job);
            userareas.Add(maintenance);
            userareas.Add(liveLabour);
            userareas.Add(assignment);
            userareas.Add(downtimeReason);
            userareas.Add(rejectionReason);
            userareas.Add(unidentifiedOperator);
            userareas.Add(globaldashboard);
            userareas.Add(qualityAssurance);

            if (dbuserarea.Count <= 0)
            {
                context.UserAreas.AddRange(userareas);
                context.SaveChanges();
            }
            else
            {
                var name = dbuserarea.Select(c => c.Name).ToList();
                var userareanotexists = userareas.Where(c => !name.Contains(c.Name)).ToList();
                if (userareanotexists.Count() > 0)
                {
                    context.UserAreas.AddRange(userareanotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the user area roles.
        /// </summary>
        /// <param name="coreContext">The core context.</param>
        private static void SeedUserAreaRoles(CoreContext coreContext)
        {
            var configuration = coreContext.FactoriesConfigurations.FirstOrDefault();
            var userAreaRoleList = coreContext.UserAreaRoles.ToList();
            var factoryAreaList = coreContext.UserAreas.ToList();
            var roles = coreContext.Roles.ToList();
            if (userAreaRoleList.Count <= 0)
            {
                foreach (var area in factoryAreaList)
                {
                    foreach (var role in roles)
                    {
                        ////if (configuration != null && !configuration.IsParent && string.Equals(role.Name, Roles.GlobalViewer.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        ////{
                        ////    continue;
                        ////}

                        var userAreaRole = GetNewRole(role, area);
                        coreContext.UserAreaRoles.Add(userAreaRole);
                    }
                }

                coreContext.SaveChanges();
            }
            else
            {
                foreach (var area in factoryAreaList)
                {
                    foreach (var role in roles)
                    {
                        var isUserAreaRoleExist = userAreaRoleList.Any(uar => uar.RoleId == role.Id && uar.UserAreaId == area.Id);
                        if (!isUserAreaRoleExist)
                        {
                            var userAreaRole = GetNewRole(role, area);
                            coreContext.UserAreaRoles.Add(userAreaRole);
                        }
                    }
                }

                coreContext.SaveChanges();
            }

            var downtimeReason = coreContext.UserAreas.FirstOrDefault(ua => ua.Name == "DownTimeReason");
            if (downtimeReason != null)
            {
                var userarearole = coreContext.UserAreaRoles.Where(uar => uar.UserAreaId == downtimeReason.Id).ToList();
                if (userarearole.Count <= 0)
                {
                    foreach (var role in roles)
                    {
                        var userAreaRole = GetNewRole(role, downtimeReason);
                        coreContext.UserAreaRoles.Add(userAreaRole);
                        coreContext.SaveChanges();
                    }
                }
            }

            var rejectionReason = coreContext.UserAreas.FirstOrDefault(ua => ua.Name == "RejectionReason");
            if (rejectionReason != null)
            {
                var userarearole = coreContext.UserAreaRoles.Where(uar => uar.UserAreaId == rejectionReason.Id).ToList();
                if (userarearole.Count <= 0)
                {
                    foreach (var role in roles)
                    {
                        var userAreaRole = GetNewRole(role, rejectionReason);
                        coreContext.UserAreaRoles.Add(userAreaRole);
                        coreContext.SaveChanges();
                    }
                }
            }

            var unidentifiedOperator = coreContext.UserAreas.FirstOrDefault(ua => ua.Name == "UnidentifiedOperator");
            if (unidentifiedOperator != null)
            {
                var userarearole = coreContext.UserAreaRoles.Where(uar => uar.UserAreaId == unidentifiedOperator.Id).ToList();
                if (userarearole.Count <= 0)
                {
                    foreach (var role in roles)
                    {
                        var userAreaRole = GetNewRole(role, unidentifiedOperator);
                        coreContext.UserAreaRoles.Add(userAreaRole);
                        coreContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the new role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="area">The area.</param>
        /// <returns>UserAreaRole</returns>
        private static UserAreaRole GetNewRole(Role role, UserArea area)
        {
            UserAreaRole userAreaRole = new UserAreaRole { RoleId = role.Id, UserAreaId = area.Id, Created = DateTimeOffset.UtcNow, View = true };
            if (string.Equals(role.Name, Roles.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) && !string.Equals("GlobalDashboard", area.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                userAreaRole.Create = true;
                userAreaRole.Edit = true;
                userAreaRole.Delete = true;
            }

            if (!string.Equals(role.Name, Roles.GlobalViewer.ToString(), StringComparison.InvariantCultureIgnoreCase) && string.Equals("GlobalDashboard", area.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                userAreaRole.View = false;
            }

            return userAreaRole;
        }

        /// <summary>
        /// Seeds the rejection reason.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedRejectionReason(CoreContext context)
        {
            var dbrejectionreason = context.RejectionReasonsResponseQueryModels.FromSqlRaw("get_RejectionReasonsResults").ToList();
            List<RejectionReason> rejectionreasons = new List<RejectionReason>();
            if (dbrejectionreason.Where(c => c.Code == "SC").FirstOrDefault() == null)
            {
                RejectionReason settingScrap = new RejectionReason
                {
                    Name = "Setting Scrap",
                    Code = "SC",
                    Description = "Setting Scrap",
                    IsDefault = true,
                    Created = DateTimeOffset.UtcNow,
                };
                rejectionreasons.Add(settingScrap);
            }

            if (context.RejectionReasons.ToList().Count <= 0)
            {
                context.RejectionReasons.AddRange(rejectionreasons);
                context.SaveChanges();
            }
            else
            {
                var code = dbrejectionreason.Select(c => c.Code).ToList();
                var rejectionreasonnotexists = rejectionreasons.Where(c => !code.Contains(c.Code)).ToList();
                if (rejectionreasonnotexists.Count() > 0)
                {
                    context.RejectionReasons.AddRange(rejectionreasonnotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the downtime reasons.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedDowntimeReasons(CoreContext context)
        {
            // Note: If default downtime needs to be updated when downtime reason updated make IsUpdatable = True
            var dbdowntimeReasons = context.DowntimeReasons.ToList();
            List<DowntimeReason> downtimereasons = new List<DowntimeReason>();

            DowntimeReason unclassified = new DowntimeReason
            {
                Name = "UNCLASSIFIED",
                Code = "UC",
                Description = "Un-classified",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FF0000",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(unclassified);

            DowntimeReason plannedNonRun = new DowntimeReason
            {
                Name = "PLANNED NON-RUN",
                Code = "PNR",
                Description = "Planned non-run",
                IsPlanned = true,
                IsDefault = false,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(plannedNonRun);

            DowntimeReason deviceNotInstalled = new DowntimeReason
            {
                Name = "DEVICE NOT INSTALLED",
                Code = "DNI",
                Description = "Device not installed",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(deviceNotInstalled);

            DowntimeReason noCurrentShift = new DowntimeReason
            {
                Name = "NO CURRENT SHIFT",
                Code = "NCS",
                Description = "No current shift",
                IsPlanned = true,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(noCurrentShift);

            DowntimeReason deviceNotConnected = new DowntimeReason
            {
                Name = "Powered Off",
                Code = "DNC",
                Description = "Device not connected",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(deviceNotConnected);

            DowntimeReason noOperator = new DowntimeReason
            {
                Name = "NO OPERATOR",
                Code = "NO",
                Description = "No Operator",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(noOperator);

            DowntimeReason settingTime = new DowntimeReason
            {
                Name = "Setting Time",
                Code = "ST",
                Description = "Setting Time",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(settingTime);

            DowntimeReason woMismatch = new DowntimeReason
            {
                Name = "Workorder mismatch",
                Code = "WO",
                Description = "Running and telemetry job work orders do not match",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(woMismatch);

            DowntimeReason njf = new DowntimeReason
            {
                Name = "No Job Found",
                Code = "NJF",
                Description = "No Job Found",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(njf);

            DowntimeReason js = new DowntimeReason
            {
                Name = "Job Stopped",
                Code = "JS",
                Description = "Job Stopped",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FFA500",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(js);

            DowntimeReason ms = new DowntimeReason
            {
                Name = "Micro stop",
                Code = "MS",
                Description = "Micro stop",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#FF0000",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(ms);

            if (dbdowntimeReasons.Count <= 0)
            {
                context.DowntimeReasons.AddRange(downtimereasons);
                context.SaveChanges();
            }
            else
            {
                var reasoncode = dbdowntimeReasons.Select(c => c.Code).ToList();
                var downtimereason = downtimereasons.Where(c => !reasoncode.Contains(c.Code)).ToList();
                if (downtimereason.Count() > 0)
                {
                    context.DowntimeReasons.AddRange(downtimereason);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the type of the equipment.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedEquipmentType(CoreContext context)
        {
            var dbequipmenttype = context.EquipmentTypes.ToList();
            List<EquipmentType> equipmenttypes = new List<EquipmentType>();
            EquipmentType injectionMoulding = new EquipmentType
            {
                Name = "Injection Moulding",
                CalculateDowntime = true,
                CalculateOee = true,
                CalculateEnergy = true,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(injectionMoulding);

            EquipmentType conversion = new EquipmentType
            {
                Name = "Conversion",
                CalculateDowntime = true,
                CalculateOee = true,
                CalculateEnergy = true,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(conversion);

            EquipmentType silos = new EquipmentType
            {
                Name = "Silos",
                CalculateDowntime = false,
                CalculateOee = false,
                CalculateEnergy = false,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(silos);

            EquipmentType auxiliary = new EquipmentType
            {
                Name = "Auxiliary",
                CalculateDowntime = true,
                CalculateOee = false,
                CalculateEnergy = false,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(auxiliary);

            EquipmentType extrusion = new EquipmentType
            {
                Name = "Extrusion",
                CalculateDowntime = true,
                CalculateOee = true,
                CalculateEnergy = true,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(extrusion);

            EquipmentType camera = new EquipmentType
            {
                Name = "Camera",
                CalculateDowntime = false,
                CalculateOee = false,
                CalculateEnergy = false,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(camera);

            EquipmentType assemblyline = new EquipmentType
            {
                Name = "Assembly Line",
                CalculateDowntime = true,
                CalculateOee = true,
                CalculateEnergy = false,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(assemblyline);

            EquipmentType packing = new EquipmentType
            {
                Name = "Packing",
                CalculateDowntime = true,
                CalculateOee = true,
                CalculateEnergy = true,
                Created = DateTimeOffset.UtcNow
            };
            equipmenttypes.Add(packing);

            if (dbequipmenttype.Count() <= 0)
            {
                foreach (var item in equipmenttypes)
                {
                    context.EquipmentTypes.Add(item);
                    context.SaveChanges();
                }
            }
            else
            {
                var name = dbequipmenttype.Select(c => c.Name).ToList();
                var equipmenttypesnotexists = equipmenttypes.Where(c => !name.Contains(c.Name)).ToList();
                if (equipmenttypesnotexists.Count() > 0)
                {
                    foreach (var item in equipmenttypesnotexists)
                    {
                        context.EquipmentTypes.Add(item);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedMaintenanceDowntimeReason(CoreContext context)
        {
            var dbdowntimereason = context.DowntimeReasons.ToList();
            List<DowntimeReason> downtimereasons = new List<DowntimeReason>();
            DowntimeReason maintenance = new DowntimeReason
            {
                Name = "MAINTENANCE",
                Code = "EM",
                Description = "Equipment Maintenance",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#338DFF",
                Created = DateTimeOffset.UtcNow,
            };

            downtimereasons.Add(maintenance);

            DowntimeReason toolChange = new DowntimeReason
            {
                Name = "TOOL CHANGE",
                Code = "TC",
                Description = "Tool Change",
                IsPlanned = false,
                IsDefault = true,
                AutoClose = true,
                Colour = "#00AAFF",
                Created = DateTimeOffset.UtcNow,
            };
            downtimereasons.Add(toolChange);

            if (dbdowntimereason.Count() <= 0)
            {
                context.DowntimeReasons.AddRange(downtimereasons);
                context.SaveChanges();
            }
            else
            {
                var code = dbdowntimereason.Select(c => c.Code).ToList();
                var downtimereasonsnotexists = downtimereasons.Where(c => !code.Contains(c.Code)).ToList();
                if (downtimereasonsnotexists.Count() > 0)
                {
                    context.DowntimeReasons.AddRange(downtimereasonsnotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the type of the part.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedPartType(CoreContext context)
        {
            var dbparttypes = context.PartTypes.ToList();
            List<PartType> partTypes = new List<PartType>();
            PartType partType = new PartType
            {
                Name = "General",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            partTypes.Add(partType);
            if (dbparttypes.Count() <= 0)
            {
                context.PartTypes.AddRange(partTypes);
                context.SaveChanges();
            }
            else
            {
                var partnames = dbparttypes.Select(c => c.Name).ToList();
                var parttypenotexists = partTypes.Where(c => !partnames.Contains(c.Name)).ToList();
                if (parttypenotexists.Count() > 0)
                {
                    context.PartTypes.AddRange(parttypenotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the maintenance priorities.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedMaintenancePriorities(CoreContext context)
        {
            var dbmaintenancepriority = context.MaintenancePriorities.ToList();
            List<MaintenancePriority> priorities = new List<MaintenancePriority>();
            MaintenancePriority high = new MaintenancePriority
            {
                Name = "High",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            priorities.Add(high);

            MaintenancePriority medium = new MaintenancePriority
            {
                Name = "Medium",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            priorities.Add(medium);

            MaintenancePriority low = new MaintenancePriority
            {
                Name = "Low",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            priorities.Add(low);
            if (dbmaintenancepriority.Count() <= 0)
            {
                context.MaintenancePriorities.AddRange(priorities);
                context.SaveChanges();
            }
            else
            {
                var names = dbmaintenancepriority.Select(c => c.Name).ToList();
                var prioritiesnotexits = priorities.Where(c => !names.Contains(c.Name)).ToList();
                if (prioritiesnotexits.Count() > 0)
                {
                    context.MaintenancePriorities.AddRange(prioritiesnotexits);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the maintenance status.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedMaintenanceStatus(CoreContext context)
        {
            var dbmaintenancestatues = context.MaintenanceStatuses.ToList();
            List<MaintenanceStatus> maintenancestatues = new List<MaintenanceStatus>();
            MaintenanceStatus inProgress = new MaintenanceStatus
            { Name = "In Progress", ColourCode = "#1CAC78", Created = DateTimeOffset.UtcNow, IsDeleted = false };

            maintenancestatues.Add(inProgress);

            MaintenanceStatus overDue = new MaintenanceStatus
            { Name = "Over Due", ColourCode = "#FF2B2B", Created = DateTimeOffset.UtcNow, IsDeleted = false };
            maintenancestatues.Add(overDue);

            MaintenanceStatus notStarted = new MaintenanceStatus
            { Name = "Not Started", ColourCode = "#FF0000", Created = DateTimeOffset.UtcNow, IsDeleted = false };
            maintenancestatues.Add(notStarted);

            MaintenanceStatus schedule = new MaintenanceStatus
            { Name = "Scheduled", ColourCode = "#FFA500", Created = DateTimeOffset.UtcNow, IsDeleted = false };
            maintenancestatues.Add(schedule);

            MaintenanceStatus complete = new MaintenanceStatus
            { Name = "Completed", ColourCode = "#E67E22", Created = DateTimeOffset.UtcNow, IsDeleted = false };
            maintenancestatues.Add(complete);
            if (dbmaintenancestatues.Count() <= 0)
            {
                context.MaintenanceStatuses.AddRange(maintenancestatues);
                context.SaveChanges();
            }
            else
            {
                var names = dbmaintenancestatues.Select(c => c.Name).ToList();
                var maintaincestatusnotexits = maintenancestatues.Where(c => !names.Contains(c.Name)).ToList();
                if (maintaincestatusnotexits.Count() > 0)
                {
                    context.MaintenanceStatuses.AddRange(maintaincestatusnotexits);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the maintenance types.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedMaintenanceTypes(CoreContext context)
        {
            var dbmaintancetypes = context.MaintenanceTypes.ToList();
            List<MaintenanceType> maintancetypes = new List<MaintenanceType>();
            MaintenanceType vInspection = new MaintenanceType
            {
                Name = "Visual Inspection",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintancetypes.Add(vInspection);

            MaintenanceType minorMaintain = new MaintenanceType
            {
                Name = "Minor Maintenance",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintancetypes.Add(minorMaintain);

            MaintenanceType majorMaintenance = new MaintenanceType
            {
                Name = "Major Maintenance",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintancetypes.Add(majorMaintenance);

            MaintenanceType repair = new MaintenanceType
            {
                Name = "Repair",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintancetypes.Add(repair);

            MaintenanceType replacement = new MaintenanceType
            {
                Name = "Replacement",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintancetypes.Add(replacement);

            MaintenanceType service = new MaintenanceType
            {
                Name = "Service",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintancetypes.Add(service);

            if (context.MaintenanceTypes.Count() <= 0)
            {
                context.MaintenanceTypes.AddRange(maintancetypes);
                context.SaveChanges();
            }
            else
            {
                var names = dbmaintancetypes.Select(c => c.Name).ToList();
                var maintancetypesnotexits = maintancetypes.Where(c => !names.Contains(c.Name)).ToList();
                if (maintancetypesnotexits.Count() > 0)
                {
                    context.MaintenanceTypes.AddRange(maintancetypesnotexits);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the maintenance reasons.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedMaintenanceReasons(CoreContext context)
        {
            var dbmaintenancereason = context.MaintenanceReasons.ToList();
            List<MaintenanceReason> maintenancereasons = new List<MaintenanceReason>();
            MaintenanceReason abnormalNoise = new MaintenanceReason
            {
                Name = "Abnormal noise",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(abnormalNoise);

            MaintenanceReason abnormalWear = new MaintenanceReason
            {
                Name = "Abnormal wear",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(abnormalWear);

            MaintenanceReason contaminated = new MaintenanceReason
            {
                Name = "Contaminated",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(contaminated);

            MaintenanceReason controlMalfunction = new MaintenanceReason
            {
                Name = "Control malfunction",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(controlMalfunction);

            MaintenanceReason corroded = new MaintenanceReason
            {
                Name = "Corroded",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(corroded);

            MaintenanceReason damaged = new MaintenanceReason
            {
                Name = "Damaged",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(damaged);

            MaintenanceReason excessivewear = new MaintenanceReason
            {
                Name = "Excessive wear",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(excessivewear);

            MaintenanceReason failstest = new MaintenanceReason
            {
                Name = "Fails test/check",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(failstest);

            MaintenanceReason fittedincorrectly = new MaintenanceReason
            {
                Name = "Fitted incorrectly",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(fittedincorrectly);

            MaintenanceReason inoutputincorrect = new MaintenanceReason
            {
                Name = "In/Output incorrect",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(inoutputincorrect);

            MaintenanceReason incompletefill = new MaintenanceReason
            {
                Name = "Incomplete fill",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(incompletefill);

            MaintenanceReason leaking = new MaintenanceReason
            {
                Name = "Leaking",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(leaking);

            MaintenanceReason levelincorrect = new MaintenanceReason
            {
                Name = "Level incorrect",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(levelincorrect);

            MaintenanceReason loose = new MaintenanceReason
            {
                Name = "Loose",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(loose);

            MaintenanceReason misaligned = new MaintenanceReason
            {
                Name = "Misaligned",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(misaligned);

            MaintenanceReason nofault = new MaintenanceReason
            {
                Name = "No fault found",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(nofault);

            MaintenanceReason service = new MaintenanceReason
            {
                Name = "Service",
                Created = DateTimeOffset.UtcNow,
                IsDeleted = false,
            };
            maintenancereasons.Add(service);
            if (dbmaintenancereason.Count() <= 0)
            {
                context.MaintenanceReasons.AddRange(maintenancereasons);
                context.SaveChanges();
            }
            else
            {
                var names = dbmaintenancereason.Select(c => c.Name).ToList();
                var maintenancereasonsnotexists = maintenancereasons.Where(c => !names.Contains(c.Name)).ToList();
                if (maintenancereasonsnotexists.Count() > 0)
                {
                    context.MaintenanceReasons.AddRange(maintenancereasonsnotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the type of the equipment sub.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedEquipmentSubType(CoreContext context)
        {
            var dbequipmentsubtype = context.EquipmentSubTypes.ToList();
            List<EquipmentSubType> equipmentsubtypes = new List<EquipmentSubType>();
            EquipmentSubType greenlightHoughton = new EquipmentSubType
            {
                Name = "Greenlight Houghton",
                EquipmentTypeId = EquipmentTypes.Auxiliary,
                Created = DateTimeOffset.UtcNow,
            };
            equipmentsubtypes.Add(greenlightHoughton);
            if (dbequipmentsubtype.Count() <= 0)
            {
                context.EquipmentSubTypes.AddRange(equipmentsubtypes);
                context.SaveChanges();
            }
            else
            {
                var name = dbequipmentsubtype.Select(c => c.Name).ToList();
                var equipmentsubtypenotexists = equipmentsubtypes.Where(c => !name.Contains(c.Name)).ToList();
                if (equipmentsubtypenotexists.Count() > 0)
                {
                    context.EquipmentSubTypes.AddRange(equipmentsubtypenotexists);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the type of the notification.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedNotificationType(CoreContext context)
        {
            foreach (var type in Enum.GetNames(typeof(NotificationTypes)))
            {
                if (!context.NotificationTypes.Any(n => n.Type == type))
                {
                    NotificationType types = new NotificationType
                    {
                        Type = type,
                    };

                    context.NotificationTypes.Add(types);
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the notifications.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedNotifications(CoreContext context)
        {
            foreach (var notifcationId in Enum.GetValues(typeof(Notifications)))
            {
                var notifcationName = Enum.GetName(typeof(Notifications), notifcationId);
                if (!context.Notifications.Any(n => n.Name == notifcationName))
                {
                    Notification notification = new Notification
                    {
                        Name = notifcationName,
                    };
                    context.Notifications.Add(notification);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Seeds the notification settings.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedNotificationSettings(CoreContext context)
        {
            var users = context.Users.ToList();
            foreach (var user in users)
            {
                foreach (Notifications notifcationId in Enum.GetValues(typeof(Notifications)))
                {
                    if (!context.NotificationSettings.Any(ns => ns.UserId == user.Id && ns.NotificationId == Convert.ToInt32(notifcationId)))
                    {
                        NotificationSetting notificationSetting = new NotificationSetting
                        {
                            UserId = user.Id,
                            NotificationId = Convert.ToInt32(notifcationId),
                            IsSubscribed = false,
                        };
                        context.NotificationSettings.Add(notificationSetting);
                    }
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the prefrences.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedPreferences(CoreContext context)
        {
            var users = context.Users.ToList();
            foreach (var user in users)
            {
                foreach (Notifications notifcationId in Enum.GetValues(typeof(Notifications)))
                {
                    foreach (NotificationTypes type in Enum.GetValues(typeof(NotificationTypes)))
                    {
                        if (!context.Preferences.Any(p => p.UserId == user.Id && p.NotificationId == Convert.ToInt32(notifcationId) && p.NotificationTypeId == Convert.ToInt32(type)))
                        {
                            Preference preferences = new Preference
                            {
                                UserId = user.Id,
                                NotificationId = Convert.ToInt32(notifcationId),
                                NotificationTypeId = Convert.ToInt32(type),
                                IsSubscribed = false,
                            };
                            context.Preferences.Add(preferences);
                        }
                    }
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the prefrences.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedNotificationTemplates(CoreContext context)
        {
            foreach (Notifications notifcationId in Enum.GetValues(typeof(Notifications)))
            {
                foreach (NotificationTypes type in Enum.GetValues(typeof(NotificationTypes)))
                {
                    var template = Gettemplate(Convert.ToInt32(notifcationId), Convert.ToInt32(type));
                    if (!string.IsNullOrEmpty(template))
                    {
                        if (!context.NotificationTemplates.Any(p => p.NotificationId == Convert.ToInt32(notifcationId) && p.NotificationTypeId == Convert.ToInt32(type)))
                        {
                            NotificationTemplate notificationTemplate = new NotificationTemplate
                            {
                                NotificationId = Convert.ToInt32(notifcationId),
                                NotificationTypeId = Convert.ToInt32(type),
                                Template = template,
                            };
                            context.NotificationTemplates.Add(notificationTemplate);
                        }
                    }
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Seeds the Step source.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void SeedStepSource(CoreContext context)
        {
            List<Step> customsteps = new List<Step>();
            customsteps.Add(new Step { StepName = "Independent 24V DC Power Supply.", Created = DateTimeOffset.UtcNow, IsDeleted = false });
            customsteps.Add(new Step { StepName = "Run Signal Volt Free Connected through the relay and validated using multimeter wires are shielded.", Created = DateTimeOffset.UtcNow, IsDeleted = false });
            customsteps.Add(new Step { StepName = "Antenna Installed Outside.", Created = DateTimeOffset.UtcNow, IsDeleted = false });
            customsteps.Add(new Step { StepName = "Confirm Device ID.", Created = DateTimeOffset.UtcNow, IsDeleted = false });
            customsteps.Add(new Step { StepName = "Installer Name.", Created = DateTimeOffset.UtcNow, IsDeleted = false });
            var equipmentTypes = context.EquipmentTypes.ToList();
            List<Step> steps = new List<Step>();

            foreach (var type in equipmentTypes)
            {
                foreach (var newsteps in customsteps)
                {
                    var checkstep = context.Steps.Where(c => c.EquipmentTypeId == type.Id && c.StepName.ToLower() == newsteps.StepName.ToLower()).FirstOrDefault();
                    if (checkstep == null)
                    {
                        steps.Add(new Step { StepName = newsteps.StepName, Created = newsteps.Created, Updated = newsteps.Updated, IsDeleted = newsteps.IsDeleted, EquipmentTypeId = type.Id });
                    }
                }
            }

            if (steps.Count > 0)
            {
                context.Steps.AddRange(steps);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gettemplates the specified notification identifier.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <returns>Template</returns>
        private static string Gettemplate(long notificationId, long notificationTypeId)
        {
            var template = string.Empty;
            if (notificationId == Convert.ToInt32(Notifications.FactoryShiftReport) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Factory shift report for Shift : {shiftName} for the Date : {date} is ready.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.FactoryShiftReport) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Factory shift report for factory: <b>{factoryName}</b> for Shift : <b>{shiftName}</b> for the Date : <b>{date}</b> is attached. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.KPIReport) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "KPI report for Shift : {shiftName} for the Date : {date} is ready.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.KPIReport) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "KPI report for factory: <b>{factoryName}</b> for Shift : <b>{shiftName}</b> for the Date : <b>{date}</b> is attached. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DowntimeUnclassifiedMoreThan10Minutes) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Unclassified downtime is more than 10 minutes for the Equipment : {EquipmentId}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DowntimeUnclassifiedMoreThan10Minutes) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Unclassified downtime is more than 10 minutes for the Equipment : <b>{EquipmentId}</b>. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DowntimeUnclassifiedMoreThan20Minutes) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Unclassified downtime is more than 20 minutes for the Equipment : {EquipmentId}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DowntimeUnclassifiedMoreThan20Minutes) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Unclassified downtime is more than 20 minutes for the Equipment : <b>{EquipmentId}</b>. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DowntimeUnclassifiedMoreThan15Minutes) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Unclassified downtime is more than 15 minutes for the Equipment : {EquipmentId}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DowntimeUnclassifiedMoreThan15Minutes) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Unclassified downtime is more than 15 minutes for the Equipment : <b>{EquipmentId}</b>. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.OeeBelowThreshold) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Oee for Equipment : {EquipmentId} for Shift : {shiftName} is below than the OEEThreshold : {OEEThreshold}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.OeeBelowThreshold) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Oee for Equipment : <b>{EquipmentId}</b> for Shift : <b>{shiftName}</b> is <b>{oee}%</b>. This is below the threshold : <b>{OEEThreshold}%</b>. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.JobNearCompletion) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The job : {JobName} on Equipment : {EquipmentId} is about to finish. Estimated completion time is : {EstimatedCompletion}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.JobNearCompletion) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The job : <b>{JobName}</b> on Equipment : <b>{EquipmentId}</b> is about to finish. Estimated completion time is :  <b>{EstimatedCompletion}</b>.<br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DownTimeEnergyConsumption) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The Downtime : {DowntimeReason} on Equipment : {EquipmentId} has consumed energy more than {kwh} Kwh.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DownTimeEnergyConsumption) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The Downtime : <b>{DowntimeReason}</b> on Equipment : <b>{EquipmentId}</b> has consumed energy more than <b>{kwh}</b> Kwh.</b>.<br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.EquipmentEnergyConsumption) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Energy consumption for Equipment : {EquipmentId} is {kwhperKG} Kwh/KG. Which is above the standard limit : {LimitInKwhPerKG} Kwh/KG.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.EquipmentEnergyConsumption) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Energy consumption for Equipment : <b>{EquipmentId}</b> is <b>{kwhperKG}</b> Kwh/KG. Which is above the standard limit : <b>{LimitInKwhPerKG}</b> Kwh/KG.</b><br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.LowOutput) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The equipment {EquipmentId} job {JobWorkOrder} has low output, it is running at {RunningSpeed} Kg/Hr. Which is low then the expected speed : {ExpectedSpeed} Kg/Hr.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.LowOutput) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The job <b>{JobWorkOrder}</b> has low output, it is running at <b>{RunningSpeed}</b> Kg/Hr. Which is low then the expected speed : <b>{ExpectedSpeed}</b> Kg/Hr.</b><br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.ScrapAlert) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The job {JobWorkOrder} has running scrap : {RunningScrap} %. Which is above the standard limit : 5% .";
            }
            else if (notificationId == Convert.ToInt32(Notifications.ScrapAlert) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The job {JobWorkOrder} has running scrap : <b>{RunningScrap} %</b>.Which is above the standard limit : <b>5%</b>.</b><br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.SilosLowLevel) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The silos {silosid} is estimated to empty within 6 hours.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.SilosLowLevel) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The silos <b>{silosid} </b> is estimated to empty within 6 hours.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DailyFactoryReport) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The report for factory : {factoryName} for the Date : {date} is ready.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DailyFactoryReport) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The report for factory: <b>{factoryName}</b> for the Date : <b>{date}</b> is attached. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.EnergyConsumptionWhenMachineIsDown) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The equipment {equipmentId} is down due to the {downtimeReason} reason from last 15 minutes and consuming the energy.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.EnergyConsumptionWhenMachineIsDown) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The equipment <b>{equipmentId}</b> is down due to the <b>{downtimeReason}</b> reason from last 15 minutes and consuming the energy. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.NewOperatorAddedToUnidentifiedGallery) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "A new operator has been added to the unidentified gallery with TrackerId {TrackerId}. Please update the worker details.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.NewOperatorAddedToUnidentifiedGallery) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "A new operator has been added to the unidentified gallery with TrackerId <b>{TrackerId}</b>. Please update the worker details. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.NewWorkerSkillAdded) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "ThingTrax has added product {ToolNumber} in the skiil of Worker {OperatorName}. Please verify the same.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.NewWorkerSkillAdded) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "ThingTrax has added product <b>{ToolNumber}</b> in the skiil of Worker <b>{OperatorName}</b>. Please verify the same. <br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.ThingtraxApprovedWorking) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The Thingtrax Approved Operator : {LabourId} is working on Tool : {ToolNumber}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.ThingtraxApprovedWorking) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The Thingtrax Approved Operator :<b>{LabourId} </b> is working on Tool : <b> {ToolNumber} </b>.<br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.VisionAssembly) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The step {name} missed on {equipment} start Date {date}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.VisionAssembly) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The step <b>{name}</b> missed on <b>{equipment}</b> start Date <b>{date}</b>.<br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DisApprovedWorkerWorking) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "Disapproved worker {Operator} working  on {Equipment} for Product {ToolNumber}.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.DisApprovedWorkerWorking) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "Disapproved worker <b>{Operator}</b> working on <b>{Equipment}</b> for Product <b>{ToolNumber}</b>.<br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.WorkerUnwellAlert) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The worker {Operator} having high body temperature({BodyTemp}) is directed to be in the waiting area.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.WorkerUnwellAlert) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The worker <b>{Operator}</b> having high body temperature(<b>{BodyTemp}</b>) is directed to be in the waiting area.<br /> <br /> Thanks <br /> ThingTrax Support";
            }
            else if (notificationId == Convert.ToInt32(Notifications.SocialDistancViolation) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The social distance violation has been noticed for (<b>{totalTime}</b>) in the (<b>{region}</b>) region.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.SocialDistancViolation) && notificationTypeId == Convert.ToInt32(NotificationTypes.Web))
            {
                template = "The social distance violation has been noticed for (<b>{totalTime}</b>) in the (<b>{region}</b>) region.";
            }
            else if (notificationId == Convert.ToInt32(Notifications.WrongEnergyConsumption) && notificationTypeId == Convert.ToInt32(NotificationTypes.Email))
            {
                template = "The wrong energy consumption has been noticed for EquipmentId (<b>{equipmentid}</b>). Start Reading (<b>{startReading}</b> <br /> End Reading (<b>{endReading}</b> <br />. Created Date Of Energy (<b>{createddate}</b> <br />." +
                    "Updated Date of energy(<b>{updateddate}</b> <br />. EnergyId (<b>{energyId}</b> <br />. DowntimeId (<b>{downtimeid}</b> <br />.)";
            }

            return template;
        }
    }
}
