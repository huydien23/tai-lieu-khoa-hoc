using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("DanhGiaTaiLieu")]
    public class DanhGiaTaiLieu
    {
        [Key]
        public int MaDanhGia { get; set; }

        [Required]
        [Display(Name = "Mã tài liệu")]
        public int MaTaiLieu { get; set; }

        [Required]
        [Display(Name = "Mã người dùng")]
        public string MaNguoiDung { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5")]
        [Display(Name = "Điểm đánh giá")]
        public int DiemDanhGia { get; set; }

        [StringLength(500, ErrorMessage = "Nhận xét không được vượt quá 500 ký tự")]
        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Required]
        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Foreign Keys
        [ForeignKey("MaTaiLieu")]
        public virtual TaiLieu? TaiLieu { get; set; }

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
