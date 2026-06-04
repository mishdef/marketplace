using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace VetClassLibrary.Interfaces
{
    public interface IPicturesService
    {
        Task<string> UploadPictureAsync(IFormFile file);
    }
}