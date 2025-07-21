using System;

namespace QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels
{
    public class LapPhieuMuonViewModel
    {
        public int MaPhieu { get; set; }
        // Thông tin sinh viên
        public string HoTen { get; set; } = string.Empty;
        public string MSSV { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ChuyenNganh { get; set; } = string.Empty;
        // Thông tin tài liệu
        public string TenTaiLieu { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        // Thông tin mượn
        public DateTime NgayMuon { get; set; }
        public string LyDo { get; set; } = string.Empty;
        // Ngày trả thực tế (thủ thư nhập)
        public DateTime? NgayTra { get; set; }
        public bool IsFromRequest { get; set; }
    }
} 