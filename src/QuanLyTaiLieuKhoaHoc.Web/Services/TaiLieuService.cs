using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public class TaiLieuService : ITaiLieuService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<TaiLieuService> _logger;

        public TaiLieuService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<TaiLieuService> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<TaiLieuListViewModel> GetDanhSachTaiLieuAsync(int trang = 1, int kichThuocTrang = 10,
            string? timKiem = null, int? maChuyenNganh = null, int? maLoaiTaiLieu = null, string? sapXep = null)
        {
            var query = _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.NguoiTaiLen)
                .Include(t => t.DanhGiaTaiLieu)
                .Where(t => t.TrangThai == TrangThaiTaiLieu.DaDuyet);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(timKiem))
            {
                query = query.Where(t => t.TenTaiLieu.Contains(timKiem) || t.MoTa.Contains(timKiem));
            }

            // Lọc theo chuyên ngành
            if (maChuyenNganh.HasValue)
            {
                query = query.Where(t => t.MaChuyenNganh == maChuyenNganh.Value);
            }

            // Lọc theo loại tài liệu
            if (maLoaiTaiLieu.HasValue)
            {
                query = query.Where(t => t.MaLoaiTaiLieu == maLoaiTaiLieu.Value);
            }

            // Sắp xếp
            query = sapXep switch
            {
                "ngay_desc" => query.OrderByDescending(t => t.NgayTaiLen),
                "ngay_asc" => query.OrderBy(t => t.NgayTaiLen),
                "luot_tai_desc" => query.OrderByDescending(t => t.LuotTai),
                "luot_tai_asc" => query.OrderBy(t => t.LuotTai),
                "ten_asc" => query.OrderBy(t => t.TenTaiLieu),
                "ten_desc" => query.OrderByDescending(t => t.TenTaiLieu),
                _ => query.OrderByDescending(t => t.NgayTaiLen)
            };

            var tongSo = await query.CountAsync();
            var tongSoTrang = (int)Math.Ceiling((double)tongSo / kichThuocTrang);

            var taiLieu = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var result = new TaiLieuListViewModel
            {
                DanhSachTaiLieu = taiLieu.Select(MapToViewModel).ToList(),
                TrangHienTai = trang,
                TongSoTrang = tongSoTrang,
                TongSoTaiLieu = tongSo,
                TimKiem = timKiem,
                MaChuyenNganh = maChuyenNganh,
                MaLoaiTaiLieu = maLoaiTaiLieu,
                SapXep = sapXep
            };

            return result;
        }

        public async Task<TaiLieuViewModel?> GetTaiLieuByIdAsync(int maTaiLieu)
        {
            var taiLieu = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.NguoiTaiLen)
                .Include(t => t.DanhGiaTaiLieu)
                .FirstOrDefaultAsync(t => t.MaTaiLieu == maTaiLieu);

            return taiLieu != null ? MapToViewModel(taiLieu) : null;
        }

        public async Task<bool> TaoTaiLieuAsync(TaiLieuViewModel model, string maNguoiDung)
        {
            try
            {
                // Kiểm tra file bắt buộc
                if (model.FileTaiLieu == null || model.FileTaiLieu.Length == 0)
                {
                    _logger.LogWarning("Không có file được upload");
                    return false;
                }

                // Xử lý upload file trước
                var uploadResult = await UploadFileAsync(model.FileTaiLieu);

                var taiLieu = new TaiLieu
                {
                    TenTaiLieu = model.TenTaiLieu,
                    MoTa = model.MoTa,
                    MaChuyenNganh = model.MaChuyenNganh,
                    MaLoaiTaiLieu = model.MaLoaiTaiLieu,
                    MaNguoiTaiLen = maNguoiDung,
                    NgayTaiLen = DateTime.Now,
                    TrangThai = TrangThaiTaiLieu.ChoDuyet,
                    DuongDanFile = uploadResult.FilePath,
                    LoaiFile = uploadResult.FileExtension,
                    KichThuocFile = uploadResult.FileSize,
                    LuotTai = 0
                };

                _context.TaiLieu.Add(taiLieu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tài liệu mới");
                return false;
            }
        }

        public async Task<bool> CapNhatTaiLieuAsync(TaiLieuViewModel model)
        {
            try
            {
                var taiLieu = await _context.TaiLieu.FindAsync(model.MaTaiLieu);
                if (taiLieu == null) return false;

                taiLieu.TenTaiLieu = model.TenTaiLieu;
                taiLieu.MoTa = model.MoTa;
                taiLieu.MaChuyenNganh = model.MaChuyenNganh;
                taiLieu.MaLoaiTaiLieu = model.MaLoaiTaiLieu;

                // Xử lý upload file mới nếu có
                if (model.FileTaiLieu != null)
                {
                    // Xóa file cũ
                    if (!string.IsNullOrEmpty(taiLieu.DuongDanFile))
                    {
                        DeleteFile(taiLieu.DuongDanFile);
                    }

                    var uploadResult = await UploadFileAsync(model.FileTaiLieu);
                    taiLieu.DuongDanFile = uploadResult.FilePath;
                    taiLieu.LoaiFile = uploadResult.FileExtension;
                    taiLieu.KichThuocFile = uploadResult.FileSize;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tài liệu");
                return false;
            }
        }

        public async Task<bool> XoaTaiLieuAsync(int maTaiLieu)
        {
            try
            {
                var taiLieu = await _context.TaiLieu.FindAsync(maTaiLieu);
                if (taiLieu == null) return false;

                // Xóa file vật lý
                if (!string.IsNullOrEmpty(taiLieu.DuongDanFile))
                {
                    DeleteFile(taiLieu.DuongDanFile);
                }

                _context.TaiLieu.Remove(taiLieu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tài liệu");
                return false;
            }
        }

        public async Task<bool> DuyetTaiLieuAsync(int maTaiLieu, bool duyet)
        {
            try
            {
                var taiLieu = await _context.TaiLieu.FindAsync(maTaiLieu);
                if (taiLieu == null) return false;

                taiLieu.TrangThai = duyet ? TrangThaiTaiLieu.DaDuyet : TrangThaiTaiLieu.TuChoi;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi duyệt tài liệu");
                return false;
            }
        }

        public async Task<string> TaiFileAsync(int maTaiLieu, string maNguoiDung, string? diaChiIP = null)
        {
            try
            {
                var taiLieu = await _context.TaiLieu.FindAsync(maTaiLieu);
                if (taiLieu == null || string.IsNullOrEmpty(taiLieu.DuongDanFile))
                    return string.Empty;

                // Cập nhật lượt tải
                taiLieu.LuotTai++;

                // Lưu lịch sử tải
                var lichSu = new LichSuTaiTaiLieu
                {
                    MaTaiLieu = maTaiLieu,
                    MaNguoiDung = maNguoiDung,
                    ThoiGianTai = DateTime.Now,
                    DiaChiIP = diaChiIP
                };
                _context.LichSuTaiTaiLieu.Add(lichSu);

                await _context.SaveChangesAsync();
                return taiLieu.DuongDanFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải file");
                return string.Empty;
            }
        }

        public async Task<List<TaiLieuViewModel>> GetTaiLieuMoiNhatAsync(int soLuong = 10)
        {
            var taiLieu = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.NguoiTaiLen)
                .Include(t => t.DanhGiaTaiLieu)
                .Where(t => t.TrangThai == TrangThaiTaiLieu.DaDuyet)
                .OrderByDescending(t => t.NgayTaiLen)
                .Take(soLuong)
                .ToListAsync();

            return taiLieu.Select(MapToViewModel).ToList();
        }

        public async Task<List<TaiLieuViewModel>> GetTaiLieuPhoBienAsync(int soLuong = 10)
        {
            var taiLieu = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.NguoiTaiLen)
                .Include(t => t.DanhGiaTaiLieu)
                .Where(t => t.TrangThai == TrangThaiTaiLieu.DaDuyet)
                .OrderByDescending(t => t.LuotTai)
                .Take(soLuong)
                .ToListAsync();

            return taiLieu.Select(MapToViewModel).ToList();
        }

        public async Task<TaiLieuListViewModel> GetTaiLieuCuaNguoiDungAsync(string maNguoiDung, int trang = 1, int kichThuocTrang = 10)
        {
            var tongSoTaiLieu = await _context.TaiLieu
                .CountAsync(t => t.MaNguoiTaiLen == maNguoiDung);

            var taiLieu = await _context.TaiLieu
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .Include(t => t.DanhGiaTaiLieu)
                .Where(t => t.MaNguoiTaiLen == maNguoiDung)
                .OrderByDescending(t => t.NgayTaiLen)
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            return new TaiLieuListViewModel
            {
                DanhSachTaiLieu = taiLieu.Select(MapToViewModel).ToList(),
                TrangHienTai = trang,
                TongSoTrang = (int)Math.Ceiling((double)tongSoTaiLieu / kichThuocTrang),
                TongSoTaiLieu = tongSoTaiLieu
            };
        }

        private TaiLieuViewModel MapToViewModel(TaiLieu taiLieu)
        {
            return new TaiLieuViewModel
            {
                MaTaiLieu = taiLieu.MaTaiLieu,
                TenTaiLieu = taiLieu.TenTaiLieu,
                MoTa = taiLieu.MoTa,
                TacGia = taiLieu.TacGia,
                MaChuyenNganh = taiLieu.MaChuyenNganh,
                MaLoaiTaiLieu = taiLieu.MaLoaiTaiLieu,
                MaNguoiTaiLen = taiLieu.MaNguoiTaiLen,
                TenChuyenNganh = taiLieu.ChuyenNganh?.TenChuyenNganh,
                TenLoaiTaiLieu = taiLieu.LoaiTaiLieu?.TenLoaiTaiLieu,
                TenNguoiTaiLen = taiLieu.NguoiTaiLen?.HoTen,
                NgayTaiLen = taiLieu.NgayTaiLen,
                LuotTai = taiLieu.LuotTai,
                LuotMuon = taiLieu.PhieuMuonTras != null ? taiLieu.PhieuMuonTras.Count : 0,
                TrangThai = (int)taiLieu.TrangThai,
                DuongDanFile = taiLieu.DuongDanFile,
                LoaiFile = taiLieu.LoaiFile,
                KichThuocFile = taiLieu.KichThuocFile,
                DiemDanhGiaTrungBinh = taiLieu.DanhGiaTaiLieu.Any() ?
                    taiLieu.DanhGiaTaiLieu.Average(d => d.DiemDanhGia) : 0,
                SoLuotDanhGia = taiLieu.DanhGiaTaiLieu.Count
            };
        }

        private async Task<(string FilePath, string FileExtension, long FileSize)> UploadFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return ($"/uploads/documents/{fileName}", Path.GetExtension(file.FileName), file.Length);
        }

        private void DeleteFile(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa file: {FilePath}", filePath);
            }
        }
    }
}
