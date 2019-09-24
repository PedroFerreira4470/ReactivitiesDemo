using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
   public interface IPhotoAcessor
    {
        PhotoUploadResult AddPhoto(IFormFile file);

        string DeletePhoto(string publicId);
    }
}
