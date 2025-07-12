using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    [Authorize]
    public class TaiLieuController : Controller
    {
        private readonly ITaiLieuService _taiLieuService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public TaiLieuController(ITaiLieuService taiLieuService, ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _taiLieuService = taiLieuService;
            _context = context;
            _userManager = userManager;
        }

        // API lấy tóm tắt tài liệu cho modal (JSON)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetDocumentSummary(int id)
        {
            var taiLieu = await _taiLieuService.GetTaiLieuByIdAsync(id);
            if (taiLieu == null)
                return NotFound();
            var moTaTomTat = taiLieu.MoTa != null && taiLieu.MoTa.Length > 100 ? taiLieu.MoTa.Substring(0, 100) + "..." : taiLieu.MoTa;
            return Json(new
            {
                tenTaiLieu = taiLieu.TenTaiLieu,
                tenNguoiTaiLen = taiLieu.TenNguoiTaiLen,
                ngayTaiLen = taiLieu.NgayTaiLen.ToString("dd/MM/yyyy HH:mm"),
                tenChuyenNganh = taiLieu.TenChuyenNganh,
                tenLoaiTaiLieu = taiLieu.TenLoaiTaiLieu,
                luotTai = taiLieu.LuotTai,
                kichThuocFile = (taiLieu.KichThuocFile / 1024.0).ToString("F1") + " KB",
                diemDanhGiaTrungBinh = taiLieu.DiemDanhGiaTrungBinh,
                moTa = moTaTomTat,
                maTaiLieu = taiLieu.MaTaiLieu
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int trang = 1, string? timKiem = null, int? maChuyenNganh = null,
            int? maLoaiTaiLieu = null, string? sapXep = null)
        {
            ViewData["Title"] = "Danh sách Tài liệu";

            // Chuẩn bị data cho dropdown
            await LoadDropdownData();

            var model = await _taiLieuService.GetDanhSachTaiLieuAsync(trang, 12, timKiem, maChuyenNganh, maLoaiTaiLieu, sapXep);
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var taiLieu = await _taiLieuService.GetTaiLieuByIdAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            ViewData["Title"] = $"Chi tiết: {taiLieu.TenTaiLieu}";
            return View(taiLieu);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm Tài liệu mới";
            await LoadDropdownData();
            return View(new TaiLieuViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiLieuViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra file upload
                if (model.FileTaiLieu == null || model.FileTaiLieu.Length == 0)
                {
                    ModelState.AddModelError("FileTaiLieu", "Vui lòng chọn file tài liệu để upload.");
                    await LoadDropdownData();
                    return View(model);
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var result = await _taiLieuService.TaoTaiLieuAsync(model, currentUser.Id);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Thêm tài liệu thành công! Tài liệu đang chờ duyệt.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm tài liệu. Vui lòng thử lại.");
                    }
                }
            }

            await LoadDropdownData();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var taiLieu = await _taiLieuService.GetTaiLieuByIdAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || (currentUser.VaiTro != VaiTroNguoiDung.ThuThu &&
                await _context.TaiLieu.AnyAsync(t => t.MaTaiLieu == id && t.MaNguoiTaiLen != currentUser.Id)))
            {
                return Forbid();
            }

            ViewData["Title"] = $"Chỉnh sửa: {taiLieu.TenTaiLieu}";
            await LoadDropdownData();
            return View(taiLieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaiLieuViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _taiLieuService.CapNhatTaiLieuAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật tài liệu thành công!";
                    return RedirectToAction("Details", new { id = model.MaTaiLieu });
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật tài liệu.");
                }
            }

            await LoadDropdownData();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var taiLieu = await _taiLieuService.GetTaiLieuByIdAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || (currentUser.VaiTro != VaiTroNguoiDung.ThuThu &&
                taiLieu.MaNguoiTaiLen != currentUser.Id))
            {
                return Forbid();
            }

            ViewData["Title"] = $"Xóa: {taiLieu.TenTaiLieu}";
            return View(taiLieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _taiLieuService.XoaTaiLieuAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Xóa tài liệu thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa tài liệu.";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "GiangVien,ThuThu")]
        public async Task<IActionResult> Download(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser?.Id ?? "anonymous";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var filePath = await _taiLieuService.TaiFileAsync(id, userId, ipAddress);
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(fullPath);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            return File(fileBytes, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> MyDocuments(int trang = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Tài liệu của tôi";
            var taiLieu = await _taiLieuService.GetTaiLieuCuaNguoiDungAsync(currentUser.Id, trang);
            return View(taiLieu);
        }

        private async Task LoadDropdownData()
        {
            ViewBag.ChuyenNganh = new SelectList(
                await _context.ChuyenNganh.Where(cn => cn.TrangThaiHoatDong).ToListAsync(),
                "MaChuyenNganh", "TenChuyenNganh");

            ViewBag.LoaiTaiLieu = new SelectList(
                await _context.LoaiTaiLieu.Where(lt => lt.TrangThaiHoatDong).ToListAsync(),
                "MaLoaiTaiLieu", "TenLoaiTaiLieu");
        }
    }
}
