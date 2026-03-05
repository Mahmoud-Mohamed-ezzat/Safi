using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Safi.Services
{
    public class ImageService
    {
        private readonly string _imagesFolder;
        private const string _imagesUrlPrefix = "/images/";

        public ImageService(IWebHostEnvironment env)
        {
            _imagesFolder = Path.Combine(env.WebRootPath, "images");
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid image file format. Allowed formats: jpg, jpeg, png, gif, bmp");

            EnsureFolderExists();

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_imagesFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return the relative URL that the frontend can use directly
            return $"{_imagesUrlPrefix}{fileName}";
        }

        public Task<bool> DeleteImageAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Task.FromResult(false);

            // Support both full URL path "/images/file.jpg" and bare filename "file.jpg"
            var fileName = imageUrl.StartsWith(_imagesUrlPrefix, StringComparison.OrdinalIgnoreCase)
                ? imageUrl[_imagesUrlPrefix.Length..]
                : imageUrl;

            var filePath = Path.Combine(_imagesFolder, fileName);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(false);
        }

        public async Task<string> UpdateImageAsync(string? oldImageUrl, IFormFile newImageFile)
        {
            if (newImageFile == null || newImageFile.Length == 0)
                throw new ArgumentException("Image file is required");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(newImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid image file format. Allowed formats: jpg, jpeg, png, gif, bmp");

            // Delete old image if it exists
            if (!string.IsNullOrWhiteSpace(oldImageUrl))
                await DeleteImageAsync(oldImageUrl);

            EnsureFolderExists();

            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var newFilePath = Path.Combine(_imagesFolder, newFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await newImageFile.CopyToAsync(stream);
            }

            // Return the relative URL that the frontend can use directly
            return $"{_imagesUrlPrefix}{newFileName}";
        }

        private void EnsureFolderExists()
        {
            if (!Directory.Exists(_imagesFolder))
                Directory.CreateDirectory(_imagesFolder);
        }
    }
}
