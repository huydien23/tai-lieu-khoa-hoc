using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    [Table("GiaoTrinh")]
    public class GiaoTrinh
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TenGiaoTrinh { get; set; } = string.Empty;
        public string? MonHocLienQuan { get; set; }
        public string? DonViPhatHanh { get; set; }
        public int? NamXuatBan { get; set; }
        public int? SoTinChi { get; set; }

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
