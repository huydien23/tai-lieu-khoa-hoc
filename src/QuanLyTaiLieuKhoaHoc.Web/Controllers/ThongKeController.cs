using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;
using System.Globalization;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITaiLieuService _taiLieuService;
        private readonly IPhieuMuonTraService _phieuMuonTraService;

        public ThongKeController(ApplicationDbContext context, ITaiLieuService taiLieuService, IPhieuMuonTraService phieuMuonTraService)
        {
            _context = context;
            _taiLieuService = taiLieuService;
            _phieuMuonTraService = phieuMuonTraService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Thống kê & Báo cáo";
            
            var viewModel = new DashboardViewModel();
            
            // Thống kê tổng quan
            viewModel.TongSoTaiLieu = await _context.TaiLieu.CountAsync();
            viewModel.TongSoLuotMuon = await _context.PhieuMuonTra.CountAsync();
            viewModel.TongSoNguoiDung = await _context.Users.CountAsync();
            viewModel.TongSoLuotTai = await _context.TaiLieu.SumAsync(t => t.LuotTai);

            // Thống kê theo loại tài liệu
            viewModel.SoLuongBaiBaoKhoaHoc = await _context.TaiLieu.CountAsync(t => t.LoaiTaiLieu.TenLoaiTaiLieu == "Bài báo khoa học");
            viewModel.SoLuongDeTaiNghienCuu = await _context.TaiLieu.CountAsync(t => t.LoaiTaiLieu.TenLoaiTaiLieu == "Đề tài nghiên cứu");
            viewModel.SoLuongGiaoTrinh = await _context.TaiLieu.CountAsync(t => t.LoaiTaiLieu.TenLoaiTaiLieu == "Giáo trình");

            // Thống kê phiếu mượn trả
            viewModel.SoPhieuDangMuon = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaDuyet);
            viewModel.SoPhieuChoDuyet = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.ChoDuyet);
            viewModel.SoPhieuDaTra = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra);
            viewModel.SoPhieuTuChoi = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.TuChoi);

            // Thống kê theo chuyên ngành
            var thongKeChuyenNganh = await _context.TaiLieu
                .GroupBy(t => t.ChuyenNganh.TenChuyenNganh)
                .Select(g => new { ChuyenNganh = g.Key, SoLuong = g.Count() })
                .ToListAsync();
            
            viewModel.ThongKeTheoChuyenNganh = thongKeChuyenNganh.ToDictionary(x => x.ChuyenNganh, x => x.SoLuong);

            // Thống kê theo loại tài liệu
            var thongKeLoaiTaiLieu = await _context.TaiLieu
                .GroupBy(t => t.LoaiTaiLieu.TenLoaiTaiLieu)
                .Select(g => new { LoaiTaiLieu = g.Key, SoLuong = g.Count() })
                .ToListAsync();
            
            viewModel.ThongKeTheoLoaiTaiLieu = thongKeLoaiTaiLieu.ToDictionary(x => x.LoaiTaiLieu, x => x.SoLuong);

            // Thống kê yêu thích
            viewModel.SoLuongYeuThich = await _context.YeuThichTaiLieu.CountAsync();

            // Thống kê lượt xem gần đây (sử dụng LuotTai thay vì SoLuotXem)
            viewModel.SoLuotXemGanDay = await _context.TaiLieu.SumAsync(t => t.LuotTai);

            // Thống kê lượt tải
            viewModel.LuotTaiTrongTuan = await _context.TaiLieu.SumAsync(t => t.LuotTai);

            // Lịch sử mượn trả gần đây
            viewModel.LichSuMuonTra = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .OrderByDescending(p => p.NgayMuon)
                .Take(10)
                .ToListAsync();

            // Thống kê theo tháng
            var currentYear = DateTime.Now.Year;
            var monthlyStats = new Dictionary<string, int>();
            
            for (int month = 1; month <= 12; month++)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                var borrowCount = await _context.PhieuMuonTra
                    .CountAsync(p => p.NgayMuon.Year == currentYear && p.NgayMuon.Month == month);
                monthlyStats[monthName] = borrowCount;
            }
            ViewBag.MonthlyStats = monthlyStats;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DocumentStats()
        {
            var stats = new
            {
                totalDocuments = await _context.TaiLieu.CountAsync(),
                totalBorrows = await _context.PhieuMuonTra.CountAsync(),
                totalUsers = await _context.Users.CountAsync(),
                popularCategories = await _context.TaiLieu
                    .GroupBy(t => t.ChuyenNganh.TenChuyenNganh)
                    .Select(g => new { name = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Take(5)
                    .ToListAsync(),
                documentTypes = await _context.TaiLieu
                    .GroupBy(t => t.LoaiTaiLieu.TenLoaiTaiLieu)
                    .Select(g => new { name = g.Key, count = g.Count() })
                    .ToListAsync(),
                recentDocuments = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .OrderByDescending(t => t.NgayTaiLen)
                    .Take(10)
                    .Select(t => new { 
                        t.TenTaiLieu, 
                        ChuyenNganh = t.ChuyenNganh.TenChuyenNganh,
                        t.LuotTai,
                        NgayTao = t.NgayTaiLen
                    })
                    .ToListAsync()
            };

            return Json(stats);
        }

        [HttpGet]
        public async Task<IActionResult> BorrowStats(string period = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            var currentDate = DateTime.Now;
            var data = new List<int>();
            var labels = new List<string>();

            // Handle custom date range
            if (startDate.HasValue && endDate.HasValue)
            {
                var days = (endDate.Value - startDate.Value).Days;
                for (int i = 0; i <= days; i++)
                {
                    var date = startDate.Value.AddDays(i);
                    var count = await _context.PhieuMuonTra
                        .CountAsync(p => p.NgayMuon.Date == date.Date);
                    data.Add(count);
                    labels.Add(date.ToString("dd/MM"));
                }
                return Json(new { data, labels, period = "custom" });
            }

            switch (period)
            {
                case "7days":
                    for (int i = 6; i >= 0; i--)
                    {
                        var date = currentDate.AddDays(-i);
                        var count = await _context.PhieuMuonTra
                            .CountAsync(p => p.NgayMuon.Date == date.Date);
                        data.Add(count);
                        labels.Add(date.ToString("dd/MM"));
                    }
                    break;

                case "30days":
                    for (int i = 29; i >= 0; i--)
                    {
                        var date = currentDate.AddDays(-i);
                        var count = await _context.PhieuMuonTra
                            .CountAsync(p => p.NgayMuon.Date == date.Date);
                        data.Add(count);
                        labels.Add(date.ToString("dd/MM"));
                    }
                    break;

                case "3months":
                    for (int i = 89; i >= 0; i--)
                    {
                        var date = currentDate.AddDays(-i);
                        var count = await _context.PhieuMuonTra
                            .CountAsync(p => p.NgayMuon.Date == date.Date);
                        data.Add(count);
                        labels.Add(date.ToString("dd/MM"));
                    }
                    break;

                case "6months":
                    for (int i = 179; i >= 0; i--)
                    {
                        var date = currentDate.AddDays(-i);
                        var count = await _context.PhieuMuonTra
                            .CountAsync(p => p.NgayMuon.Date == date.Date);
                        data.Add(count);
                        labels.Add(date.ToString("dd/MM"));
                    }
                    break;

                case "1year":
                    for (int i = 364; i >= 0; i--)
                    {
                        var date = currentDate.AddDays(-i);
                        var count = await _context.PhieuMuonTra
                            .CountAsync(p => p.NgayMuon.Date == date.Date);
                        data.Add(count);
                        labels.Add(date.ToString("dd/MM"));
                    }
                    break;

                case "year":
                default:
                    for (int month = 1; month <= 12; month++)
                    {
                        var count = await _context.PhieuMuonTra
                            .CountAsync(p => p.NgayMuon.Year == currentDate.Year && p.NgayMuon.Month == month);
                        data.Add(count);
                        labels.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month));
                    }
                    break;
            }

            return Json(new { data, labels, period });
        }

        [HttpGet]
        public async Task<IActionResult> UserStats()
        {
            var stats = new
            {
                totalUsers = await _context.Users.CountAsync(),
                userByRole = await _context.Users
                    .GroupBy(u => u.VaiTro)
                    .Select(g => new { role = g.Key, count = g.Count() })
                    .ToListAsync(),
                recentActivity = await _context.PhieuMuonTra
                    .Include(p => p.NguoiMuon)
                    .Include(p => p.TaiLieu)
                    .OrderByDescending(p => p.NgayMuon)
                    .Take(10)
                    .Select(p => new {
                        user = p.NguoiMuon.HoTen,
                        document = p.TaiLieu.TenTaiLieu,
                        action = p.TrangThai.ToString(),
                        date = p.NgayMuon
                    })
                    .ToListAsync(),
                favoriteDocuments = await _context.YeuThichTaiLieu
                    .Include(y => y.TaiLieu)
                    .Include(y => y.NguoiDung)
                    .GroupBy(y => y.TaiLieu.TenTaiLieu)
                    .Select(g => new { 
                        document = g.Key, 
                        count = g.Count(),
                        users = g.Select(y => y.NguoiDung.HoTen).ToList()
                    })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToListAsync()
            };

            return Json(stats);
        }

        [HttpGet]
        public async Task<IActionResult> ExportReport(string type, string format = "excel")
        {
            try
            {
                var data = type switch
                {
                    "documents" => await GetDocumentReportData(),
                    "borrow" => await GetBorrowReportData(),
                    "users" => await GetUserReportData(),
                    "downloads" => await GetDownloadReportData(),
                    "all" => new
                    {
                        documents = await GetDocumentReportData(),
                        borrow = await GetBorrowReportData(),
                        users = await GetUserReportData(),
                        downloads = await GetDownloadReportData()
                    },
                    _ => null
                };

                if (data != null)
                {
                    // Simulate export process
                    await Task.Delay(2000); // Simulate processing time
                    
                    TempData["SuccessMessage"] = $"Đã xuất báo cáo {type} thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Loại báo cáo không hợp lệ!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xuất báo cáo: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        private async Task<object> GetDocumentReportData()
        {
            return await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Select(t => new
                {
                    t.TenTaiLieu,
                    ChuyenNganh = t.ChuyenNganh.TenChuyenNganh,
                    LoaiTaiLieu = t.LoaiTaiLieu.TenLoaiTaiLieu,
                    t.LuotTai,
                    NgayTao = t.NgayTaiLen
                })
                .ToListAsync();
        }

        private async Task<object> GetBorrowReportData()
        {
            return await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .Select(p => new
                {
                    TaiLieu = p.TaiLieu.TenTaiLieu,
                    NguoiMuon = p.NguoiMuon.HoTen,
                    p.NgayMuon,
                    p.NgayTraDuKien,
                    p.NgayTra,
                    p.TrangThai
                })
                .ToListAsync();
        }

        private async Task<object> GetUserReportData()
        {
            return await _context.Users
                .Select(u => new
                {
                    u.HoTen,
                    u.Email,
                    u.VaiTro,
                    NgayTao = u.NgayTao,
                    SoPhieuMuon = _context.PhieuMuonTra.Count(p => p.MaNguoiMuon == u.Id)
                })
                .ToListAsync();
        }

        private async Task<object> GetDownloadReportData()
        {
            return await _context.LichSuTaiTaiLieu
                .Include(l => l.NguoiDung)
                .Include(l => l.TaiLieu)
                    .ThenInclude(t => t.LoaiTaiLieu)
                .OrderByDescending(l => l.ThoiGianTai)
                .Select(l => new
                {
                    NguoiTai = l.NguoiDung.HoTen,
                    Email = l.NguoiDung.Email,
                    VaiTro = l.NguoiDung.VaiTro,
                    TaiLieu = l.TaiLieu.TenTaiLieu,
                    TacGia = l.TaiLieu.TacGia,
                    LoaiTaiLieu = l.TaiLieu.LoaiTaiLieu.TenLoaiTaiLieu,
                    ThoiGianTai = l.ThoiGianTai,
                    DiaChiIP = l.DiaChiIP
                })
                .ToListAsync();
        }
    }
}
