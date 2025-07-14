using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Data;

public class ApplicationDbContext : IdentityDbContext<NguoiDung>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets cho các entities
    public DbSet<TaiLieu> TaiLieu { get; set; }
    public DbSet<ChuyenNganh> ChuyenNganh { get; set; }
    public DbSet<LoaiTaiLieu> LoaiTaiLieu { get; set; }
    public DbSet<LichSuTaiTaiLieu> LichSuTaiTaiLieu { get; set; }
    public DbSet<DanhGiaTaiLieu> DanhGiaTaiLieu { get; set; }
    public DbSet<PhieuMuonTra> PhieuMuonTra { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Cấu hình tên bảng
        builder.Entity<NguoiDung>().ToTable("NguoiDung");
        builder.Entity<TaiLieu>().ToTable("TaiLieu");
        builder.Entity<ChuyenNganh>().ToTable("ChuyenNganh");
        builder.Entity<LoaiTaiLieu>().ToTable("LoaiTaiLieu");
        builder.Entity<LichSuTaiTaiLieu>().ToTable("LichSuTaiTaiLieu");
        builder.Entity<DanhGiaTaiLieu>().ToTable("DanhGiaTaiLieu");

        // Cấu hình tên bảng Identity
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("VaiTro");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("NguoiDung_VaiTro");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("NguoiDung_Claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("NguoiDung_Logins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("NguoiDung_Tokens");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("VaiTro_Claims");

        // Cấu hình relationships
        builder.Entity<TaiLieu>()
            .HasOne(t => t.ChuyenNganh)
            .WithMany(cn => cn.TaiLieu)
            .HasForeignKey(t => t.MaChuyenNganh)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaiLieu>()
            .HasOne(t => t.LoaiTaiLieu)
            .WithMany(lt => lt.TaiLieu)
            .HasForeignKey(t => t.MaLoaiTaiLieu)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<NguoiDung>()
            .HasOne(nd => nd.ChuyenNganh)
            .WithMany(cn => cn.NguoiDung)
            .HasForeignKey(nd => nd.MaChuyenNganh)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<LichSuTaiTaiLieu>()
            .HasOne(ls => ls.TaiLieu)
            .WithMany(t => t.LichSuTaiTaiLieu)
            .HasForeignKey(ls => ls.MaTaiLieu)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LichSuTaiTaiLieu>()
            .HasOne(ls => ls.NguoiDung)
            .WithMany(nd => nd.LichSuTaiTaiLieu)
            .HasForeignKey(ls => ls.MaNguoiDung)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        // Seed ChuyenNganhx`
        builder.Entity<ChuyenNganh>().HasData(
            new ChuyenNganh { MaChuyenNganh = 1, TenChuyenNganh = "Công nghệ Thông tin", MoTa = "Chuyên ngành về tin học và công nghệ", NgayTao = DateTime.Now },
            new ChuyenNganh { MaChuyenNganh = 2, TenChuyenNganh = "Kinh tế", MoTa = "Chuyên ngành về kinh tế và quản lý", NgayTao = DateTime.Now },
            new ChuyenNganh { MaChuyenNganh = 3, TenChuyenNganh = "Kỹ thuật", MoTa = "Các chuyên ngành kỹ thuật", NgayTao = DateTime.Now },
            new ChuyenNganh { MaChuyenNganh = 4, TenChuyenNganh = "Ngoại ngữ", MoTa = "Chuyên ngành về ngôn ngữ", NgayTao = DateTime.Now }
        );

        // Seed LoaiTaiLieu
        builder.Entity<LoaiTaiLieu>().HasData(
            new LoaiTaiLieu { MaLoaiTaiLieu = 1, TenLoaiTaiLieu = "Giáo trình", MoTa = "Tài liệu giảng dạy chính thức", BieuTuong = "📚", NgayTao = DateTime.Now },
            new LoaiTaiLieu { MaLoaiTaiLieu = 2, TenLoaiTaiLieu = "Bài giảng", MoTa = "Slide bài giảng của giảng viên", BieuTuong = "📖", NgayTao = DateTime.Now },
            new LoaiTaiLieu { MaLoaiTaiLieu = 3, TenLoaiTaiLieu = "Luận văn", MoTa = "Luận văn tốt nghiệp", BieuTuong = "🎓", NgayTao = DateTime.Now },
            new LoaiTaiLieu { MaLoaiTaiLieu = 4, TenLoaiTaiLieu = "Tài liệu tham khảo", MoTa = "Tài liệu bổ sung", BieuTuong = "📑", NgayTao = DateTime.Now }
        );
    }
}
