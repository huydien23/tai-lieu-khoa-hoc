Tóm tắt các điểm cần sửa và giải pháp phù hợp với yêu cầu thực tế và ý kiến của thầy:

1. Phân quyền chức năng tải tài liệu
Sinh viên:
Không được tải file gốc, chỉ được xem/truy cập nội dung trên web.
Ẩn hoàn toàn nút "Tải xuống" trên mọi giao diện.
Giảng viên:
Được phép tải file tài liệu về máy.
Hiển thị nút "Tải xuống" trên trang chi tiết/modal tài liệu.
Thủ thư:
Không cần nút tải xuống (trừ khi muốn kiểm tra file, có thể để tùy chọn).

Giải pháp:
Sửa UI: Ẩn/hiện nút "Tải xuống" theo vai trò.
Sửa backend: Endpoint tải file chỉ cho phép role "Giảng viên" (và "Thủ thư" nếu cần).

2. Chức năng mượn/trả tài liệu
Chỉ thủ thư được thao tác mượn/trả tài liệu.
Sinh viên, giảng viên chỉ được xem, tìm kiếm, tra cứu tài liệu.
Ẩn hoặc loại bỏ toàn bộ nút/logic mượn/trả khỏi giao diện sinh viên, giảng viên.
Đảm bảo backend/API không cho phép sinh viên, giảng viên thao tác mượn/trả.

3. Thống kê trên dashboard
Bổ sung thống kê số lượng tài liệu theo loại, danh mục (category) trên dashboard sinh viên và giảng viên.
Có thể dùng biểu đồ, bảng số liệu để trực quan.

4. Kiểm tra và tối ưu UI/UX
Đảm bảo dashboard sinh viên, giảng viên chỉ có các chức năng: xem, tìm kiếm, tra cứu, xem thống kê, đề xuất, yêu thích.
Loại bỏ các thao tác không phù hợp với vai trò.

5. Kiểm tra backend
Đảm bảo các API thao tác nhạy cảm (tải file, mượn/trả) đều kiểm tra đúng vai trò người dùng.
Trả về lỗi hoặc thông báo rõ ràng nếu truy cập sai quyền.