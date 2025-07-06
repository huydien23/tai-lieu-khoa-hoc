using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class TimKiemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimKiemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? q = null, int? chuyenNganh = null, int? loaiTaiLieu = null, int trang = 1)
        {
            ViewData["Title"] = "Tìm kiếm tài liệu";

            var query = _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.NguoiTaiLen)
                .Where(t => t.TrangThai == TrangThaiTaiLieu.DaDuyet);

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(t => t.TenTaiLieu.Contains(q) || t.MoTa.Contains(q));
                ViewBag.TimKiem = q;
            }

            // Lọc theo chuyên ngành
            if (chuyenNganh.HasValue)
            {
                query = query.Where(t => t.MaChuyenNganh == chuyenNganh.Value);
                ViewBag.ChuyenNganhSelected = chuyenNganh.Value;
            }

            // Lọc theo loại tài liệu
            if (loaiTaiLieu.HasValue)
            {
                query = query.Where(t => t.MaLoaiTaiLieu == loaiTaiLieu.Value);
                ViewBag.LoaiTaiLieuSelected = loaiTaiLieu.Value;
            }

            // Phân trang
            const int kichThuocTrang = 12;
            var tongSoKetQua = await query.CountAsync();
            var taiLieu = await query
                .OrderByDescending(t => t.NgayTaiLen)
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = (int)Math.Ceiling((double)tongSoKetQua / kichThuocTrang);
            ViewBag.TongSoKetQua = tongSoKetQua;

            // Dropdown data
            ViewBag.ChuyenNganh = await _context.ChuyenNganh
                .Where(cn => cn.TrangThaiHoatDong)
                .ToListAsync();

            ViewBag.LoaiTaiLieu = await _context.LoaiTaiLieu
                .Where(lt => lt.TrangThaiHoatDong)
                .ToListAsync();

            return View(taiLieu);
        }

        [HttpGet]
        public async Task<IActionResult> Suggestions(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new string[0]);
            }

            var suggestions = await _context.TaiLieu
                .Where(t => t.TrangThai == TrangThaiTaiLieu.DaDuyet && t.TenTaiLieu.Contains(term))
                .Select(t => t.TenTaiLieu)
                .Distinct()
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }
    }
}
