using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("LoaiTaiLieu")]
    public class LoaiTaiLieu
    {
        [Key]
        public int MaLoaiTaiLieu { get; set; }

        [Required(ErrorMessage = "Tên loại tài liệu không được để trống")]
        [StringLength(100, ErrorMessage = "Tên loại tài liệu không được vượt quá 100 ký tự")]
        [Display(Name = "Tên loại tài liệu")]
        public string TenLoaiTaiLieu { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Mô tả không được vượt quá 300 ký tự")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [StringLength(50, ErrorMessage = "Biểu tượng không được vượt quá 50 ký tự")]
        [Display(Name = "Biểu tượng")]
        public string? BieuTuong { get; set; }

        [Required]
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThaiHoatDong { get; set; } = true;

        public virtual ICollection<TaiLieu> TaiLieu { get; set; } = new List<TaiLieu>();
    }
}
