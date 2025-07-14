Dưới đây là tổng hợp toàn bộ các chức năng, luồng nghiệp vụ và cấu trúc hệ thống cho dự án Quản lý tài liệu khoa học tại thư viện Nam Cần Thơ:

1. Cấu trúc database
BaiBaoKhoaHoc: Quản lý bài báo khoa học.
DeTaiNghienCuu: Quản lý đề tài nghiên cứu khoa học.
GiaoTrinh: Quản lý giáo trình/tài liệu giảng dạy.
LoaiTaiLieu: Phân loại tài liệu (bài báo, đề tài, giáo trình).
PhieuMuonTra: Quản lý phiếu mượn/trả tài liệu (chỉ sinh viên được mượn).
NguoiDung: Quản lý thông tin người dùng (sinh viên, giảng viên, thủ thư).
Các bảng hệ thống của Identity (đăng nhập, phân quyền).

2. Luồng nghiệp vụ
Đăng ký/Đăng nhập
Sinh viên/giảng viên đăng ký tài khoản trên web (sinh viên: MSSV, lớp; giảng viên: mã GV, bộ môn).
Thủ thư có thể tạo tài khoản sinh viên khi mượn trực tiếp.

Tra cứu & Xem tài liệu
Người dùng đăng nhập, tra cứu tài liệu theo 3 loại.
Mỗi thẻ tài liệu có nút “Xem thử” (modal hiển thị bìa/mô tả ngắn).

Yêu cầu mượn tài liệu (chỉ sinh viên)
Sinh viên nhấn “Yêu cầu mượn”, nhập ngày giờ dự kiến đến lấy và lý do.
Thông tin yêu cầu lưu lại, thủ thư xem và xác nhận.

Mượn trực tiếp tại thư viện
Sinh viên đến quầy, đọc MSSV cho thủ thư kiểm tra.
Nếu có tài khoản và yêu cầu hợp lệ, thủ thư lập phiếu mượn và giao tài liệu.
Nếu chưa có tài khoản, thủ thư tạo mới và lập phiếu mượn.

Trả tài liệu
Khi sinh viên trả tài liệu, thủ thư cập nhật trạng thái phiếu mượn.

3. Dashboard từng vai trò
Sinh viên
Thông tin cá nhân.
Trạng thái mượn hiện tại.
Lịch sử mượn tài liệu.
Danh sách yêu cầu mượn đã gửi.

Giảng viên
Thông tin tài khoản.
Lịch sử xem tài liệu.

Thủ thư
Quản lý tài liệu.
Xử lý yêu cầu mượn.
Lập phiếu mượn/trả.
Tạo tài khoản sinh viên.

4. Công việc cần làm
Tạo migration và khởi tạo lại database.
Seed dữ liệu mẫu cho các bảng.
Sửa form đăng ký/đăng nhập cho từng vai trò.
Sửa giao diện và logic các trang (trang chủ, tài liệu, dashboard).
Xây dựng logic nghiệp vụ mượn/trả đúng thực tế.
Kiểm thử và hoàn thiện UI/UX.