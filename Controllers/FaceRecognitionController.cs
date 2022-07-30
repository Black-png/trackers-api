//-----------------------------------------------------------------------
// <copyright file="FaceRecognitionController.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>Face recognition controller class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TT.Core.Models;
    using TT.Core.Models.RequestModels;
    using TT.Core.Models.ResponseModels;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The face recognition controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class FaceRecognitionController : Controller
    {
        private IFaceRecognitionService faceRecognitionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceRecognitionController"/> class.
        /// </summary>
        /// <param name="faceRecognitionService">The face recognition service.</param>
        /// <exception cref="ArgumentNullException">faceRecognitionService</exception>
        public FaceRecognitionController(IFaceRecognitionService faceRecognitionService)
        {
            this.faceRecognitionService = faceRecognitionService ?? throw new ArgumentNullException("faceRecognitionService");
        }

        /// <summary>
        /// Gets the unidentified operator equipment.
        /// </summary>
        /// <returns>The list of equipments.</returns>
        [HttpGet("visionequipments")]
        public async Task<List<string>> GetUnidentifiedOperatorEquipment()
        {
            return await this.faceRecognitionService.GetUnidentifiedOperatorEquipment();
        }

        /// <summary>
        /// Posts the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        [HttpPost("RegisterPerson")]
        public async Task Post([FromBody]RegisterUnidentifiedPersonModel model)
        {
            if (string.IsNullOrEmpty(model.FolderName))
            {
                throw new ArgumentNullException("FolderName");
            }

            if (model.FaceImages.Count <= 0)
            {
                throw new ArgumentNullException("Images");
            }

            if (model.Personimage == null)
            {
                throw new ArgumentNullException("Personimage");
            }

            await this.faceRecognitionService.RegisterPersonToKairos(model);
        }

        /// <summary>
        /// Posts the specified upload image model.
        /// </summary>
        /// <param name="uploadImageModel">The upload image model.</param>
        /// <exception cref="ArgumentNullException">
        /// FolderName
        /// or
        /// BlobImages
        /// </exception>
        /// <returns>The task.</returns>
        [HttpPost("UpdateOperatorImages")]
        public async Task Post([FromBody]UploadImagesToKairosRequestModel uploadImageModel)
        {
            if (string.IsNullOrEmpty(uploadImageModel.FolderName))
            {
                throw new ArgumentNullException("FolderName");
            }

            if (uploadImageModel.BlobImages.Count <= 0)
            {
                throw new ArgumentNullException("BlobImages");
            }

            await this.faceRecognitionService.UpdateImagesToKairos(uploadImageModel);
        }

        /// <summary>
        /// Deletes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentNullException">
        /// FolderName
        /// or
        /// Images
        /// </exception>
        [HttpPost("DeleteVisionImages")]
        public async Task Delete([FromBody]RegisterUnidentifiedPersonModel model)
        {
            if (string.IsNullOrEmpty(model.FolderName))
            {
                throw new ArgumentNullException("FolderName");
            }

            if (model.FaceImages.Count <= 0)
            {
                throw new ArgumentNullException("Images");
            }

            await this.faceRecognitionService.DeleteMultipleImages(model.FolderName, model.FaceImages);
        }

        /// <summary>
        /// Gets the folder images.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The images.</returns>
        /// <exception cref="ArgumentNullException">containerName</exception>
        [HttpGet("getfolderimages")]
        public async Task<VisionImageModel> GetFolderImages(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException("containerName");
            }

            return await this.faceRecognitionService.GetUnidentifiedPersonImages(containerName);
        }

        /// <summary>
        /// Gets the face recognition gallery data.
        /// </summary>
        /// <returns>The face recognition gallery data.</returns>
        [HttpGet("GalleryData")]
        public async Task<FaceRecognitionGalleryResponseModel> GetFaceRecognitionGalleryData()
        {
            return await this.faceRecognitionService.FaceRecognitionGalleryData();
        }
    }
}
