using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VetClassLibrary.Interfaces;

namespace VetClassLibrary.Services
{
    public class PicturesService : IPicturesService
    {
        private readonly string _storagePath;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public PicturesService(IWebHostEnvironment env)
        {
            _storagePath = Path.Combine(env.ContentRootPath, "UploadedPictures");

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> UploadPictureAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Файл не выбран или пуст.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Недопустимый формат файла.");
            }

            var pictureId = Guid.NewGuid().ToString();
            var fileName = $"{pictureId}{extension}";
            var filePath = Path.Combine(_storagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}