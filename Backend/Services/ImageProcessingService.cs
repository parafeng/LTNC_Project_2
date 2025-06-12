using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MiniPhotoshop.Backend.Filters;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniPhotoshop.Backend.Services
{
    public class ImageProcessingService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly FileStorageService _fileStorageService;
        private readonly Dictionary<string, IImageFilter> _filters;
        
        // Dictionary để lưu trữ các bộ lọc đã áp dụng cho mỗi ảnh
        private readonly Dictionary<string, List<AppliedFilter>> _appliedFilters = new Dictionary<string, List<AppliedFilter>>();
        
        // Dictionary để lưu trữ ảnh gốc cho mỗi ảnh đã xử lý
        private readonly Dictionary<string, string> _originalImages = new Dictionary<string, string>();
        
        public ImageProcessingService(
            IWebHostEnvironment environment,
            ILogger<ImageProcessingService> logger,
            FileStorageService fileStorageService,
            IEnumerable<IImageFilter> filters)
        {
            _environment = environment;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _filters = filters.ToDictionary(f => f.Name);
            
            _logger.LogInformation("Đã đăng ký {0} bộ lọc", _filters.Count);
        }
        
        /// <summary>
        /// Áp dụng bộ lọc lên ảnh
        /// </summary>
        /// <param name="imagePath">Đường dẫn tương đối của ảnh gốc</param>
        /// <param name="filterName">Tên bộ lọc</param>
        /// <param name="parameters">Tham số cho bộ lọc</param>
        /// <returns>Đường dẫn tương đối của ảnh đã xử lý</returns>
        public async Task<string> ApplyFilterAsync(string imagePath, string filterName, Dictionary<string, object> parameters)
        {
            _logger.LogInformation("Áp dụng bộ lọc {0} cho ảnh {1}", filterName, imagePath);

            if (!_filters.TryGetValue(filterName, out var filter))
            {
                throw new ArgumentException($"Bộ lọc '{filterName}' không tồn tại");
            }
            
            // Chuyển đổi tham số từ chuỗi sang kiểu dữ liệu phù hợp
            var convertedParameters = ConvertParameters(parameters);

            // Đường dẫn đầy đủ đến file ảnh
            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));

            // Tạo tên file mới
            var newFileName = $"{Path.GetFileNameWithoutExtension(fullPath)}_filtered{Path.GetExtension(fullPath)}";
            var newFilePath = Path.Combine(_fileStorageService.UploadDirectory, newFileName);
            var newFullPath = Path.Combine(_environment.WebRootPath, newFilePath);

            // Tạo thư mục nếu chưa tồn tại
            var directory = Path.GetDirectoryName(newFullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Lưu thông tin về ảnh gốc
            var relativePath = $"/{newFilePath.Replace('\\', '/')}";
            if (!_originalImages.ContainsKey(relativePath))
            {
                // Nếu ảnh hiện tại đã là ảnh đã xử lý, sử dụng ảnh gốc của nó
                if (_originalImages.ContainsKey(imagePath))
                {
                    _originalImages[relativePath] = _originalImages[imagePath];
                }
                else
                {
                    _originalImages[relativePath] = imagePath;
                }
            }

            // Lưu thông tin về bộ lọc đã áp dụng
            if (!_appliedFilters.ContainsKey(relativePath))
            {
                _appliedFilters[relativePath] = new List<AppliedFilter>();
            }

            // Sao chép các bộ lọc từ ảnh trước nếu có
            if (_appliedFilters.ContainsKey(imagePath))
            {
                _appliedFilters[relativePath].AddRange(_appliedFilters[imagePath]);
            }

            // Thêm bộ lọc mới
            _appliedFilters[relativePath].Add(new AppliedFilter
            {
                FilterName = filterName,
                Parameters = convertedParameters
            });

            // Áp dụng tất cả các bộ lọc từ ảnh gốc
            using (var image = await Image.LoadAsync(Path.Combine(_environment.WebRootPath, _originalImages[relativePath].TrimStart('/'))))
            {
                // Áp dụng tất cả các bộ lọc theo thứ tự
                foreach (var appliedFilter in _appliedFilters[relativePath])
                {
                    if (_filters.TryGetValue(appliedFilter.FilterName, out var appliedFilterObj))
                    {
                        await appliedFilterObj.ApplyAsync(image, appliedFilter.Parameters);
                    }
                }
                
                await image.SaveAsync(newFullPath);
            }

            return relativePath;
        }
        
        /// <summary>
        /// Xem trước bộ lọc mà không lưu ảnh
        /// </summary>
        /// <param name="imagePath">Đường dẫn tương đối của ảnh gốc</param>
        /// <param name="filterName">Tên bộ lọc</param>
        /// <param name="parameters">Tham số cho bộ lọc</param>
        /// <returns>Ảnh đã xử lý</returns>
        public async Task<Image> PreviewFilterAsync(string imagePath, string filterName, Dictionary<string, object> parameters)
        {
            _logger.LogInformation("Xem trước bộ lọc {0} cho ảnh {1}", filterName, imagePath);

            if (!_filters.TryGetValue(filterName, out var filter))
            {
                throw new ArgumentException($"Bộ lọc '{filterName}' không tồn tại");
            }
            
            // Chuyển đổi tham số từ chuỗi sang kiểu dữ liệu phù hợp
            var convertedParameters = ConvertParameters(parameters);

            // Xác định ảnh gốc
            string originalImagePath = imagePath;
            if (_originalImages.ContainsKey(imagePath))
            {
                originalImagePath = _originalImages[imagePath];
            }

            // Đường dẫn đầy đủ đến file ảnh gốc
            var fullPath = Path.Combine(_environment.WebRootPath, originalImagePath.TrimStart('/'));

            // Tạo danh sách các bộ lọc cần áp dụng
            var filtersToApply = new List<AppliedFilter>();
            
            // Thêm các bộ lọc đã áp dụng trước đó
            if (_appliedFilters.ContainsKey(imagePath))
            {
                filtersToApply.AddRange(_appliedFilters[imagePath]);
            }
            
            // Thêm bộ lọc mới đang xem trước
            filtersToApply.Add(new AppliedFilter
            {
                FilterName = filterName,
                Parameters = convertedParameters
            });

            // Áp dụng tất cả các bộ lọc
            var image = await Image.LoadAsync(fullPath);
            foreach (var appliedFilter in filtersToApply)
            {
                if (_filters.TryGetValue(appliedFilter.FilterName, out var appliedFilterObj))
                {
                    await appliedFilterObj.ApplyAsync(image, appliedFilter.Parameters);
                }
            }
            
            return image;
        }
        
        /// <summary>
        /// Chuyển đổi tham số từ chuỗi sang kiểu dữ liệu phù hợp
        /// </summary>
        private Dictionary<string, object> ConvertParameters(Dictionary<string, object> parameters)
        {
            var result = new Dictionary<string, object>();
            
            foreach (var param in parameters)
            {
                if (param.Value is string stringValue)
                {
                    // Thử chuyển đổi sang số thực
                    if (float.TryParse(stringValue, out float floatValue))
                    {
                        result[param.Key] = floatValue;
                    }
                    // Thử chuyển đổi sang số nguyên
                    else if (int.TryParse(stringValue, out int intValue))
                    {
                        result[param.Key] = intValue;
                    }
                    // Giữ nguyên giá trị chuỗi
                    else
                    {
                        result[param.Key] = stringValue;
                    }
                }
                else
                {
                    // Giữ nguyên giá trị không phải chuỗi
                    result[param.Key] = param.Value;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Đặt lại ảnh về trạng thái gốc
        /// </summary>
        /// <param name="imagePath">Đường dẫn tương đối của ảnh</param>
        /// <returns>Đường dẫn tương đối của ảnh gốc</returns>
        public string ResetImage(string imagePath)
        {
            if (_originalImages.ContainsKey(imagePath))
            {
                var originalPath = _originalImages[imagePath];
                _appliedFilters.Remove(imagePath);
                return originalPath;
            }
            
            return imagePath;
        }
        
        /// <summary>
        /// Lấy danh sách các bộ lọc đã áp dụng cho ảnh
        /// </summary>
        /// <param name="imagePath">Đường dẫn tương đối của ảnh</param>
        /// <returns>Danh sách các bộ lọc đã áp dụng</returns>
        public List<AppliedFilter> GetAppliedFilters(string imagePath)
        {
            if (_appliedFilters.ContainsKey(imagePath))
            {
                return _appliedFilters[imagePath];
            }
            
            return new List<AppliedFilter>();
        }
        
        /// <summary>
        /// Lấy danh sách các bộ lọc có sẵn
        /// </summary>
        public IEnumerable<FilterInfo> GetAvailableFilters()
        {
            return _filters.Values.Select(f => new FilterInfo
            {
                Name = f.Name,
                Description = f.Description
            });
        }
    }
    
    public class FilterInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    
    public class AppliedFilter
    {
        public string FilterName { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
} 