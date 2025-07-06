# Sơ đồ Quan hệ Thực thể (ERD) - Hệ thống Quản lý Tài liệu Khoa học

## Mô tả các Bảng và Quan hệ

### 1. Bảng NguoiDung (AspNetUsers)
**Mục đích**: Quản lý thông tin người dùng trong hệ thống
- **Id** (PK): Khóa chính, định danh duy nhất
- **UserName**: Tên đăng nhập
- **Email**: Email người dùng
- **HoTen**: Họ và tên đầy đủ
- **MaSo**: Mã số sinh viên/giảng viên
- **VaiTro**: Vai trò (0=Quản trị viên, 1=Giảng viên, 2=Sinh viên)
- **MaChuyenNganh** (FK): Khóa ngoại đến bảng ChuyenNganh
- **KhoaHoc**: Khóa học (dành cho sinh viên)
- **NgayTao**: Ngày tạo tài khoản
- **TrangThaiHoatDong**: Trạng thái hoạt động

### 2. Bảng ChuyenNganh
**Mục đích**: Quản lý danh mục chuyên ngành
- **MaChuyenNganh** (PK): Khóa chính
- **TenChuyenNganh**: Tên chuyên ngành
- **MoTa**: Mô tả về chuyên ngành
- **NgayTao**: Ngày tạo
- **TrangThaiHoatDong**: Trạng thái hoạt động

### 3. Bảng LoaiTaiLieu
**Mục đích**: Phân loại tài liệu theo loại
- **MaLoaiTaiLieu** (PK): Khóa chính
- **TenLoaiTaiLieu**: Tên loại tài liệu
- **MoTa**: Mô tả loại tài liệu
- **BieuTuong**: Biểu tượng hiển thị
- **NgayTao**: Ngày tạo
- **TrangThaiHoatDong**: Trạng thái hoạt động

### 4. Bảng TaiLieu
**Mục đích**: Lưu trữ thông tin tài liệu chính
- **MaTaiLieu** (PK): Khóa chính
- **TenTaiLieu**: Tên tài liệu
- **MoTa**: Mô tả chi tiết
- **DuongDanFile**: Đường dẫn đến file
- **LoaiFile**: Phần mở rộng file (.pdf, .docx, ...)
- **KichThuocFile**: Kích thước file (bytes)
- **NgayTaiLen**: Ngày tải lên
- **LuotTai**: Số lượt tải xuống
- **TrangThai**: Trạng thái duyệt (0=Chờ duyệt, 1=Đã duyệt, 2=Từ chối, 3=Ẩn)
- **MaChuyenNganh** (FK): Thuộc chuyên ngành nào
- **MaLoaiTaiLieu** (FK): Thuộc loại tài liệu nào  
- **MaNguoiTaiLen** (FK): Người tải lên

### 5. Bảng LichSuTaiTaiLieu
**Mục đích**: Theo dõi lịch sử tải xuống tài liệu
- **MaLichSu** (PK): Khóa chính
- **MaTaiLieu** (FK): Tài liệu được tải
- **MaNguoiDung** (FK): Người tải xuống
- **ThoiGianTai**: Thời gian tải
- **DiaChiIP**: Địa chỉ IP
- **UserAgent**: Thông tin trình duyệt

### 6. Bảng DanhGiaTaiLieu
**Mục đích**: Lưu đánh giá và nhận xét về tài liệu
- **MaDanhGia** (PK): Khóa chính
- **MaTaiLieu** (FK): Tài liệu được đánh giá
- **MaNguoiDung** (FK): Người đánh giá
- **DiemDanhGia**: Điểm số (1-5 sao)
- **NhanXet**: Nhận xét chi tiết
- **NgayDanhGia**: Ngày đánh giá

## Quan hệ giữa các Bảng

### Quan hệ 1-N (One-to-Many):

1. **ChuyenNganh → TaiLieu**: Một chuyên ngành có nhiều tài liệu
2. **ChuyenNganh → NguoiDung**: Một chuyên ngành có nhiều người dùng
3. **LoaiTaiLieu → TaiLieu**: Một loại tài liệu có nhiều tài liệu
4. **NguoiDung → TaiLieu**: Một người dùng có thể tải lên nhiều tài liệu
5. **TaiLieu → LichSuTaiTaiLieu**: Một tài liệu có nhiều lượt tải
6. **NguoiDung → LichSuTaiTaiLieu**: Một người dùng có nhiều lần tải
7. **TaiLieu → DanhGiaTaiLieu**: Một tài liệu có nhiều đánh giá
8. **NguoiDung → DanhGiaTaiLieu**: Một người dùng có thể đánh giá nhiều tài liệu

## Các Ràng buộc Toàn vẹn

1. **Khóa chính**: Mỗi bảng có khóa chính duy nhất
2. **Khóa ngoại**: Đảm bảo tính toàn vẹn tham chiếu
3. **NOT NULL**: Các trường quan trọng không được để trống
4. **UNIQUE**: Email người dùng phải duy nhất
5. **CHECK**: Điểm đánh giá phải từ 1-5

## Chỉ mục (Index)

1. Index trên **Email** (NguoiDung) - để tìm kiếm nhanh
2. Index trên **TenTaiLieu** (TaiLieu) - để tìm kiếm nhanh
3. Index trên **NgayTaiLen** (TaiLieu) - để sắp xếp theo thời gian
4. Index trên **LuotTai** (TaiLieu) - để sắp xếp theo độ phổ biến

## Ưu điểm của Thiết kế

1. **Tách bạch dữ liệu**: Các loại thông tin được tách thành bảng riêng
2. **Dễ mở rộng**: Có thể thêm chuyên ngành, loại tài liệu mới
3. **Theo dõi hoạt động**: Lưu lịch sử tải và đánh giá
4. **Bảo mật**: Phân quyền theo vai trò người dùng
5. **Hiệu suất**: Có chỉ mục cho các truy vấn thường xuyên

## Dữ liệu Mẫu

### Chuyên ngành:
- Công nghệ Thông tin
- Kinh tế  
- Kỹ thuật
- Ngoại ngữ

### Loại tài liệu:
- Giáo trình
- Bài giảng
- Đề thi
- Bài tập
- Luận văn
- Tài liệu tham khảo
