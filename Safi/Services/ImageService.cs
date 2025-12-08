using Microsoft.AspNetCore.Http;

namespace Safi.Services
{
    public class ImageService
    {
        private readonly string _imagesFolder = @"D:\Safi\Safi\Safi\Images";

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Image file is required");
            }

            // Validate file is an image
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid image file format. Allowed formats: jpg, jpeg, png, gif, bmp");
            }

            // Create Images folder if it doesn't exist
            if (!Directory.Exists(_imagesFolder))
            {
                Directory.CreateDirectory(_imagesFolder);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_imagesFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return the filename
            return fileName;
        }

        public Task<bool> DeleteImageAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Task.FromResult(false);
            }

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

        public async Task<string> UpdateImageAsync(string oldFileName, IFormFile newImageFile)
        {
            if (newImageFile == null || newImageFile.Length == 0)
            {
                throw new ArgumentException("Image file is required");
            }

            // Validate file is an image
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(newImageFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid image file format. Allowed formats: jpg, jpeg, png, gif, bmp");
            }

            // Delete old image if it exists
            if (!string.IsNullOrWhiteSpace(oldFileName))
            {
                await DeleteImageAsync(oldFileName);
            }

            // Create Images folder if it doesn't exist
            if (!Directory.Exists(_imagesFolder))
            {
                Directory.CreateDirectory(_imagesFolder);
            }

            // Generate unique filename for new image
            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var newFilePath = Path.Combine(_imagesFolder, newFileName);

            // Save the new file
            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await newImageFile.CopyToAsync(stream);
            }

            // Return the new filename
            return newFileName;
        }
    }
}
