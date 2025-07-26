using System;

namespace QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels
{
    public class EditTaiLieuViewModel
    {
        public int MaTaiLieu { get; set; }
        public string TenTaiLieu { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public int MaChuyenNganh { get; set; }
        public int MaLoaiTaiLieu { get; set; }
        public bool ChoPhepTaiFile { get; set; }
        // --- Bài báo khoa học ---
        public string? TieuDe { get; set; }
        public string? TapChiHoiNghi { get; set; }
        public DateTime? NgayCongBo { get; set; }
        public string? DOI { get; set; }
        public string? ISSN { get; set; }
        public string? CapDo { get; set; }
        // --- Đề tài nghiên cứu khoa học ---
        public string? TenDeTai { get; set; }
        public string? MaSoDeTai { get; set; }
        public string? CapDeTai { get; set; }
        public string? ThoiGianThucHien { get; set; }
        public string? CoQuanChuTri { get; set; }
        public string? ChuNhiemDeTai { get; set; }
        // --- Giáo trình - tài liệu giảng dạy ---
        public string? TenGiaoTrinh { get; set; }
        public string? MonHocLienQuan { get; set; }
        public string? DonViPhatHanh { get; set; }
        public int? NamXuatBan { get; set; }
        public int? SoTinChi { get; set; }
        // Số lượng
        public int SoLuong { get; set; } = 1;
        public int SoLuongDaMuon { get; set; } = 0;
        // File upload
        public Microsoft.AspNetCore.Http.IFormFile? FileTaiLieu { get; set; }
    }
}
