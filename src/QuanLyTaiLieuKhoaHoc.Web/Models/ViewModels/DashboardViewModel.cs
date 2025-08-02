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

        public List<YeuThichTaiLieu> TaiLieuYeuThich { get; set; } = new();
        public int SoLuongYeuThich { get; set; }
        public int SoPhieuDangMuon { get; set; }
        public int SoPhieuChoDuyet { get; set; }
        public int SoPhieuDaTra { get; set; }
        public int SoPhieuTuChoi { get; set; }

        public int SoLuotXemGanDay { get; set; }
        public int LuotTaiTrongTuan { get; set; }
        public int LuotTaiTuanTruoc { get; set; }

        public List<TaiLieuViewModel> TaiLieuXemGanDay { get; set; } = new();
        public List<PhieuMuonTra> TaiLieuDangMuon { get; set; } = new List<PhieuMuonTra>();
      

        // Danh sách yêu cầu mượn chờ duyệt
        public IEnumerable<QuanLyTaiLieuKhoaHoc.Web.Models.PhieuMuonTra> YeuCauMuonTra { get; set; } = new List<QuanLyTaiLieuKhoaHoc.Web.Models.PhieuMuonTra>();

        // Lịch sử mượn trả
        public IEnumerable<QuanLyTaiLieuKhoaHoc.Web.Models.PhieuMuonTra> LichSuMuonTra { get; set; } = new List<QuanLyTaiLieuKhoaHoc.Web.Models.PhieuMuonTra>();
        public IEnumerable<PhieuMuonTra> PhieuMuonTra { get; set; } = new List<PhieuMuonTra>();
        public int SoLuotTaiXuong { get; set; }
        public List<QuanLyTaiLieuKhoaHoc.Web.Models.LichSuTaiTaiLieu> LichSuTaiXuong { get; set; } = new();

        // Hoạt động hệ thống
        public List<SystemActivity> HoạtĐộngHệThống { get; set; } = new List<SystemActivity>();
    }

    public class SystemActivity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ActivityType LoạiHoạtĐộng { get; set; }
        public string TiêuĐề { get; set; } = string.Empty;
        public string MôTả { get; set; } = string.Empty;
        public DateTime ThờiGian { get; set; }
        public ActivityPriority MứcĐộƯuTiên { get; set; }
        public string? DữLiệuBổSung { get; set; }
        public bool ĐãXửLý { get; set; } = false;
        public string? NgườiThựcHiện { get; set; }
        public string? LiênKết { get; set; }
    }

    public enum ActivityType
    {
        NgườiDùngMớiĐăngKý,
        YêuCầuMượnTàiLiệu,
        TàiLiệuĐượcTrả,
        CảnhBáoQuáHạn,
        TàiLiệuQuáHạn,
        TàiLiệuMớiĐượcThêm,
        ThốngKêHoạtĐộng,
        ThôngBáoHệThống,
        BulkUserActivation
    }

    public enum ActivityPriority
    {
        Thấp,
        TrungBình,
        Cao
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
