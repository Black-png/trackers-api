//-----------------------------------------------------------------------
// <copyright file="ReportsController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Reports controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models;
    using TT.Core.Models.RequestModels;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Repository.Sql.ResponseQueryEntities;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The reports controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        /// <summary>
        /// The report service
        /// </summary>
        private IReportService reportService;

        /// <summary>
        /// The report service
        /// </summary>
        private IDataExportService dataExportService;

        /// <summary>
        /// The report service
        /// </summary>
        private IKPIService kpiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsController" /> class.
        /// </summary>
        /// <param name="reportService">The report service.</param>
        /// <param name="dataExportService">dataExportService</param>
        /// <param name="kpiService">kpiService</param>
        public ReportsController(IReportService reportService, IDataExportService dataExportService, IKPIService kpiService)
        {
            this.reportService = reportService ?? throw new ArgumentNullException("reportService");
            this.dataExportService = dataExportService ?? throw new ArgumentNullException("dataExportService");
            this.kpiService = kpiService ?? throw new ArgumentNullException("reportService");
        }

        /// <summary>
        /// factory report
        /// </summary>
        /// <param name="factoryId">factoryId</param>
        /// <param name="groupId">groupId</param>
        /// <param name="fromDate">fromDate</param>
        /// <param name="toDate">toDate</param>
        /// <returns>Return factory report data</returns>
        [HttpGet("FactoryReport/{factoryId}/{groupId}/{fromDate}/{toDate}")]
        public async Task<FactortyReportResponseModel> GetFactoryReport(long factoryId, long groupId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetFactoryReport(factoryId, groupId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the factory Report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="unit">Unit</param>
        /// <param name="fromDate">From date</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date</param>
        /// <param name="toshift">To shift</param>
        /// <returns>
        /// factory report
        /// </returns>
        [HttpGet("FactoryReports/{factoryId}/{groupId}/{unit}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<FactoryReportResponseModel> GetFactoryReports(long factoryId, long groupId, int unit, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetFactoryReport(factoryId, groupId, unit, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the factory day report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="reportDate">The date.</param>
        /// <returns>The factory day data.</returns>
        [HttpGet("FactoryDayReport/{factoryId}/{reportDate}")]
        public async Task<List<FactoryDayReportResponseModel>> GetFactoryDayReport(long factoryId, DateTimeOffset reportDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (reportDate == null)
            {
                throw new ArgumentNullException("reportDate");
            }

            return await this.reportService.GetFactoryDayReport(factoryId, reportDate);
        }

        /// <summary>
        /// Gets the factory day report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="unit">Unit</param>
        /// <param name="reportDate">The date.</param>
        /// <returns>The factory day data.</returns>
        [HttpGet("FactoryDayReport/{factoryId}/{unit}/{reportDate}")]
        public async Task<List<FactoryDayReportModel>> GetFactoryDayReport(long factoryId, int unit, DateTimeOffset reportDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (reportDate == null)
            {
                throw new ArgumentNullException("reportDate");
            }

            return await this.reportService.GetFactoryDayReport(factoryId, reportDate, unit);
        }

        /// <summary>
        /// Gets the enhanced factory report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The enhanced factory report response model.</returns>
        /// <exception cref="ArgumentNullException">
        /// model.FactoryId
        /// or
        /// model.FromDate
        /// or
        /// model.ToDate
        /// </exception>
        [HttpGet("EnhancedFactoryReport/{factoryId}/{groupId}/{fromDate}/{toDate}")]
        public async Task<EnhancedFactoryReportResponseModel> GetEnhancedFactoryReport(long factoryId, long groupId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetEnhancedFactoryReport(factoryId, groupId, fromDate, toDate);
        }

        /// <summary>
        /// Exports the factory report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The excel file.</returns>
        [HttpGet("ExportFactoryReport/{factoryId}/{groupId}/{fromDate}/{toDate}")]
        public async Task<FileResult> ExportFactoryReport(long factoryId, long groupId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var reportData = await this.dataExportService.ExportFactoryReport(factoryId, groupId, fromDate, toDate);
            return this.File(Encoding.ASCII.GetBytes(reportData.ToString()), "text/csv", "FactoryReport_" + DateTimeOffset.UtcNow.Date.ToString() + ".csv");
        }

        /// <summary>
        /// Gets the equipment report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>Return equipment report</returns>
        /// <exception cref="ArgumentNullException">
        /// model.FactoryId
        /// or
        /// model.EquipmentId
        /// or
        /// model.FromDate
        /// or
        /// model.ToDate
        /// </exception>
        [HttpGet("EquipmentReport/{factoryId}/{groupId}/{equipmentId}/{fromDate}/{toDate}")]
        public async Task<EquipmentReportResponseModel> GetEquipmentReport(long factoryId, long groupId, long equipmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetEquipmentReport(factoryId, groupId, equipmentId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the shift report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="shiftId">The shift identifier.</param>
        /// <param name="dateTimeOffset">To date.</param>
        /// <returns>Return shift report</returns>
        /// <exception cref="ArgumentNullException">
        /// model
        /// or
        /// model.FactoryId
        /// or
        /// model.ShiftId
        /// or
        /// model.Date
        /// </exception>
        [HttpGet("ShiftReport/{factoryId}/{groupId}/{equipmentId}/{shiftId}/{dateTimeOffset}")]
        public async Task<ShiftReportResponseModel> GetShiftReport(long factoryId, long groupId, long equipmentId, long shiftId, DateTimeOffset dateTimeOffset)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (shiftId <= 0)
            {
                throw new ArgumentNullException("shiftId");
            }

            if (dateTimeOffset == null)
            {
                throw new ArgumentNullException("date");
            }

            return await this.reportService.GetShiftReport(factoryId, groupId, equipmentId, shiftId, dateTimeOffset);
        }

        /// <summary>
        /// Gets the shift report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The Group identifier.</param>
        /// <param name="equipmentId">The Equipment identifier.</param>
        /// <param name="shiftId">The Shift identifier.</param>
        /// <param name="unit">unit.</param>
        /// <param name="reportDate">To date.</param>
        /// <returns>Return shift report</returns>
        /// <exception cref="ArgumentNullException">
        /// model
        /// or
        /// model.FactoryId
        /// or
        /// model.reportDate
        /// </exception>
        [HttpGet("ShiftReport/{factoryId}/{groupId}/{equipmentId}/{shiftId}/{unit}/{reportDate}")]
        public async Task<List<FactoryShiftReportResponseModel>> GetShiftReport(long factoryId, long groupId, long equipmentId, long shiftId, int unit, DateTimeOffset reportDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (reportDate == null)
            {
                throw new ArgumentNullException("reportDate");
            }

            return await this.reportService.GetShiftReport(factoryId, groupId, equipmentId, shiftId, unit, reportDate);
        }

        /// <summary>
        /// Gets the Equipment efficiency report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="equipmentId">The equipment id.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>Return shift report</returns>
        /// <exception cref="ArgumentNullException">
        /// model
        /// or
        /// model.FactoryId
        /// or
        /// model.reportDate
        /// </exception>
        [HttpGet("EquipmentEfficiencyReport/{factoryId}/{equipmentId}/{unit}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<EquipmentEfficiencyReportModel> GetEquipmentEfficiencyReport(long factoryId, long equipmentId, int unit, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromdate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("todate");
            }

            return await this.reportService.GetEquipmentEfficiencyReport(factoryId, equipmentId, unit, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the Equipment efficiency report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="toolId">The tool id.</param>
        /// <param name="productId">The product Id.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>Return shift report</returns>
        /// <exception cref="ArgumentNullException">
        /// model
        /// or
        /// model.FactoryId
        /// or
        /// model.reportDate
        /// </exception>
        [HttpGet("ProductEfficiencyReport/{factoryId}/{toolId}/{productId}/{unit}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<ProductEfficiencyReportWrapperModel> GetProductEfficiencyReport(long factoryId, long toolId, long productId, int unit, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromdate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("todate");
            }

            return await this.reportService.GetProductEfficiencyReport(factoryId, toolId, productId, unit, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the detail factory report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>Return shift report</returns>
        /// <exception cref="ArgumentNullException">
        /// model
        /// or
        /// model.FactoryId
        /// or
        /// model.reportDate
        /// </exception>
        [HttpGet("DetailFactoryReport/{factoryId}/{unit}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<List<FactoryDetailReportModel>> GetDetailFactoryReport(long factoryId, int unit, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromdate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("todate");
            }

            return await this.reportService.GetDetailFactoryReport(factoryId, unit, fromDate, fromshift, toDate, toshift).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list of downtimes.
        /// </summary>
        /// <returns>The list of downtimes.</returns>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        [HttpGet("GetDownTime/{factoryId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public Task<Tuple<Dictionary<string, List<DownTimeModel>>, Dictionary<string, List<DownTimeModel>>>> GetDownTime(long factoryId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            var downtimes = this.reportService.GetAllDowntimesByTime(factoryId, fromDate, fromshift, toDate, toshift);
            return downtimes;
        }

        /// <summary>
        /// Gets the product downtimes.
        /// </summary>
        /// <param name="reasonId"> reasonid.</param>
        /// <param name="factoryId"> factoryid.</param>
        /// <param name="fromDate">from Date.</param>
        /// <param name="fromshift">froms hift.</param>
        /// <param name="toDate">To.</param>
        /// <param name="toshift">to shift.</param>
        /// <returns>The list of downtimes.</returns>
        [HttpGet("GetDownTimeWithToolId/{reasonId}/{factoryId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public Task<Tuple<Dictionary<string, List<DownTimeModel>>, Dictionary<long?, List<DownTimeModel>>, List<DowntimeAnalysis>>> GetDownTimeWithToolId(int reasonId, long factoryId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            var downtimes = this.reportService.GetDownTimeWithToolId(reasonId, factoryId, fromDate, fromshift, toDate, toshift);
            return downtimes;
        }

        /// <summary>
        /// Gets the job report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Return job report</returns>
        /// <exception cref="ArgumentNullException">jobId</exception>
        [HttpGet("JobReport/{factoryId}/{jobId}")]
        public async Task<JobReportResponseModel> GetJobReport(long factoryId, long jobId)
        {
            if (jobId <= 0)
            {
                throw new ArgumentNullException("jobId");
            }

            return await this.reportService.GetJobReport(factoryId, jobId);
        }

        /// <summary>
        /// Gets the job report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Return job detail report</returns>
        /// <exception cref="ArgumentNullException">jobId</exception>
        [HttpGet("GetDetailJobReport/{factoryId}/{jobId}")]
        public async Task<JobReportResponseModel> GetDetailJobReport(long factoryId, long jobId)
        {
            if (jobId <= 0)
            {
                throw new ArgumentNullException("jobId");
            }

            return await this.reportService.GetJobDetailReport(factoryId, jobId);
        }

        /// <summary>
        /// Gets the product report.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The product report response model.</returns>
        /// <exception cref="ArgumentNullException">productId</exception>
        [HttpGet("ProductReport/{productId}/{fromDate}/{toDate}")]
        public async Task<ProductReportResponseModel> GetProductReport(long productId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (productId <= 0)
            {
                throw new ArgumentNullException("productId");
            }

            return await this.reportService.GetProductReport(productId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the energy report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>Energy report</returns>
        /// <exception cref="ArgumentNullException">productId</exception>
        [HttpGet("EnergyReport/{factoryId}/{groupId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<EnergyReportResponseModel> GetEnergyReport(long factoryId, long groupId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.reportService.GetEnergiesReport(factoryId, groupId, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the factory oee report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>The factory oee report response model.</returns>
        [HttpGet("FactoryOeeReport/{factoryId}/{groupId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<FactoryOeeReportResponseModel> GetFactoryOeeReport(long factoryId, long groupId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetFactoryOeeReport(factoryId, groupId, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the factory availability report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>The factory availability report response model.</returns>
        [HttpGet("FactoryAvailabilityReport/{factoryId}/{groupId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<FactoryAvailabilityReportResponseModel> GetFactoryAvailabilityReport(long factoryId, long groupId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetFactoryAvailabilityReport(factoryId, groupId, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the factory performance report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>The factory performance report response model.</returns>
        [HttpGet("FactoryPerformanceReport/{factoryId}/{groupId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<FactoryPerformanceReportResponseModel> GetFactoryPerformanceReport(long factoryId, long groupId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetFactoryPerformanceReport(factoryId, groupId, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the factory quality report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">To date.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>The factory quality report response model.</returns>
        [HttpGet("FactoryQualityReport/{factoryId}/{groupId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<FactoryQualityReportResponseModel> GetFactoryQualityReport(long factoryId, long groupId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("model.FactoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("model.FromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("model.ToDate");
            }

            return await this.reportService.GetFactoryQualityReport(factoryId, groupId, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the equipment oee report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The equipment oee report response model.</returns>
        /// <exception cref="ArgumentNullException">
        /// factoryId
        /// or
        /// fromDate
        /// or
        /// toDate
        /// </exception>
        [HttpGet("EquipmentOeeReport/{factoryId}/{groupId}/{equipmentId}/{fromDate}/{toDate}")]
        public async Task<EquipmentOEEReportResponseModel> GetEquipmentOeeReport(long factoryId, long groupId, long equipmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetEquipmentOeeReport(factoryId, groupId, equipmentId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the equipment availability report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The equipment availability report.</returns>
        /// <exception cref="ArgumentNullException">
        /// factoryId
        /// or
        /// equipmentId
        /// or
        /// fromDate
        /// or
        /// toDate
        /// </exception>
        [HttpGet("EquipmentAvailabilityReport/{factoryId}/{groupId}/{equipmentId}/{fromDate}/{toDate}")]
        public async Task<EquipmentAvailabilityReportResponseModel> GetEquipmentAvailabilityReport(long factoryId, long groupId, long equipmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetEquipmentAvailabilityReport(factoryId, groupId, equipmentId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the equipment performance report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The equipment performance report response model.</returns>
        /// <exception cref="ArgumentNullException">
        /// factoryId
        /// or
        /// equipmentId
        /// or
        /// fromDate
        /// or
        /// toDate
        /// </exception>
        [HttpGet("EquipmentPerformanceReport/{factoryId}/{groupId}/{equipmentId}/{fromDate}/{toDate}")]
        public async Task<EquipmentPerformanceReportResponseModel> GetEquipmentPerformanceReport(long factoryId, long groupId, long equipmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetEquipmentPerformanceReport(factoryId, groupId, equipmentId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the equipment quality report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>The equipment quality report response model.</returns>
        /// <exception cref="ArgumentNullException">
        /// factoryId
        /// or
        /// equipmentId
        /// or
        /// fromDate
        /// or
        /// toDate
        /// </exception>
        [HttpGet("EquipmentQualityReport/{factoryId}/{groupId}/{equipmentId}/{fromDate}/{toDate}")]
        public async Task<EquipmentQualityReportResponseModel> GetEquipmentQualityReport(long factoryId, long groupId, long equipmentId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (equipmentId <= 0)
            {
                throw new ArgumentNullException("equipmentId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetEquipmentQualityReport(factoryId, groupId, equipmentId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the kpi report.
        /// </summary>
        /// <param name="factoryId">The factory Identifier.</param>
        /// <returns>The list of kpi data.</returns>
        /// <exception cref="ArgumentNullException">productId</exception>
        [HttpGet("KPIReport/{factoryId}")]
        public async Task<KpiReportResponseModel> GetKPIReport(long factoryId)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.kpiService.CalculateKpi(factoryId);
        }

        /// <summary>
        /// Get Dwell time report.
        /// </summary>
        /// <param name="factoryId">factoryId</param>
        /// <param name="departmentId">departmentId</param>
        /// <param name="trackerId">trackerId</param>
        /// <param name="regionOfInterestId">regionOfInterestId</param>
        /// <param name="fromDate">fromDate</param>
        /// <param name="toDate">toDate</param>
        /// <returns>Returns dwell time report.</returns>
        [HttpGet("DwellTimeReport/{factoryId}/{departmentId}/{trackerId}/{regionOfInterestId}/{fromDate}/{toDate}")]
        public async Task<List<DwellTimeReportResponseModel>> GetLabourDwellTimeReport(long factoryId, long departmentId, long trackerId, long regionOfInterestId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            return await this.reportService.GetLabourDwellTimeReport(factoryId, departmentId, trackerId, regionOfInterestId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the energy report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>Downtime energy report</returns>
        /// <exception cref="ArgumentNullException">groupId</exception>
        [HttpGet("DowntimeReport/{factoryId}/{groupId}/{fromDate}/{toDate}")]
        public async Task<DowntimeReportResponseModel> GetDowntimeReport(long factoryId, long groupId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetDowntimeReport(factoryId, groupId, fromDate, toDate);
        }

        /// <summary>
        /// Gets the energy report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>Downtime energy report</returns>
        /// <exception cref="ArgumentNullException">groupId</exception>
        [HttpGet("DowntimeByGroupReport/{factoryId}/{groupId}/{fromDate}/{toDate}")]
        public async Task<DowntimeReportResponseModel> GetDowntimeByGroupReport(long factoryId, long groupId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null)
            {
                throw new ArgumentNullException("fromDate");
            }

            if (toDate == null)
            {
                throw new ArgumentNullException("toDate");
            }

            return await this.reportService.GetDowntimeByGroupReport(factoryId, groupId, fromDate, toDate);
        }

        /////// <summary>
        /////// Gets the labour journey report.
        /////// </summary>
        /////// <param name="factoryId">The factory identifier.</param>
        /////// <param name="areaId">The area identifier.</param>
        /////// <param name="fromDate">From date.</param>
        /////// <param name="toDate">To date.</param>
        /////// <returns>The list of labour journey report model.</returns>
        /////// <exception cref="ArgumentNullException">
        /////// factoryId
        /////// or
        /////// areaId
        /////// or
        /////// fromDate
        /////// or
        /////// toDate
        /////// </exception>
        ////[HttpGet("LabourJourneyReport/{factoryId}/{areaId}/{fromDate}/{toDate}")]
        ////public async Task<List<LabourJourneyReportModel>> GetLabourJourneyReport(long factoryId, long areaId, DateTimeOffset fromDate, DateTimeOffset toDate)
        ////{
        ////    if (factoryId <= 0)
        ////    {
        ////        throw new ArgumentNullException("factoryId");
        ////    }

        ////    if (areaId <= 0)
        ////    {
        ////        throw new ArgumentNullException("areaId");
        ////    }

        ////    if (fromDate == null)
        ////    {
        ////        throw new ArgumentNullException("fromDate");
        ////    }

        ////    if (toDate == null)
        ////    {
        ////        throw new ArgumentNullException("toDate");
        ////    }

        ////    return await this.reportService.GetLabourJourneyReport(factoryId, areaId, fromDate, toDate);
        ////}

        /// <summary>
        /// Gets the rejection report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="equipmentId">The equipment identifier.</param>
        /// <param name="fromDate">The fromDate.</param>
        /// <param name="fromshift">From shift</param>
        /// <param name="toDate">The toDate.</param>
        /// <param name="toshift">To shift</param>
        /// <returns>The list of rejections.</returns>
        /// <exception cref="ArgumentNullException">
        /// factoryId
        /// or
        /// date
        /// </exception>
        [HttpGet("RejectionReport/{factoryId}/{groupId}/{equipmentId}/{fromDate}/{fromshift}/{toDate}/{toshift}")]
        public async Task<List<RejectionReportResponseModel>> GetRejectionReport(long factoryId, long groupId, long equipmentId, DateTimeOffset fromDate, long fromshift, DateTimeOffset toDate, long toshift)
        {
            if (factoryId <= 0)
            {
                throw new ArgumentNullException("factoryId");
            }

            if (fromDate == null || toDate == null)
            {
                throw new ArgumentNullException("date");
            }

            return await this.reportService.GetRjectionReport(factoryId, groupId, equipmentId, fromDate, fromshift, toDate, toshift);
        }

        /// <summary>
        /// Gets the operator dwell time report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="jobTitleId">The job title identifier.</param>
        /// <param name="labourId">The labour identifier.</param>
        /// <param name="shiftId">The shift identifier.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        /// <returns>The operator dwell time report data.</returns>
        [HttpGet("OperatorDwellTimeReport")]
        public async Task<List<OperatorDwellTimeReportResponseQueryModel>> GetOperatorDwellTimeReport(long factoryId, long? jobTitleId, long? labourId, long? shiftId, DateTimeOffset? fromTime, DateTimeOffset? toTime)
        {
            var reportData = await this.reportService.GetOperatorDwellTime(factoryId, jobTitleId ?? 0, labourId ?? 0, shiftId ?? 0, fromTime, toTime);
            return reportData;
        }

        /// <summary>
        /// Gets the Unit Type.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>Gets the Unit Type data.</returns>
        [HttpGet("GetUnitTypeByFactory")]
        public async Task<List<UnitTypeResponseQueryModel>> GetUnitTypeByFactory(long factoryId)
        {
            var reportData = await this.reportService.GetUnitTypeByFactoryId(factoryId);
            return reportData;
        }

        /// <summary>
        /// Gets the operator check in report.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="jobTitleId">The job title identifier.</param>
        /// <param name="labourId">The labour identifier.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        /// <returns>The operators check in data.</returns>
        [HttpGet("OperatorCheckInReport")]
        public async Task<List<OperatorsCheckInDataReportResponseModel>> GetOperatorCheckInReport(long factoryId, long? jobTitleId, long? labourId, DateTimeOffset? fromTime, DateTimeOffset? toTime)
        {
            var reportdata = await this.reportService.GetOperatorsCheckInReport(factoryId, jobTitleId, labourId, fromTime, toTime);
            return reportdata;
        }
    }
}
