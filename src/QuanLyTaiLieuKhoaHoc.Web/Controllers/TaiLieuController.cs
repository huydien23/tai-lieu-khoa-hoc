using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;
using System.Security.Claims;


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
                diemDanhGiaTrungBinh = 0,
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
            if (!string.IsNullOrEmpty(q) && string.IsNullOrEmpty(timKiem))
            {
                timKiem = q;
            }

            ViewData["Title"] = "Danh sách / Tìm kiếm tài liệu";

            await LoadDropdownData();

            // Set ViewBag cho form tìm kiếm
            ViewBag.TimKiem = timKiem;
            ViewBag.SearchAction = "Index";
            ViewBag.SearchController = "TaiLieu";

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

            var model = await _taiLieuService.GetDanhSachTaiLieuAsync(trang, 12, timKiem, maChuyenNganh, maLoaiTaiLieu, sapXep, vaiTro);
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
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

            var taiLieu = await _taiLieuService.GetTaiLieuByIdAsync(id, vaiTro);
            if (taiLieu == null)
            {
                // Kiểm tra nếu không có quyền truy cập
                if (User.Identity?.IsAuthenticated == true && vaiTro == "SinhVien")
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập tài liệu này.";
                    return RedirectToAction("Index");
                }
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

            // Lấy thông tin tài liệu để kiểm tra quyền tải
            var taiLieu = await _context.TaiLieu.FindAsync(id);
            if (taiLieu == null || string.IsNullOrEmpty(taiLieu.DuongDanFile))
            {
                return NotFound();
            }

            // Kiểm tra quyền tải dựa trên ChoPhepTaiFile
            if (User.IsInRole("SinhVien") && !taiLieu.ChoPhepTaiFile)
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

        [Authorize(Roles = "GiangVien,ThuThu,SinhVien")]
        public async Task<IActionResult> ViewPdf(int id)
        {
            var taiLieu = await _taiLieuService.GetTaiLieuByIdAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            if (!taiLieu.ChoPhepTaiFile)
            {
                TempData["ErrorMessage"] = "Tài liệu này không cho phép xem online.";
                return RedirectToAction("Details", new { id });
            }

            // Kiểm tra file có phải PDF không
            var fileExtension = Path.GetExtension(taiLieu.DuongDanFile)?.ToLower();
            if (fileExtension != ".pdf")
            {
                TempData["ErrorMessage"] = "Chỉ hỗ trợ xem file PDF online.";
                return RedirectToAction("Details", new { id });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taiLieu.DuongDanFile.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "File không tồn tại.";
                return RedirectToAction("Details", new { id });
            }

            // Ghi log xem tài liệu
            var lichSuTai = new LichSuTaiTaiLieu
            {
                MaTaiLieu = taiLieu.MaTaiLieu,
                MaNguoiDung = currentUser.Id,
                ThoiGianTai = DateTime.Now,
                TrangThai = "Xem online",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            _context.LichSuTaiTaiLieu.Add(lichSuTai);
            await _context.SaveChangesAsync();

            return View(taiLieu);
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
        public async Task<IActionResult> EditTaiLieu([FromForm] EditTaiLieuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var taiLieu = await _context.TaiLieu.FindAsync(model.MaTaiLieu);
            if (taiLieu == null)
                return NotFound();

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

            if (model.FileTaiLieu != null && model.FileTaiLieu.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}_{model.FileTaiLieu.FileName}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileTaiLieu.CopyToAsync(stream);
                }
                taiLieu.DuongDanFile = "/uploads/" + fileName;
                taiLieu.LoaiFile = Path.GetExtension(fileName);
                taiLieu.KichThuocFile = model.FileTaiLieu.Length;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemYeuThich(int maTaiLieu)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var daTonTai = await _context.YeuThichTaiLieu
                .AnyAsync(y => y.MaTaiLieu == maTaiLieu && y.UserId == userId);

            if (!daTonTai)
            {
                var yeuThich = new YeuThichTaiLieu
                {
                    MaTaiLieu = maTaiLieu,
                    UserId = userId
                };

                _context.YeuThichTaiLieu.Add(yeuThich);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = maTaiLieu });
        }

        [Authorize]
        public async Task<IActionResult> YeuThich()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var nguoiDung = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            var danhSachYeuThich = await _context.YeuThichTaiLieu
                .Include(y => y.TaiLieu)
                .Where(y => y.UserId == userId && y.TaiLieu != null)
                .Select(y => new YeuThichTaiLieuItem
                {
                    Id = y.Id,
                    MaTaiLieu = y.MaTaiLieu,
                    TenTaiLieu = y.TaiLieu!.TenTaiLieu,
                    TacGia = y.TaiLieu.TacGia,
                    LoaiFile = y.TaiLieu.LoaiFile,
                    KichThuocMB = Math.Round(y.TaiLieu.KichThuocFile / 1024.0, 1),
                    ChoPhepTaiFile = y.TaiLieu.ChoPhepTaiFile,
                    NgayYeuThich = y.NgayYeuThich
                })
                .ToListAsync();

            var viewModel = new TaiLieuYeuThichViewModel
            {
                HoTenNguoiDung = nguoiDung?.HoTen ?? "Người dùng",
                MaSoNguoiDung = nguoiDung?.MaSo,
                VaiTroNguoiDung = nguoiDung?.VaiTro.ToString() ?? "",
                DanhSachYeuThich = danhSachYeuThich
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BoYeuThich(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Tìm tài liệu yêu thích của người dùng
            var yeuThich = await _context.YeuThichTaiLieu
                .FirstOrDefaultAsync(y => y.Id == id && y.UserId == userId);

            if (yeuThich != null)
            {
                _context.YeuThichTaiLieu.Remove(yeuThich);
                await _context.SaveChangesAsync();
            }

            //  Xác định vai trò để chuyển hướng
            if (User.IsInRole("GiangVien"))
            {
                return RedirectToAction("Dashboard-Lecturer", "Lecturer");
            }
            else if (User.IsInRole("SinhVien"))
            {
                return RedirectToAction("Dashboard-Student", "Student");
            }

            // Nếu không có vai trò cụ thể, chuyển về trang chủ
            return RedirectToAction("Index", "Home");
        }

        // API lấy tài liệu liên quan
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTaiLieuLienQuan(int id)
        {
            try
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

                var taiLieuLienQuan = await _taiLieuService.GetTaiLieuLienQuanAsync(id, 2, vaiTro);
                return Json(new { success = true, data = taiLieuLienQuan });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy tài liệu liên quan." }); // ex không dùng, không cần khai báo
            }
        }

    }


}

