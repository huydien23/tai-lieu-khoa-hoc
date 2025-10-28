using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("TaiLieu")]
    public class TaiLieu
    {
        [Display(Name = "Cho phép tải file")]
        public bool ChoPhepTaiFile { get; set; } = false;
        // --- Bài báo khoa học ---
        public string? TieuDe { get; set; }
        public string? TapChiHoiNghi { get; set; }
        public DateTime? NgayCongBo { get; set; }
        public string? DOI { get; set; }
        public string? ISSN { get; set; }
        public string? CapDo { get; set; }

        // --- Đề tài nghiên cứu khoa học ---
        public string? TenDeTai { get; set; }
        public string? MaSoDeTai { get; set; }
        public string? CapDeTai { get; set; }
        public string? ThoiGianThucHien { get; set; }
        public string? CoQuanChuTri { get; set; }
        public string? ChuNhiemDeTai { get; set; }

        // --- Giáo trình - tài liệu giảng dạy ---
        public string? TenGiaoTrinh { get; set; }
        public string? MonHocLienQuan { get; set; }
        public string? DonViPhatHanh { get; set; }
        public int? NamXuatBan { get; set; }
        public int? SoTinChi { get; set; }
        [NotMapped]
        public int LuotMuon
        {
            get { return PhieuMuonTras?.Count ?? 0; }
        }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; } = 1;

        [Display(Name = "Số lượng đã mượn")]
        public int SoLuongDaMuon { get; set; } = 0;

        [NotMapped]
        [Display(Name = "Số lượng còn lại")]
        public int SoLuongConLai => SoLuong - SoLuongDaMuon;

        public virtual ICollection<PhieuMuonTra>? PhieuMuonTras { get; set; }

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

        [Required(ErrorMessage = "Tác giả không được để trống")]
        [StringLength(200, ErrorMessage = "Tên tác giả không được vượt quá 200 ký tự")]
        [Display(Name = "Tác giả")]
        public string TacGia { get; set; } = string.Empty;

        [Display(Name = "Đường dẫn file")]
        public string DuongDanFile { get; set; } = string.Empty;

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


        // Navigation Properties
        [ForeignKey("MaChuyenNganh")]
        public virtual ChuyenNganh? ChuyenNganh { get; set; }

        [ForeignKey("MaLoaiTaiLieu")]
        public virtual LoaiTaiLieu? LoaiTaiLieu { get; set; }


        public virtual ICollection<LichSuTaiTaiLieu> LichSuTaiTaiLieu { get; set; } = new List<LichSuTaiTaiLieu>();
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
