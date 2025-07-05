# 📚 HỆ THỐNG QUẢN LÝ TÀI LIỆU KHOA HỌC

> **Ứng dụng quản lý thư viện tài liệu khoa học hiện đại, thân thiện và dễ sử dụng**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-orange.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple.svg)](https://getbootstrap.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-red.svg)](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

---

## 🎯 TỔNG QUAN DỰ ÁN

**Hệ thống Quản lý Tài liệu Khoa học** là ứng dụng web được thiết kế để hỗ trợ thư viện và các tổ chức giáo dục quản lý, tìm kiếm và thống kê tài liệu khoa học một cách hiệu quả trên nền tảng web.

### ✨ ĐIỂM NỔI BẬT
- 🌟 **Giao diện đẹp mắt**: Thiết kế hiện đại, responsive với Bootstrap 5
- 🔍 **Tìm kiếm thông minh**: Hỗ trợ tìm kiếm đa tiêu chí với AJAX live search
- 📊 **Báo cáo trực quan**: Chart.js biểu đồ tương tác và thống kê chi tiết
- 🔐 **Phân quyền rõ ràng**: Hệ thống ASP.NET Core Identity với role-based access
- 📱 **Responsive**: Hoạt động mượt mà trên mọi thiết bị (desktop, tablet, mobile)
- 🇻🇳 **100% Tiếng Việt**: Hoàn toàn bằng tiếng Việt, dễ sử dụng
- 🌐 **Multi-browser**: Tương thích với Chrome, Firefox, Safari, Edge

---

## 📋 CHỨC NĂNG CHÍNH

### 🔐 HỆ THỐNG PHÂN QUYỀN

| Vai trò | Quyền hạn |
|---------|-----------|
| **🏛️ Thủ thư** | • Quản lý tài liệu (thêm/sửa/xóa)<br>• Xử lý mượn/trả<br>• Quản lý in sao/bán lại<br>• Xem tất cả báo cáo<br>• Quản lý người dùng |
| **👨‍🏫 Giảng viên** | • Xem và tìm kiếm tài liệu<br>• Xem báo cáo thống kê<br>• Tra cứu thông tin |
| **🎓 Sinh viên** | • Xem và tìm kiếm tài liệu<br>• Xem thống kê cơ bản |

### 📚 QUẢN LÝ TÀI LIỆU

#### **1. Bài báo khoa học**
- 📄 Tiêu đề, tạp chí/hội nghị xuất bản
- 📅 Ngày công bố
- 🔗 Mã DOI, ISSN
- 🌍 Cấp độ (Quốc tế, Trong nước, Tạp chí chuyên ngành)

#### **2. Đề tài nghiên cứu khoa học**
- 📝 Tên đề tài, mã số đề tài
- 🏢 Cấp (Trường, Sở, Quốc gia...)
- ⏰ Thời gian thực hiện
- 🏛️ Cơ quan chủ trì, chủ nhiệm đề tài

#### **3. Giáo trình - Tài liệu giảng dạy**
- 📖 Tên giáo trình, tác giả/đồng tác giả
- 🎓 Môn học liên quan
- 🏢 Đơn vị phát hành, năm xuất bản
- 📊 Số tín chỉ

### 🔍 TÌM KIẾM & TRA CỨU
- **Tìm kiếm nhanh**: Theo tên, từ khóa
- **Tìm kiếm nâng cao**: Theo loại tài liệu, chuyên ngành, tác giả, nhà xuất bản, năm xuất bản, cấp độ
- **Gợi ý thông minh**: Autocomplete và search suggestions
- **Lọc kết quả**: Multiple filters và sorting options

### 📊 BÁO CÁO & THỐNG KÊ

#### **📈 Dashboard Overview**
- Tổng quan số lượng tài liệu
- Xu hướng mượn/trả theo thời gian
- Top tài liệu được quan tâm nhất

#### **📋 Báo cáo chi tiết**
- **Theo loại tài liệu**: Phân bố bài báo, đề tài, giáo trình
- **Theo danh mục**: Thống kê theo chuyên ngành
- **Theo hoạt động**: Số lượt mượn, in sao, bán lại
- **Theo tác giả/NXB**: Ranking các tác giả, nhà xuất bản được yêu thích

#### **🎯 Báo cáo chuyên biệt**
- Tài liệu quá hạn chưa trả
- Tài liệu chưa từng được mượn
- Thống kê theo khoảng thời gian tùy chỉnh
- Export báo cáo ra Excel/PDF

### 💼 QUẢN LÝ NGHIỆP VỤ
- **📥 Nhập tài liệu mới**: Ghi nhận tài liệu vào kho
- **📚 Mượn/Trả tài liệu**: Workflow hoàn chỉnh cho thủ thư
- **🖨️ In sao tài liệu**: Quản lý việc photocopy, in ra giấy/đĩa
- **💰 Bán lại tài liệu**: Ghi nhận việc bán tài liệu đã in
- **📎 File đính kèm**: Upload/Download PDF, Word, hình ảnh

---

## 🏗️ KIẾN TRÚC HỆ THỐNG

### 📐 Clean Architecture (Web)
```
┌─────────────────────────────────────┐
│        🌐 Presentation Layer        │
│    (ASP.NET Core MVC + Razor)       │
├─────────────────────────────────────┤
│          ⚡ Application Layer        │
│     (Services, Commands, Queries)   │
├─────────────────────────────────────┤
│            🏛️ Domain Layer           │
│    (Entities, Business Rules)       │
├─────────────────────────────────────┤
│         🔧 Infrastructure Layer     │
│   (Database, Files, External APIs)  │
└─────────────────────────────────────┘
```

### 🛠️ CÔNG NGHỆ SỬ DỤNG

| Lớp | Công nghệ | Mục đích |
|-----|-----------|----------|
| **Frontend** | ASP.NET Core MVC + Razor Pages | Giao diện web |
| **UI Framework** | Bootstrap 5 + jQuery | Responsive design & interactions |
| **Charts** | Chart.js | Biểu đồ tương tác |
| **Backend** | .NET 8 + C# 12 | Logic nghiệp vụ |
| **Database** | Entity Framework Core + SQL Server LocalDB | Lưu trữ dữ liệu |
| **Authentication** | ASP.NET Core Identity | Quản lý người dùng & phân quyền |
| **Mapping** | AutoMapper | Chuyển đổi objects |
| **Validation** | FluentValidation + jQuery Validation | Kiểm tra dữ liệu client & server |
| **File Upload** | IFormFile + Azure Blob (tùy chọn) | Quản lý file đính kèm |
| **Logging** | Serilog + Seq | Ghi log hệ thống |
| **Testing** | xUnit + Moq + Selenium | Unit & Integration testing |

---

## 📁 CẤU TRÚC DỰ ÁN

```
📦 ScientificDocumentManagement/
├── 📂 src/
│   ├── 🏛️ ScientificDocumentManagement.Domain/
│   │   ├── 📂 Entities/           # Domain entities
│   │   ├── 📂 ValueObjects/       # Value objects  
│   │   ├── 📂 Enums/             # Enumerations
│   │   ├── 📂 Interfaces/        # Domain interfaces
│   │   └── 📂 Services/          # Domain services
│   │
│   ├── ⚡ ScientificDocumentManagement.Application/
│   │   ├── 📂 Commands/          # CQRS Commands
│   │   ├── 📂 Queries/           # CQRS Queries  
│   │   ├── 📂 DTOs/              # Data transfer objects
│   │   ├── 📂 Services/          # Application services
│   │   ├── 📂 Validators/        # FluentValidation rules
│   │   └── 📂 Mappings/          # AutoMapper profiles
│   │
│   ├── 🔧 ScientificDocumentManagement.Infrastructure/
│   │   ├── 📂 Data/              # EF Core context & configs
│   │   ├── 📂 Repositories/      # Repository implementations
│   │   ├── 📂 Services/          # Infrastructure services
│   │   ├── 📂 Migrations/        # Database migrations
│   │   └── 📂 Files/             # File storage service
│   │
│   ├── 🌐 ScientificDocumentManagement.Web/
│   │   ├── 📂 Controllers/       # MVC Controllers
│   │   ├── 📂 Views/             # Razor Views
│   │   ├── 📂 Areas/             # Area-based organization
│   │   │   ├── 📂 Admin/         # Admin area (Thủ thư)
│   │   │   ├── 📂 Lecturer/      # Lecturer area (Giảng viên)
│   │   │   └── 📂 Student/       # Student area (Sinh viên)
│   │   ├── 📂 Models/            # View models
│   │   ├── 📂 wwwroot/           # Static files
│   │   │   ├── 📂 css/           # Stylesheets
│   │   │   ├── 📂 js/            # JavaScript files
│   │   │   ├── 📂 images/        # Images & icons
│   │   │   └── 📂 uploads/       # User uploaded files
│   │   ├── 📂 Filters/           # Action filters
│   │   ├── 📂 Middlewares/       # Custom middlewares
│   │   └── 📂 Hubs/              # SignalR hubs (real-time)
│   │
│   └── 🔗 ScientificDocumentManagement.Shared/
│       ├── 📂 Constants/         # Application constants
│       ├── 📂 Extensions/        # Extension methods
│       └── 📂 Helpers/           # Utility helpers
│
├── 📂 tests/
│   ├── 🧪 UnitTests/             # Unit tests
│   ├── 🔧 IntegrationTests/      # Integration tests
│   └── 🌐 Web.Tests/             # Selenium UI tests
│
├── 📂 docs/
│   ├── 📋 database-design.md     # Database schema
│   ├── 🎨 ui-mockups/            # UI design mockups
│   ├── 📊 use-cases.md           # Use case scenarios
│   └── 🚀 deployment.md          # Deployment guide
│
├── 📂 scripts/
│   ├── 🗄️ database-setup.sql     # Database initialization
│   └── 📊 sample-data.sql        # Sample data for testing
│
└── 📋 README.md                   # Tài liệu này
```

---

## 🚀 HƯỚNG DẪN CÀI ĐẶT

### 📋 YÊU CẦU HỆ THỐNG
- **Operating System**: Windows 10/11, macOS, Linux
- **.NET Runtime**: .NET 8.0 hoặc mới hơn
- **Database**: SQL Server LocalDB hoặc SQL Server Express
- **Web Browser**: Chrome 90+, Firefox 88+, Safari 14+, Edge 90+
- **Memory**: Tối thiểu 4GB RAM
- **Storage**: 1GB dung lượng trống

### 🔧 CÀI ĐẶT PHÁT TRIỂN

1. **Clone repository**
   ```bash
   git clone https://github.com/your-repo/scientific-document-management.git
   cd scientific-document-management
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Setup database**
   ```bash
   dotnet ef database update --project src/ScientificDocumentManagement.Infrastructure
   ```

4. **Build solution**
   ```bash
   dotnet build
   ```

5. **Run web application**
   ```bash
   dotnet run --project src/ScientificDocumentManagement.Web
   ```

6. **Mở trình duyệt**
   ```
   Truy cập: https://localhost:5001 hoặc http://localhost:5000
   ```

### 👤 TẠO TÀI KHOẢN MẶC ĐỊNH

Hệ thống sẽ tự động tạo các tài khoản mặc định:

| Tên đăng nhập | Mật khẩu | Vai trò |
|---------------|----------|---------|
| `thuthu` | `123456` | Thủ thư |
| `giangvien` | `123456` | Giảng viên |
| `sinhvien` | `123456` | Sinh viên |

> ⚠️ **Lưu ý**: Đổi mật khẩu ngay sau lần đăng nhập đầu tiên!

---

## 📖 HƯỚNG DẪN SỬ DỤNG

### 🏠 MÀNG HÌNH CHÍNH

1. **Đăng nhập**: Chọn vai trò và nhập thông tin đăng nhập
2. **Dashboard**: Xem tổng quan thống kê và thông tin quan trọng
3. **Menu điều hướng**: Truy cập các chức năng theo phân quyền

### 📚 QUẢN LÝ TÀI LIỆU (Dành cho Thủ thư)

#### **Thêm tài liệu mới**
1. Vào menu **"Quản lý tài liệu"** → **"Thêm mới"**
2. Chọn loại tài liệu (Bài báo/Đề tài/Giáo trình)
3. Điền đầy đủ thông tin theo form
4. Upload file đính kèm (nếu có)
5. Nhấn **"Lưu"** để hoàn tất

#### **Chỉnh sửa tài liệu**
1. Tìm tài liệu cần sửa
2. Nhấn nút **"Chỉnh sửa"** 
3. Cập nhật thông tin
4. Nhấn **"Cập nhật"**

#### **Xóa tài liệu**
1. Chọn tài liệu cần xóa
2. Nhấn nút **"Xóa"**
3. Xác nhận trong hộp thoại

### 🔍 TÌM KIẾM TÀI LIỆU

#### **Tìm kiếm nhanh**
- Gõ từ khóa vào ô **"Tìm kiếm"**
- Hệ thống sẽ gợi ý kết quả phù hợp
- Nhấn Enter hoặc click **"Tìm"**

#### **Tìm kiếm nâng cao**
1. Nhấn **"Tìm kiếm nâng cao"**
2. Chọn các tiêu chí lọc:
   - Loại tài liệu
   - Chuyên ngành
   - Tác giả
   - Năm xuất bản
   - Cấp độ
3. Nhấn **"Áp dụng bộ lọc"**

### 📊 XEM BÁO CÁO & THỐNG KÊ

1. Vào menu **"Báo cáo & Thống kê"**
2. Chọn loại báo cáo muốn xem
3. Chọn khoảng thời gian (nếu cần)
4. Nhấn **"Tạo báo cáo"**
5. Có thể xuất ra Excel/PDF

### 📚 MƯỢN/TRẢ TÀI LIỆU (Dành cho Thủ thư)

#### **Cho mượn tài liệu**
1. Tìm tài liệu cần cho mượn
2. Nhấn **"Cho mượn"**
3. Nhập thông tin người mượn
4. Chọn ngày hẹn trả
5. Xác nhận cho mượn

#### **Nhận trả tài liệu**
1. Vào **"Quản lý mượn trả"**
2. Tìm giao dịch mượn
3. Nhấn **"Nhận trả"**
4. Kiểm tra tình trạng tài liệu
5. Xác nhận hoàn tất

---

## 🎨 GIAO DIỆN NGƯỜI DÙNG

### 🌈 THIẾT KẾ

- **CSS Framework**: Bootstrap 5 với custom theme
- **Color Scheme**: Material Design với màu chủ đạo xanh dương nhẹ nhàng
- **Typography**: Roboto cho tiêu đề, Inter cho nội dung
- **Icons**: Bootstrap Icons + Font Awesome 6
- **Layout**: CSS Grid + Flexbox responsive design
- **Animations**: CSS transitions và JavaScript animations mượt mà
- **Dark Mode**: Hỗ trợ chế độ tối/sáng

### 📱 RESPONSIVE DESIGN

Giao diện tự động điều chỉnh theo:
- **Mobile First**: Thiết kế từ mobile lên desktop
- **Breakpoints**: 576px, 768px, 992px, 1200px, 1400px
- **Touch Friendly**: Buttons và controls phù hợp với touch
- **Cross-browser**: Tương thích với tất cả trình duyệt hiện đại
- **Performance**: Optimized loading và lazy loading images

### ♿ ACCESSIBILITY

- **WCAG 2.1 AA compliance**: Tuân thủ tiêu chuẩn accessibility
- **Keyboard navigation**: Điều hướng bằng phím
- **Screen reader support**: Hỗ trợ đọc màn hình
- **High contrast mode**: Chế độ tương phản cao
- **Font scaling**: Phóng to/thu nhỏ font chữ
- **Focus indicators**: Highlight rõ ràng khi focus
- **ARIA labels**: Semantic markup cho accessibility

---

## 📊 CƠ SỞ DỮ LIỆU

### 🗄️ SCHEMA CHÍNH

```sql
-- Bảng chính
Users              -- Người dùng hệ thống
Authors            -- Tác giả
Publishers         -- Nhà xuất bản
Documents          -- Tài liệu (TPH inheritance)
DocumentFiles      -- File đính kèm
Categories         -- Danh mục chuyên ngành

-- Bảng quan hệ
DocumentAuthors    -- Many-to-many: Tài liệu ↔ Tác giả
BorrowTransactions -- Giao dịch mượn/trả
PrintTransactions  -- Giao dịch in sao/bán

-- Bảng hệ thống
AuditLogs         -- Log hoạt động
SystemSettings    -- Cài đặt hệ thống
```

### 🔐 SECURITY FEATURES

- **Password hashing**: BCrypt với salt
- **SQL Injection prevention**: EF Core parameterized queries
- **XSS protection**: Input sanitization
- **File upload security**: Type validation và virus scanning
- **Session management**: Secure session handling
- **Audit logging**: Đầy đủ logs cho mọi thao tác

---

## 🧪 TESTING

### 📝 TEST STRATEGY

- **Unit Tests**: Minimum 80% code coverage với xUnit
- **Integration Tests**: Database và API testing với WebApplicationFactory
- **UI Tests**: Selenium automation tests cho user workflows
- **Performance Tests**: Load testing với NBomber
- **Security Tests**: OWASP security testing
- **Accessibility Tests**: Pa11y automation testing

### 🏃‍♂️ CHẠY TESTS

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/UnitTests/

# Run Selenium tests
dotnet test tests/Web.Tests/ --filter Category=UI
```

---

## 🚀 DEPLOYMENT

### 📦 BUILD PRODUCTION

```bash
# Build release version
dotnet publish -c Release -o ./publish

# Deploy to IIS
# Copy files từ ./publish đến IIS wwwroot

# Deploy to Docker
docker build -t scientific-document-management .
docker run -p 80:80 scientific-document-management

# Deploy to Azure App Service
# Sử dụng Azure DevOps hoặc GitHub Actions
```

### 💾 DATABASE DEPLOYMENT

1. **Backup existing data** (nếu upgrade)
2. **Run migrations**: `dotnet ef database update`
3. **Seed initial data**: Chạy scripts/sample-data.sql
4. **Verify integrity**: Run database consistency checks

### 🔄 UPDATE PROCESS

1. Backup database và files
2. Stop web application/IIS
3. Deploy new version files
4. Run database migrations
5. Clear cache và restart application
6. Verify functionality
7. Monitor logs

---

## 🐛 TROUBLESHOOTING

### ❓ CÁC LỖI THƯỜNG GẶP

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-------------|-----------|
| **"Không kết nối được database"** | SQL Server LocalDB chưa cài | Cài Visual Studio hoặc SQL Server Express |
| **"File không upload được"** | Không đủ quyền ghi file | Kiểm tra permissions thư mục wwwroot/uploads |
| **"Lỗi đăng nhập"** | Sai username/password | Dùng tài khoản mặc định hoặc reset |
| **"Website không load"** | .NET Runtime thiếu | Cài .NET 8 Runtime |
| **"HTTPS certificate error"** | SSL certificate không hợp lệ | Sử dụng dotnet dev-certs https --trust |

### 📞 HỖ TRỢ

- **📧 Email**: support@scientificdms.com
- **📱 Hotline**: 1900-xxxx-xxx
- **💬 Chat**: Zalo/Telegram support
- **📖 Wiki**: Xem docs/ folder để biết chi tiết

---

## 🔮 PHÁT TRIỂN TƯƠNG LAI

### 🎯 ROADMAP

#### **Version 2.0** (Q3 2025)
- 🚀 **Performance optimization**: Caching, CDN integration
- 📱 **Progressive Web App (PWA)**: Offline support, push notifications
- 🔗 **API integration**: Google Scholar, CrossRef, ORCID
- 🌐 **Multi-tenancy**: Hỗ trợ nhiều tổ chức cùng lúc
- 📊 **Advanced analytics**: Machine learning insights

#### **Version 3.0** (Q1 2026)
- 🤖 **AI features**: Smart document classification, auto-tagging
- � **Mobile app**: React Native hoặc Flutter
- 🔄 **Real-time collaboration**: SignalR cho multi-user editing
- 🌍 **Multi-language**: English, Chinese support
- ☁️ **Cloud-native**: Microservices với Docker & Kubernetes

### 💡 Ý TƯỞNG TÍNH NĂNG MỚI

- **QR Code scanning**: Quét mã QR để mượn/trả nhanh
- **Email notifications**: Thông báo tự động về hạn trả
- **Digital library**: Đọc tài liệu trực tuyến
- **Citation generator**: Tạo trích dẫn tự động
- **Social features**: Rating, review, favorite
- **Integration**: Google Drive, OneDrive, Dropbox

---

## 👨‍💻 THÔNG TIN PHÁT TRIỂN

### 👥 TEAM

- **🏗️ Lead Developer**: [Tên của bạn]
- **🎨 UI/UX Designer**: [Tên designer]  
- **🧪 QA Tester**: [Tên tester]
- **📝 Technical Writer**: [Tên writer]

### 📅 TIMELINE

| Giai đoạn | Thời gian | Mô tả |
|-----------|-----------|-------|
| **Planning & Design** | 2 tuần | Phân tích yêu cầu, thiết kế database & UI |
| **Core Development** | 6 tuần | Phát triển tính năng chính |
| **Testing & Polish** | 2 tuần | Test, fix bugs, polish UI/UX |
| **Deployment** | 1 tuần | Deploy và training người dùng |

### 💰 BUDGET ESTIMATION

| Hạng mục | Chi phí | Ghi chú |
|----------|---------|---------|
| **Development** | 80% | Coding, testing, documentation |
| **Design** | 10% | UI/UX design, graphics |
| **Infrastructure** | 5% | Hosting, tools, licenses |
| **Contingency** | 5% | Buffer cho unplanned work |

---

## 📄 LICENSE

Dự án này được phát triển theo **MIT License**. Xem file [LICENSE.md](LICENSE.md) để biết chi tiết.

---

## 🙏 LỜI CẢM ƠN

Cảm ơn các thư viện và tools đã hỗ trợ trong quá trình phát triển:

- **Microsoft** - .NET Framework & Visual Studio
- **Entity Framework Team** - ORM tuyệt vời
- **AutoMapper Contributors** - Object mapping
- **FluentValidation Team** - Validation framework  
- **LiveCharts Contributors** - Beautiful charts
- **Material Design Team** - Design system
- **Stack Overflow Community** - Giải đáp thắc mắc 😄

---

## 📞 LIÊN HỆ

**📧 Email**: nhdiendnc.dev@gmail.com
**🌐 Website**: https://diendev.netlify.app  
**📱 Phone**: +84 945700813
**💼 LinkedIn**: linkedin.com/in/huydien23  
**🐙 GitHub**: github.com/huydien23 

---

<div align="center">

**🎉 Chúc bạn thành công với dự án! 🎉**

*Made with ❤️ for Vietnamese education*

**⭐ Nếu thấy hữu ích, đừng quên cho project một star nhé! ⭐**

</div>

---

*📝 Cập nhật lần cuối: Tháng 7/2025*  
*📊 Version: 1.0.0*  
*👨‍💻 Tác giả: [Huy Điền]*
