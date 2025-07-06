using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("LichSuTaiTaiLieu")]
    public class LichSuTaiTaiLieu
    {
        [Key]
        public int MaLichSu { get; set; }

        [Required]
        [Display(Name = "Mã tài liệu")]
        public int MaTaiLieu { get; set; }

        [Required]
        [Display(Name = "Mã người dùng")]
        public string MaNguoiDung { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Thời gian tải")]
        public DateTime ThoiGianTai { get; set; } = DateTime.Now;

        [StringLength(45, ErrorMessage = "Địa chỉ IP không được vượt quá 45 ký tự")]
        [Display(Name = "Địa chỉ IP")]
        public string? DiaChiIP { get; set; }

        [StringLength(200, ErrorMessage = "User Agent không được vượt quá 200 ký tự")]
        [Display(Name = "Thông tin trình duyệt")]
        public string? UserAgent { get; set; }

        // Foreign Keys
        [ForeignKey("MaTaiLieu")]
        public virtual TaiLieu? TaiLieu { get; set; }

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
