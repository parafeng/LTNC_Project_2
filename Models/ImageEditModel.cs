namespace MiniPhotoshop.Models
{
    public class ImageEditModel
    {
        public string? ImagePath { get; set; }
        public string? OriginalImagePath { get; set; }
        public List<EditOperation> EditHistory { get; set; } = new List<EditOperation>();
        public int CurrentHistoryIndex { get; set; } = -1;
    }

    public class EditOperation
    {
        public string Operation { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public string ResultImagePath { get; set; } = string.Empty;
    }

    public class ImageEditRequest
    {
        public string ImagePath { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
} 