using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public static class DatabaseSeeder
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            // Kiểm tra xem đã có dữ liệu user chưa
            if (await userManager.Users.AnyAsync())
                return;

            // Tạo admin user
            var adminUser = new NguoiDung
            {
                UserName = "admin@qlkh.edu.vn",
                Email = "admin@qlkh.edu.vn",
                EmailConfirmed = true,
                HoTen = "Quản trị viên hệ thống",
                VaiTro = VaiTroNguoiDung.QuanTriVien,
                MaChuyenNganh = 1,
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };
            await userManager.CreateAsync(adminUser, "123456");

            // Tạo giảng viên
            var lecturerUser = new NguoiDung
            {
                UserName = "giangvien@qlkh.edu.vn",
                Email = "giangvien@qlkh.edu.vn",
                EmailConfirmed = true,
                HoTen = "PGS.TS Nguyễn Văn A",
                VaiTro = VaiTroNguoiDung.GiangVien,
                MaChuyenNganh = 1,
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };
            await userManager.CreateAsync(lecturerUser, "123456");

            // Tạo sinh viên
            var studentUser = new NguoiDung
            {
                UserName = "sinhvien@qlkh.edu.vn",
                Email = "sinhvien@qlkh.edu.vn",
                EmailConfirmed = true,
                HoTen = "Trần Thị B",
                VaiTro = VaiTroNguoiDung.SinhVien,
                MaChuyenNganh = 1,
                MaSo = "20210001",
                KhoaHoc = "2021",
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };
            await userManager.CreateAsync(studentUser, "123456");

            // Chờ để đảm bảo users được tạo thành công
            await context.SaveChangesAsync();

            // Kiểm tra xem đã có tài liệu chưa
            if (await context.TaiLieu.AnyAsync())
                return;

            // Lấy lại users đã tạo để có ID
            var admin = await userManager.FindByEmailAsync("admin@qlkh.edu.vn");
            var lecturer = await userManager.FindByEmailAsync("giangvien@qlkh.edu.vn");
            var student = await userManager.FindByEmailAsync("sinhvien@qlkh.edu.vn");

            // Đảm bảo users được tạo thành công
            if (admin == null || lecturer == null || student == null)
                return;

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
                    MaNguoiTaiLen = lecturer.Id
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
                    MaNguoiTaiLen = lecturer.Id
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
                    MaNguoiTaiLen = lecturer.Id
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
                    MaNguoiTaiLen = lecturer.Id
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
                    MaNguoiTaiLen = student.Id
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
                    MaNguoiTaiLen = lecturer.Id
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
                    MaNguoiTaiLen = lecturer.Id
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
                    MaNguoiTaiLen = lecturer.Id
                }
            };

            context.TaiLieu.AddRange(sampleDocuments);
            await context.SaveChangesAsync(); // Lưu tài liệu trước để có ID

            // Tạo một số đánh giá mẫu sử dụng ID thực tế
            var docs = await context.TaiLieu.Take(3).ToListAsync();
            var sampleRatings = new List<DanhGiaTaiLieu>
            {
                new DanhGiaTaiLieu
                {
                    MaTaiLieu = docs[0].MaTaiLieu,
                    MaNguoiDung = student.Id,
                    DiemDanhGia = 5,
                    NhanXet = "Tài liệu rất hữu ích và dễ hiểu!",
                    NgayDanhGia = DateTime.Now.AddDays(-5)
                },
                new DanhGiaTaiLieu
                {
                    MaTaiLieu = docs[0].MaTaiLieu,
                    MaNguoiDung = admin.Id,
                    DiemDanhGia = 4,
                    NhanXet = "Nội dung tốt, trình bày khá rõ ràng.",
                    NgayDanhGia = DateTime.Now.AddDays(-3)
                },
                new DanhGiaTaiLieu
                {
                    MaTaiLieu = docs[1].MaTaiLieu,
                    MaNguoiDung = student.Id,
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
