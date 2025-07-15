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

        private readonly IPhieuMuonTraService _phieuMuonTraService;

        public TaiLieuController(ITaiLieuService taiLieuService, ApplicationDbContext context, UserManager<NguoiDung> userManager, IPhieuMuonTraService phieuMuonTraService)
        {
            _taiLieuService = taiLieuService;
            _context = context;
            _userManager = userManager;
            _phieuMuonTraService = phieuMuonTraService;
        }
        [HttpPost]
        [Authorize(Roles = "SinhVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiYeuCauMuon(int maTaiLieu, DateTime ngayMuon, string lyDo)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để gửi yêu cầu mượn.";
                return RedirectToAction("Details", new { id = maTaiLieu });
            }

            // Gửi yêu cầu mượn với ghi chú gồm ngày mượn và lý do
            string ghiChu = $"Ngày đến mượn: {ngayMuon:dd/MM/yyyy HH:mm}\nLý do: {lyDo}";
            var result = await _phieuMuonTraService.GuiYeuCauMuonAsync(maTaiLieu, currentUser.Id, ghiChu);
            if (result)
            {
                TempData["SuccessMessage"] = "Gửi yêu cầu mượn thành công! Vui lòng chờ duyệt.";
            }
            else
            {
                TempData["ErrorMessage"] = "Bạn đã gửi yêu cầu mượn cho tài liệu này hoặc chưa trả tài liệu.";
            }
            return RedirectToAction("Details", new { id = maTaiLieu });
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
        [Route("TaiLieu")]
        [Route("TaiLieu/Index")]
        [Route("TimKiem")]
        [Route("TimKiem/Index")]
        public async Task<IActionResult> Index(int trang = 1, string? timKiem = null, int? maChuyenNganh = null,
            int? maLoaiTaiLieu = null, string? sapXep = null, string? q = null)
        {
            // Hỗ trợ tham số 'q' từ trang TimKiem cũ
            if (!string.IsNullOrEmpty(q) && string.IsNullOrEmpty(timKiem))
            {
                timKiem = q;
            }

            ViewData["Title"] = "Danh sách / Tìm kiếm tài liệu";

            // Chuẩn bị data cho dropdown
            await LoadDropdownData();

            // Set ViewBag cho form tìm kiếm
            ViewBag.TimKiem = timKiem;
            ViewBag.SearchAction = "Index";
            ViewBag.SearchController = "TaiLieu";

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
            if (currentUser == null || (currentUser.VaiTro != VaiTroNguoiDung.ThuThu && currentUser.VaiTro != VaiTroNguoiDung.GiangVien))
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
            if (currentUser == null || (currentUser.VaiTro != VaiTroNguoiDung.ThuThu && currentUser.VaiTro != VaiTroNguoiDung.GiangVien))
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


        [Authorize(Roles = "GiangVien,ThuThu,SinhVien")]
        public async Task<IActionResult> Download(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser?.Id ?? "anonymous";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Lấy thông tin tài liệu để kiểm tra loại
            var taiLieu = await _context.TaiLieu.FindAsync(id);
            if (taiLieu == null || string.IsNullOrEmpty(taiLieu.DuongDanFile))
            {
                return NotFound();
            }

            // Nếu là sinh viên, chỉ cho phép tải các loại đặc biệt
            if (User.IsInRole("SinhVien") && (taiLieu.MaLoaiTaiLieu != 10 && taiLieu.MaLoaiTaiLieu != 20 && taiLieu.MaLoaiTaiLieu != 30))
            {
                return Forbid();
            }

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

        [HttpGet]
        [AllowAnonymous]
        [Route("TaiLieu/Suggestions")]
        [Route("TimKiem/Suggestions")]
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

        private async Task LoadDropdownData()
        {
            ViewBag.ChuyenNganh = new SelectList(
                await _context.ChuyenNganh.Where(cn => cn.TrangThaiHoatDong).ToListAsync(),
                "MaChuyenNganh", "TenChuyenNganh");

            ViewBag.LoaiTaiLieu = new SelectList(
                await _context.LoaiTaiLieu.Where(lt => lt.TrangThaiHoatDong).ToListAsync(),
                "MaLoaiTaiLieu", "TenLoaiTaiLieu");
        }
        // API cập nhật tài liệu từ modal (AJAX)
        [HttpPost]
        [Authorize(Roles = "ThuThu,GiangVien")]
        [Route("TaiLieu/EditTaiLieu")]
        public async Task<IActionResult> EditTaiLieu([FromBody] EditTaiLieuViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ");

            var taiLieu = await _context.TaiLieu.FindAsync(model.Id);
            if (taiLieu == null)
                return NotFound();

            taiLieu.TenTaiLieu = model.TenTaiLieu;
            taiLieu.MoTa = model.MoTa;
            taiLieu.TacGia = model.TacGia;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}
