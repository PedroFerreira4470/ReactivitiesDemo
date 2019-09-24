using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Infrastructure.Photos
{
    public class PhotoAcessor : IPhotoAcessor
    {
        private readonly Cloudinary cloudinary;

        public PhotoAcessor(IOptions<CloudinarySettings> config)
        {
            var cloud = config.Value;
            var acc = new Account(
                cloud.CloudName,
                cloud.ApiKey,
                cloud.ApiSecret
           );

            this.cloudinary = new Cloudinary(acc);
        }

        public PhotoUploadResult AddPhoto(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName,stream),
                        Transformation = new Transformation().Height(500).Width(500)
                          .Crop("fill").Gravity("face"),
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUri.AbsoluteUri,
            };
        }

        public string DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = cloudinary.Destroy(deleteParams);

            return result.Result == "ok" ? result.Result : null;
        }
    }
}
