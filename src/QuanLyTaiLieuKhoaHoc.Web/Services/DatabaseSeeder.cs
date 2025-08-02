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
            int idDeTai = loaiDeTai.MaLoaiTaiLieu;
            int idGiaoTrinh = loaiGiaoTrinh.MaLoaiTaiLieu;

            // Seed 15 tài liệu mẫu 
            var sampleDocuments = new List<TaiLieu>
            {
                // 1. Deep Learning
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
                // 2. Attention Is All You Need
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
                // 3. Generative Adversarial Nets
                new TaiLieu
                {
                    TenTaiLieu = "Generative Adversarial Nets",
                    TacGia = "Ian Goodfellow, Jean Pouget-Abadie, et al.",
                    MoTa = "Bài báo giới thiệu GANs.",
                    DuongDanFile = "/uploads/documents/gan.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 1700000,
                    NgayTaiLen = DateTime.Now.AddDays(-16),
                    LuotTai = 100,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.48550/arXiv.1406.2661",
                    ISSN = "N/A",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "NeurIPS",
                    NgayCongBo = new DateTime(2014, 6, 10),
                    TieuDe = "Generative Adversarial Nets"
                },
                // 4. ImageNet Classification with Deep Convolutional Neural Networks
                new TaiLieu
                {
                    TenTaiLieu = "ImageNet Classification with Deep Convolutional Neural Networks",
                    TacGia = "Alex Krizhevsky, Ilya Sutskever, Geoffrey Hinton",
                    MoTa = "Bài báo về CNN và ImageNet.",
                    DuongDanFile = "/uploads/documents/imagenet-cnn.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 1900000,
                    NgayTaiLen = DateTime.Now.AddDays(-14),
                    LuotTai = 105,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.1145/3065386",
                    ISSN = "N/A",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "NeurIPS",
                    NgayCongBo = new DateTime(2012, 12, 3),
                    TieuDe = "ImageNet Classification with Deep Convolutional Neural Networks"
                },
                // 5. Quantum supremacy using a programmable superconducting processor
                new TaiLieu
                {
                    TenTaiLieu = "Quantum supremacy using a programmable superconducting processor",
                    TacGia = "Frank Arute, Kunal Arya, Ryan Babbush, et al.",
                    MoTa = "Bài báo về quantum supremacy.",
                    DuongDanFile = "/uploads/documents/quantum-supremacy.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2100000,
                    NgayTaiLen = DateTime.Now.AddDays(-12),
                    LuotTai = 95,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idBaiBao,
                    DOI = "10.1038/s41586-019-1666-5",
                    ISSN = "0028-0836",
                    CapDo = "Quốc tế",
                    TapChiHoiNghi = "Nature",
                    NgayCongBo = new DateTime(2019, 10, 23),
                    TieuDe = "Quantum supremacy using a programmable superconducting processor"
                },

                // 5 Đề tài nghiên cứu khoa học
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
                    MaChuyenNganh = 2,
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
                new TaiLieu
                {
                    TenTaiLieu = "Phát triển hệ thống nhận diện khuôn mặt cho thành phố thông minh",
                    TacGia = "TS. Trần Thị Minh",
                    MoTa = "Đề tài nghiên cứu về nhận diện khuôn mặt.",
                    DuongDanFile = "/uploads/documents/face-recognition.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2600000,
                    NgayTaiLen = DateTime.Now.AddDays(-8),
                    LuotTai = 60,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Phát triển hệ thống nhận diện khuôn mặt cho thành phố thông minh",
                    MaSoDeTai = "DT2023-003",
                    CapDeTai = "Cấp Thành phố",
                    ThoiGianThucHien = "2023-2025",
                    CoQuanChuTri = "Đại học Công nghệ TP.HCM",
                    ChuNhiemDeTai = "TS. Trần Thị Minh"
                },
                new TaiLieu
                {
                    TenTaiLieu = "Nghiên cứu tối ưu hóa thuật toán sắp xếp dữ liệu lớn",
                    TacGia = "TS. Nguyễn Văn B",
                    MoTa = "Đề tài nghiên cứu về tối ưu hóa thuật toán sắp xếp.",
                    DuongDanFile = "/uploads/documents/bigdata-sort.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2400000,
                    NgayTaiLen = DateTime.Now.AddDays(-7),
                    LuotTai = 65,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Nghiên cứu tối ưu hóa thuật toán sắp xếp dữ liệu lớn",
                    MaSoDeTai = "DT2020-004",
                    CapDeTai = "Cấp Trường",
                    ThoiGianThucHien = "2020-2022",
                    CoQuanChuTri = "Đại học Quốc gia TP.HCM",
                    ChuNhiemDeTai = "TS. Nguyễn Văn B"
                },
                new TaiLieu
                {
                    TenTaiLieu = "Ứng dụng Blockchain trong quản lý chuỗi cung ứng",
                    TacGia = "TS. Lê Văn C",
                    MoTa = "Đề tài nghiên cứu về blockchain trong chuỗi cung ứng.",
                    DuongDanFile = "/uploads/documents/blockchain-supplychain.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 2300000,
                    NgayTaiLen = DateTime.Now.AddDays(-6),
                    LuotTai = 55,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 2,
                    MaLoaiTaiLieu = idDeTai,
                    TenDeTai = "Ứng dụng Blockchain trong quản lý chuỗi cung ứng",
                    MaSoDeTai = "DT2021-005",
                    CapDeTai = "Cấp Bộ",
                    ThoiGianThucHien = "2021-2023",
                    CoQuanChuTri = "Đại học Kinh tế Quốc dân",
                    ChuNhiemDeTai = "TS. Lê Văn C"
                },

                // 5 Giáo trình - Tài liệu giảng dạy
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Cơ sở dữ liệu",
                    TacGia = "ThS. Trần Văn C",
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
                    TacGia = "ThS. Nguyễn Thị D",
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
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Mạng máy tính",
                    TacGia = "PGS.TS Lê Văn E",
                    MoTa = "Giáo trình mạng máy tính.",
                    DuongDanFile = "/uploads/documents/network.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3700000,
                    NgayTaiLen = DateTime.Now.AddDays(-3),
                    LuotTai = 160,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Mạng máy tính",
                    MonHocLienQuan = "Mạng máy tính",
                    DonViPhatHanh = "Nhà xuất bản Bách Khoa",
                    NamXuatBan = 2019,
                    SoTinChi = 3
                },
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Trí tuệ nhân tạo",
                    TacGia = "TS. Nguyễn Văn F",
                    MoTa = "Giáo trình trí tuệ nhân tạo.",
                    DuongDanFile = "/uploads/documents/ai-textbook.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3900000,
                    NgayTaiLen = DateTime.Now.AddDays(-2),
                    LuotTai = 140,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Trí tuệ nhân tạo",
                    MonHocLienQuan = "Trí tuệ nhân tạo",
                    DonViPhatHanh = "Nhà xuất bản Khoa học & Kỹ thuật",
                    NamXuatBan = 2022,
                    SoTinChi = 3
                },
                new TaiLieu
                {
                    TenTaiLieu = "Giáo trình Phân tích và thiết kế hệ thống thông tin",
                    TacGia = "ThS. Phạm Thị G",
                    MoTa = "Giáo trình phân tích và thiết kế hệ thống thông tin.",
                    DuongDanFile = "/uploads/documents/pttkhttt.pdf",
                    LoaiFile = ".pdf",
                    KichThuocFile = 3200000,
                    NgayTaiLen = DateTime.Now.AddDays(-1),
                    LuotTai = 120,
                    TrangThai = TrangThaiTaiLieu.DaDuyet,
                    MaChuyenNganh = 1,
                    MaLoaiTaiLieu = idGiaoTrinh,
                    TenGiaoTrinh = "Giáo trình Phân tích và thiết kế hệ thống thông tin",
                    MonHocLienQuan = "Phân tích hệ thống",
                    DonViPhatHanh = "Nhà xuất bản Thống Kê",
                    NamXuatBan = 2023,
                    SoTinChi = 2
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