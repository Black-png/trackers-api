//-----------------------------------------------------------------------
// <copyright file="ProductController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Product controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services.Interfaces;
    using TT.Core.Models;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository.Sql.Entities;

    /// <summary>
    /// Product controller class.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        /// <summary>
        /// The product service
        /// </summary>
        private IProductService productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController" /> class.
        /// </summary>
        /// <param name="productService">The entity service.</param>
        /// <param name="applicationSettingsConfig">Application configuration settings </param>
        public ProductController(IOptions<ApplicationSettings> applicationSettingsConfig, IProductService productService)
        {
            this.productService = productService ?? throw new ArgumentNullException("productService");
            this.ApplicationSettings = applicationSettingsConfig.Value;
        }

        /// <summary>
        /// Gets or sets the Application Setting
        /// </summary>
        private ApplicationSettings ApplicationSettings { get; set; }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <returns>The list of products</returns>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await this.productService.GetAll();
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="pageNo">Page Number</param>
        /// <param name="searchText">Serach text </param>
        [HttpGet("GetSearched")]
        public Tuple<IEnumerable<ProductDataModel>, int> GetSearched(int pageNo, string searchText)
        {
            var products = this.productService.GetSearchdata(pageNo, this.ApplicationSettings.PageSize, searchText, out int totalCount);
            return Tuple.Create(products, totalCount);
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <returns>The list of products</returns>
        /// <param name="searchText">Serach text </param>
        /// <param name="factoryId">The factory identifier.</param>

        [HttpGet("GetproductSearch/{searchText}/{factoryId}")]
        public async Task<IEnumerable<ProductDataModel>> GetSearchedByName(string searchText, long factoryId)
        {
            var products = await this.productService.GetSearchProductByName(searchText, factoryId);
            return products;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The product</returns>
        [HttpGet("{id}")]
        public async Task<Product> Get(long id)
        {
            return await this.productService.Get(id);
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The product
        /// </returns>
        [HttpGet("GetProductById/{id}")]
        public async Task<ProductDataModel> GetProductById(long id)
        {
            return await this.productService.GetProductById(id);
        }

        /// <summary>
        /// Posts the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPost]
        public async Task<Product> Post([FromBody] Product product)
        {
            return await this.productService.Create(product);
        }

        /// <summary>
        /// Puts the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>The task</returns>
        [Authorize(Policy = "CustomAuthorization")]
        [HttpPut]
        public async Task Put([FromBody] Product product)
        {
            await this.productService.Update(product);
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
            await this.productService.Delete(id);
        }

        /// <summary>
        /// Post File.
        /// </summary>
        /// <param name="file">The product.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The task</returns>
        [HttpPost("postfile/{factoryId}")]
        public async Task<List<string>> PostFile(IFormFile file, long factoryId)
        {
            List<string> errorList = new List<string>();
            if (file == null)
            {
                errorList.Add(string.Format("File is null."));
            }

            var stream = file.OpenReadStream();
            errorList = await this.productService.ReadDataFromFile(stream, factoryId);
            return errorList;
        }

        /// <summary>
        /// Gets the list of product as filestream
        /// </summary>
        /// <returns>Returns the file stream  </returns>
        /// <param name="filetype">The FileType.</param>
        /// <param name="factoryId">The factory identifier.</param>
        [HttpGet("ExportProducts")]
        public async Task<FileStreamResult> GetProducts(string filetype, long factoryId)
        {
            var productList = await this.productService.GetProductExportData(factoryId);
            MemoryStream memStream = new MemoryStream();
            string contentType = "text/csv";
            StringBuilder str = new StringBuilder();

            if (filetype == "excel")
            {
                contentType = "application/pdf";
                str.Append(string.Format("{0},{1},{2},{3},{4},{5},{6}", "ProductCode", "Name", "Tool Id", " Tool Cavity", "Weight (Gram)", "IdealCycleTime (Sec)", "Measurement Unit") + Environment.NewLine);

                foreach (ProductExportModel product in productList)
                {
                    str.Append(string.Format("{0},{1},{2},{3},{4},{5},{6}", product.ProductCode, product.Name, product.MouldNumber, product.Cavity.ToString(), product.Weight.ToString(), product.IdealCycleTime.ToString(), product.MeasurementUnit) + Environment.NewLine);
                }
            }
            else
            {
                str.Append("<table border=" + "1px" + " color= black width=500>");
                str.Append("<tr>");
                str.Append("<td><b><font color= black>Product Code</font></b></td>");
                str.Append("<td><b><font color= black>Name</font></b></td>");
                str.Append("<td><b><font color= black>Cavity</font></b></td>");
                str.Append("<td><b><font color= black>Weight</font></b></td>");
                str.Append("<td><b><font color= black>Ideal Cycle Time</font></b></td>");
                str.Append("<td><b><font color= black>Tool Id</font></b></td>");
                str.Append("<td><b><font color= black>Measurement Unit</font></b></td>");
                str.Append("</tr>");

                foreach (ProductExportModel product in productList)
                {
                    str.Append("<tr>");
                    str.Append("<td><font color= #6c6c6c width=100>" + product.ProductCode.ToString() + "</font></td>");
                    str.Append("<td><font color= #6c6c6c width=80>" + product.Name.ToString() + "</font></td>");
                    str.Append("<td><font color= #6c6c6c>" + product.Cavity.ToString() + "</font></td>");
                    str.Append("<td><font color= #6c6c6c>" + product.Weight.ToString() + "</font></td>");
                    str.Append("<td><font color= #6c6c6c>" + product.IdealCycleTime.ToString() + "</font></td>");
                    str.Append("<td><font color= #6c6c6c>" + product.MouldNumber.ToString() + "</font></td>");
                    str.Append("<td><font color= #6c6c6c>" + product.MeasurementUnit + "</font></td>");
                    str.Append("</tr>");
                }

                str.Append("</table>");
            }

            try
            {
                byte[] fileData = System.Text.Encoding.UTF8.GetBytes(str.ToString());
                memStream.Write(fileData, 0, fileData.Length);
                memStream.Position = 0;

                this.Response.Headers.Add("Content-Type", contentType);
                return new FileStreamResult(memStream, contentType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the products by tool identifier.
        /// </summary>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>The list of praduct name and id.</returns>
        [HttpGet("getproductsbytool/{toolId}")]
        public async Task<IEnumerable<ProductSelectionModel>> GetProductsByToolId(long toolId)
        {
            return await this.productService.GetProductsByToolId(toolId);
        }

        /// <summary>
        /// Gets the products by tool identifier.
        /// </summary>
        /// <param name="toolId">The tool identifier.</param>
        /// <returns>The list of praduct name and id.</returns>
        [HttpGet("getproductsbytoolid/{toolId}")]
        public async Task<IEnumerable<Product>> GetProductsByTool(long toolId)
        {
            return await this.productService.GetProductsByTool(toolId);
        }

        /// <summary>
        /// Gets the measurement unit for product.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <returns>The measurement unit.</returns>
        [HttpGet("getmeasurementunit/{productCode}")]
        public async Task<int?> GetMeasurementUnitForProduct(string productCode)
        {
            return await this.productService.GetMeasurementUnitForProduct(productCode);
        }

        /// <summary>
        /// Gets the products by tool identifier.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of praduct name and id.</returns>
        [HttpGet("getproductsbyfactory/{factoryId}")]
        public async Task<IEnumerable<Product>> GetProductsByFactoryId(long factoryId)
        {
            return await this.productService.GetProductsByFactoryId(factoryId);
        }

        /// <summary>
        /// Gets the factory products.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The list of product details.</returns>
        [HttpGet("getfactoryproducts/{factoryId}")]
        public async Task<IEnumerable<ProductDataModel>> GetFactoryProducts(long factoryId)
        {
            return await this.productService.GetFactoryProducts(factoryId);
        }

        /// <summary>
        /// Determines whether [is product code exists] [the specified product code].
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>true or false.</returns>
        [HttpGet("isproductcodeavailable/{productCode}/{factoryId}")]
        public async Task<bool> IsProductCodeExists(string productCode, long factoryId)
        {
            return await this.productService.IsProductCodeExists(productCode, factoryId);
        }

        /// <summary>
        /// Determines whether [is product assinged] [the specified productid].
        /// </summary>
        /// <param name="productid">The productid.</param>
        /// <returns>Return flag whether product is assigned or not.</returns>
        [HttpGet("IsProductAssinged/{productid}")]
        public async Task<bool> IsProductAssinged(long productid)
        {
            return await this.productService.IsProductAssinged(productid);
        }
    }
}
