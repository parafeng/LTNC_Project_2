using Microsoft.AspNetCore.Mvc;
using MiniPhotoshop.Models;
using MiniPhotoshop.Services;
using System.Text.Json;

namespace MiniPhotoshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ImageProcessingService _imageService;
        private const string SessionKey = "ImageEditSession";

        public HomeController(ImageProcessingService imageService)
        {
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file." });
            }

            try
            {
                var imagePath = await _imageService.SaveUploadedImageAsync(file);
                
                var editModel = new ImageEditModel
                {
                    ImagePath = imagePath,
                    OriginalImagePath = imagePath
                };

                HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(editModel));

                return Json(new { success = true, imagePath = imagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessImage([FromBody] ImageEditRequest request)
        {
            try
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                if (string.IsNullOrEmpty(sessionData))
                {
                    return Json(new { success = false, message = "No image session found." });
                }

                var editModel = JsonSerializer.Deserialize<ImageEditModel>(sessionData);
                if (editModel == null)
                {
                    return Json(new { success = false, message = "Invalid session data." });
                }

                var resultPath = await _imageService.ProcessImageAsync(request.ImagePath, request.Operation, request.Parameters);

                // Add to history
                var operation = new EditOperation
                {
                    Operation = request.Operation,
                    Parameters = request.Parameters,
                    ResultImagePath = resultPath
                };

                // Remove any operations after current index (for redo functionality)
                if (editModel.CurrentHistoryIndex < editModel.EditHistory.Count - 1)
                {
                    editModel.EditHistory.RemoveRange(editModel.CurrentHistoryIndex + 1, 
                        editModel.EditHistory.Count - editModel.CurrentHistoryIndex - 1);
                }

                editModel.EditHistory.Add(operation);
                editModel.CurrentHistoryIndex = editModel.EditHistory.Count - 1;
                editModel.ImagePath = resultPath;

                HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(editModel));

                return Json(new { success = true, imagePath = resultPath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Undo()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                if (string.IsNullOrEmpty(sessionData))
                {
                    return Json(new { success = false, message = "No image session found." });
                }

                var editModel = JsonSerializer.Deserialize<ImageEditModel>(sessionData);
                if (editModel == null || editModel.CurrentHistoryIndex <= 0)
                {
                    return Json(new { success = false, message = "Nothing to undo." });
                }

                editModel.CurrentHistoryIndex--;
                if (editModel.CurrentHistoryIndex == -1)
                {
                    editModel.ImagePath = editModel.OriginalImagePath;
                }
                else
                {
                    editModel.ImagePath = editModel.EditHistory[editModel.CurrentHistoryIndex].ResultImagePath;
                }

                HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(editModel));

                return Json(new { success = true, imagePath = editModel.ImagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Redo()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                if (string.IsNullOrEmpty(sessionData))
                {
                    return Json(new { success = false, message = "No image session found." });
                }

                var editModel = JsonSerializer.Deserialize<ImageEditModel>(sessionData);
                if (editModel == null || editModel.CurrentHistoryIndex >= editModel.EditHistory.Count - 1)
                {
                    return Json(new { success = false, message = "Nothing to redo." });
                }

                editModel.CurrentHistoryIndex++;
                editModel.ImagePath = editModel.EditHistory[editModel.CurrentHistoryIndex].ResultImagePath;

                HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(editModel));

                return Json(new { success = true, imagePath = editModel.ImagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Download()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                if (string.IsNullOrEmpty(sessionData))
                {
                    return BadRequest("No image session found.");
                }

                var editModel = JsonSerializer.Deserialize<ImageEditModel>(sessionData);
                if (editModel == null || string.IsNullOrEmpty(editModel.ImagePath))
                {
                    return BadRequest("No image to download.");
                }

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", editModel.ImagePath.TrimStart('/'));
                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound("Image file not found.");
                }

                var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(fileBytes, "image/jpeg", "edited_image.jpg");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 