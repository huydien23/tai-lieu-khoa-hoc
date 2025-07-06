# Hướng dẫn Mở và Chạy Dự án trong Visual Studio

## Yêu cầu hệ thống
- **Visual Studio 2022** (Community/Professional/Enterprise)
- **.NET 8.0 SDK**
- **SQL Server** (LocalDB hoặc SQL Server Express)

## Các bước thực hiện

### 1. Mở dự án trong Visual Studio

1. **Mở Visual Studio 2022**
2. **File** → **Open** → **Project/Solution**
3. Chọn file `QuanLyTaiLieuKhoaHoc.sln` trong thư mục gốc
4. Visual Studio sẽ load toàn bộ dự án

### 2. Cấu hình Connection String

1. Mở file `appsettings.json`
2. Kiểm tra connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=HUYDIEN;Database=QuanLyTaiLieuKhoaHoc;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

**Lưu ý**: Nếu server SQL của bạn khác "HUYDIEN", hãy thay đổi tên server tương ứng.

### 3. Restore NuGet Packages

Visual Studio sẽ tự động restore packages, nhưng bạn có thể:
1. **Right-click** vào Solution
2. Chọn **Restore NuGet Packages**

### 4. Chạy Database Migration

1. Mở **Package Manager Console**: **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. Chạy lệnh:
```powershell
Update-Database
```

Hoặc sử dụng **Developer PowerShell**:
```powershell
dotnet ef database update
```

### 5. Build Solution

1. **Build** → **Build Solution** (Ctrl+Shift+B)
2. Đảm bảo không có lỗi build

### 6. Chạy ứng dụng

1. Chọn **QuanLyTaiLieuKhoaHoc.Web** làm Startup Project
2. Nhấn **F5** hoặc click nút **Start**
3. Browser sẽ tự động mở với địa chỉ `https://localhost:5001`

## Cấu trúc Project trong Visual Studio

```
QuanLyTaiLieuKhoaHoc.sln
└── QuanLyTaiLieuKhoaHoc.Web/
    ├── Controllers/           # MVC Controllers
    ├── Models/               # Entity Models & ViewModels
    ├── Views/                # Razor Views
    ├── Services/             # Business Logic Services
    ├── Data/                 # DbContext & Migrations
    ├── wwwroot/              # Static Files
    ├── Areas/                # Identity Area
    ├── appsettings.json      # Configuration
    └── Program.cs            # Entry Point
```

## Tính năng trong Visual Studio

### 1. IntelliSense & Code Completion
- Visual Studio sẽ cung cấp auto-complete cho tất cả các class, method
- Error highlighting và suggestions

### 2. Debugging
- Đặt breakpoints bằng cách click vào lề trái
- F5 để chạy với debug, Ctrl+F5 để chạy không debug

### 3. Entity Framework Tools
- **View** → **Other Windows** → **Package Manager Console**
- Các lệnh EF Core: `Add-Migration`, `Update-Database`, `Remove-Migration`

### 4. Database Connection
- **View** → **SQL Server Object Explorer**
- Kết nối và xem database trực tiếp

## Tài khoản Test

Sau khi chạy lần đầu, hệ thống tự động tạo các tài khoản mẫu:

| Vai trò | Email | Password |
|---------|-------|----------|
| Admin | admin@admin.com | Password123 |
| Giảng viên | gv@gv.com | Password123 |
| Sinh viên | sv@sv.com | Password123 |

## Troubleshooting

### Lỗi Connection String
```
Cannot open database "QuanLyTaiLieuKhoaHoc" requested by the login
```
**Giải pháp**: Kiểm tra tên SQL Server instance và quyền truy cập

### Lỗi Migration
```
Unable to create an object of type 'ApplicationDbContext'
```
**Giải pháp**: 
1. Rebuild solution
2. Chạy lại `Update-Database`

### Lỗi NuGet Packages
```
Package restore failed
```
**Giải pháp**:
1. **Tools** → **Options** → **NuGet Package Manager**
2. Clear All NuGet Cache(s)
3. Restore packages

### Lỗi SSL Certificate
```
The SSL connection could not be established
```
**Giải pháp**: Thêm `TrustServerCertificate=true` vào connection string

## Hot Reload & Live Updates

Visual Studio 2022 hỗ trợ Hot Reload:
- Thay đổi code C# và thấy kết quả ngay lập tức
- Chỉnh sửa Razor views và refresh browser

## Performance Profiling

1. **Debug** → **Performance Profiler**
2. Chọn loại profiling cần thiết
3. Phân tích hiệu suất ứng dụng

## Extensions khuyến nghị

1. **Web Essentials** - HTML/CSS/JS tools
2. **Productivity Power Tools** - Enhanced IDE experience
3. **CodeMaid** - Code cleanup and organization
4. **Git Extensions** - Advanced Git integration

---

**Lưu ý**: Đảm bảo Visual Studio đã được cập nhật lên version mới nhất để có trải nghiệm tốt nhất với .NET 8.0
