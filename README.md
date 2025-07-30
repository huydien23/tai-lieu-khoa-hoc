# Hệ thống Quản lý Tài liệu Khoa học

## Mô tả dự án

Hệ thống Quản lý Tài liệu Khoa học là một ứng dụng web được phát triển bằng ASP.NET Core MVC, phục vụ cho việc quản lý và tìm kiếm tài liệu khoa học tại Trường Đại học Nam Cần Thơ.

## Tính năng chính

### 🔐 Quản lý người dùng
- **Sinh viên**: Xem, tìm kiếm, mượn trả tài liệu
- **Giảng viên**: Quản lý tài liệu cá nhân, hướng dẫn sinh viên
- **Thủ thư**: Quản lý toàn bộ hệ thống, phê duyệt yêu cầu

### 📚 Quản lý tài liệu
- Thêm, sửa, xóa tài liệu
- Phân loại theo chuyên ngành và loại tài liệu
- Tìm kiếm nâng cao với nhiều tiêu chí
- Upload và quản lý file tài liệu

### 📋 Quản lý mượn trả
- Tạo phiếu mượn trả
- Theo dõi tình trạng mượn trả
- Thông báo tài liệu quá hạn
- Lịch sử mượn trả

### 📊 Thống kê và báo cáo
- Thống kê tài liệu theo chuyên ngành
- Báo cáo mượn trả
- Dashboard cho từng vai trò

## Thiết kế giao diện

### 🎨 Header mới
- **Top Bar**: Hiển thị thông tin trường đại học và thời gian thực
- **Navigation Bar**: Menu chính với gradient xanh hiện đại
- **Brand**: Logo và tên hệ thống với hiệu ứng hover
- **Responsive**: Tối ưu cho mobile và tablet

### 🦶 Footer mới
- **4 cột thông tin**:
  - Thông tin trường đại học và liên hệ
  - Liên kết nhanh
  - Chuyên ngành
  - Hỗ trợ và mạng xã hội
- **Footer Bottom**: Copyright và thống kê real-time
- **Hiệu ứng**: Animation và hover effects

### 🎯 Tính năng thiết kế
- **Gradient Backgrounds**: Sử dụng màu xanh gradient hiện đại
- **Glass Morphism**: Hiệu ứng trong suốt với backdrop-filter
- **Smooth Animations**: Transition mượt mà cho tất cả elements
- **Responsive Design**: Tối ưu cho mọi kích thước màn hình
- **Accessibility**: Hỗ trợ keyboard navigation và screen readers

## Công nghệ sử dụng

### Backend
- **ASP.NET Core 7.0**: Framework chính
- **Entity Framework Core**: ORM cho database
- **Identity Framework**: Authentication và Authorization
- **SQL Server**: Database

### Frontend
- **Bootstrap 5**: CSS Framework
- **Font Awesome 6**: Icons
- **jQuery**: JavaScript library
- **Custom CSS**: Thiết kế riêng với CSS Variables

### Database
- **SQL Server**: Database chính
- **Code First**: Migration approach
- **Seed Data**: Dữ liệu mẫu

## Cấu trúc dự án

```
QL-Tai-Lieu-Khoa-Hoc/
├── src/
│   └── QuanLyTaiLieuKhoaHoc.Web/
│       ├── Controllers/          # Controllers cho từng vai trò
│       ├── Models/              # Entity models và ViewModels
│       ├── Views/               # Razor views
│       ├── Services/            # Business logic services
│       ├── Data/                # Database context
│       └── wwwroot/             # Static files (CSS, JS, Images)
├── README.md
└── QuanLyTaiLieuKhoaHoc.sln
```

## Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 7.0 SDK
- SQL Server 2019+
- Visual Studio 2022 hoặc VS Code

### Các bước cài đặt

1. **Clone repository**
   ```bash
   git clone [repository-url]
   cd QL-Tai-Lieu-Khoa-Hoc
   ```

2. **Cài đặt dependencies**
   ```bash
   dotnet restore
   ```

3. **Cấu hình database**
   - Cập nhật connection string trong `appsettings.json`
   - Chạy migrations:
   ```bash
   dotnet ef database update
   ```

4. **Chạy ứng dụng**
   ```bash
   dotnet run
   ```

5. **Truy cập ứng dụng**
   - URL: `https://localhost:7001`
   - Tài khoản mặc định: `admin@nctu.edu.vn` / `Admin123!`

## Tài khoản mẫu

### Thủ thư
- Email: `admin@nctu.edu.vn`
- Password: `Admin123!`

### Giảng viên
- Email: `lecturer@nctu.edu.vn`
- Password: `Lecturer123!`

### Sinh viên
- Email: `student@nctu.edu.vn`
- Password: `Student123!`

## Tính năng mới trong thiết kế

### 🆕 Header Enhancements
- **Top Bar**: Thông tin trường đại học và thời gian thực
- **Enhanced Brand**: Logo với hiệu ứng hover và subtitle
- **Improved Navigation**: Menu dropdown và responsive design
- **Real-time Clock**: Hiển thị ngày giờ hiện tại

### 🆕 Footer Enhancements
- **Multi-column Layout**: 4 cột thông tin chi tiết
- **Contact Information**: Địa chỉ, điện thoại, email
- **Social Links**: Facebook, YouTube, LinkedIn
- **Real-time Stats**: Số lượng người dùng và tài liệu
- **Responsive Design**: Tối ưu cho mobile

### 🆕 Visual Improvements
- **Gradient Backgrounds**: Màu xanh gradient hiện đại
- **Glass Morphism**: Hiệu ứng trong suốt
- **Smooth Animations**: Transition mượt mà
- **Hover Effects**: Tương tác người dùng tốt hơn
- **Accessibility**: Hỗ trợ accessibility standards

## Đóng góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## License

Dự án này được phát triển cho Trường Đại học Nam Cần Thơ.

## Liên hệ

- **Trường Đại học Nam Cần Thơ**
- **Email**: thuvien@nctu.edu.vn
- **Điện thoại**: 0292.3.xxx.xxx
- **Địa chỉ**: 123 Đường ABC, Quận XYZ, TP. Cần Thơ

---

© 2025 - Trường Đại học Nam Cần Thơ. Tất cả quyền được bảo lưu.
