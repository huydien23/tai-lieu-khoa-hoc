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

        [StringLength(20, ErrorMessage = "Mã số không được vượt quá 20 ký tự")]
        [Display(Name = "Mã số (Sinh viên/Giảng viên)")]
        public string? MaSo { get; set; }

        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

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

        public virtual ICollection<TaiLieu> TaiLieuDaTaiLen { get; set; } = new List<TaiLieu>();
        public virtual ICollection<LichSuTaiTaiLieu> LichSuTaiTaiLieu { get; set; } = new List<LichSuTaiTaiLieu>();
        public virtual ICollection<YeuThichTaiLieu> TaiLieuYeuThich { get; set; } = new List<YeuThichTaiLieu>();

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
