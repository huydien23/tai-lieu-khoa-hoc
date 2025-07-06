using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public static class DatabaseSeeder
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có dữ liệu chưa
            if (await context.TaiLieu.AnyAsync())
                return;

            // Tạo một admin user mẫu
            var adminUser = new NguoiDung
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true,
                HoTen = "Quản trị viên",
                VaiTro = VaiTroNguoiDung.QuanTriVien,
                MaChuyenNganh = 1,
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };

            // Tạo một giảng viên mẫu
            var lecturerUser = new NguoiDung
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "gv@gv.com",
                Email = "gv@gv.com",
                EmailConfirmed = true,
                HoTen = "PGS.TS Nguyễn Văn A",
                VaiTro = VaiTroNguoiDung.GiangVien,
                MaChuyenNganh = 1,
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };

            // Tạo một sinh viên mẫu
            var studentUser = new NguoiDung
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "sv@sv.com",
                Email = "sv@sv.com",
                EmailConfirmed = true,
                HoTen = "Trần Thị B",
                VaiTro = VaiTroNguoiDung.SinhVien,
                MaChuyenNganh = 1,
                MaSo = "20210001",
                KhoaHoc = "2021",
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };

            context.Users.AddRange(adminUser, lecturerUser, studentUser);

            // Tạo tài liệu mẫu
            var sampleDocuments = new List<TaiLieu>
            {
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Lập trình C# cơ bản",
                    MoTa = "Tài liệu hướng dẫn lập trình C# từ cơ bản đến nâng cao, bao gồm cú pháp, OOP, và các framework phổ biến.",
                    DuongDanFile = "/uploads/documents/csharp-basic.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2048000,
                    NgayTaiLen = DateTime.Now.AddDays(-10),
                    LuotTai = 150,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = 1,
                    MaNguoiTaiLen = lecturerUser.Id
                },
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Machine Learning",
                    MoTa = "Slide bài giảng về Machine Learning, bao gồm các thuật toán cơ bản như Linear Regression, Decision Tree, Neural Networks.",
                    DuongDanFile = "/uploads/documents/ml-slides.pptx",
                    LoaiFile = ".pptx",
                    KichThuocFile = 5120000,
                    NgayTaiLen = DateTime.Now.AddDays(-5),
                    LuotTai = 89,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = 2,
                    MaNguoiTaiLen = lecturerUser.Id
                },
                new TaiLieu
                {
                    TenTaiLieu = "Đề thi Cấu trúc dữ liệu và Giải thuật",
                    MoTa = "Đề thi giữa kỳ môn Cấu trúc dữ liệu và Giải thuật, bao gồm các bài tập về stack, queue, tree, và thuật toán sắp xếp.",
                    DuongDanFile = "/uploads/documents/dsa-exam.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 512000,
                    NgayTaiLen = DateTime.Now.AddDays(-15),
                    LuotTai = 234,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = 3,
                    MaNguoiTaiLen = lecturerUser.Id
                },
                new TaiLieu
                {
                    TenTaiLieu = "Bài tập ASP.NET Core MVC",
                    MoTa = "Tập hợp các bài tập thực hành ASP.NET Core MVC, từ cơ bản đến nâng cao, có kèm lời giải chi tiết.",
                    DuongDanFile = "/uploads/documents/aspnet-exercises.docx",
                    LoaiFile = ".docx",
                    KichThuocFile = 1024000,
                    NgayTaiLen = DateTime.Now.AddDays(-3),
                    LuotTai = 67,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = 4,
                    MaNguoiTaiLen = lecturerUser.Id
                },
                new TaiLieu
                {
                    TenTaiLieu = "Luận văn: Ứng dụng AI trong y tế",
                    MoTa = "Luận văn thạc sĩ về ứng dụng trí tuệ nhân tạo trong chẩn đoán và điều trị y tế, nghiên cứu trường hợp thực tế.",
                    DuongDanFile = "/uploads/documents/ai-healthcare-thesis.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 8192000,
                    NgayTaiLen = DateTime.Now.AddDays(-30),
                    LuotTai = 45,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = 5,
                    MaNguoiTaiLen = studentUser.Id
                },
                new TaiLieu
                {
                    TenTaiLieu = "Tài liệu tham khảo Database Design",
                    MoTa = "Tài liệu tham khảo về thiết kế cơ sở dữ liệu, bao gồm ERD, normalization, và các best practices.",
                    DuongDanFile = "/uploads/documents/database-design-ref.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3072000,
                    NgayTaiLen = DateTime.Now.AddDays(-7),
                    LuotTai = 123,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = 6,
                    MaNguoiTaiLen = lecturerUser.Id
                },
                // Thêm tài liệu cho chuyên ngành Kinh tế
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Kinh tế học vĩ mô",
                    MoTa = "Giáo trình về kinh tế học vĩ mô, bao gồm các chỉ số kinh tế, chính sách tài khóa và tiền tệ.",
                    DuongDanFile = "/uploads/documents/macro-economics.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4096000,
                    NgayTaiLen = DateTime.Now.AddDays(-12),
                    LuotTai = 98,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = 1,
                    MaNguoiTaiLen = lecturerUser.Id
                },
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Marketing Digital",
                    MoTa = "Slide bài giảng về marketing kỹ thuật số, SEO, SEM, và social media marketing trong thời đại 4.0.",
                    DuongDanFile = "/uploads/documents/digital-marketing.pptx",
                    LoaiFile = ".pptx",
                    KichThuocFile = 6144000,
                    NgayTaiLen = DateTime.Now.AddDays(-2),
                    LuotTai = 156,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = 2,
                    MaNguoiTaiLen = lecturerUser.Id
                }
            };

            context.TaiLieu.AddRange(sampleDocuments);
            await context.SaveChangesAsync(); // Lưu tài liệu trước để có ID

            // Tạo một số đánh giá mẫu
            var sampleRatings = new List<DanhGiaTaiLieu>
            {
                new DanhGiaTaiLieu
                {
                    MaTaiLieu = sampleDocuments[0].MaTaiLieu,
                    MaNguoiDung = studentUser.Id,
                    DiemDanhGia = 5,
                    NhanXet = "Tài liệu rất hữu ích và dễ hiểu!",
                    NgayDanhGia = DateTime.Now.AddDays(-5)
                },
                new DanhGiaTaiLieu
                {
                    MaTaiLieu = sampleDocuments[0].MaTaiLieu,
                    MaNguoiDung = adminUser.Id,
                    DiemDanhGia = 4,
                    NhanXet = "Nội dung tốt, trình bày khá rõ ràng.",
                    NgayDanhGia = DateTime.Now.AddDays(-3)
                },
                new DanhGiaTaiLieu
                {
                    MaTaiLieu = sampleDocuments[1].MaTaiLieu,
                    MaNguoiDung = studentUser.Id,
                    DiemDanhGia = 5,
                    NhanXet = "Slide rất chi tiết và có nhiều ví dụ thực tế.",
                    NgayDanhGia = DateTime.Now.AddDays(-1)
                }
            };

            context.DanhGiaTaiLieu.AddRange(sampleRatings);

            await context.SaveChangesAsync();
        }
    }
}
