using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("YeuThichTaiLieu")]
    public class YeuThichTaiLieu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaTaiLieu { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Ngày yêu thích")]
        public DateTime NgayYeuThich { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("MaTaiLieu")]
        public virtual TaiLieu? TaiLieu { get; set; }

        [ForeignKey("UserId")]
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}
