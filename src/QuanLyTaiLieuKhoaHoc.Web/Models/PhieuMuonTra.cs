using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("PhieuMuonTra")]
    public class PhieuMuonTra
    {
        [Key]
        public int MaPhieu { get; set; }

        [Required]
        [Display(Name = "Mã tài liệu")]
        public int MaTaiLieu { get; set; }

        [Display(Name = "Mã người mượn")]
        public string? MaNguoiMuon { get; set; }

        // Thông tin người mượn không phải thành viên hệ thống
        [Display(Name = "Họ tên người mượn")]
        public string? HoTenNguoiMuon { get; set; }

        [Display(Name = "Mã số người mượn")]
        public string? MaSoNguoiMuon { get; set; }

        [Display(Name = "Email người mượn")]
        public string? EmailNguoiMuon { get; set; }

        [Display(Name = "Số điện thoại người mượn")]
        public string? SoDienThoaiNguoiMuon { get; set; }

        [Display(Name = "Loại người mượn")]
        public string? LoaiNguoiMuon { get; set; }

        [Display(Name = "Số lượng mượn")]
        public int SoLuongMuon { get; set; } = 1;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Ngày mượn")]
        public DateTime NgayMuon { get; set; } = DateTime.Now;

        [Display(Name = "Ngày trả dự kiến")]
        public DateTime? NgayTraDuKien { get; set; }

        [Display(Name = "Ngày trả")]
        public DateTime? NgayTra { get; set; }

        [Display(Name = "Tình trạng sau trả")]
        public string? TinhTrangSauTra { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.ChoDuyet;

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Mã thủ thư duyệt")]
        public string? MaThuThuDuyet { get; set; }

        // Foreign Keys
        [ForeignKey("MaTaiLieu")]
        public virtual TaiLieu? TaiLieu { get; set; }

        [ForeignKey("MaNguoiMuon")]
        public virtual NguoiDung? NguoiMuon { get; set; }

        [ForeignKey("MaThuThuDuyet")]
        public virtual NguoiDung? ThuThuDuyet { get; set; }
    }

    public enum TrangThaiPhieu
    {
        ChoDuyet = 0,
        DaDuyet = 1,
        DaTra = 2,
        TuChoi = 3
    }
}
