 # Mini Photoshop Online

Ứng dụng web xử lý ảnh cơ bản được xây dựng bằng ASP.NET Core và ImageSharp.

## Tính năng

### Chức năng cơ bản:
- ✅ Upload ảnh (JPG, PNG, GIF)
- ✅ Xoay ảnh (90 độ)
- ✅ Crop ảnh
- ✅ Resize ảnh

### Tính năng nâng cao:
- ✅ Điều chỉnh độ sáng (Brightness)
- ✅ Điều chỉnh độ tương phản (Contrast)
- ✅ Đảo ngược màu (Invert)
- ✅ Undo/Redo
- ✅ Export ảnh sau chỉnh sửa

## Công nghệ sử dụng

- **ASP.NET Core 8.0**: Framework web chính
- **ImageSharp**: Thư viện xử lý ảnh mạnh mẽ
- **Bootstrap 5**: UI framework responsive
- **Font Awesome**: Icons
- **jQuery**: JavaScript library

## Cách chạy ứng dụng

### Yêu cầu hệ thống:
- .NET 8.0 SDK hoặc cao hơn
- Trình duyệt web hiện đại

### Các bước chạy:

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Build project:**
   ```bash
   dotnet build
   ```

3. **Chạy ứng dụng:**
   ```bash
   dotnet run
   ```

4. **Mở trình duyệt và truy cập:**
   ```
   https://localhost:5001
   hoặc
   http://localhost:5000
   ```

## Cách sử dụng

1. **Upload ảnh**: Click vào vùng "Click để chọn ảnh" để upload ảnh từ máy tính
2. **Chỉnh sửa cơ bản**:
   - Xoay 90°: Click nút "Xoay 90°"
   - Resize: Click "Resize" và nhập kích thước mới
   - Crop: Click "Crop" và nhập tọa độ cắt
3. **Áp dụng hiệu ứng**:
   - Kéo thanh Brightness để điều chỉnh độ sáng
   - Kéo thanh Contrast để điều chỉnh độ tương phản
   - Click "Invert" để đảo ngược màu
4. **Undo/Redo**: Sử dụng các nút Undo/Redo để hoàn tác hoặc làm lại
5. **Export**: Click "Tải xuống" để tải ảnh đã chỉnh sửa

## Cấu trúc project

```
MiniPhotoshop/
├── Controllers/
│   └── HomeController.cs          # Controller chính xử lý requests
├── Models/
│   └── ImageEditModel.cs          # Models cho session và edit operations
├── Services/
│   └── ImageProcessingService.cs  # Service xử lý ảnh với ImageSharp
├── Views/
│   ├── Home/
│   │   └── Index.cshtml           # Giao diện chính
│   ├── Shared/
│   │   └── _Layout.cshtml         # Layout chung
│   └── _ViewStart.cshtml
├── wwwroot/
│   └── uploads/                   # Thư mục lưu ảnh upload
├── Program.cs                     # Entry point
├── MiniPhotoshop.csproj          # Project file
└── README.md                     # File hướng dẫn
```

## Session Management

Ứng dụng sử dụng session để lưu trữ:
- Đường dẫn ảnh hiện tại
- Lịch sử các thao tác chỉnh sửa
- Hỗ trợ undo/redo

## Lưu ý

- Ảnh upload sẽ được lưu trong thư mục `wwwroot/uploads/`
- Session timeout: 30 phút
- Các ảnh tạm thời sẽ được tự động dọn dẹp
- Ứng dụng hỗ trợ các định dạng ảnh phổ biến: JPG, PNG, GIF

## Troubleshooting

### Lỗi thường gặp:

1. **"No image session found"**: Session đã hết hạn, vui lòng upload lại ảnh
2. **Upload failed**: Kiểm tra định dạng file và kích thước ảnh
3. **Processing error**: Thử refresh trang và upload lại ảnh

### Performance:

- Với ảnh lớn, quá trình xử lý có thể mất vài giây
- Khuyến nghị sử dụng ảnh có kích thước hợp lý (< 5MB) để có hiệu suất tốt nhất 