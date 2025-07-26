using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using System;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public class PhieuMuonTraService : IPhieuMuonTraService
    {
        private readonly ApplicationDbContext _context;
        public PhieuMuonTraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> GuiYeuCauMuonAsync(int maTaiLieu, string maNguoiMuon, string? ghiChu)
        {
            // Chặn gửi yêu cầu trùng khi chưa được duyệt hoặc chưa trả
            var daCoYeuCau = await _context.PhieuMuonTra.AnyAsync(p =>
                p.MaTaiLieu == maTaiLieu &&
                p.MaNguoiMuon == maNguoiMuon &&
                (p.TrangThai == TrangThaiPhieu.ChoDuyet || p.TrangThai == TrangThaiPhieu.DaDuyet));
            if (daCoYeuCau) return false;

            var phieu = new PhieuMuonTra
            {
                MaTaiLieu = maTaiLieu,
                MaNguoiMuon = maNguoiMuon,
                GhiChu = ghiChu,
                TrangThai = TrangThaiPhieu.ChoDuyet
            };
            _context.PhieuMuonTra.Add(phieu);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PhieuMuonTra>> LayDanhSachYeuCauChoDuyetAsync()
        {
            return await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .Where(p => p.TrangThai == TrangThaiPhieu.ChoDuyet)
                .ToListAsync();
        }

        public async Task<bool> DuyetYeuCauMuonAsync(int maPhieu, string maThuThu)
        {
            var phieu = await _context.PhieuMuonTra.FindAsync(maPhieu);
            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.ChoDuyet) return false;
            phieu.TrangThai = TrangThaiPhieu.DaDuyet;
            phieu.MaThuThuDuyet = maThuThu;
            phieu.NgayMuon = System.DateTime.Now;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> TuChoiYeuCauMuonAsync(int maPhieu, string maThuThu, string? lyDo)
        {
            var phieu = await _context.PhieuMuonTra.FindAsync(maPhieu);
            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.ChoDuyet) return false;
            phieu.TrangThai = TrangThaiPhieu.TuChoi;
            phieu.MaThuThuDuyet = maThuThu;
            phieu.GhiChu = lyDo;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> XacNhanTraTaiLieuAsync(int maPhieu, string maThuThu)
        {
            var phieu = await _context.PhieuMuonTra.FindAsync(maPhieu);
            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.DaDuyet) return false;
            phieu.TrangThai = TrangThaiPhieu.DaTra;
            phieu.NgayTra = System.DateTime.Now;
            phieu.MaThuThuDuyet = maThuThu;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> TraTaiLieuAsync(int maPhieu, string maThuThu, DateTime ngayTra, string? ghiChu, string tinhTrang)
        {
            var phieu = await _context.PhieuMuonTra.FindAsync(maPhieu);
            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.DaDuyet) return false;
            phieu.TrangThai = TrangThaiPhieu.DaTra;
            phieu.NgayTra = ngayTra;
            phieu.GhiChu = ghiChu;
            phieu.MaThuThuDuyet = maThuThu;
            phieu.TinhTrangSauTra = tinhTrang;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LapPhieuMuonAsync(int maPhieu, string maThuThu, DateTime ngayMuon, DateTime ngayTraDuKien)
        {
            var phieu = await _context.PhieuMuonTra.FindAsync(maPhieu);
            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.ChoDuyet) return false;
            phieu.TrangThai = TrangThaiPhieu.DaDuyet;
            phieu.MaThuThuDuyet = maThuThu;
            phieu.NgayMuon = ngayMuon;
            phieu.NgayTraDuKien = ngayTraDuKien;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PhieuMuonTra>> LayLichSuMuonTraCuaNguoiDungAsync(string maNguoiDung)
        {
            return await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Where(p => p.MaNguoiMuon == maNguoiDung)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();
        }

        public async Task<PhieuMuonTra?> LayPhieuMuonTraByIdAsync(int maPhieu)
        {
            return await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .Include(p => p.ThuThuDuyet)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);
        }
    }
}
