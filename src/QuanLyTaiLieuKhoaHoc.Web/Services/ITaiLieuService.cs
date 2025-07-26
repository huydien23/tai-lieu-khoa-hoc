using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public interface ITaiLieuService
    {
        Task<TaiLieuListViewModel> GetDanhSachTaiLieuAsync(int trang = 1, int kichThuocTrang = 10,
            string? timKiem = null, int? maChuyenNganh = null, int? maLoaiTaiLieu = null, string? sapXep = null, string? vaiTro = null);

        Task<TaiLieuViewModel?> GetTaiLieuByIdAsync(int maTaiLieu, string? vaiTro = null);

        Task<bool> TaoTaiLieuAsync(TaiLieuViewModel model, string maNguoiDung);

        Task<bool> CapNhatTaiLieuAsync(TaiLieuViewModel model);

        Task<bool> XoaTaiLieuAsync(int maTaiLieu);

        Task<bool> DuyetTaiLieuAsync(int maTaiLieu, bool duyet);

        Task<string> TaiFileAsync(int maTaiLieu, string maNguoiDung, string? diaChiIP = null);

        Task<List<TaiLieuViewModel>> GetTaiLieuMoiNhatAsync(int soLuong = 10, string? vaiTro = null);

        Task<List<TaiLieuViewModel>> GetTaiLieuPhoBienAsync(int soLuong = 10, string? vaiTro = null);

        Task<TaiLieuListViewModel> GetTaiLieuCuaNguoiDungAsync(string maNguoiDung, int trang = 1, int kichThuocTrang = 10);

        Task<List<TaiLieuViewModel>> GetTaiLieuLienQuanAsync(int maTaiLieu, int soLuong = 2, string? vaiTro = null);
    }
}
