using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using System;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public interface IPhieuMuonTraService
    {
        Task<bool> GuiYeuCauMuonAsync(int maTaiLieu, string maNguoiMuon, string? ghiChu);
        Task<List<PhieuMuonTra>> LayDanhSachYeuCauChoDuyetAsync();
        Task<bool> DuyetYeuCauMuonAsync(int maPhieu, string maThuThu);
        Task<bool> TuChoiYeuCauMuonAsync(int maPhieu, string maThuThu, string? lyDo);
        Task<bool> XacNhanTraTaiLieuAsync(int maPhieu, string maThuThu);
        Task<bool> TraTaiLieuAsync(int maPhieu, string maThuThu, DateTime ngayTra, string? ghiChu, string tinhTrang);
        Task<List<PhieuMuonTra>> LayLichSuMuonTraCuaNguoiDungAsync(string maNguoiDung);
        Task<PhieuMuonTra?> LayPhieuMuonTraByIdAsync(int maPhieu);
        Task<bool> LapPhieuMuonAsync(int maPhieu, string maThuThu, DateTime ngayMuon, DateTime ngayTraDuKien);
        
        // Chức năng báo tài liệu quá hạn
        Task<List<PhieuMuonTra>> LayDanhSachTaiLieuQuaHanAsync();
        Task<int> DemSoTaiLieuQuaHanAsync();
        Task<List<PhieuMuonTra>> LayTaiLieuQuaHanTheoNguoiDungAsync(string maNguoiDung);
    }
}
