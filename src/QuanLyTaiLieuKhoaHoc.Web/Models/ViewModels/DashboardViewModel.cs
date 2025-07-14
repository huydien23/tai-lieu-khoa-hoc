using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TongSoTaiLieu { get; set; }
        public int TongSoLuotMuon { get; set; }
        // Thống kê số lượng từng nhóm tài liệu lớn
        public int SoLuongBaiBaoKhoaHoc { get; set; }
        public int SoLuongDeTaiNghienCuu { get; set; }
        public int SoLuongGiaoTrinh { get; set; }
        public int TongSoNguoiDung { get; set; }
        public int TongSoLuotTai { get; set; }
        public int TaiLieuMoiTrongThang { get; set; }

        public List<TaiLieuViewModel> TaiLieuMoiNhat { get; set; } = new List<TaiLieuViewModel>();
        public List<TaiLieuViewModel> TaiLieuPhoBien { get; set; } = new List<TaiLieuViewModel>();

        // Danh sách tài liệu cho từng mục trên trang chủ
        public List<TaiLieuViewModel> BaiBaoKhoaHocList { get; set; } = new();
        public List<TaiLieuViewModel> DeTaiNghienCuuList { get; set; } = new();
        public List<TaiLieuViewModel> GiaoTrinhList { get; set; } = new();

        public Dictionary<string, int> ThongKeTheoChuyenNganh { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ThongKeTheoLoaiTaiLieu { get; set; } = new Dictionary<string, int>();
    }

    public class ThongKeViewModel
    {
        public int TongSoTaiLieu { get; set; }
        public int TongSoNguoiDung { get; set; }
        public int TongSoLuotTai { get; set; }
        public int TaiLieuTrongThang { get; set; }
        public int NguoiDungMoiTrongThang { get; set; }

        public List<ThongKeTheoNgay> BieuDoLuotTai { get; set; } = new List<ThongKeTheoNgay>();
        public List<ThongKeTheoChuyenNganh> BieuDoChuyenNganh { get; set; } = new List<ThongKeTheoChuyenNganh>();
        public List<ThongKeTheoLoaiTaiLieu> BieuDoLoaiTaiLieu { get; set; } = new List<ThongKeTheoLoaiTaiLieu>();
    }

    public class ThongKeTheoNgay
    {
        public DateTime Ngay { get; set; }
        public int SoLuotTai { get; set; }
        public int SoTaiLieuMoi { get; set; }
    }

    public class ThongKeTheoChuyenNganh
    {
        public string TenChuyenNganh { get; set; } = string.Empty;
        public int SoLuongTaiLieu { get; set; }
        public int SoLuotTai { get; set; }
    }

    public class ThongKeTheoLoaiTaiLieu
    {
        public string TenLoaiTaiLieu { get; set; } = string.Empty;
        public int SoLuongTaiLieu { get; set; }
        public int SoLuotTai { get; set; }
    }
}
