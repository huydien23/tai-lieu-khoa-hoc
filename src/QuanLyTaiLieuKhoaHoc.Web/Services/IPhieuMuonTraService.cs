using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public interface IPhieuMuonTraService
    {
        Task<bool> GuiYeuCauMuonAsync(int maTaiLieu, string maNguoiMuon, string? ghiChu);
        Task<List<PhieuMuonTra>> LayDanhSachYeuCauChoDuyetAsync();
        Task<bool> DuyetYeuCauMuonAsync(int maPhieu, string maThuThu);
        Task<bool> TuChoiYeuCauMuonAsync(int maPhieu, string maThuThu, string? lyDo);
        Task<bool> XacNhanTraTaiLieuAsync(int maPhieu, string maThuThu);
        Task<List<PhieuMuonTra>> LayLichSuMuonTraCuaNguoiDungAsync(string maNguoiDung);
        Task<PhieuMuonTra?> LayPhieuMuonTraByIdAsync(int maPhieu);
    }
}
