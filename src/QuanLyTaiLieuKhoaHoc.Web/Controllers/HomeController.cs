using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaiLieuService _taiLieuService;
        private readonly ApplicationDbContext _context;

        public HomeController(ITaiLieuService taiLieuService, ApplicationDbContext context)
        {
            _taiLieuService = taiLieuService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy vai trò người dùng hiện tại
            string? vaiTro = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("SinhVien"))
                    vaiTro = "SinhVien";
                else if (User.IsInRole("GiangVien"))
                    vaiTro = "GiangVien";
                else if (User.IsInRole("ThuThu"))
                    vaiTro = "ThuThu";
            }

            // Truy vấn đúng tên loại tài liệu đã seed
            var loaiBaiBao = await _context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Bài báo khoa học");
            var loaiDeTai = await _context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Đề tài nghiên cứu khoa học");
            var loaiGiaoTrinh = await _context.LoaiTaiLieu.FirstOrDefaultAsync(l => l.TenLoaiTaiLieu == "Giáo trình - Tài liệu giảng dạy");

            var baiBaoList = loaiBaiBao != null ? (await _taiLieuService.GetDanhSachTaiLieuAsync(1, 6, null, null, loaiBaiBao.MaLoaiTaiLieu, null, vaiTro)).DanhSachTaiLieu : new List<TaiLieuViewModel>();
            var deTaiList = loaiDeTai != null ? (await _taiLieuService.GetDanhSachTaiLieuAsync(1, 6, null, null, loaiDeTai.MaLoaiTaiLieu, null, vaiTro)).DanhSachTaiLieu : new List<TaiLieuViewModel>();
            
            // Chỉ hiển thị giáo trình cho Giảng viên và Thủ thư
            var giaoTrinhList = new List<TaiLieuViewModel>();
            if (vaiTro != "SinhVien" && loaiGiaoTrinh != null)
            {
                giaoTrinhList = (await _taiLieuService.GetDanhSachTaiLieuAsync(1, 6, null, null, loaiGiaoTrinh.MaLoaiTaiLieu, null, vaiTro)).DanhSachTaiLieu;
            }

            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotTai = await _context.TaiLieu.SumAsync(t => t.LuotTai),
                TaiLieuMoiTrongThang = await _context.TaiLieu
                    .CountAsync(t => t.NgayTaiLen.Month == DateTime.Now.Month
                                && t.NgayTaiLen.Year == DateTime.Now.Year),

                TaiLieuMoiNhat = await _taiLieuService.GetTaiLieuMoiNhatAsync(6, vaiTro),
                TaiLieuPhoBien = await _taiLieuService.GetTaiLieuPhoBienAsync(6, vaiTro),

                ThongKeTheoChuyenNganh = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),

                ThongKeTheoLoaiTaiLieu = await _context.TaiLieu
                    .Include(t => t.LoaiTaiLieu)
                    .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),

                // Thêm 3 danh sách tài liệu cho từng mục
                BaiBaoKhoaHocList = baiBaoList,
                DeTaiNghienCuuList = deTaiList,
                GiaoTrinhList = giaoTrinhList
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}