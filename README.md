# MiniPhotoshop - Ứng dụng chỉnh sửa ảnh trực tuyến

## Giới thiệu
MiniPhotoshop là ứng dụng web xử lý ảnh cơ bản được xây dựng bằng ASP.NET Core và ImageSharp, cho phép người dùng thực hiện các thao tác chỉnh sửa ảnh như xoay, thay đổi kích thước, cắt ảnh và áp dụng các bộ lọc.

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

## Yêu cầu hệ thống
- .NET SDK 9.0
- Visual Studio 2022 hoặc Visual Studio Code
- Trình duyệt web hiện đại (Chrome, Firefox, Edge)

## Cài đặt

### Bước 1: Cài đặt .NET SDK
1. Truy cập trang tải .NET SDK: https://dotnet.microsoft.com/download
2. Tải và cài đặt .NET SDK 9.0 phù hợp với hệ điều hành của bạn
3. Sau khi cài đặt, kiểm tra phiên bản .NET bằng lệnh:
   ```
   dotnet --version
   ```

### Bước 2: Tải mã nguồn
1. Tải mã nguồn từ kho lưu trữ hoặc giải nén file nếu bạn đã có
2. Mở terminal hoặc command prompt và di chuyển đến thư mục dự án

## Chạy ứng dụng

### Sử dụng Command Line
1. Di chuyển đến thư mục gốc của dự án (nơi chứa file MiniPhotoshop.csproj)
2. Chạy lệnh sau để khôi phục các gói phụ thuộc:
   ```
   dotnet restore
   ```
3. Xây dựng dự án:
   ```
   dotnet build
   ```
4. Chạy ứng dụng:
   ```
   dotnet run
   ```
5. Mở trình duyệt web và truy cập địa chỉ: http://localhost:5000

### Sử dụng Visual Studio
1. Mở file MiniPhotoshop.sln trong Visual Studio
2. Nhấn F5 hoặc nhấn nút "Start" để chạy ứng dụng
3. Trình duyệt web sẽ tự động mở với địa chỉ của ứng dụng

## Cách sử dụng ứng dụng

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

## Xử lý sự cố

### Lỗi HTTPS
Nếu bạn gặp lỗi liên quan đến HTTPS, hãy sử dụng HTTP thay thế bằng cách truy cập: http://localhost:5000

### Lỗi .NET SDK không tương thích
Nếu dự án yêu cầu phiên bản .NET khác với phiên bản đã cài đặt:
1. Mở file MiniPhotoshop.csproj
2. Thay đổi dòng `<TargetFramework>netX.0</TargetFramework>` thành phiên bản .NET phù hợp
3. Lưu file và chạy lại ứng dụng

### Lỗi màn hình đen khi sử dụng công cụ
Nếu bạn gặp lỗi màn hình đen khi sử dụng các công cụ chỉnh sửa ảnh, hãy đảm bảo rằng:
1. Ảnh được tải lên đúng định dạng (JPG, PNG, GIF)
2. Kích thước ảnh không quá lớn (khuyến nghị < 5MB)
3. Làm mới trang và thử lại

### Các lỗi thường gặp khác:
1. **"No image session found"**: Session đã hết hạn, vui lòng upload lại ảnh
2. **Upload failed**: Kiểm tra định dạng file và kích thước ảnh
3. **Processing error**: Thử refresh trang và upload lại ảnh

## Cấu trúc dự án

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
│   ├── css/                       # CSS files
│   ├── js/                        # JavaScript files
│   └── uploads/                   # Thư mục lưu ảnh upload
├── Program.cs                     # Entry point
├── MiniPhotoshop.csproj           # Project file
└── README.md                      # File hướng dẫn
```

## Công nghệ sử dụng

- **ASP.NET Core**: Framework web chính
- **ImageSharp**: Thư viện xử lý ảnh mạnh mẽ
- **Bootstrap 5**: UI framework responsive
- **Font Awesome**: Icons
- **jQuery**: JavaScript library

## Lưu ý

- Ảnh upload sẽ được lưu trong thư mục `wwwroot/uploads/`
- Session timeout: 30 phút
- Các ảnh tạm thời sẽ được tự động dọn dẹp
- Ứng dụng hỗ trợ các định dạng ảnh phổ biến: JPG, PNG, GIF
- Với ảnh lớn, quá trình xử lý có thể mất vài giây

## Liên hệ hỗ trợ
Nếu bạn gặp vấn đề khi cài đặt hoặc sử dụng ứng dụng, vui lòng liên hệ với người phát triển để được hỗ trợ. 