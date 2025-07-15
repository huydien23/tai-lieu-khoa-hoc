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
            // Seed các loại tài liệu đặc biệt nếu chưa có (không gán MaLoaiTaiLieu, để tự tăng)
            LoaiTaiLieu? loaiBaiBao = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Bài báo khoa học");
            if (loaiBaiBao == null)
            {
                loaiBaiBao = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Bài báo khoa học",
                    MoTa = "Tài liệu dạng bài báo khoa học, có thể tải trực tiếp.",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiBaiBao);
            }
            LoaiTaiLieu? loaiDeTai = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Đề tài nghiên cứu khoa học");
            if (loaiDeTai == null)
            {
                loaiDeTai = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Đề tài nghiên cứu khoa học",
                    MoTa = "Tài liệu dạng đề tài nghiên cứu khoa học, có thể tải trực tiếp.",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiDeTai);
            }
            LoaiTaiLieu? loaiGiaoTrinh = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Giáo trình - Tài liệu giảng dạy");
            if (loaiGiaoTrinh == null)
            {
                loaiGiaoTrinh = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Giáo trình - Tài liệu giảng dạy",
                    MoTa = "Tài liệu dạng giáo trình, tài liệu giảng dạy, có thể tải trực tiếp.",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiGiaoTrinh);
            }
            await context.SaveChangesAsync();

            // Lấy lại ID các loại tài liệu
            int idBaiBao = loaiBaiBao.MaLoaiTaiLieu;
            int idDeTai = loaiDeTai.MaLoaiTaiLieu;
            int idGiaoTrinh = loaiGiaoTrinh.MaLoaiTaiLieu;
            {
                // Kiểm tra xem đã có dữ liệu user chưa
                if (await userManager.Users.AnyAsync())
                    return;

                // Tạo thủ thư
                var librarianUser = new NguoiDung
                {
                    UserName = "thuthu@library.edu.vn",
                    Email = "thuthu@library.edu.vn",
                    EmailConfirmed = true,
                    HoTen = "Thủ Thư",
                    VaiTro = VaiTroNguoiDung.ThuThu,
                    MaChuyenNganh = 1,
                    MaSo = "TT001",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                await userManager.CreateAsync(librarianUser, "ThuThu@2024");

                // Tạo giảng viên
                var lecturerUser = new NguoiDung
                {
                    UserName = "giangvien@university.edu.vn",
                    Email = "giangvien@university.edu.vn",
                    EmailConfirmed = true,
                    HoTen = "PGS.TS Nguyễn Văn An",
                    VaiTro = VaiTroNguoiDung.GiangVien,
                    MaChuyenNganh = 1,
                    MaSo = "GV001",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                await userManager.CreateAsync(lecturerUser, "GiangVien@2024");

                // Tạo sinh viên
                var studentUser = new NguoiDung
                {
                    UserName = "sinhvien@student.edu.vn",
                    Email = "sinhvien@student.edu.vn",
                    EmailConfirmed = true,
                    HoTen = "Trần Thị Bình",
                    VaiTro = VaiTroNguoiDung.SinhVien,
                    MaChuyenNganh = 1,
                    MaSo = "20210001",
                    KhoaHoc = "2021",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                await userManager.CreateAsync(studentUser, "SinhVien@2024");
                await context.SaveChangesAsync();

                // Xóa toàn bộ dữ liệu cũ bảng TaiLieu
                context.TaiLieu.RemoveRange(context.TaiLieu);
                await context.SaveChangesAsync();

                // Lấy lại users đã tạo để có ID
                var librarian = await userManager.FindByEmailAsync("thuthu@library.edu.vn");
                var lecturer = await userManager.FindByEmailAsync("giangvien@university.edu.vn");
                var student = await userManager.FindByEmailAsync("sinhvien@student.edu.vn");
                if (librarian == null || lecturer == null || student == null)
                    return;

                // Thêm dữ liệu mẫu cho từng loại
                var sampleDocuments = new List<TaiLieu>
            {
                // Bài báo khoa học
                new TaiLieu
                {
                    TenTaiLieu = "Ứng dụng AI trong xử lý ngôn ngữ tự nhiên",
                    TacGia = "PGS.TS Nguyễn Văn An",
                    MoTa = "Bài báo về ứng dụng trí tuệ nhân tạo trong xử lý ngôn ngữ tự nhiên.",
                    DuongDanFile = "/uploads/documents/ai-nlp.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2048000,
                    NgayTaiLen = DateTime.Now.AddDays(-20),
                    LuotTai = 120,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiBao,
                    // Thông tin chuyên biệt
                    DOI = "10.1234/ai.nlu.2025.001",
                    ISSN = "1859-1234",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "International Journal of AI",
                    NgayCongBo = new DateTime(2025, 5, 10),
                    TieuDe = "Ứng dụng AI trong xử lý ngôn ngữ tự nhiên"
                },
                // Đề tài nghiên cứu khoa học
                new TaiLieu
                {
                    TenTaiLieu = "Nghiên cứu phát triển hệ thống IoT cho nông nghiệp",
                    TacGia = "TS. Lê Thị Bích",
                    MoTa = "Đề tài nghiên cứu về ứng dụng IoT trong nông nghiệp thông minh.",
                    DuongDanFile = "/uploads/documents/iot-agriculture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3072000,
                    NgayTaiLen = DateTime.Now.AddDays(-15),
                    LuotTai = 80,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idDeTai,
                    // Thông tin chuyên biệt
                    TenDeTai = "Nghiên cứu phát triển hệ thống IoT cho nông nghiệp",
                    MaSoDeTai = "DT2025-001",
                    CapDeTai = "Cấp Trường",
                    ThoiGianThucHien = "2024-2025",
                    CoQuanChuTri = "Trường Đại học Nông nghiệp",
                    ChuNhiemDeTai = "TS. Lê Thị Bích"
                },
                // Giáo trình - tài liệu giảng dạy
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Cơ sở dữ liệu quan hệ",
                    TacGia = "ThS. Trần Văn C, ThS. Nguyễn Thị D",
                    MoTa = "Giáo trình phục vụ giảng dạy môn Cơ sở dữ liệu quan hệ.",
                    DuongDanFile = "/uploads/documents/db-relational.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4096000,
                    NgayTaiLen = DateTime.Now.AddDays(-10),
                    LuotTai = 200,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    // Thông tin chuyên biệt
                    TenGiaoTrinh = "Giáo trình Cơ sở dữ liệu quan hệ",
                    MonHocLienQuan = "Cơ sở dữ liệu",
                    DonViPhatHanh = "Nhà xuất bản Giáo dục",
                    NamXuatBan = 2024,
                    SoTinChi = 3
                }
            };

                context.TaiLieu.AddRange(sampleDocuments);
                await context.SaveChangesAsync();
            }
        }
    }
}