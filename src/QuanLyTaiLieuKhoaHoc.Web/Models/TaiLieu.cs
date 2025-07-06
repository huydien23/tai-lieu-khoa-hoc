using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("TaiLieu")]
    public class TaiLieu
    {
        [Key]
        public int MaTaiLieu { get; set; }

        [Required(ErrorMessage = "Tên tài liệu không được để trống")]
        [StringLength(200, ErrorMessage = "Tên tài liệu không được vượt quá 200 ký tự")]
        [Display(Name = "Tên tài liệu")]
        public string TenTaiLieu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả không được để trống")]
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đường dẫn file không được để trống")]
        [Display(Name = "Đường dẫn file")]
        public string DuongDanFile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại file không được để trống")]
        [StringLength(10, ErrorMessage = "Loại file không được vượt quá 10 ký tự")]
        [Display(Name = "Loại file")]
        public string LoaiFile { get; set; } = string.Empty;

        [Display(Name = "Kích thước file (KB)")]
        public long KichThuocFile { get; set; }

        [Required]
        [Display(Name = "Ngày tải lên")]
        public DateTime NgayTaiLen { get; set; } = DateTime.Now;

        [Display(Name = "Lượt tải")]
        public int LuotTai { get; set; } = 0;

        [Display(Name = "Trạng thái")]
        public TrangThaiTaiLieu TrangThai { get; set; } = TrangThaiTaiLieu.ChoDuyet;

        // Foreign Keys
        [Required(ErrorMessage = "Chuyên ngành không được để trống")]
        [Display(Name = "Chuyên ngành")]
        public int MaChuyenNganh { get; set; }

        [Required(ErrorMessage = "Loại tài liệu không được để trống")]
        [Display(Name = "Loại tài liệu")]
        public int MaLoaiTaiLieu { get; set; }

        [Required(ErrorMessage = "Người tải lên không được để trống")]
        [Display(Name = "Người tải lên")]
        public string MaNguoiTaiLen { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("MaChuyenNganh")]
        public virtual ChuyenNganh? ChuyenNganh { get; set; }

        [ForeignKey("MaLoaiTaiLieu")]
        public virtual LoaiTaiLieu? LoaiTaiLieu { get; set; }

        [ForeignKey("MaNguoiTaiLen")]
        public virtual NguoiDung? NguoiTaiLen { get; set; }

        public virtual ICollection<LichSuTaiTaiLieu> LichSuTaiTaiLieu { get; set; } = new List<LichSuTaiTaiLieu>();
        public virtual ICollection<DanhGiaTaiLieu> DanhGiaTaiLieu { get; set; } = new List<DanhGiaTaiLieu>();
    }

    public enum TrangThaiTaiLieu
    {
        [Display(Name = "Chờ duyệt")]
        ChoDuyet = 0,

        [Display(Name = "Đã duyệt")]
        DaDuyet = 1,

        [Display(Name = "Từ chối")]
        TuChoi = 2,

        [Display(Name = "Ẩn")]
        An = 3
    }
}
