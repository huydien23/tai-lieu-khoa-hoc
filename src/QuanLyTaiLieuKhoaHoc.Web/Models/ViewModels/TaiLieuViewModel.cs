using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels
{
    public class TaiLieuViewModel
    {
        [Display(Name = "Cho phép tải file")]
        public bool ChoPhepTaiFile { get; set; } = false;
        public int MaTaiLieu { get; set; }

        [Required(ErrorMessage = "Tên tài liệu không được để trống")]
        [Display(Name = "Tên tài liệu")]
        public string TenTaiLieu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả không được để trống")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tác giả không được để trống")]
        [Display(Name = "Tác giả")]
        public string TacGia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chuyên ngành không được để trống")]
        [Display(Name = "Chuyên ngành")]
        public int MaChuyenNganh { get; set; }

        [Required(ErrorMessage = "Loại tài liệu không được để trống")]
        [Display(Name = "Loại tài liệu")]

        public int MaLoaiTaiLieu { get; set; }
        // Phân loại tài liệu (dùng cho view phân nhóm)
        public string? LoaiTaiLieu { get; set; }

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

        [Display(Name = "File tài liệu")]
        public IFormFile? FileTaiLieu { get; set; }

        // For tracking
        public string? MaNguoiTaiLen { get; set; }

        // Properties for display
        public string? TenChuyenNganh { get; set; }
        public string? TenLoaiTaiLieu { get; set; }
        public string? TenNguoiTaiLen { get; set; }
        public DateTime NgayTaiLen { get; set; }
        public int LuotTai { get; set; }
        // Số lượt mượn (tổng số phiếu mượn)
        public int LuotMuon { get; set; }
        public int TrangThai { get; set; } // 0: ChoDuyet, 1: DaDuyet, 2: TuChoi, 3: An
        public string? DuongDanFile { get; set; }
        public string? LoaiFile { get; set; }
        public long KichThuocFile { get; set; }
        public double KichThuocMB => Math.Round(KichThuocFile / 1024.0, 1);

        public double DiemDanhGiaTrungBinh { get; set; }
        public int SoLuotDanhGia { get; set; }
    }

    public class TaiLieuListViewModel
    {
        public List<TaiLieuViewModel> DanhSachTaiLieu { get; set; } = new List<TaiLieuViewModel>();
        public int TrangHienTai { get; set; } = 1;
        public int TongSoTrang { get; set; } = 1;
        public int TongSoTaiLieu { get; set; } = 0;
        public string? TimKiem { get; set; }
        public int? MaChuyenNganh { get; set; }
        public int? MaLoaiTaiLieu { get; set; }
        public string? SapXep { get; set; }
    }
}

