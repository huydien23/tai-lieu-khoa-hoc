using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("NguoiDung")]
    public class NguoiDung : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;


        // Sinh viên: MSSV, lớp
        [StringLength(20, ErrorMessage = "MSSV không được vượt quá 20 ký tự")]
        [Display(Name = "Mã số sinh viên")]
        public string? MSSV { get; set; }

        [StringLength(20, ErrorMessage = "Lớp không được vượt quá 20 ký tự")]
        [Display(Name = "Lớp")]
        public string? Lop { get; set; }

        // Giảng viên: mã GV, bộ môn
        [StringLength(20, ErrorMessage = "Mã giảng viên không được vượt quá 20 ký tự")]
        [Display(Name = "Mã giảng viên")]
        public string? MaGV { get; set; }

        [StringLength(50, ErrorMessage = "Bộ môn không được vượt quá 50 ký tự")]
        [Display(Name = "Bộ môn")]
        public string? BoMon { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [Display(Name = "Vai trò")]
        public VaiTroNguoiDung VaiTro { get; set; }

        [Display(Name = "Chuyên ngành")]
        public int? MaChuyenNganh { get; set; }

        [StringLength(4, ErrorMessage = "Khóa học không được vượt quá 4 ký tự")]
        [Display(Name = "Khóa học")]
        public string? KhoaHoc { get; set; }

        [Required]
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật cuối")]
        public DateTime? NgayCapNhatCuoi { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThaiHoatDong { get; set; } = true;

        // Foreign Keys
        [ForeignKey("MaChuyenNganh")]
        public virtual ChuyenNganh? ChuyenNganh { get; set; }

        // Navigation Properties
        public virtual ICollection<TaiLieu> TaiLieuDaTaiLen { get; set; } = new List<TaiLieu>();
        public virtual ICollection<LichSuTaiTaiLieu> LichSuTaiTaiLieu { get; set; } = new List<LichSuTaiTaiLieu>();
    }

    public enum VaiTroNguoiDung
    {
        [Display(Name = "Thủ thư")]
        ThuThu = 0,

        [Display(Name = "Giảng viên")]
        GiangVien = 1,

        [Display(Name = "Sinh viên")]
        SinhVien = 2
    }
}
