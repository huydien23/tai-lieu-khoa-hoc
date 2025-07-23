using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    [Authorize]
    public class LibrarianController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITaiLieuService _taiLieuService;
        private readonly UserManager<NguoiDung> _userManager;

        public LibrarianController(ApplicationDbContext context, ITaiLieuService taiLieuService, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _taiLieuService = taiLieuService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ManageBorrowReturn()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách yêu cầu mượn chờ duyệt
            var yeuCauMuonTra = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .Where(p => p.TrangThai == TrangThaiPhieu.ChoDuyet)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();

            // Lấy lịch sử mượn trả (tất cả phiếu đã duyệt hoặc đã trả)
            var lichSuMuonTra = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .Where(p => p.TrangThai == TrangThaiPhieu.DaDuyet || p.TrangThai == TrangThaiPhieu.DaTra)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();

            // Lấy danh sách tài liệu đang mượn (phiếu DaDuyet và chưa trả)
            var taiLieuDangMuon = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .Where(p => p.TrangThai == TrangThaiPhieu.DaDuyet && p.NgayTra == null)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                YeuCauMuonTra = yeuCauMuonTra,
                LichSuMuonTra = lichSuMuonTra,
                TaiLieuDangMuon = taiLieuDangMuon
            };
            ViewData["Title"] = "Quản lý mượn trả";
            return View("ManageBorrowReturn", model);
        }

        // API tìm kiếm thành viên hệ thống cho autocomplete phiếu mượn
        [HttpGet]
        public async Task<IActionResult> SearchMember(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Json(Array.Empty<object>());

            var users = await _context.Users
                .Where(u => (u.MaSo != null && u.MaSo.Contains(q)) || (u.HoTen != null && u.HoTen.Contains(q)) || (u.Email != null && u.Email.Contains(q)))
                .Select(u => new
                {
                    maSo = u.MaSo,
                    hoTen = u.HoTen,
                    email = u.Email,
                    sdt = u.PhoneNumber,
                    loai = u.VaiTro // Ví dụ: "SinhVien", "GiangVien", "ThuThu"
                })
                .Take(10)
                .ToListAsync();

            return Json(users);
        }

        [ActionName("Dashboard-Librarian")]
        public async Task<IActionResult> Dashboard()
        {
            // Kiểm tra quyền Thủ thư
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotMuon = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaDuyet || p.TrangThai == TrangThaiPhieu.DaTra),
                TaiLieuMoiTrongThang = await _context.TaiLieu
                    .CountAsync(t => t.NgayTaiLen.Month == DateTime.Now.Month
                                && t.NgayTaiLen.Year == DateTime.Now.Year),
                TaiLieuMoiNhat = await _taiLieuService.GetTaiLieuMoiNhatAsync(10),
                TaiLieuPhoBien = await _taiLieuService.GetTaiLieuPhoBienAsync(10),
                ThongKeTheoChuyenNganh = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                ThongKeTheoLoaiTaiLieu = await _context.TaiLieu
                    .Include(t => t.LoaiTaiLieu)
                    .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                    .ToDictionaryAsync(g => g.Key, g => g.Count())
            };

            ViewData["Title"] = "Dashboard Thủ thư";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }
            var users = await _context.Users
                .Include(u => u.ChuyenNganh)
                .OrderBy(u => u.HoTen)
                .ToListAsync();

            ViewData["Title"] = "Quản lý Người dùng";
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDocuments()
        {
            var documents = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .OrderByDescending(t => t.NgayTaiLen)
                .ToListAsync();

            ViewData["Title"] = "Quản lý Tài liệu";
            return View(documents);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDocument(int id)
        {
            var document = await _context.TaiLieu.Where(t => t.MaTaiLieu == id).FirstOrDefaultAsync();
            if (document != null)
            {
                document.TrangThai = TrangThaiTaiLieu.DaDuyet;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã duyệt tài liệu thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài liệu!";
            }

            return RedirectToAction(nameof(ManageDocuments));
        }

        [HttpPost]
        public async Task<IActionResult> RejectDocument(int id, string? lyDo)
        {
            var document = await _context.TaiLieu.Where(t => t.MaTaiLieu == id).FirstOrDefaultAsync();
            if (document != null)
            {
                document.TrangThai = TrangThaiTaiLieu.TuChoi;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã từ chối tài liệu!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài liệu!";
            }

            return RedirectToAction(nameof(ManageDocuments));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.TrangThaiHoatDong = !user.TrangThaiHoatDong;
                    user.NgayCapNhatCuoi = DateTime.Now;
                    await _context.SaveChangesAsync();

                    string status = user.TrangThaiHoatDong ? "kích hoạt" : "vô hiệu hóa";
                    return Json(new { success = true, message = $"Đã {status} tài khoản '{user.HoTen}' thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // QUẢN LÝ CHUYÊN NGÀNH
        [HttpGet]
        public async Task<IActionResult> ManageCategories()
        {
            var categories = await _context.ChuyenNganh
                .Include(c => c.TaiLieu)
                .Include(c => c.NguoiDung)
                .OrderBy(c => c.TenChuyenNganh)
                .ToListAsync();

            ViewData["Title"] = "Quản lý Chuyên ngành";
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            ViewData["Title"] = "Thêm chuyên ngành mới";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(ChuyenNganh model)
        {
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                _context.ChuyenNganh.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm chuyên ngành thành công!";
                return RedirectToAction(nameof(ManageCategories));
            }

            ViewData["Title"] = "Thêm chuyên ngành mới";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.ChuyenNganh.Where(c => c.MaChuyenNganh == id).FirstOrDefaultAsync();
            if (category == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chuyên ngành!";
                return RedirectToAction(nameof(ManageCategories));
            }

            ViewData["Title"] = "Chỉnh sửa chuyên ngành";
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(ChuyenNganh model)
        {
            if (ModelState.IsValid)
            {
                _context.ChuyenNganh.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật chuyên ngành thành công!";
                return RedirectToAction(nameof(ManageCategories));
            }

            ViewData["Title"] = "Chỉnh sửa chuyên ngành";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCategoryStatus(int id)
        {
            var category = await _context.ChuyenNganh.Where(c => c.MaChuyenNganh == id).FirstOrDefaultAsync();
            if (category != null)
            {
                category.TrangThaiHoatDong = !category.TrangThaiHoatDong;
                await _context.SaveChangesAsync();

                string status = category.TrangThaiHoatDong ? "kích hoạt" : "vô hiệu hóa";
                TempData["SuccessMessage"] = $"Đã {status} chuyên ngành {category.TenChuyenNganh} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy chuyên ngành!";
            }

            return RedirectToAction(nameof(ManageCategories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.ChuyenNganh
                    .Include(c => c.TaiLieu)
                    .Include(c => c.NguoiDung)
                    .Where(c => c.MaChuyenNganh == id)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy chuyên ngành!" });
                }

                // Kiểm tra xem có tài liệu nào thuộc chuyên ngành này không
                if (category.TaiLieu.Count > 0)
                {
                    return Json(new { success = false, message = $"Không thể xóa chuyên ngành '{category.TenChuyenNganh}' vì còn có {category.TaiLieu.Count} tài liệu thuộc chuyên ngành này!" });
                }

                // Kiểm tra xem có người dùng nào thuộc chuyên ngành này không
                if (category.NguoiDung.Count > 0)
                {
                    return Json(new { success = false, message = $"Không thể xóa chuyên ngành '{category.TenChuyenNganh}' vì còn có {category.NguoiDung.Count} người dùng thuộc chuyên ngành này!" });
                }

                _context.ChuyenNganh.Remove(category);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã xóa chuyên ngành '{category.TenChuyenNganh}' thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa chuyên ngành: " + ex.Message });
            }
        }

        // QUẢN LÝ LOẠI TÀI LIỆU
        [HttpGet]
        public async Task<IActionResult> ManageDocumentTypes()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }
            var documentTypes = await _context.LoaiTaiLieu
                .Include(l => l.TaiLieu)
                .OrderBy(l => l.TenLoaiTaiLieu)
                .ToListAsync();

            ViewData["Title"] = "Quản lý Loại tài liệu";
            return View(documentTypes);
        }

        public IActionResult CreateDocumentType()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }
            ViewData["Title"] = "Thêm loại tài liệu mới";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocumentType(LoaiTaiLieu model)
        {
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                _context.LoaiTaiLieu.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm loại tài liệu thành công!";
                return RedirectToAction(nameof(ManageDocumentTypes));
            }

            ViewData["Title"] = "Thêm loại tài liệu mới";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditDocumentType(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }
            var documentType = await _context.LoaiTaiLieu.Where(l => l.MaLoaiTaiLieu == id).FirstOrDefaultAsync();
            if (documentType == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại tài liệu!";
                return RedirectToAction(nameof(ManageDocumentTypes));
            }

            ViewData["Title"] = "Chỉnh sửa loại tài liệu";
            return View(documentType);
        }

        [HttpPost]
        public async Task<IActionResult> EditDocumentType(LoaiTaiLieu model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện chức năng này!";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.LoaiTaiLieu.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật loại tài liệu thành công!";
                return RedirectToAction(nameof(ManageDocumentTypes));
            }

            ViewData["Title"] = "Chỉnh sửa loại tài liệu";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDocumentTypeStatus(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện chức năng này!";
                return RedirectToAction("Index", "Home");
            }
            var documentType = await _context.LoaiTaiLieu.Where(l => l.MaLoaiTaiLieu == id).FirstOrDefaultAsync();
            if (documentType != null)
            {
                documentType.TrangThaiHoatDong = !documentType.TrangThaiHoatDong;
                await _context.SaveChangesAsync();

                string status = documentType.TrangThaiHoatDong ? "kích hoạt" : "vô hiệu hóa";
                TempData["SuccessMessage"] = $"Đã {status} loại tài liệu {documentType.TenLoaiTaiLieu} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại tài liệu!";
            }

            return RedirectToAction(nameof(ManageDocumentTypes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }
            try
            {
                var documentType = await _context.LoaiTaiLieu
                    .Include(l => l.TaiLieu)
                    .Where(l => l.MaLoaiTaiLieu == id)
                    .FirstOrDefaultAsync();

                if (documentType == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy loại tài liệu!" });
                }

                // Kiểm tra xem có tài liệu nào thuộc loại này không
                if (documentType.TaiLieu.Count > 0)
                {
                    return Json(new { success = false, message = $"Không thể xóa loại tài liệu '{documentType.TenLoaiTaiLieu}' vì còn có {documentType.TaiLieu.Count} tài liệu thuộc loại này!" });
                }

                _context.LoaiTaiLieu.Remove(documentType);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã xóa loại tài liệu '{documentType.TenLoaiTaiLieu}' thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa loại tài liệu: " + ex.Message });
            }
        }

        // THỐNG KÊ
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var tongSoLuotMuon = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaDuyet || p.TrangThai == TrangThaiPhieu.DaTra);
            var tongSoLuotTra = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra);
            var tongSoLuotTai = await _context.LichSuTaiTaiLieu.CountAsync();
            var lichSuTaiTaiLieu = await _context.LichSuTaiTaiLieu
                .Include(l => l.NguoiDung)
                .Include(l => l.TaiLieu)
                .OrderByDescending(l => l.ThoiGianTai)
                .Take(200)
                .ToListAsync();
            ViewBag.LichSuTaiTaiLieu = lichSuTaiTaiLieu;
            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotMuon = tongSoLuotMuon,
                SoPhieuDaTra = tongSoLuotTra,
                TongSoLuotTai = tongSoLuotTai,
                TaiLieuMoiTrongThang = await _context.TaiLieu
                    .CountAsync(t => t.NgayTaiLen.Month == DateTime.Now.Month
                                && t.NgayTaiLen.Year == DateTime.Now.Year),
                ThongKeTheoChuyenNganh = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                ThongKeTheoLoaiTaiLieu = await _context.TaiLieu
                    .Include(t => t.LoaiTaiLieu)
                    .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                    .ToDictionaryAsync(g => g.Key, g => g.Count())
            };

            // Thống kê số lượt mượn theo tháng (12 tháng gần nhất)
            var monthlyStats = new Dictionary<string, int>();
            for (int i = 11; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var count = await _context.PhieuMuonTra
                    .CountAsync(p => (p.TrangThai == TrangThaiPhieu.DaDuyet || p.TrangThai == TrangThaiPhieu.DaTra) && p.NgayMuon.Month == month.Month && p.NgayMuon.Year == month.Year);
                monthlyStats.Add(month.ToString("MM/yyyy"), count);
            }
            ViewBag.MonthlyStats = monthlyStats;

            // Thống kê người dùng theo vai trò
            var userRoleStats = await _context.Users
                .GroupBy(u => u.VaiTro)
                .ToDictionaryAsync(g => g.Key.ToString(), g => g.Count());
            ViewBag.UserRoleStats = userRoleStats;

            ViewData["Title"] = "Thống kê tổng quan";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentDetails(int id)
        {
            var document = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.DanhGiaTaiLieu)
                .Include(t => t.PhieuMuonTras)
                .Where(t => t.MaTaiLieu == id)
                .FirstOrDefaultAsync();

            ViewBag.ChuyenNganh = new SelectList(
                await _context.ChuyenNganh.Where(c => c.TrangThaiHoatDong).OrderBy(c => c.TenChuyenNganh).ToListAsync(),
                "MaChuyenNganh", "TenChuyenNganh");
            ViewBag.LoaiTaiLieu = new SelectList(
                await _context.LoaiTaiLieu.Where(l => l.TrangThaiHoatDong).OrderBy(l => l.TenLoaiTaiLieu).ToListAsync(),
                "MaLoaiTaiLieu", "TenLoaiTaiLieu");

            if (document == null)
            {
                return NotFound();
            }

            return PartialView("_DocumentDetailsModal", document);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietTaiLieu(int id)
        {
            var document = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.DanhGiaTaiLieu)
                .Include(t => t.PhieuMuonTras)
                .Where(t => t.MaTaiLieu == id)
                .FirstOrDefaultAsync();

            ViewBag.ChuyenNganh = new SelectList(
                await _context.ChuyenNganh.Where(c => c.TrangThaiHoatDong).OrderBy(c => c.TenChuyenNganh).ToListAsync(),
                "MaChuyenNganh", "TenChuyenNganh");
            ViewBag.LoaiTaiLieu = new SelectList(
                await _context.LoaiTaiLieu.Where(l => l.TrangThaiHoatDong).OrderBy(l => l.TenLoaiTaiLieu).ToListAsync(),
                "MaLoaiTaiLieu", "TenLoaiTaiLieu");

            if (document == null)
            {
                return PartialView("_DocumentDetailsModal", new QuanLyTaiLieuKhoaHoc.Web.Models.TaiLieu());
            }

            return PartialView("_DocumentDetailsModal", document);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDocument()
        {
            var model = new TaiLieu();
            ViewBag.ChuyenNganh = await _context.ChuyenNganh
                .Where(c => c.TrangThaiHoatDong)
                .OrderBy(c => c.TenChuyenNganh)
                .ToListAsync();
            ViewBag.LoaiTaiLieu = await _context.LoaiTaiLieu
                .Where(l => l.TrangThaiHoatDong)
                .OrderBy(l => l.TenLoaiTaiLieu)
                .ToListAsync();

            return PartialView("_CreateDocumentModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument(TaiLieu model, IFormFile? taiLieuFile)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Không thể xác định người dùng hiện tại!" });
                }

                // Validate file upload
                if (taiLieuFile == null || taiLieuFile.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn file tài liệu để tải lên!" });
                }

                // Validate các trường chung
                if (string.IsNullOrWhiteSpace(model.TenTaiLieu))
                {
                    return Json(new { success = false, message = "Tên tài liệu không được để trống!" });
                }
                if (string.IsNullOrWhiteSpace(model.MoTa))
                {
                    return Json(new { success = false, message = "Mô tả không được để trống!" });
                }
                if (string.IsNullOrWhiteSpace(model.TacGia))
                {
                    return Json(new { success = false, message = "Tác giả không được để trống!" });
                }
                if (model.MaChuyenNganh <= 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn chuyên ngành!" });
                }
                if (model.MaLoaiTaiLieu <= 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn loại tài liệu!" });
                }

                // Lấy loại tài liệu động từ form (nếu có)
                var documentType = Request.Form["DocumentType"].ToString();

                // Validate động các trường theo loại tài liệu
                if (documentType == "baibao")
                {
                    if (string.IsNullOrWhiteSpace(model.TieuDe) || string.IsNullOrWhiteSpace(model.TapChiHoiNghi))
                    {
                        return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin bài báo khoa học!" });
                    }
                }
                else if (documentType == "detai")
                {
                    if (string.IsNullOrWhiteSpace(model.TenDeTai) || string.IsNullOrWhiteSpace(model.MaSoDeTai))
                    {
                        return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin đề tài nghiên cứu khoa học!" });
                    }
                }
                else if (documentType == "giaotrinh")
                {
                    if (string.IsNullOrWhiteSpace(model.TenGiaoTrinh) || string.IsNullOrWhiteSpace(model.MonHocLienQuan))
                    {
                        return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin giáo trình/tài liệu giảng dạy!" });
                    }
                }

                // Xử lý upload file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + taiLieuFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await taiLieuFile.CopyToAsync(fileStream);
                }

                model.DuongDanFile = "/uploads/documents/" + uniqueFileName;
                model.LoaiFile = Path.GetExtension(taiLieuFile.FileName).TrimStart('.');
                model.KichThuocFile = taiLieuFile.Length / 1024; // KB
                model.NgayTaiLen = DateTime.Now;
                model.TrangThai = TrangThaiTaiLieu.DaDuyet; // Thủ thư tự động duyệt

                _context.TaiLieu.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm tài liệu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                var taiLieu = await _context.TaiLieu.FindAsync(id);
                if (taiLieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài liệu!" });
                }

                // Xóa file vật lý nếu tồn tại
                if (!string.IsNullOrEmpty(taiLieu.DuongDanFile))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taiLieu.DuongDanFile.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.TaiLieu.Remove(taiLieu);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa tài liệu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa tài liệu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng!" });
                }


                int userDocuments = 0;
                if (userDocuments > 0)
                {
                    return Json(new { success = false, message = $"Không thể xóa người dùng '{user.HoTen}' vì họ còn có {userDocuments} tài liệu trong hệ thống!" });
                }

                // Xóa các bản ghi liên quan khác nếu có
                var userRatings = await _context.DanhGiaTaiLieu.Where(d => d.MaNguoiDung == id).ToListAsync();
                _context.DanhGiaTaiLieu.RemoveRange(userRatings);

                var userDownloads = await _context.LichSuTaiTaiLieu.Where(l => l.MaNguoiDung == id).ToListAsync();
                _context.LichSuTaiTaiLieu.RemoveRange(userDownloads);

                await _context.SaveChangesAsync();

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = $"Đã xóa người dùng '{user.HoTen}' thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi xóa người dùng: " + string.Join(", ", result.Errors.Select(e => e.Description)) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa người dùng: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult CaiDat()
        {
            ViewData["Title"] = "Cài đặt hệ thống";
            return View();
        }

        // Sao lưu dữ liệu 
        [HttpPost]
        public IActionResult SaoLuuDuLieu()
        {
            try
            {
                // Lấy thông tin kết nối
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                var dbName = builder.InitialCatalog;
                var server = builder.DataSource;

                // Đường dẫn file backup
                var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);
                var backupFile = Path.Combine(backupFolder, $"{dbName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak");

                // Tạo câu lệnh backup
                var sql = $"BACKUP DATABASE [{dbName}] TO DISK = N'{backupFile}' WITH INIT, NAME = 'Full Backup'";

                // Xác định loại xác thực
                string args;
                if (builder.IntegratedSecurity)
                {
                    // Xác thực Windows
                    args = $"-S {server} -E -Q \"{sql}\"";
                }
                else
                {
                    // Xác thực SQL Server
                    var user = builder.UserID;
                    var password = builder.Password;
                    args = $"-S {server} -U {user} -P {password} -Q \"{sql}\"";
                }

                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "sqlcmd";
                process.StartInfo.Arguments = args;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0 && System.IO.File.Exists(backupFile))
                {
                    return Json(new { success = true, message = $"Đã sao lưu dữ liệu thành công! File: {backupFile}" });
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi khi sao lưu: {error}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi sao lưu: " + ex.Message });
            }
        }

        // LẬP PHIẾU MƯỢN TÀI LIỆU
        [HttpPost]
        public async Task<IActionResult> LapPhieuMuon(int maPhieu, DateTime ngayMuon, DateTime ngayTraDuKien)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            var phieuYeuCau = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieuYeuCau == null)
            {
                return Json(new { success = false, message = $"Không tìm thấy phiếu! Mã phiếu: {maPhieu}" });
            }
            if (phieuYeuCau.TrangThai != TrangThaiPhieu.ChoDuyet)
            {
                return Json(new { success = false, message = $"Không thể lập phiếu! Trạng thái hiện tại: {phieuYeuCau.TrangThai} (0=Chờ duyệt, 1=Đã duyệt, 2=Đã trả, 3=Từ chối). Mã phiếu: {maPhieu}, Mã tài liệu: {phieuYeuCau.MaTaiLieu}, Người mượn: {phieuYeuCau.MaNguoiMuon}" });
            }

            var maTaiLieu = phieuYeuCau.MaTaiLieu;

            // Kiểm tra nếu đã có phiếu mượn ĐANG HOẠT ĐỘNG cho tài liệu này
            var phieuDangMuon = await _context.PhieuMuonTra
                .Where(p => p.MaTaiLieu == maTaiLieu && p.TrangThai == TrangThaiPhieu.DaDuyet)
                .FirstOrDefaultAsync();
            if (phieuDangMuon != null)
            {
                return Json(new { success = false, message = "Tài liệu này đang được mượn, không thể lập phiếu mới!" });
            }

            // Cập nhật phiếu thành phiếu mượn
            phieuYeuCau.TrangThai = TrangThaiPhieu.DaDuyet;
            phieuYeuCau.NgayMuon = ngayMuon;
            phieuYeuCau.NgayTra = ngayTraDuKien;
            phieuYeuCau.MaThuThuDuyet = currentUser.Id;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Lập phiếu mượn thành công!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.Users.Include(u => u.ChuyenNganh).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return Json(null);

            return Json(new
            {
                id = user.Id,
                hoTen = user.HoTen,
                email = user.Email,
                maSo = user.MaSo,
                vaiTro = user.VaiTro.ToString(),
                chuyenNganh = user.ChuyenNganh?.TenChuyenNganh
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string HoTen, string Email, string MaSo, string VaiTro, string ChuyenNganh)
        {
            // Kiểm tra trùng email hoặc mã số
            if (await _context.Users.AnyAsync(u => u.Email == Email || u.MaSo == MaSo))
                return Json(new { success = false, message = "Email hoặc mã số đã tồn tại!" });

            var user = new NguoiDung
            {
                HoTen = HoTen,
                Email = Email,
                UserName = Email,
                MaSo = MaSo,
                NgayTao = DateTime.Now,
                TrangThaiHoatDong = true
            };
            if (Enum.TryParse<VaiTroNguoiDung>(VaiTro, out var vaitroValue))
                user.VaiTro = vaitroValue;
            else
                return Json(new { success = false, message = "Vai trò không hợp lệ!" });

            if (!string.IsNullOrEmpty(ChuyenNganh))
            {
                var chuyenNganh = await _context.ChuyenNganh.FirstOrDefaultAsync(c => c.TenChuyenNganh == ChuyenNganh);
                if (chuyenNganh != null)
                    user.MaChuyenNganh = chuyenNganh.MaChuyenNganh;
            }

            // Tạo mật khẩu mặc định
            var password = "123456";
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Thêm người dùng thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LichSuTaiTaiLieu()
        {
            var list = await _context.LichSuTaiTaiLieu
                .Include(l => l.NguoiDung)
                .Include(l => l.TaiLieu)
                .OrderByDescending(l => l.ThoiGianTai)
                .Take(200)
                .ToListAsync();
            return View(list);
        }
    }
}