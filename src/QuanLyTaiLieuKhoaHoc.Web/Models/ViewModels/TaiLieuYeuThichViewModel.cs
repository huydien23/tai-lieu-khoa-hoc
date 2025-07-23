using QuanLyTaiLieuKhoaHoc.Web.Models;
using System;
using System.Collections.Generic;

namespace QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels
{
    public class TaiLieuYeuThichViewModel
    {
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string? MaSoNguoiDung { get; set; }
        public string VaiTroNguoiDung { get; set; } = string.Empty;
        public List<YeuThichTaiLieuItem> DanhSachYeuThich { get; set; } = new();
    }

    public class YeuThichTaiLieuItem
    {
        public int Id { get; set; }
        public int MaTaiLieu { get; set; }
        public string TenTaiLieu { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public string LoaiFile { get; set; } = string.Empty;
        public double KichThuocMB { get; set; }
        public bool ChoPhepTaiFile { get; set; }
        public DateTime NgayYeuThich { get; set; }       
    }
}

