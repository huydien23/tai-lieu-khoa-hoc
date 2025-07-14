using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("DeTaiNghienCuu")]
    public class DeTaiNghienCuu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TenDeTai { get; set; } = string.Empty;
        public string? MaSoDeTai { get; set; }
        public string? CapDeTai { get; set; }
        public string? ThoiGianThucHien { get; set; }
        public string? CoQuanChuTri { get; set; }
        public string? ChuNhiemDeTai { get; set; }

        [Required]
        public string TacGia { get; set; } = string.Empty;
        public string DuongDanFile { get; set; } = string.Empty;
        public long KichThuocFile { get; set; }
        public DateTime NgayTaiLen { get; set; } = DateTime.Now;
        public int LuotTai { get; set; } = 0;
        public int MaChuyenNganh { get; set; }
        public int MaLoaiTaiLieu { get; set; }
        public string MaNguoiTaiLen { get; set; } = string.Empty;
    }
}
