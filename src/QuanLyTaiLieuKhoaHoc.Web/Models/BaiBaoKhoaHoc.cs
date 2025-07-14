using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("BaiBaoKhoaHoc")]
    public class BaiBaoKhoaHoc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        public string? TapChiHoiNghi { get; set; }
        public DateTime? NgayCongBo { get; set; }
        public string? DOI { get; set; }
        public string? ISSN { get; set; }
        public string? CapDo { get; set; }

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
