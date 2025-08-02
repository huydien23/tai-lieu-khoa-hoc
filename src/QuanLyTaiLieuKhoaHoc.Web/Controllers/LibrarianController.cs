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
        private readonly ISystemActivityService _systemActivityService;
        private readonly UserManager<NguoiDung> _userManager;

        public LibrarianController(ApplicationDbContext context, ITaiLieuService taiLieuService, ISystemActivityService systemActivityService, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _taiLieuService = taiLieuService;
            _systemActivityService = systemActivityService;
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
                    sdt = u.SoDienThoai,
                    loai = u.VaiTro.ToString() // Ví dụ: "SinhVien", "GiangVien", "ThuThu"
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

            // Lấy số lượng tài liệu quá hạn
            var soTaiLieuQuaHan = await _context.PhieuMuonTra
                .CountAsync(p => p.TrangThai == TrangThaiPhieu.DaDuyet && p.NgayTraDuKien < DateTime.Now);

            // Lấy người dùng mới đăng ký trong 24h qua
            var nguoiDungMoi = await _context.Users
                .Where(u => u.NgayTao >= DateTime.Now.AddDays(-1))
                .OrderByDescending(u => u.NgayTao)
                .FirstOrDefaultAsync();

            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotMuon = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaDuyet || p.TrangThai == TrangThaiPhieu.DaTra),
                TongSoLuotTai = await _context.LichSuTaiTaiLieu.CountAsync(),
                SoPhieuDangMuon = soTaiLieuQuaHan, // Sử dụng cho tài liệu quá hạn
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
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                HoạtĐộngHệThống = await _systemActivityService.GetActivitiesForDashboardAsync()
            };

            // Thêm thông tin người dùng mới vào ViewBag
            ViewBag.NguoiDungMoi = nguoiDungMoi;

            ViewData["Title"] = "Dashboard Thủ thư";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers(int page = 1, string search = "", string role = "", string status = "", string major = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            const int pageSize = 10;
            var query = _context.Users
                .Include(u => u.ChuyenNganh)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.HoTen.Contains(search) || u.Email.Contains(search) || u.MaSo.Contains(search));
            }

            // Apply role filter
            if (!string.IsNullOrEmpty(role))
            {
                if (Enum.TryParse<VaiTroNguoiDung>(role, out var roleEnum))
                {
                    query = query.Where(u => u.VaiTro == roleEnum);
                }
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                if (bool.TryParse(status, out var statusBool))
                {
                    query = query.Where(u => u.TrangThaiHoatDong == statusBool);
                }
            }

            // Apply major filter
            if (!string.IsNullOrEmpty(major))
            {
                query = query.Where(u => u.ChuyenNganh.TenChuyenNganh == major);
            }

            // Order by name
            query = query.OrderBy(u => u.HoTen);

            var users = await PaginatedList<NguoiDung>.CreateAsync(query, page, pageSize);

            // Pass filter values to view
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentRole"] = role;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentMajor"] = major;
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
        public async Task<IActionResult> HideDocument(int id)
        {
            var document = await _context.TaiLieu.Where(t => t.MaTaiLieu == id).FirstOrDefaultAsync();
            if (document != null)
            {
                document.TrangThai = TrangThaiTaiLieu.An;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã ẩn tài liệu thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy tài liệu!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShowDocument(int id)
        {
            var document = await _context.TaiLieu.Where(t => t.MaTaiLieu == id).FirstOrDefaultAsync();
            if (document != null)
            {
                document.TrangThai = TrangThaiTaiLieu.DaDuyet;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã hiện tài liệu thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy tài liệu!" });
            }
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
        public async Task<IActionResult> ManageCategories(int page = 1, string search = "", string status = "")
        {
            var query = _context.ChuyenNganh
                .Include(c => c.TaiLieu)
                .Include(c => c.NguoiDung)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.TenChuyenNganh.Contains(search) || 
                                        (c.MoTa != null && c.MoTa.Contains(search)));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status.ToLower() == "active";
                query = query.Where(c => c.TrangThaiHoatDong == isActive);
            }

            // Order by name
            query = query.OrderBy(c => c.TenChuyenNganh);

            // Apply pagination
            var pageSize = 10;
            var categories = await PaginatedList<ChuyenNganh>.CreateAsync(query, page, pageSize);

            // Pass filter values to view
            ViewData["Search"] = search;
            ViewData["Status"] = status;
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

        [HttpPost]
        public async Task<IActionResult> BulkActivateCategories(List<int> categoryIds)
        {
            try
            {
                var categories = await _context.ChuyenNganh
                    .Where(c => categoryIds.Contains(c.MaChuyenNganh))
                    .ToListAsync();

                foreach (var category in categories)
                {
                    category.TrangThaiHoatDong = true;
                }

                await _context.SaveChangesAsync();

                // Log activity
                var currentUser = await _userManager.GetUserAsync(User);
                var activity = new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.BulkCategoryActivation,
                    TiêuĐề = $"Kích hoạt hàng loạt {categories.Count} chuyên ngành",
                    MôTả = $"Đã kích hoạt {categories.Count} chuyên ngành thành công",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.TrungBình,
                    NgườiThựcHiện = currentUser?.Id ?? "System"
                };
                await _systemActivityService.CreateActivityAsync(activity);

                return Json(new { success = true, message = $"Đã kích hoạt {categories.Count} chuyên ngành thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportCategories(string? search, string? status)
        {
            try
            {
                var query = _context.ChuyenNganh
                    .Include(c => c.TaiLieu)
                    .Include(c => c.NguoiDung)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.TenChuyenNganh.Contains(search) || 
                                            (c.MoTa != null && c.MoTa.Contains(search)));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    bool isActive = status.ToLower() == "active";
                    query = query.Where(c => c.TrangThaiHoatDong == isActive);
                }

                var categories = await query.OrderBy(c => c.TenChuyenNganh).ToListAsync();

                using var package = new OfficeOpenXml.ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Chuyên ngành");

                // Set header row
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Tên chuyên ngành";
                worksheet.Cells[1, 3].Value = "Mô tả";
                worksheet.Cells[1, 4].Value = "Số tài liệu";
                worksheet.Cells[1, 5].Value = "Số người dùng";
                worksheet.Cells[1, 6].Value = "Ngày tạo";
                worksheet.Cells[1, 7].Value = "Trạng thái";

                // Style header row
                var headerRange = worksheet.Cells[1, 1, 1, 7];
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(52, 73, 94));
                headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Populate data
                for (int i = 0; i < categories.Count; i++)
                {
                    var category = categories[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = category.TenChuyenNganh;
                    worksheet.Cells[row, 3].Value = category.MoTa ?? "";
                    worksheet.Cells[row, 4].Value = category.TaiLieu.Count;
                    worksheet.Cells[row, 5].Value = category.NguoiDung.Count;
                    worksheet.Cells[row, 6].Value = category.NgayTao.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 7].Value = category.TrangThaiHoatDong ? "Hoạt động" : "Vô hiệu hóa";

                    // Color code status column
                    if (category.TrangThaiHoatDong)
                    {
                        worksheet.Cells[row, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(76, 175, 80));
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }
                    else
                    {
                        worksheet.Cells[row, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 67, 54));
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Add borders
                var dataRange = worksheet.Cells[1, 1, categories.Count + 1, 7];
                dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                var fileName = $"DanhSachChuyenNganh_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(package.GetAsByteArray(), contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất file: " + ex.Message;
                return RedirectToAction(nameof(ManageCategories));
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
            // Sửa logic thống kê phiếu mượn trả
            var tongSoPhieu = await _context.PhieuMuonTra.CountAsync();
            var soPhieuChoDuyet = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.ChoDuyet);
            var soPhieuDangMuon = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaDuyet && p.NgayTra == null);
            var soPhieuDaTra = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaTra);
            var soPhieuTuChoi = await _context.PhieuMuonTra.CountAsync(p => p.TrangThai == TrangThaiPhieu.TuChoi);
            
            // Load lịch sử mượn trả
            var lichSuMuonTra = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Include(p => p.NguoiMuon)
                .OrderByDescending(p => p.NgayMuon)
                .Take(20)
                .ToListAsync();
            
            // Thống kê theo loại tài liệu
            var soLuongBaiBaoKhoaHoc = await _context.TaiLieu.CountAsync(t => !string.IsNullOrEmpty(t.TieuDe));
            var soLuongDeTaiNghienCuu = await _context.TaiLieu.CountAsync(t => !string.IsNullOrEmpty(t.TenDeTai));
            var soLuongGiaoTrinh = await _context.TaiLieu.CountAsync(t => !string.IsNullOrEmpty(t.TenGiaoTrinh));
            
            // Thống kê lượt tải và yêu thích
            var soLuongYeuThich = await _context.YeuThichTaiLieu.CountAsync();
            var luotTaiTrongTuan = await _context.LichSuTaiTaiLieu.CountAsync(l => l.ThoiGianTai >= DateTime.Today.AddDays(-7));
            var luotTaiTuanTruoc = await _context.LichSuTaiTaiLieu.CountAsync(l => 
                l.ThoiGianTai >= DateTime.Today.AddDays(-14) && l.ThoiGianTai < DateTime.Today.AddDays(-7));
            var soLuotXemGanDay = await _context.LichSuTaiTaiLieu.CountAsync(l => l.ThoiGianTai >= DateTime.Today.AddDays(-1));
            
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
                TongSoLuotMuon = tongSoPhieu, // Sửa: Tổng số phiếu
                SoPhieuChoDuyet = soPhieuChoDuyet, // Thêm: Phiếu chờ duyệt
                SoPhieuDangMuon = soPhieuDangMuon, // Thêm: Phiếu đang mượn
                SoPhieuDaTra = soPhieuDaTra, // Sửa: Phiếu đã trả
                SoPhieuTuChoi = soPhieuTuChoi, // Thêm: Phiếu từ chối
                TongSoLuotTai = tongSoLuotTai,
                LichSuMuonTra = lichSuMuonTra, // Thêm: Lịch sử mượn trả
                SoLuongBaiBaoKhoaHoc = soLuongBaiBaoKhoaHoc, // Thêm: Số lượng bài báo khoa học
                SoLuongDeTaiNghienCuu = soLuongDeTaiNghienCuu, // Thêm: Số lượng đề tài nghiên cứu
                SoLuongGiaoTrinh = soLuongGiaoTrinh, // Thêm: Số lượng giáo trình
                SoLuongYeuThich = soLuongYeuThich, // Thêm: Số lượng yêu thích
                LuotTaiTrongTuan = luotTaiTrongTuan, // Thêm: Lượt tải trong tuần
                LuotTaiTuanTruoc = luotTaiTuanTruoc, // Thêm: Lượt tải tuần trước
                SoLuotXemGanDay = soLuotXemGanDay, // Thêm: Lượt xem gần đây
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
                
                // Validation số lượng
                if (model.SoLuong <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
                }

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
                model.TrangThai = TrangThaiTaiLieu.DaDuyet; // Thủ thư tự động duyệt khi tạo

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
        public async Task<IActionResult> EditDocument(Models.ViewModels.EditTaiLieuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var taiLieu = await _context.TaiLieu.FindAsync(model.MaTaiLieu);
            if (taiLieu == null)
                return Json(new { success = false, message = "Không tìm thấy tài liệu!" });

            taiLieu.TenTaiLieu = model.TenTaiLieu;
            taiLieu.MoTa = model.MoTa;
            taiLieu.TacGia = model.TacGia;
            taiLieu.MaChuyenNganh = model.MaChuyenNganh;
            taiLieu.MaLoaiTaiLieu = model.MaLoaiTaiLieu;
            taiLieu.ChoPhepTaiFile = model.ChoPhepTaiFile;
            taiLieu.TieuDe = model.TieuDe;
            taiLieu.TapChiHoiNghi = model.TapChiHoiNghi;
            taiLieu.NgayCongBo = model.NgayCongBo;
            taiLieu.DOI = model.DOI;
            taiLieu.ISSN = model.ISSN;
            taiLieu.CapDo = model.CapDo;
            taiLieu.TenDeTai = model.TenDeTai;
            taiLieu.MaSoDeTai = model.MaSoDeTai;
            taiLieu.CapDeTai = model.CapDeTai;
            taiLieu.ThoiGianThucHien = model.ThoiGianThucHien;
            taiLieu.CoQuanChuTri = model.CoQuanChuTri;
            taiLieu.ChuNhiemDeTai = model.ChuNhiemDeTai;
            taiLieu.TenGiaoTrinh = model.TenGiaoTrinh;
            taiLieu.MonHocLienQuan = model.MonHocLienQuan;
            taiLieu.DonViPhatHanh = model.DonViPhatHanh;
            taiLieu.NamXuatBan = model.NamXuatBan;
            taiLieu.SoTinChi = model.SoTinChi;

            // Validation số lượng
            if (model.SoLuong < model.SoLuongDaMuon)
            {
                return Json(new { success = false, message = "Số lượng mới không được nhỏ hơn số lượng đang mượn!" });
            }
            taiLieu.SoLuong = model.SoLuong;

            if (model.FileTaiLieu != null && model.FileTaiLieu.Length > 0)
            {
                if (!string.IsNullOrEmpty(taiLieu.DuongDanFile))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taiLieu.DuongDanFile.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                // Upload file mới
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FileTaiLieu.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileTaiLieu.CopyToAsync(fileStream);
                }
                taiLieu.DuongDanFile = "/uploads/documents/" + uniqueFileName;
                taiLieu.LoaiFile = Path.GetExtension(model.FileTaiLieu.FileName).TrimStart('.');
                taiLieu.KichThuocFile = model.FileTaiLieu.Length / 1024; // KB
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật tài liệu thành công!" });
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

        [HttpPost]
        public async Task<IActionResult> EditUser(string Id, string HoTen, string Email, string MaSo, string SoDienThoai, string VaiTro, string ChuyenNganh)
        {
            var user = await _context.Users.Include(u => u.ChuyenNganh).FirstOrDefaultAsync(u => u.Id == Id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng!" });
            if (!string.IsNullOrWhiteSpace(HoTen)) user.HoTen = HoTen;
            if (!string.IsNullOrWhiteSpace(Email)) user.Email = Email;
            if (!string.IsNullOrWhiteSpace(MaSo)) user.MaSo = MaSo;
            if (!string.IsNullOrWhiteSpace(SoDienThoai)) user.SoDienThoai = SoDienThoai;
            if (!string.IsNullOrWhiteSpace(VaiTro) && Enum.TryParse<VaiTroNguoiDung>(VaiTro, out var vaitroValue)) user.VaiTro = vaitroValue;
            if (!string.IsNullOrWhiteSpace(ChuyenNganh))
            {
                var chuyenNganh = await _context.ChuyenNganh.FirstOrDefaultAsync(c => c.TenChuyenNganh == ChuyenNganh);
                if (chuyenNganh != null)
                    user.MaChuyenNganh = chuyenNganh.MaChuyenNganh;
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật người dùng thành công!" });
        }

        [HttpGet]
        public IActionResult CaiDat()
        {
            ViewData["Title"] = "Cài đặt hệ thống";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                // Tổng số người dùng
                var totalUsers = await _context.Users.CountAsync();

                // Tổng số tài liệu
                var totalDocuments = await _context.TaiLieu.CountAsync();

                // Phiếu mượn hôm nay
                var todayBorrows = await _context.PhieuMuonTra
                    .Where(p => p.NgayMuon.Date == today)
                    .CountAsync();

                // Yêu cầu chờ duyệt
                var pendingRequests = await _context.PhieuMuonTra
                    .Where(p => p.TrangThai == TrangThaiPhieu.ChoDuyet)
                    .CountAsync();

                // Phiếu mượn trong tháng
                var monthlyBorrows = await _context.PhieuMuonTra
                    .Where(p => p.NgayMuon >= startOfMonth)
                    .CountAsync();

                                       // Tài liệu được thêm mới trong tháng
                       var newDocuments = await _context.TaiLieu
                           .Where(t => t.NgayTaiLen >= startOfMonth)
                           .CountAsync();

                // Người dùng mới trong tháng
                var newUsers = await _context.Users
                    .Where(u => u.NgayTao >= startOfMonth)
                    .CountAsync();

                // Tài liệu quá hạn
                var overdueDocuments = await _context.PhieuMuonTra
                    .Where(p => p.TrangThai == TrangThaiPhieu.DaDuyet && 
                               p.NgayTraDuKien < today && 
                               p.NgayTra == null)
                    .CountAsync();

                return Json(new
                {
                    success = true,
                    totalUsers,
                    totalDocuments,
                    todayBorrows,
                    pendingRequests,
                    monthlyBorrows,
                    newDocuments,
                    newUsers,
                    overdueDocuments
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public async Task<IActionResult> LapPhieuMuonTrucTiep(int maTaiLieu, string hoTenNguoiMuon, string maSoNguoiMuon, 
            string emailNguoiMuon, string soDienThoaiNguoiMuon, string loaiNguoiMuon, 
            DateTime ngayMuon, DateTime ngayTraDuKien, int soLuongMuon, string ghiChu)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            // Kiểm tra tài liệu
            var taiLieu = await _context.TaiLieu.FindAsync(maTaiLieu);
            if (taiLieu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy tài liệu!" });
            }

            // Kiểm tra số lượng còn lại
            if (taiLieu.SoLuongConLai < soLuongMuon)
            {
                return Json(new { success = false, message = $"Tài liệu chỉ còn {taiLieu.SoLuongConLai} bản, không đủ để mượn {soLuongMuon} bản!" });
            }

            // Tạo phiếu mượn mới
            var phieuMuon = new PhieuMuonTra
            {
                MaTaiLieu = maTaiLieu,
                MaNguoiMuon = null, // Không phải thành viên hệ thống
                HoTenNguoiMuon = hoTenNguoiMuon,
                MaSoNguoiMuon = maSoNguoiMuon,
                EmailNguoiMuon = emailNguoiMuon,
                SoDienThoaiNguoiMuon = soDienThoaiNguoiMuon,
                LoaiNguoiMuon = loaiNguoiMuon,
                NgayMuon = ngayMuon,
                NgayTraDuKien = ngayTraDuKien,
                SoLuongMuon = soLuongMuon,
                TrangThai = TrangThaiPhieu.DaDuyet, // Trực tiếp duyệt
                MaThuThuDuyet = currentUser.Id,
                NgayTao = DateTime.Now,
                GhiChu = ghiChu
            };

            _context.PhieuMuonTra.Add(phieuMuon);

            // Cập nhật số lượng đã mượn
            taiLieu.SoLuongDaMuon += soLuongMuon;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Lập phiếu mượn trực tiếp thành công!" });
        }

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

            // Kiểm tra số lượng còn lại
            if (phieuYeuCau.TaiLieu?.SoLuongConLai <= 0)
            {
                return Json(new { success = false, message = "Tài liệu đã hết, không thể mượn!" });
            }

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
            phieuYeuCau.NgayTraDuKien = ngayTraDuKien;
            phieuYeuCau.MaThuThuDuyet = currentUser.Id;

            // Cập nhật số lượng đã mượn
            if (phieuYeuCau.TaiLieu != null)
            {
                phieuYeuCau.TaiLieu.SoLuongDaMuon++;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Lập phiếu mượn thành công!" });
        }

        [HttpPost]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> XacNhanTraTaiLieu(int maPhieu, DateTime? ngayTra, string? tinhTrang, string? ghiChu)
        {
            try
            {
                var phieu = await _context.PhieuMuonTra
                    .Include(p => p.TaiLieu)
                    .Include(p => p.NguoiMuon)
                    .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);
                
                if (phieu == null)
                    return Json(new { success = false, message = "Không tìm thấy phiếu mượn!" });
                
                if (phieu.TrangThai != TrangThaiPhieu.DaDuyet)
                    return Json(new { success = false, message = "Phiếu này không ở trạng thái có thể trả!" });

                // Cập nhật thông tin phiếu
                phieu.TrangThai = TrangThaiPhieu.DaTra;
                phieu.NgayTra = ngayTra ?? DateTime.Now;
                phieu.MaThuThuDuyet = (await _userManager.GetUserAsync(User))?.Id;
                
                // Cập nhật ghi chú với thông tin tình trạng
                var ghiChuMoi = "";
                if (!string.IsNullOrWhiteSpace(tinhTrang))
                {
                    ghiChuMoi += $"Tình trạng tài liệu: {tinhTrang}. ";
                }
                if (!string.IsNullOrWhiteSpace(ghiChu))
                {
                    ghiChuMoi += ghiChu;
                }
                if (!string.IsNullOrWhiteSpace(ghiChuMoi))
                {
                    phieu.GhiChu = ghiChuMoi;
                }

                // Giảm số lượng đã mượn
                if (phieu.TaiLieu != null)
                {
                    // Đảm bảo SoLuongMuon có giá trị hợp lệ (mặc định là 1 nếu null hoặc 0)
                    var soLuongMuon = phieu.SoLuongMuon > 0 ? phieu.SoLuongMuon : 1;
                    phieu.TaiLieu.SoLuongDaMuon -= soLuongMuon;
                    
                    // Đảm bảo không âm
                    if (phieu.TaiLieu.SoLuongDaMuon < 0)
                    {
                        phieu.TaiLieu.SoLuongDaMuon = 0;
                    }
                }

                await _context.SaveChangesAsync();

                // Tạo thông báo thành công chi tiết
                var tenNguoiMuon = phieu.NguoiMuon?.HoTen ?? phieu.HoTenNguoiMuon ?? "Không rõ";
                var message = $"Đã xác nhận trả tài liệu '{phieu.TaiLieu?.TenTaiLieu}' cho {tenNguoiMuon} thành công!";
                if (!string.IsNullOrWhiteSpace(tinhTrang))
                {
                    message += $" Tình trạng: {tinhTrang}.";
                }

                return Json(new { success = true, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xác nhận trả tài liệu: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.Users.Include(u => u.ChuyenNganh).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng" });

            return Json(new
            {
                success = true,
                id = user.Id,
                hoTen = user.HoTen,
                email = user.Email ?? "",
                maSo = user.MaSo ?? "",
                soDienThoai = user.SoDienThoai ?? "",
                vaiTro = user.VaiTro.ToString(),
                chuyenNganh = user.ChuyenNganh?.TenChuyenNganh ?? ""
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
                // Gán role cho Identity
                string identityRole = user.VaiTro switch
                {
                    VaiTroNguoiDung.GiangVien => "GiangVien",
                    VaiTroNguoiDung.ThuThu => "ThuThu",
                    _ => "SinhVien"
                };
                if (!await _userManager.IsInRoleAsync(user, identityRole))
                {
                    // Tạo role nếu chưa có
                    var roleManager = (RoleManager<IdentityRole>?)HttpContext.RequestServices.GetService(typeof(RoleManager<IdentityRole>));
                    if (roleManager != null && !await roleManager.RoleExistsAsync(identityRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole(identityRole));
                    }
                    await _userManager.AddToRoleAsync(user, identityRole);
                }
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

        [HttpGet]
        public async Task<IActionResult> GetTaiLieuSoLuong(int maTaiLieu)
        {
            var taiLieu = await _context.TaiLieu.FindAsync(maTaiLieu);
            if (taiLieu == null)
                return Json(new { success = false });
                
            return Json(new { 
                success = true, 
                soLuong = taiLieu.SoLuong,
                soLuongDaMuon = taiLieu.SoLuongDaMuon,
                soLuongConLai = taiLieu.SoLuongConLai
            });
        }

        [HttpPost]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> FixSoLuongMuonData()
        {
            try
            {
                // Cập nhật tất cả phiếu mượn có SoLuongMuon = 0 hoặc null thành 1
                var phieuMuonWithZeroSoLuong = await _context.PhieuMuonTra
                    .Where(p => p.SoLuongMuon <= 0)
                    .ToListAsync();

                foreach (var phieu in phieuMuonWithZeroSoLuong)
                {
                    phieu.SoLuongMuon = 1;
                }

                // Tính lại số lượng đã mượn cho tất cả tài liệu
                var allTaiLieu = await _context.TaiLieu.ToListAsync();
                foreach (var taiLieu in allTaiLieu)
                {
                    var soLuongDaMuon = await _context.PhieuMuonTra
                        .Where(p => p.MaTaiLieu == taiLieu.MaTaiLieu && 
                                   p.TrangThai == TrangThaiPhieu.DaDuyet && 
                                   !p.NgayTra.HasValue)
                        .SumAsync(p => p.SoLuongMuon);
                    
                    taiLieu.SoLuongDaMuon = soLuongDaMuon;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã cập nhật lại {phieuMuonWithZeroSoLuong.Count} phiếu mượn và đồng bộ số lượng cho {allTaiLieu.Count} tài liệu!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> ChangeUserPassword(string userId, string newPassword)
        {
            try
            {
                // Kiểm tra quyền thủ thư
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
                {
                    return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
                }

                // Tìm người dùng cần đổi mật khẩu
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng!" });
                }

                // Kiểm tra mật khẩu mới
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    return Json(new { success = false, message = "Mật khẩu phải có ít nhất 6 ký tự!" });
                }

                // Đổi mật khẩu
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Đã đổi mật khẩu thành công cho người dùng {user.HoTen}. Vui lòng thông báo mật khẩu mới cho người dùng." 
                    });
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = $"Không thể đổi mật khẩu: {errors}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi đổi mật khẩu: " + ex.Message });
            }
        }

        // API endpoints for system activities
        [HttpGet]
        public async Task<IActionResult> GetSystemActivities()
        {
            try
            {
                var activities = await _systemActivityService.GetActivitiesForDashboardAsync();
                return Json(new { success = true, data = activities });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkActivityAsProcessed(string activityId)
        {
            try
            {
                var result = await _systemActivityService.MarkActivityAsProcessedAsync(activityId);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBorrowRequest(int phieuId)
        {
            try
            {
                var phieu = await _context.PhieuMuonTra
                    .Include(p => p.TaiLieu)
                    .Include(p => p.NguoiMuon)
                    .FirstOrDefaultAsync(p => p.MaPhieu == phieuId);

                if (phieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu mượn" });
                }

                phieu.TrangThai = TrangThaiPhieu.DaDuyet;
                await _context.SaveChangesAsync();

                // Tạo hoạt động mới
                var activity = new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.YêuCầuMượnTàiLiệu,
                    TiêuĐề = "Yêu cầu mượn đã được duyệt",
                    MôTả = $"Đã duyệt yêu cầu mượn \"{phieu.TaiLieu?.TenTaiLieu}\" của {phieu.NguoiMuon?.HoTen}",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.Thấp,
                    ĐãXửLý = true,
                    NgườiThựcHiện = User.Identity?.Name
                };

                await _systemActivityService.CreateActivityAsync(activity);

                return Json(new { success = true, message = "Đã duyệt yêu cầu mượn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectBorrowRequest(int phieuId, string lyDo)
        {
            try
            {
                var phieu = await _context.PhieuMuonTra
                    .Include(p => p.TaiLieu)
                    .Include(p => p.NguoiMuon)
                    .FirstOrDefaultAsync(p => p.MaPhieu == phieuId);

                if (phieu == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu mượn" });
                }

                phieu.TrangThai = TrangThaiPhieu.TuChoi;
                phieu.GhiChu = lyDo;
                await _context.SaveChangesAsync();

                // Tạo hoạt động mới
                var activity = new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.YêuCầuMượnTàiLiệu,
                    TiêuĐề = "Yêu cầu mượn đã bị từ chối",
                    MôTả = $"Đã từ chối yêu cầu mượn \"{phieu.TaiLieu?.TenTaiLieu}\" của {phieu.NguoiMuon?.HoTen}. Lý do: {lyDo}",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.Thấp,
                    ĐãXửLý = true,
                    NgườiThựcHiện = User.Identity?.Name
                };

                await _systemActivityService.CreateActivityAsync(activity);

                return Json(new { success = true, message = "Đã từ chối yêu cầu mượn" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Bulk activate users
        [HttpPost]
        public async Task<IActionResult> BulkActivateUsers(List<string> userIds)
        {
            try
            {
                if (userIds == null || !userIds.Any())
                {
                    return Json(new { success = false, message = "Không có người dùng nào được chọn!" });
                }

                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.TrangThaiHoatDong = true;
                }

                await _context.SaveChangesAsync();

                // Ghi log hoạt động
                var activity = new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.BulkUserActivation,
                    TiêuĐề = "Kích hoạt hàng loạt người dùng",
                    MôTả = $"Đã kích hoạt {users.Count} người dùng trong hệ thống",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.Thấp,
                    ĐãXửLý = true,
                    NgườiThựcHiện = User.Identity?.Name,
                    DữLiệuBổSung = string.Join(",", userIds)
                };

                await _systemActivityService.CreateActivityAsync(activity);

                return Json(new { success = true, message = $"Đã kích hoạt {users.Count} người dùng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        // Export users to Excel
        [HttpGet]
        public async Task<IActionResult> ExportUsers(string? search, string? role, string? status, string? major)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.ChuyenNganh)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u => u.HoTen.Contains(search) || u.Email.Contains(search) || u.MaSo.Contains(search));
                }

                if (!string.IsNullOrEmpty(role))
                {
                    if (Enum.TryParse<VaiTroNguoiDung>(role, out var roleEnum))
                    {
                        query = query.Where(u => u.VaiTro == roleEnum);
                    }
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (bool.TryParse(status, out var statusBool))
                    {
                        query = query.Where(u => u.TrangThaiHoatDong == statusBool);
                    }
                }

                if (!string.IsNullOrEmpty(major))
                {
                    query = query.Where(u => u.ChuyenNganh != null && u.ChuyenNganh.TenChuyenNganh == major);
                }

                var users = await query.ToListAsync();

                // Generate Excel file using EPPlus
                using var package = new OfficeOpenXml.ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Danh sách người dùng");

                // Set header row
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Họ tên";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Mã số";
                worksheet.Cells[1, 5].Value = "Vai trò";
                worksheet.Cells[1, 6].Value = "Chuyên ngành";
                worksheet.Cells[1, 7].Value = "Trạng thái";
                worksheet.Cells[1, 8].Value = "Ngày tạo";

                // Style header row
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(52, 73, 94));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Add data rows
                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = user.HoTen;
                    worksheet.Cells[row, 3].Value = user.Email;
                    worksheet.Cells[row, 4].Value = user.MaSo;
                    worksheet.Cells[row, 5].Value = user.VaiTro.ToString();
                    worksheet.Cells[row, 6].Value = user.ChuyenNganh?.TenChuyenNganh ?? "";
                    worksheet.Cells[row, 7].Value = user.TrangThaiHoatDong ? "Hoạt động" : "Không hoạt động";
                    worksheet.Cells[row, 8].Value = user.NgayTao.ToString("dd/MM/yyyy");

                    // Style status column
                    if (user.TrangThaiHoatDong)
                    {
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                    }
                    else
                    {
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Add borders
                using (var range = worksheet.Cells[1, 1, users.Count + 1, 8])
                {
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                var fileName = $"danh_sach_nguoi_dung_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var bytes = package.GetAsByteArray();

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xuất dữ liệu: {ex.Message}";
                return RedirectToAction("ManageUsers");
            }
        }
    }
}