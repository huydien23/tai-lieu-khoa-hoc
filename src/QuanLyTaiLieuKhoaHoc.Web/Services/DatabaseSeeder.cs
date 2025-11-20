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
            // Seed các loại tài liệu đặc biệt
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

            LoaiTaiLieu? loaiBaiGiang = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Bài giảng");
            if (loaiBaiGiang == null)
            {
                loaiBaiGiang = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Bài giảng",
                    MoTa = "Slide bài giảng của giảng viên",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiBaiGiang);
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

            LoaiTaiLieu? loaiGiaoTrinh = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Giáo trình");
            if (loaiGiaoTrinh == null)
            {
                loaiGiaoTrinh = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Giáo trình",
                    MoTa = "Tài liệu giảng dạy chính thức",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiGiaoTrinh);
            }

            LoaiTaiLieu? loaiGiaoTrinhDay = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Giáo trình - Tài liệu giảng dạy");
            if (loaiGiaoTrinhDay == null)
            {
                loaiGiaoTrinhDay = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Giáo trình - Tài liệu giảng dạy",
                    MoTa = "Tài liệu dạng giáo trình, tài liệu giảng dạy, có thể tải trực tiếp.",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiGiaoTrinhDay);
            }

            LoaiTaiLieu? loaiLuanVan = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Luận văn");
            if (loaiLuanVan == null)
            {
                loaiLuanVan = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Luận văn",
                    MoTa = "Luận văn tốt nghiệp",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiLuanVan);
            }

            LoaiTaiLieu? loaiThamKhao = await context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Tài liệu tham khảo");
            if (loaiThamKhao == null)
            {
                loaiThamKhao = new LoaiTaiLieu
                {
                    TenLoaiTaiLieu = "Tài liệu tham khảo",
                    MoTa = "Tài liệu bổ sung",
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true
                };
                context.LoaiTaiLieu.Add(loaiThamKhao);
            }

            await context.SaveChangesAsync();

            // Tạo thủ thư 
            var librarianUser = await userManager.FindByEmailAsync("thuthu@library.edu.vn");
            if (librarianUser == null)
            {
                librarianUser = new NguoiDung
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
                await userManager.CreateAsync(librarianUser, "thuthu123");
            }

            // Lấy lại ID các loại tài liệu
            int idBaiBao = loaiBaiBao.MaLoaiTaiLieu;
            int idBaiGiang = loaiBaiGiang.MaLoaiTaiLieu;
            int idDeTai = loaiDeTai.MaLoaiTaiLieu;
            int idGiaoTrinh = loaiGiaoTrinh.MaLoaiTaiLieu;
            int idGiaoTrinhDay = loaiGiaoTrinhDay.MaLoaiTaiLieu;
            int idLuanVan = loaiLuanVan.MaLoaiTaiLieu;
            int idThamKhao = loaiThamKhao.MaLoaiTaiLieu;

            // Seed tài liệu mẫu phong phú
            var sampleDocuments = new List<TaiLieu>
            {
                // === BÀI BÁO KHOA HỌC - CÔNG NGHỆ THÔNG TIN ===
                new TaiLieu
                {
                    TenTaiLieu = "Deep Learning",
                    TacGia = "Yann LeCun, Yoshua Bengio, Geoffrey Hinton",
                    MoTa = "Bài báo tổng quan về deep learning.",
                    DuongDanFile = "/uploads/documents/deep-learning.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2048000,
                    NgayTaiLen = DateTime.Now.AddDays(-20),
                    LuotTai = 120,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.1038/nature14539",
                    ISSN = "0028-0836",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "Nature",
                    NgayCongBo = new DateTime(2015, 5, 28),
                    TieuDe = "Deep Learning"
                },
                new TaiLieu
                {
                    TenTaiLieu = "Attention Is All You Need",
                    TacGia = "Ashish Vaswani, Noam Shazeer, Niki Parmar, et al.",
                    MoTa = "Bài báo giới thiệu mô hình Transformer.",
                    DuongDanFile = "/uploads/documents/attention.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 1800000,
                    NgayTaiLen = DateTime.Now.AddDays(-18),
                    LuotTai = 110,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.48550/arXiv.1706.03762",
                    ISSN = "N/A",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "NeurIPS",
                    NgayCongBo = new DateTime(2017, 12, 1),
                    TieuDe = "Attention Is All You Need"
                },

                // === BÀI BÁO KHOA HỌC - KINH TẾ ===
                new TaiLieu
                {
                    TenTaiLieu = "The Impact of Digital Transformation on Business Performance",
                    TacGia = "Dr. Sarah Johnson, Prof. Michael Chen",
                    MoTa = "Nghiên cứu về tác động của chuyển đổi số đến hiệu suất doanh nghiệp.",
                    DuongDanFile = "/uploads/documents/digital-transformation.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 1500000,
                    NgayTaiLen = DateTime.Now.AddDays(-15),
                    LuotTai = 85,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.1016/j.jbusres.2023.01.015",
                    ISSN = "0148-2963",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "Journal of Business Research",
                    NgayCongBo = new DateTime(2023, 3, 15),
                    TieuDe = "The Impact of Digital Transformation on Business Performance"
                },

                // === BÀI GIẢNG - CÔNG NGHỆ THÔNG TIN ===
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Lập trình Java cơ bản",
                    TacGia = "ThS. Nguyễn Văn An",
                    MoTa = "Slide bài giảng môn Lập trình Java cơ bản.",
                    DuongDanFile = "/uploads/documents/java-basic-lecture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2500000,
                    NgayTaiLen = DateTime.Now.AddDays(-13),
                    LuotTai = 150,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiGiang,
                    MonHocLienQuan = "Lập trình Java"
                },
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Cấu trúc dữ liệu và giải thuật",
                    TacGia = "PGS.TS Trần Thị Bình",
                    MoTa = "Slide bài giảng môn Cấu trúc dữ liệu và giải thuật.",
                    DuongDanFile = "/uploads/documents/data-structures-lecture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2800000,
                    NgayTaiLen = DateTime.Now.AddDays(-12),
                    LuotTai = 130,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiGiang,
                    MonHocLienQuan = "Cấu trúc dữ liệu và giải thuật"
                },

                // === BÀI GIẢNG - KINH TẾ ===
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Marketing căn bản",
                    TacGia = "TS. Lê Văn Cường",
                    MoTa = "Slide bài giảng môn Marketing căn bản.",
                    DuongDanFile = "/uploads/documents/marketing-basic-lecture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2200000,
                    NgayTaiLen = DateTime.Now.AddDays(-11),
                    LuotTai = 95,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idBaiGiang,
                    MonHocLienQuan = "Marketing căn bản"
                },

                // === ĐỀ TÀI NGHIÊN CỨU - CÔNG NGHỆ THÔNG TIN ===
                new TaiLieu
                {
                    TenTaiLieu = "Nghiên cứu phát triển hệ thống IoT cho nông nghiệp thông minh",
                    TacGia = "TS. Lê Thị Bích",
                    MoTa = "Đề tài nghiên cứu về ứng dụng IoT trong nông nghiệp thông minh.",
                    DuongDanFile = "/uploads/documents/iot-agriculture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3072000,
                    NgayTaiLen = DateTime.Now.AddDays(-10),
                    LuotTai = 80,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Nghiên cứu phát triển hệ thống IoT cho nông nghiệp thông minh",
                    MaSoDeTai = "DT2021-001",
                    CapDeTai = "Cấp Trường",
                    ThoiGianThucHien = "2021-2023",
                    CoQuanChuTri = "Trường Đại học Nông nghiệp Hà Nội",
                    ChuNhiemDeTai = "TS. Lê Thị Bích"
                },
                new TaiLieu
                {
                    TenTaiLieu = "Ứng dụng AI trong chẩn đoán hình ảnh y tế",
                    TacGia = "PGS.TS Nguyễn Văn An",
                    MoTa = "Đề tài nghiên cứu ứng dụng AI trong y tế.",
                    DuongDanFile = "/uploads/documents/ai-medical.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2500000,
                    NgayTaiLen = DateTime.Now.AddDays(-9),
                    LuotTai = 70,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Ứng dụng AI trong chẩn đoán hình ảnh y tế",
                    MaSoDeTai = "DT2022-002",
                    CapDeTai = "Cấp Bộ",
                    ThoiGianThucHien = "2022-2024",
                    CoQuanChuTri = "Đại học Bách Khoa Hà Nội",
                    ChuNhiemDeTai = "PGS.TS Nguyễn Văn An"
                },

                // === ĐỀ TÀI NGHIÊN CỨU - KINH TẾ ===
                new TaiLieu
                {
                    TenTaiLieu = "Nghiên cứu tác động của Fintech đến ngành ngân hàng Việt Nam",
                    TacGia = "TS. Phạm Thị Dung",
                    MoTa = "Đề tài nghiên cứu về tác động của công nghệ tài chính.",
                    DuongDanFile = "/uploads/documents/fintech-banking.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2400000,
                    NgayTaiLen = DateTime.Now.AddDays(-8),
                    LuotTai = 65,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Nghiên cứu tác động của Fintech đến ngành ngân hàng Việt Nam",
                    MaSoDeTai = "DT2023-003",
                    CapDeTai = "Cấp Bộ",
                    ThoiGianThucHien = "2023-2025",
                    CoQuanChuTri = "Đại học Kinh tế Quốc dân",
                    ChuNhiemDeTai = "TS. Phạm Thị Dung"
                },

                // === GIÁO TRÌNH - CÔNG NGHỆ THÔNG TIN ===
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Cơ sở dữ liệu",
                    TacGia = "ThS. Trần Văn Cường",
                    MoTa = "Giáo trình phục vụ giảng dạy môn Cơ sở dữ liệu.",
                    DuongDanFile = "/uploads/documents/db-relational.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4096000,
                    NgayTaiLen = DateTime.Now.AddDays(-5),
                    LuotTai = 200,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Cơ sở dữ liệu",
                    MonHocLienQuan = "Cơ sở dữ liệu",
                    DonViPhatHanh = "Nhà xuất bản Giáo dục",
                    NamXuatBan = 2020,
                    SoTinChi = 3
                },
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Lập trình Python cơ bản",
                    TacGia = "ThS. Nguyễn Thị Dung",
                    MoTa = "Giáo trình lập trình Python cơ bản.",
                    DuongDanFile = "/uploads/documents/python-basic.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3500000,
                    NgayTaiLen = DateTime.Now.AddDays(-4),
                    LuotTai = 180,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Lập trình Python cơ bản",
                    MonHocLienQuan = "Lập trình Python",
                    DonViPhatHanh = "Nhà xuất bản Đại học Quốc gia",
                    NamXuatBan = 2021,
                    SoTinChi = 2
                },

                // === GIÁO TRÌNH - KINH TẾ ===
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Kinh tế vi mô",
                    TacGia = "PGS.TS Nguyễn Văn Em",
                    MoTa = "Giáo trình kinh tế vi mô cho sinh viên kinh tế.",
                    DuongDanFile = "/uploads/documents/microeconomics.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3800000,
                    NgayTaiLen = DateTime.Now.AddDays(-3),
                    LuotTai = 160,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Kinh tế vi mô",
                    MonHocLienQuan = "Kinh tế vi mô",
                    DonViPhatHanh = "Nhà xuất bản Kinh tế",
                    NamXuatBan = 2022,
                    SoTinChi = 3
                },

                // === GIÁO TRÌNH - TÀI LIỆU GIẢNG DẠY ===
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Mạng máy tính",
                    TacGia = "PGS.TS Lê Văn Em",
                    MoTa = "Giáo trình mạng máy tính.",
                    DuongDanFile = "/uploads/documents/network.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3700000,
                    NgayTaiLen = DateTime.Now.AddDays(-3),
                    LuotTai = 160,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinhDay,
                    TenGiaoTrinh = "Giáo trình Mạng máy tính",
                    MonHocLienQuan = "Mạng máy tính",
                    DonViPhatHanh = "Nhà xuất bản Bách Khoa",
                    NamXuatBan = 2019,
                    SoTinChi = 3
                },
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Trí tuệ nhân tạo",
                    TacGia = "TS. Nguyễn Văn Phúc",
                    MoTa = "Giáo trình trí tuệ nhân tạo.",
                    DuongDanFile = "/uploads/documents/ai-textbook.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3900000,
                    NgayTaiLen = DateTime.Now.AddDays(-2),
                    LuotTai = 140,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinhDay,
                    TenGiaoTrinh = "Giáo trình Trí tuệ nhân tạo",
                    MonHocLienQuan = "Trí tuệ nhân tạo",
                    DonViPhatHanh = "Nhà xuất bản Khoa học & Kỹ thuật",
                    NamXuatBan = 2022,
                    SoTinChi = 3
                },

                // === LUẬN VĂN - CÔNG NGHỆ THÔNG TIN ===
                new TaiLieu
                {
                    TenTaiLieu = "Xây dựng hệ thống quản lý thư viện trực tuyến",
                    TacGia = "Nguyễn Văn Giang - SV2021001",
                    MoTa = "Luận văn tốt nghiệp về hệ thống quản lý thư viện.",
                    DuongDanFile = "/uploads/documents/library-management-thesis.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4500000,
                    NgayTaiLen = DateTime.Now.AddDays(-7),
                    LuotTai = 90,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idLuanVan
                },

                // === LUẬN VĂN - KINH TẾ ===
                new TaiLieu
                {
                    TenTaiLieu = "Phân tích hiệu quả hoạt động kinh doanh của các doanh nghiệp vừa và nhỏ",
                    TacGia = "Lê Thị Hương - SV2022001",
                    MoTa = "Luận văn tốt nghiệp về phân tích hiệu quả kinh doanh.",
                    DuongDanFile = "/uploads/documents/business-efficiency-thesis.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4200000,
                    NgayTaiLen = DateTime.Now.AddDays(-6),
                    LuotTai = 85,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idLuanVan
                },

                // === TÀI LIỆU THAM KHẢO - CÔNG NGHỆ THÔNG TIN ===
                new TaiLieu
                {
                    TenTaiLieu = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    TacGia = "Robert C. Martin",
                    MoTa = "Sách tham khảo về viết code sạch.",
                    DuongDanFile = "/uploads/documents/clean-code.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3200000,
                    NgayTaiLen = DateTime.Now.AddDays(-5),
                    LuotTai = 110,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idThamKhao
                },

                // === TÀI LIỆU THAM KHẢO - KINH TẾ ===
                new TaiLieu
                {
                    TenTaiLieu = "Principles of Economics",
                    TacGia = "N. Gregory Mankiw",
                    MoTa = "Sách tham khảo về nguyên lý kinh tế học.",
                    DuongDanFile = "/uploads/documents/principles-economics.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3500000,
                    NgayTaiLen = DateTime.Now.AddDays(-4),
                    LuotTai = 95,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idThamKhao
                },

                // === BÀI BÁO KHOA HỌC - KỸ THUẬT ===
                new TaiLieu
                {
                    TenTaiLieu = "Advanced Materials for Renewable Energy Applications",
                    TacGia = "Dr. James Wilson, Prof. Maria Garcia",
                    MoTa = "Nghiên cứu về vật liệu tiên tiến cho ứng dụng năng lượng tái tạo.",
                    DuongDanFile = "/uploads/documents/advanced-materials-energy.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 1800000,
                    NgayTaiLen = DateTime.Now.AddDays(-10),
                    LuotTai = 70,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 3,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.1016/j.materresbull.2023.02.001",
                    ISSN = "0025-5408",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "Materials Research Bulletin",
                    NgayCongBo = new DateTime(2023, 4, 15),
                    TieuDe = "Advanced Materials for Renewable Energy Applications"
                },

                // === BÀI BÁO KHOA HỌC - NGOẠI NGỮ ===
                new TaiLieu
                {
                    TenTaiLieu = "Second Language Acquisition in Digital Environments",
                    TacGia = "Dr. Emily Johnson, Prof. Carlos Rodriguez",
                    MoTa = "Nghiên cứu về việc học ngôn ngữ thứ hai trong môi trường số.",
                    DuongDanFile = "/uploads/documents/second-language-acquisition.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 1400000,
                    NgayTaiLen = DateTime.Now.AddDays(-8),
                    LuotTai = 60,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 4,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.1016/j.system.2023.03.005",
                    ISSN = "0346-251X",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "System",
                    NgayCongBo = new DateTime(2023, 5, 10),
                    TieuDe = "Second Language Acquisition in Digital Environments"
                },

                // === BÀI GIẢNG - KỸ THUẬT ===
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Cơ học kỹ thuật",
                    TacGia = "PGS.TS Nguyễn Văn Long",
                    MoTa = "Slide bài giảng môn Cơ học kỹ thuật.",
                    DuongDanFile = "/uploads/documents/mechanical-engineering-lecture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3000000,
                    NgayTaiLen = DateTime.Now.AddDays(-7),
                    LuotTai = 100,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 3,
                    MaLoaiTaiLieu = idBaiGiang,
                    MonHocLienQuan = "Cơ học kỹ thuật"
                },

                // === BÀI GIẢNG - NGOẠI NGỮ ===
                new TaiLieu
                {
                    TenTaiLieu = "Bài giảng Ngữ pháp tiếng Anh nâng cao",
                    TacGia = "ThS. Lê Thị Ngọc",
                    MoTa = "Slide bài giảng môn Ngữ pháp tiếng Anh nâng cao.",
                    DuongDanFile = "/uploads/documents/advanced-english-grammar-lecture.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2500000,
                    NgayTaiLen = DateTime.Now.AddDays(-5),
                    LuotTai = 120,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 4,
                    MaLoaiTaiLieu = idBaiGiang,
                    MonHocLienQuan = "Ngữ pháp tiếng Anh nâng cao"
                },

                // === ĐỀ TÀI NGHIÊN CỨU - KỸ THUẬT ===
                new TaiLieu
                {
                    TenTaiLieu = "Nghiên cứu phát triển hệ thống điều khiển thông minh cho robot công nghiệp",
                    TacGia = "TS. Phạm Văn Oanh",
                    MoTa = "Đề tài nghiên cứu về hệ thống điều khiển thông minh cho robot.",
                    DuongDanFile = "/uploads/documents/smart-robot-control.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3200000,
                    NgayTaiLen = DateTime.Now.AddDays(-4),
                    LuotTai = 55,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 3,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Nghiên cứu phát triển hệ thống điều khiển thông minh cho robot công nghiệp",
                    MaSoDeTai = "DT2023-004",
                    CapDeTai = "Cấp Bộ",
                    ThoiGianThucHien = "2023-2025",
                    CoQuanChuTri = "Đại học Bách Khoa Hà Nội",
                    ChuNhiemDeTai = "TS. Phạm Văn Oanh"
                },

                // === ĐỀ TÀI NGHIÊN CỨU - NGOẠI NGỮ ===
                new TaiLieu
                {
                    TenTaiLieu = "Nghiên cứu hiệu quả của phương pháp học tiếng Anh qua ứng dụng di động",
                    TacGia = "TS. Nguyễn Thị Phương",
                    MoTa = "Đề tài nghiên cứu về hiệu quả học tiếng Anh qua ứng dụng di động.",
                    DuongDanFile = "/uploads/documents/mobile-english-learning.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2800000,
                    NgayTaiLen = DateTime.Now.AddDays(-3),
                    LuotTai = 50,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 4,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Nghiên cứu hiệu quả của phương pháp học tiếng Anh qua ứng dụng di động",
                    MaSoDeTai = "DT2023-005",
                    CapDeTai = "Cấp Trường",
                    ThoiGianThucHien = "2023-2024",
                    CoQuanChuTri = "Đại học Ngoại ngữ",
                    ChuNhiemDeTai = "TS. Nguyễn Thị Phương"
                },

                // === GIÁO TRÌNH - KỸ THUẬT ===
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Cơ học chất lỏng",
                    TacGia = "PGS.TS Trần Văn Quang",
                    MoTa = "Giáo trình cơ học chất lỏng cho sinh viên kỹ thuật.",
                    DuongDanFile = "/uploads/documents/fluid-mechanics.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4000000,
                    NgayTaiLen = DateTime.Now.AddDays(-2),
                    LuotTai = 140,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 3,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Cơ học chất lỏng",
                    MonHocLienQuan = "Cơ học chất lỏng",
                    DonViPhatHanh = "Nhà xuất bản Khoa học & Kỹ thuật",
                    NamXuatBan = 2021,
                    SoTinChi = 3
                },

                // === GIÁO TRÌNH - NGOẠI NGỮ ===
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Tiếng Anh thương mại",
                    TacGia = "ThS. Lê Văn Rạng",
                    MoTa = "Giáo trình tiếng Anh thương mại cho sinh viên ngoại ngữ.",
                    DuongDanFile = "/uploads/documents/business-english.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3500000,
                    NgayTaiLen = DateTime.Now.AddDays(-1),
                    LuotTai = 130,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 4,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Tiếng Anh thương mại",
                    MonHocLienQuan = "Tiếng Anh thương mại",
                    DonViPhatHanh = "Nhà xuất bản Đại học Quốc gia",
                    NamXuatBan = 2023,
                    SoTinChi = 2
                },

                // === LUẬN VĂN - KỸ THUẬT ===
                new TaiLieu
                {
                    TenTaiLieu = "Thiết kế và chế tạo hệ thống điều khiển nhiệt độ thông minh",
                    TacGia = "Vũ Văn Sơn - SV2023001",
                    MoTa = "Luận văn tốt nghiệp về hệ thống điều khiển nhiệt độ thông minh.",
                    DuongDanFile = "/uploads/documents/smart-temperature-control-thesis.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4800000,
                    NgayTaiLen = DateTime.Now.AddDays(-1),
                    LuotTai = 80,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 3,
                    MaLoaiTaiLieu = idLuanVan
                },

                // === LUẬN VĂN - NGOẠI NGỮ ===
                new TaiLieu
                {
                    TenTaiLieu = "Phân tích lỗi ngữ pháp tiếng Anh của sinh viên Việt Nam",
                    TacGia = "Hoàng Thị Thu - SV2023002",
                    MoTa = "Luận văn tốt nghiệp về phân tích lỗi ngữ pháp tiếng Anh.",
                    DuongDanFile = "/uploads/documents/english-grammar-errors-thesis.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 4200000,
                    NgayTaiLen = DateTime.Now.AddDays(-1),
                    LuotTai = 75,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 4,
                    MaLoaiTaiLieu = idLuanVan
                },

                // === TÀI LIỆU THAM KHẢO - KỸ THUẬT ===
                new TaiLieu
                {
                    TenTaiLieu = "Engineering Mechanics: Dynamics",
                    TacGia = "Russell C. Hibbeler",
                    MoTa = "Sách tham khảo về cơ học kỹ thuật - động học.",
                    DuongDanFile = "/uploads/documents/engineering-mechanics-dynamics.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3800000,
                    NgayTaiLen = DateTime.Now.AddDays(-1),
                    LuotTai = 100,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 3,
                    MaLoaiTaiLieu = idThamKhao
                },

                // === TÀI LIỆU THAM KHẢO - NGOẠI NGỮ ===
                new TaiLieu
                {
                    TenTaiLieu = "The Cambridge Grammar of the English Language",
                    TacGia = "Rodney Huddleston, Geoffrey K. Pullum",
                    MoTa = "Sách tham khảo về ngữ pháp tiếng Anh Cambridge.",
                    DuongDanFile = "/uploads/documents/cambridge-grammar.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3600000,
                    NgayTaiLen = DateTime.Now.AddDays(-1),
                    LuotTai = 85,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 4,
                    MaLoaiTaiLieu = idThamKhao
                }
            };

            foreach (var doc in sampleDocuments)
            {
                if (!context.TaiLieu.Any(t => t.TenTaiLieu == doc.TenTaiLieu))
                {
                    context.TaiLieu.Add(doc);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}