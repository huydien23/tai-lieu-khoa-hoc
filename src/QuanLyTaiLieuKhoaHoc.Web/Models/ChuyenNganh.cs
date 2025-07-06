using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("ChuyenNganh")]
    public class ChuyenNganh
    {
        [Key]
        public int MaChuyenNganh { get; set; }

        [Required(ErrorMessage = "Tên chuyên ngành không được để trống")]
        [StringLength(100, ErrorMessage = "Tên chuyên ngành không được vượt quá 100 ký tự")]
        [Display(Name = "Tên chuyên ngành")]
        public string TenChuyenNganh { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThaiHoatDong { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<TaiLieu> TaiLieu { get; set; } = new List<TaiLieu>();
        public virtual ICollection<NguoiDung> NguoiDung { get; set; } = new List<NguoiDung>();
    }
}
