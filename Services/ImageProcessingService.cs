using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using MiniPhotoshop.Models;

namespace MiniPhotoshop.Services
{
    public class ImageProcessingService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsPath;

        public ImageProcessingService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            
            if (!Directory.Exists(_uploadsPath))
                Directory.CreateDirectory(_uploadsPath);
        }

        public async Task<string> SaveUploadedImageAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_uploadsPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return $"/uploads/{fileName}";
        }

        public async Task<string> ProcessImageAsync(string imagePath, string operation, Dictionary<string, object> parameters)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            var outputFileName = $"{Guid.NewGuid()}.jpg";
            var outputPath = Path.Combine(_uploadsPath, outputFileName);

            using (var image = await Image.LoadAsync(fullPath))
            {
                switch (operation.ToLower())
                {
                    case "rotate":
                        var angle = Convert.ToSingle(parameters["angle"]);
                        image.Mutate(x => x.Rotate(angle));
                        break;

                    case "resize":
                        var width = Convert.ToInt32(parameters["width"]);
                        var height = Convert.ToInt32(parameters["height"]);
                        image.Mutate(x => x.Resize(width, height));
                        break;

                    case "crop":
                        var x = Convert.ToInt32(parameters["x"]);
                        var y = Convert.ToInt32(parameters["y"]);
                        var cropWidth = Convert.ToInt32(parameters["width"]);
                        var cropHeight = Convert.ToInt32(parameters["height"]);
                        image.Mutate(img => img.Crop(new Rectangle(x, y, cropWidth, cropHeight)));
                        break;

                    case "brightness":
                        var brightness = Convert.ToSingle(parameters["value"]);
                        image.Mutate(x => x.Brightness(brightness));
                        break;

                    case "contrast":
                        var contrast = Convert.ToSingle(parameters["value"]);
                        image.Mutate(x => x.Contrast(contrast));
                        break;

                    case "invert":
                        image.Mutate(x => x.Invert());
                        break;
                }

                await image.SaveAsJpegAsync(outputPath, new JpegEncoder { Quality = 90 });
            }

            return $"/uploads/{outputFileName}";
        }

        public void DeleteImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception)
            {
                // Log error if needed
            }
        }
    }
} 